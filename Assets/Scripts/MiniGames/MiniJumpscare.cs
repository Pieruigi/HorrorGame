using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class MiniJumpscare : MonoBehaviour
	{
        [SerializeField]
        MiniGame miniGame;

        [SerializeField]
        GameObject scaryFish;

        bool alreadyUsed = false;


        private void Awake()
        {
            scaryFish.SetActive(false);


        }

        // Start is called before the first frame update
        void Start()
	    {

        }

        // Update is called once per frame
        void Update()
	    {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Play();
            //}
#endif
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
            if (Random.Range(0, 5) == 0
#if UNITY_EDITOR
                //|| true
#endif
                )
                miniGame.InitMiniJumpscare(this);
        }

        public void Play()
		{
            if(alreadyUsed) return;
            alreadyUsed = true;

		    // Get camera shake
            CameraShake shake = Camera.main.transform.root.GetComponentInChildren<CameraShake>();
            shake.PlayLetterboxJumpScare();
            JumpscareManager.Instance.PlayAudio();
        
            // Save the original parent
            var parent = transform.parent;

            // Move the scary fish to the bottom of the camera
            Transform target = shake.transform.parent;
            var origPos = target.up * -1f;
            scaryFish.transform.parent = target;
            scaryFish.transform.localPosition = origPos;
            scaryFish.transform.localRotation = Quaternion.identity;
            // Set active
            scaryFish.SetActive(true);

            // Move the scary fish
            var seq = DOTween.Sequence();
            seq.Append(scaryFish.transform.DOLocalMove(Vector3.zero, .1f));
            seq.AppendInterval(1f);
            seq.Append(scaryFish.transform.DOLocalMove(origPos, .1f));

            seq.OnComplete(() => { transform.parent = parent; scaryFish.SetActive(false); });

        }


	}
}
