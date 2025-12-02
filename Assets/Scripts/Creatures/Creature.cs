using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using TMM.Interfaces;
using TMPro;
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

		public delegate void PlayerSpottedDelegate(Creature creature, bool spotted);
		public static PlayerSpottedDelegate OnPlayerSpotted;

		[SerializeField]
		float walkSpeed = 2.5f;
		public float WalkSpeed
		{
			get { return walkSpeed; }
		}

		[SerializeField]
		float runSpeed = 3.5f;
		public float RunSpeed
		{
			get { return runSpeed; }
		}

		[SerializeField]
		float sightRange;

		[SerializeField]
		[Range(0, 180)]
		float sightAngle;

		[SerializeField]
		[Range(0, 1)]
		float hearMultiplier;

		[SerializeField]
		float smellRange;

		[SerializeField]
		float attackRange;

		[SerializeField]
		[Range(0, 180)]
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

		List<INoiser> noisers = new List<INoiser>();

		Vector3 forcedDestination;
		bool useForcedDestination;

		Transform target;

		Flashlight flashlight;



		protected virtual void Awake()
		{
			agent = GetComponent<NavMeshAgent>();

#if UNITY_EDITOR
			walkSpeed *= 0.75f; // Max 1.75
			runSpeed *= 0.75f; // Max 1.75
#endif

		}

		protected virtual void Start()
		{
			SetState(CreatureState.Patrol);
			player = FindFirstObjectByType<FirstPersonController>().gameObject;

			noisers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<INoiser>().ToList();
			flashlight = player.transform.parent.GetComponentInChildren<Flashlight>();

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
			StartCoroutine(DoChaseTarget());
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

		protected virtual void EnterForceDestination()
        {
			if (agent.isStopped) agent.isStopped = false;
			agent.speed = runSpeed;

			StopAllCoroutines();
			agent.SetDestination(forcedDestination);
        }

		IEnumerator DoChaseTarget()
		{
			while (true)
			{
				// Set player position as destination
				agent.SetDestination(target.position);

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
			if (IsTargetSpotted())
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
			if (IsTargetSpotted())
			{
				SetState(CreatureState.Chase);
				return;
			}

			if (!lastHasPath && (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial))
			{
				// Try getting a new path
				float minDist = 10;
				// Get all walkable positions
				List<Vector3> list = new List<Vector3>();
				if (useForcedDestination)
				{
					useForcedDestination = false;
					list.Add(forcedDestination);
				}
				else
				{
                	list = MazeBuilder.Instance.GetWalkableTilePositions().Where(t => Vector3.Distance(transform.position, t) > minDist).ToList();    
                }
				
				// Get a random position
				var dest = list[Random.Range(0, list.Count)];
				// Set destination
				agent.SetDestination(dest);
				Debug.Log("Monster set destination");

				return;
			}


			if (lastHasPath && !agent.hasPath)
			{
				SetState(CreatureState.Idle);
			}


		}



		protected virtual void UpdateChaseState()
		{
			if (!IsTargetSpotted())
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
			if (IsTargetSpotted())
			{
				StopAllCoroutines();
				SetState(CreatureState.Chase);
			}

		}


		protected virtual void UpdateForceDestination()
		{
			if (IsTargetSpotted())
			{
				SetState(CreatureState.Chase);
				return;
			}

            if (lastHasPath && !agent.hasPath)
			{
				SetState(CreatureState.Idle);
			}
        }



		bool IsTargetSpotted()
		{

			// Get player position
			var playerPosition = player.transform.position;

			// Get direction
			var pDir = playerPosition - transform.position;

			// First we check the smell
			if (pDir.magnitude < smellRange)
			{
				Debug.Log("Creature smelled you");
				target = player.transform;
				OnPlayerSpotted?.Invoke(this, true);
				return true;
			}



			// Then we check the noise
			// Get the closest noiser (since other noisers could hide the player noise, we check what's the most noisy object)
			var _noisers = noisers.OrderBy(n => n.GetTargetDistance(transform.position) - n.GetNoiseRange()).ToList();
			Debug.Log("Noisers[0]:" + (_noisers[0] as MonoBehaviour).gameObject.name);
			if (/*noisers[0] == player.GetComponent<INoiser>() && */pDir.magnitude < _noisers[0].GetNoiseRange() * hearMultiplier)
			{
				Debug.Log("Creature heard you");
				//target = player.transform;
				target = (_noisers[0] as MonoBehaviour).transform;
				OnPlayerSpotted?.Invoke(this, true);
				return true;
			}


			// Check distance 
			var range = (flashlight.IsOn() ? 1.5f : 1) * sightRange;
			if (pDir.magnitude > range)
			{
				OnPlayerSpotted?.Invoke(this, false);
				return false;
			}


			// Check angle
			var angle = Vector3.Angle(transform.forward, pDir);
			if (angle > sightAngle)
			{
				OnPlayerSpotted?.Invoke(this, false);
				return false;
			}


			// Raycast from monster to player
			RaycastHit hit;
			var origin = transform.position + Vector3.up;
			if (Physics.Raycast(origin, pDir, out hit, range))
			{
				if (hit.collider.gameObject != player)
				{
					OnPlayerSpotted?.Invoke(this, false);
					return false;
				}

			}

			Debug.Log("Creature saw you");
			target = player.transform;
			OnPlayerSpotted?.Invoke(this, true);
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


		public void ForcePatrol(Vector3 destination)
        {
			if (state != CreatureState.Patrol && state != CreatureState.Idle) return;

			// If idle we must tell the patrol update routine to use the given destination
			if (state == CreatureState.Idle)
			{
				forcedDestination = destination;
				useForcedDestination = true;
				SetState(CreatureState.Patrol);
			}
            else // If patrol state just switch the destination to the given one
            {
                agent.SetDestination(destination);
            }
        }


	}
}
