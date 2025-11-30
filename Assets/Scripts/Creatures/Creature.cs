using System.Collections;
using System.Linq;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

namespace TMM.AI
{
	/// <summary>
    /// Idle: doing nothing
	/// Patrol: wandering around without any specific destination
	/// Search: is looking for something in a specific area of the maze
	/// Chase: chasing you
    /// </summary>
	public enum CreatureState { Idle, Patrol, Search, Chase, Attack }

	public class Creature : MonoBehaviour
	{
		public delegate void StateChangedDelegate(Creature creature, CreatureState oldState, CreatureState newState);
		public static StateChangedDelegate OnStateChanged;

		[SerializeField]
		float walkSpeed = 2.5f;
		public float WalkSpeed
        {
            get{ return walkSpeed; }
        }

		[SerializeField]
		float runSpeed = 3.5f;
		public float RunSpeed
        {
            get{ return runSpeed; }
        }

		[SerializeField]
		float sightRange;

		[SerializeField]
		[Range(0,180)]
		float sightAngle;

		[SerializeField]
		[Range(0,1)]
		float hearMultiplier;

		[SerializeField]
		float smellRange;

		[SerializeField]
		float attackRange;

		[SerializeField]
		[Range(0,180)]
		float attackAngle;


		[SerializeField]
		float idleTimer = 3;
		float currentTimer;

		[SerializeField]
		float searchTimer = 2;
		

		NavMeshAgent agent;

		CreatureState state = CreatureState.Idle;


		bool lastHasPath = false;

		

		

		GameObject player;



		
        protected virtual void Awake()
        {
			agent = GetComponent<NavMeshAgent>();
			
        }

		protected virtual void Start()
        {
			SetState(CreatureState.Patrol);
			player = FindFirstObjectByType<FirstPersonController>().gameObject;
        }


		// Update is called once per frame
		protected virtual void Update()
		{
			UpdateState();

			UpdateLastHasPath();
		}

        void UpdateState()
        {
            switch (state)
			{
				case CreatureState.Idle:
					UpdateIdleState();
					break;
				case CreatureState.Patrol:
					UpdatePatrolState();
					break;
				case CreatureState.Chase:
					UpdateChaseState();
					break;
            }
        }

		void UpdateLastHasPath()
        {
			lastHasPath = agent.hasPath && agent.pathStatus != NavMeshPathStatus.PathInvalid && agent.pathStatus != NavMeshPathStatus.PathPartial;
        }

		public virtual void SetState(CreatureState newState)
		{
			if (state == newState) return;
			var oldState = state;
			state = newState;
			switch (state)
			{
				case CreatureState.Idle:
					EnterIdleState();
					break;
				case CreatureState.Patrol:
					EnterPatrolState();
					break;
				case CreatureState.Chase:
					EnterChaseState();
					break;
				case CreatureState.Search:
					EnterSearchState();
					break;
				case CreatureState.Attack:
					EnterAttackState();
					break;
			}

			OnStateChanged?.Invoke(this, oldState, newState);
		}

		void ResetPath()
        {
			agent.ResetPath();
			lastHasPath = false;
        }

		protected virtual void EnterAttackState()
        {
			ResetPath();
			player.GetComponent<PlayerDeath>().Die(gameObject);
        }

		protected virtual void EnterIdleState()
		{
			ResetPath();
			float ratio = idleTimer * .25f;
			currentTimer = Random.Range(idleTimer - ratio, idleTimer + ratio);
			agent.speed = walkSpeed;

		}

		protected virtual void EnterChaseState()
		{
			ResetPath();
			if (agent.isStopped) agent.isStopped = false;
			agent.speed = runSpeed;
			// Kill any previous coroutine
			StopAllCoroutines();

			// Start chasing player
			StartCoroutine(DoChasePlayer());			
		}

		protected virtual void EnterPatrolState()
		{
			ResetPath();
			agent.speed = walkSpeed;
			if (agent.isStopped) agent.isStopped = false;
		}

		protected virtual void EnterSearchState()
		{
			if (agent.isStopped) agent.isStopped = false;
			agent.speed = runSpeed;
			currentTimer = searchTimer;

			StopAllCoroutines();
			StartCoroutine(DoSearchForPlayer());
        }

		IEnumerator DoChasePlayer()
		{
			while (true)
			{
				// Set player position as destination
				agent.SetDestination(player.transform.position);

				yield return new WaitForSeconds(.5f);
			}

		}
		
		IEnumerator DoSearchForPlayer()
		{
			float time = .5f;
			while (currentTimer > 0)
			{
				agent.SetDestination(player.transform.position);

				yield return new WaitForSeconds(time);
				currentTimer -= time;
			}

			SetState(CreatureState.Idle);		

        }

		protected virtual void UpdateIdleState()
		{
			if (IsPlayerSpotted())
			{
				SetState(CreatureState.Chase);
				return;
			}

			currentTimer -= Time.deltaTime;
			if (currentTimer < 0)
			{
				SetState(CreatureState.Patrol);
			}
		}

		protected virtual void UpdatePatrolState()
		{
            if (IsPlayerSpotted())
            {
				SetState(CreatureState.Chase);
				return;
            }

			if (!lastHasPath && (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial))
			{
				// Try getting a new path
				float minDist = 10;
				// Get all walkable positions
				var list = MazeBuilder.Instance.GetWalkableTilePositions().Where(t => Vector3.Distance(transform.position, t) > minDist).ToList();
				// Get a random position
				var dest = list[Random.Range(0, list.Count)];
				// Set destination
				agent.SetDestination(dest);

				return;
			}


			if (lastHasPath && !agent.hasPath)
			{
				Debug.Log("Destination reached");
				SetState(CreatureState.Idle);
			}


		}



		protected virtual void UpdateChaseState()
		{
			if (!IsPlayerSpotted())
			{
				StopAllCoroutines();
				SetState(CreatureState.Search);
				return;
			}

            if (CanAttackPlayer())
            {
				SetState(CreatureState.Attack);
				return;
            }
		}
		
		

		protected virtual void UpdateSearchState()
		{
            if (IsPlayerSpotted())
            {
				StopAllCoroutines();
				SetState(CreatureState.Chase);
            }

		}



		bool IsPlayerSpotted()
		{
			
			// Get player position
			var playerPosition = player.transform.position;

			// Get direction
			var pDir = playerPosition - transform.position;

			// First we check the smell
			if (pDir.magnitude < smellRange)
			{
				Debug.Log("Creature smelled you");
				return true;
            }
				

			// Then we check the noise
			if (pDir.magnitude < player.GetComponent<FirstPersonController>().NoiseRange * hearMultiplier)
            {
				Debug.Log("Creature heard you");
				return true;
            }
				

			// Check distance 
			if (pDir.magnitude > sightRange)
				return false;

			// Check angle
			var angle = Vector3.Angle(transform.forward, pDir);
			if (angle > sightAngle)
				return false;

			// Raycast from monster to player
			RaycastHit hit;
			var origin = transform.position + Vector3.up;
			if(Physics.Raycast(origin, pDir, out hit, sightRange))
            {
				if (hit.collider.gameObject != player)
					return false;
            }

			Debug.Log("Creature saw you");
			return true;
		}
		
		bool CanAttackPlayer()
		{
			// Check distance
			if (Vector3.Distance(player.transform.position, transform.position) > attackRange)
				return false;

			// Compute direction
			var pDir = player.transform.position - transform.position;
			if (Vector3.Angle(transform.forward, pDir) > attackAngle)
				return false;



			return true;
        }
    }
}
