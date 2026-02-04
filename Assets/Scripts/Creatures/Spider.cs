using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
    public enum SpiderState {Hidden, Idle, ChangeTile, Attack }

	public class Spider : MonoBehaviour
	{
        public delegate void ChangeStateDelegate(SpiderState oldState, SpiderState newState);   
        public static ChangeStateDelegate OnChangeState;

        [SerializeField]
		Collider flashlightCollider;

        [SerializeField]
        GameObject model;

#if UNITY_EDITOR
        [SerializeField]
        Transform _testTile;
#endif

        Flashlight flashlight;

        FirstPersonController playerController;

        List<int> tileIndices;

        int currentTileIndex = -1;
        Transform currentTileTransform;

        SpiderState state = SpiderState.Hidden;

        float modelUpDistance = 0.25f;

        float minimumEight = 2.75f;

        float lightTargetDistance = 10f;

        float bobMax = .2f;

        Animator animator;

        float attackRange = 1.75f;

        CameraShake cameraShake;

        Transform cameraTarget;

        Tween bobTween;

      
        private void Awake()
        {
            animator = model.GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
	    {
#if UNITY_EDITOR
            if(_testTile)
            {
                playerController = FindFirstObjectByType<FirstPersonController>();
                flashlight = FindFirstObjectByType<Flashlight>();
                cameraShake = FindFirstObjectByType<CameraShake>();
                cameraTarget = playerController.transform.Find("PlayerCameraRoot");
                currentTileIndex = 0;
                currentTileTransform = _testTile;
                SetState(SpiderState.Idle);
                return;
            }
            
#endif

            currentTileIndex = GetNextTileIndex();
            currentTileTransform = MazeBuilder.Instance.GetTileMainObject(currentTileIndex).transform;
            SetState(SpiderState.Idle);

        }

	    // Update is called once per frame
	    void Update()
	    {
            UpdateState();
	    }

        private void OnEnable()
        {
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

        private void HandleOnMazeCreated()
        {
            flashlight = FindFirstObjectByType<Flashlight>();   
            playerController = FindFirstObjectByType<FirstPersonController>();
            cameraShake = FindFirstObjectByType<CameraShake>();
            cameraTarget = playerController.transform.Find("PlayerCameraRoot");
            tileIndices = MazeBuilder.Instance.GetTileWithPillarIndices();
        }

        int GetNextTileIndex()
        {
            // Copy the list of tile indices
            List<int> availableIndices = new List<int>(tileIndices);
            // Remove the last used index
            if (currentTileIndex >= 0)
                availableIndices.Remove(currentTileIndex);

            // Remove tiles too close to the player
            float distance = 6f; // Minimum distance from player
            var playerPos = playerController.transform.position;
            playerPos.y = 0; // Ignore height
            foreach (int i in tileIndices)
            {
                var coords = MazeBuilder.Instance.GetTileCoords(i);
                Vector3 pos = new Vector3(coords.x, 0, coords.y);
                if(Vector3.Distance(playerPos, pos) < distance)
                {
                    availableIndices.Remove(i);
                }
            }

            // Select a random index from the remaining ones
            int index = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
            return index;
        }

        void UpdateState()
        {
            switch (state)
            {
                case SpiderState.Idle:
                    UpdateIdleState();
                    break;
            }
        }

        void UpdateIdleState()
        {

            // Get tile to player direction
            var direction = playerController.transform.position - currentTileTransform.position;
            direction.y = 0; // Ignore height

            // Get cardinal direction
            List<Vector3> cardDirs = new List<Vector3>()
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right
            };
            var bestDir = cardDirs.OrderBy(d => Vector3.Dot(d, direction)).ToList()[0];
            bestDir *= -1;

            bool moveUp = false;
            if (flashlight.IsOn() && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, lightTargetDistance, LayerMask.GetMask(new string[] { "FlashlightTarget" })))
            {
                //bestDir *= -1;
                moveUp = true;
            }



            if (moveUp)
            {
                
                KillSpiderBob();
                float moveSpeed = 5f;
                //transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, moveSpeed * Time.deltaTime);
            }
            else
            {
                float moveSpeed = 3f;
                if(transform.position.y != minimumEight)
                    //transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, transform.position - Vector3.up, moveSpeed * Time.deltaTime);

                if (transform.position.y < minimumEight)
                {
                    transform.position = new Vector3(transform.position.x, minimumEight, transform.position.z);
                    //transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, minimumEight, transform.position.z), moveSpeed * Time.deltaTime);

                    SpiderBob(Random.Range(bobMax * .8f, bobMax));
                }
            }


            // Look at the player
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.up, bestDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

            // Check player distance
            if (direction.magnitude < attackRange && transform.position.y == minimumEight)
            {

                if (cardDirs.OrderBy(d => Vector3.Dot(d, transform.up)).ToList()[0] == -bestDir)
                {
                    // Attack the player
                    SetState(SpiderState.Attack);
                }


                return;
            }


            //transform.LookAt(transform.position+Vector3.up, Vector3.right);
        }

        void _UpdateIdleState()
        {
            
            // Get tile to player direction
            var direction = playerController.transform.position - currentTileTransform.position;
            direction.y = 0; // Ignore height

            // Get cardinal direction
            List<Vector3> cardDirs = new List<Vector3>()
            {
                Vector3.forward,
                Vector3.back,
                Vector3.left,
                Vector3.right
            };
            var bestDir = cardDirs.OrderBy(d=>Vector3.Dot(d, direction)).ToList()[0];
            bool bestDirInverted = false;
            if(!flashlight.IsOn() || !Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, lightTargetDistance, LayerMask.GetMask(new string[] { "FlashlightTarget" })))
            {
                bestDir *= -1;
                bestDirInverted = true;
            }
                


            // Look at the player
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.up, bestDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

            // Check player distance
            if(bestDirInverted && direction.magnitude < attackRange)
            {
                
                if(cardDirs.OrderBy(d => Vector3.Dot(d, transform.up)).ToList()[0] == -bestDir)
                {
                    // Attack the player
                    SetState(SpiderState.Attack);
                }
                
                
                return;
            }


            //transform.LookAt(transform.position+Vector3.up, Vector3.right);
        }

        void EnterIdleState()
        {
            StopAllCoroutines();
            model.transform.DOKill();

            model.SetActive(true);

            // Move the spider to the current tile
            transform.position = currentTileTransform.position + Vector3.up * minimumEight;
            // Set spider orientation
            transform.eulerAngles = Vector3.right * 90f;

            model.transform.localPosition = Vector3.up * modelUpDistance;

            animator.SetTrigger("Idle");
            animator.SetFloat("Direction", 1);

            // Slightly move up and down
            SpiderBob(Random.Range(bobMax * .8f, bobMax));

            StartCoroutine(ChangeTileDelayed(60f));
        }

        IEnumerator ChangeTileDelayed(float timer)
        {
            yield return new WaitForSeconds(timer);

            if(state == SpiderState.Idle)
            {
                SetState(SpiderState.ChangeTile);
            }
        }

        void EnterChangeTile()
        {
            StopAllCoroutines();
            transform.DOKill();
            // Climb wall
            transform.DOMoveY(minimumEight * 10, 2).SetEase(Ease.InSine).OnComplete(() => 
            {
                currentTileIndex = GetNextTileIndex();
                currentTileTransform = MazeBuilder.Instance.GetTileMainObject(currentTileIndex).transform;
                // Move to new tile
                transform.position = currentTileTransform.position + Vector3.up * minimumEight * 10;
                // Descend wall
                transform.DOMoveY(minimumEight, 2).SetEase(Ease.OutSine).SetDelay(1).OnComplete(() =>
                {
                    SetState(SpiderState.Idle);
                });
            });
        }

        void EnterAttackState()
        {
            StopAllCoroutines();
            model.transform.DOKill();

            // Move spider model back in position
            model.transform.DOLocalMove(Vector3.zero, .2f);

            
            // Jump to the player
            float time = .25f;
            float distance = .5f;
            var jumpPos = cameraTarget.position + cameraTarget.forward * distance *1.5f;// Camera.main.transform.position + Camera.main.transform.forward * distance * 1.5f;
            var seq = transform.DOJump(jumpPos, 1, 1, time);
            //seq.Join(transform.DORotateQuaternion(Quaternion.LookRotation(Camera.main.transform.up, Camera.main.transform.forward), time));
            seq.Join(transform.DORotateQuaternion(Quaternion.LookRotation(cameraTarget.up, cameraTarget.forward), time));
            seq.AppendCallback(() =>
            {
                transform.parent = cameraTarget;// Camera.main.transform;
                transform.position = cameraTarget.position + cameraTarget.forward * distance;//  Camera.main.transform.position + Camera.main.transform.forward * distance;
                transform.rotation = Quaternion.LookRotation(cameraTarget.up, cameraTarget.forward);

                // Apply debuff to player
                PlayerDeafDebuff.Instance.Apply();

                JumpscareManager.Instance.PlayAudio();

                cameraShake.PlayLetterboxJumpScare();

                StartCoroutine(SetHiddenDelayed(1f));
            });

           
        }

        IEnumerator SetHiddenDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.parent = null;
            float time = .5f;
            Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 8f;// + Vector3.up * 7f;
            var seq = transform.DOJump(position, 1, 1, time);
            //seq.Join(transform.DOLocalRotate(Vector3.right * 90, time));
            //seq.Join(model.transform.DOLocalMoveZ(modelUpDistance, time));
            seq.AppendCallback(() =>
            {
                SetState(SpiderState.Hidden);
            });
        }

        void EnterHiddenState()
        {
            StopAllCoroutines();
            model.SetActive(false);

            StartCoroutine(SetIdleDelayed(60)); // We only move to hidded state after attacking, so we can set idle after some time
        }

        IEnumerator SetIdleDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            currentTileIndex = GetNextTileIndex();
            currentTileTransform = MazeBuilder.Instance.GetTileMainObject(currentTileIndex).transform;
            SetState(SpiderState.Idle);
        }

        void SpiderBob(float destination)
        {
            if (bobTween != null) return;
            bobTween = model.transform.DOLocalMoveZ(destination, Random.Range(.5f, 0.75f)).SetEase(Ease.InOutSine).OnComplete(() => 
            {
                float sign = Mathf.Sign(-destination);
                animator.SetFloat("Direction", sign);
                bobTween = null;
                SpiderBob(sign * Random.Range(bobMax * .8f, bobMax));
            });
        }

        void KillSpiderBob()
        {
            if(bobTween != null)
            {
                bobTween.Kill();
                bobTween = null;
            }
                
            
        }

        void SetState(SpiderState newState)
        {
            if (state == newState) return;
            var oldState = state;
            state = newState;

            switch (newState)
            {
                case SpiderState.Idle:
                    EnterIdleState();
                    break;
                case SpiderState.Attack:
                    EnterAttackState();
                    break;
                case SpiderState.Hidden:
                    EnterHiddenState();
                    break;
                case SpiderState.ChangeTile:
                    EnterChangeTile();
                    break;
            }

            OnChangeState?.Invoke(oldState, newState);
        }
    }
}
