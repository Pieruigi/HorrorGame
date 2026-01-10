using TMM.AI;
using UnityEngine;
using UnityEngine.AI;

namespace TMM
{
	public enum ClownBState { Hidden, Chase, Attack }
	
	public class ClownB : MonoBehaviour
	{
		float checkIdleTime = 10f;

		float maxPlayerDistance = 3; // Number of tiles

		float elapsed = 0;

		float chaseTime = 10f;

		ClownBState state = ClownBState.Hidden;
		public ClownBState State
		{
			get{ return state; }
		}

		//ClownA clownA;

		NavMeshAgent agent;

		PlayerChased playerChased;

        void Awake()
        {
			agent = GetComponent<NavMeshAgent>();
        }

        // Start is called before the first frame update
        void Start()
	    {
			//clownA = FindFirstObjectByType<ClownA>(); // Already exists
			playerChased = FindFirstObjectByType<PlayerChased>();
			EnterHiddenState();
	    }

		// Update is called once per frame
		void Update()
		{
			UpdateState();
		}

		void UpdateState()
		{
			switch (state)
			{
				case ClownBState.Hidden:
					UpdateHiddenState();
					break;
				case ClownBState.Chase:
					UpdateChaseState();
					break;
			}
		}

		void UpdateHiddenState()
		{
			// Don't show up if player is chased
			if (playerChased.IsPlayerChased())
			{
				elapsed = 0;
				return;	
			};

			elapsed += Time.deltaTime;

			if(elapsed > checkIdleTime)
			{
				elapsed = 0;
				if (Random.Range(0, 4) == 0)
					SetState(ClownBState.Chase);
			}

		}

		void UpdateChaseState()
		{
			elapsed += Time.deltaTime;

			if(elapsed > chaseTime)
			{
				SetState(ClownBState.Hidden);
			}
		}



		void EnterHiddenState()
		{
			agent.isStopped = true;
			agent.enabled = false;	
			Debug.Log("TEST - ClownB - EnterHiddeState()");
		}
		
		void EnterChaseState()
		{
			agent.enabled = true;
			agent.isStopped = false;
			elapsed = 0;

			Debug.Log("TEST - ClownB - EnterChaseState()");
		}
		
		void SetState(ClownBState newState)
		{
			if (state == newState) return;
			state = newState;
			switch (state)
			{
				case ClownBState.Hidden:
					EnterHiddenState();
					break;
				case ClownBState.Chase:
					EnterChaseState();
					break;
			}
		}

	}
}
