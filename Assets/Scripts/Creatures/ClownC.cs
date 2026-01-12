using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMM.AI;
using UnityEngine;
using UnityEngine.AI;

namespace TMM
{
	public enum ClownCState { Hidden, Chase, Attack }

	public class ClownC : MonoBehaviour
	{

        [SerializeField]
        GameObject model;

        ClownCState state;

		FirstPersonController player;
        PlayerChased playerChased;
        ClownA clownA;
        ClownB clownB;

        float soloChaseTimer = 5;
        float elapsed;
        bool solo = false;

        NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            state = ClownCState.Hidden;
            EnterHiddenState();


        }

        // Start is called before the first frame update
        void Start()
	    {
			    
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
            player = FindFirstObjectByType<FirstPersonController>();
            clownA = FindFirstObjectByType<ClownA>();
            clownB = FindFirstObjectByType<ClownB>();

        }

        void UpdateState()
        {
            switch (state)
            {
                case ClownCState.Hidden:
                    UpdateHiddenState();
                    break;
                case ClownCState.Chase:
                    UpdateChaseState();
                    break;
            }
        }

        void UpdateHiddenState()
        {
            if(clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search || clownB?.State == ClownBState.Chase)
            {
                SetState(ClownCState.Chase);
            }
        }

        void UpdateChaseState()
        {
            var oldSolo = solo;
            solo = !(clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search || clownB?.State == ClownBState.Chase);
            if (oldSolo != solo)
            {
                elapsed = 0;
            }

            if(solo)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= soloChaseTimer)
                {
                    elapsed = 0;
                    SetState(ClownCState.Hidden);
                }
            }
        }

        void EnterHiddenState()
        {
            // Disable ClownC's model to hide it
            model.SetActive(false);

            agent.ResetPath();
            agent.isStopped = true;
            


            elapsed = 0;

        }

        void EnterChaseState()
        {
            model.SetActive(true);

            agent.ResetPath();
            agent.isStopped = false;

            elapsed = 0;

            

            // Get other clowns' positions
            // Let's try with clownA
            Vector3 otherPosition = Vector3.zero;
            if (clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search) // ClownA is chasing the player
            {
                otherPosition = clownA.transform.position;
            }
            else 
            {
                if(clownB.State == ClownBState.Chase) // ClownB is chasing the player
                {
                    otherPosition = clownB.transform.position;
                }
                else
                {
                    solo = true; // You should not be solo at this point
                }
            }


            // Get player position
            var playerPosition = player.transform.position;

            // Get direction from other clown to player
            var directionToPlayer = (playerPosition - otherPosition).normalized;

            // We want the clown to cut the player off so we must calculate spawn position in front of player along the direction from other clown to player
            var spawnDistanceFromPlayer = 10f; // Distance in front of player to spawn

            var allPositions = MazeBuilder.Instance.GetWalkableTilePositions();



            // Get spawn position away from player and other clowns and in front of player in order to cut them off
            //var spawnPosition = 
        }

        void SetState(ClownCState newState)
        {
            if (newState == state) return;

            Debug.Log($"TEST - ClownC - Setting state:{newState}");

            state = newState;

            switch (state)
            {
                case ClownCState.Hidden:
                    EnterHiddenState();
                    break;
                case ClownCState.Chase:
                    EnterChaseState();
                    break;
            }
        }
    }
}
