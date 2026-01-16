using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
    public enum SpiderState {Hidden, Idle, ChangingTile, Attack }

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

        // Start is called before the first frame update
        void Start()
	    {
#if UNITY_EDITOR
            if(_testTile)
            {
                playerController = FindFirstObjectByType<FirstPersonController>();
                flashlight = FindFirstObjectByType<Flashlight>();
                currentTileIndex = 0;
                currentTileTransform = _testTile;
                SetState(SpiderState.Idle);
                return;
            }
            
#endif

            currentTileIndex = GetNextTileIndex();
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

            tileIndices = MazeBuilder.Instance.GetTileWithLightIndices();
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
            var bestDir = cardDirs.OrderBy(d=>Vector3.Dot(d, direction)).ToList()[0];
            bestDir *= -1;

            Debug.Log("TEST - Spider - Best dir: " + bestDir);

            //if (!flashlight.IsOn())
            {
                // Look at the player
                Quaternion targetRotation = Quaternion.LookRotation(currentTileTransform.position + 20 * Vector3.up, bestDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

            }

            //transform.LookAt(transform.position+Vector3.up, Vector3.right);
        }

        void EnterIdleState()
        {
            StopAllCoroutines();

            // Move the spider to the current tile
            transform.position = currentTileTransform.position + Vector3.up * 4;
            // Set spider orientation
            transform.eulerAngles = Vector3.right * 90f;

            model.transform.localPosition = Vector3.up * modelUpDistance;
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
            }

            OnChangeState?.Invoke(oldState, newState);
        }
    }
}
