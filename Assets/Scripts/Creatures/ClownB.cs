using System.Collections;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using TMM.AI;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace TMM
{
	public enum ClownBState { Hidden, Chase, Attack }
	
	public class ClownB : MonoBehaviour
	{
		[SerializeField]
		GameObject model;

		[SerializeField]
		ParticleSystem spawnParticle;

		ClownAttacker attacker;

		float checkIdleTime = 30f;

		float playerDistance = 4; // Number of tiles

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

		int randomMax = 4;
		int randomMaxDefault;

		FirstPersonController playerController;



        void Awake()
        {
			agent = GetComponent<NavMeshAgent>();
			randomMaxDefault = randomMax;
			attacker = GetComponent<ClownAttacker>();
        }

        // Start is called before the first frame update
        void Start()
	    {
			playerChased = FindFirstObjectByType<PlayerChased>();
			playerController = FindFirstObjectByType<FirstPersonController>();
			EnterHiddenState();
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			//if (Input.GetKeyDown(KeyCode.X))
			//{
			//	//GetSpawnPosition();
			//	SetState(ClownBState.Chase);
			//}
#endif

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
				if (Random.Range(0, randomMax) == 0)
				{
					SetState(ClownBState.Chase);
				}
				else
				{
					if (randomMax > 3) randomMax--;
				}
		
			}

		}

		void UpdateChaseState()
		{
			if (attacker.CanAttackPlayer())
			{
				SetState(ClownBState.Attack);
				return;
			}

			elapsed += Time.deltaTime;

			if(elapsed > chaseTime)
			{
				SetState(ClownBState.Hidden);
			}
		}

		IEnumerator FollowPlayer()
		{
			yield return new WaitForSeconds(2f);

			while (state == ClownBState.Chase)
			{
				agent.SetDestination(playerController.transform.position);

				yield return new WaitForSeconds(.5f);
			}
		}

		void EnterHiddenState()
		{
			StopAllCoroutines();

			agent.isStopped = true;
			agent.enabled = false;
			randomMax = randomMaxDefault;

			model.transform.DOScale(0.1f, .5f).SetEase(Ease.OutBounce).OnComplete(()=> { model.SetActive(false); });
			
			spawnParticle.Play();
			Debug.Log("TEST - ClownB - EnterHiddeState()");
		}

		void EnterChaseState()
		{
			StopAllCoroutines();

			// Get spawn position
			var spawnPosition = GetSpawnPosition();
			// Move clown
			transform.position = spawnPosition;
			// Rotate clown
			transform.LookAt(playerController.transform.position, Vector3.up);

			agent.enabled = true;
			agent.isStopped = false;
			agent.ResetPath();
			elapsed = 0;
			randomMax = randomMaxDefault;
			model.SetActive(true);

			model.transform.DOScale(1f, .5f).SetEase(Ease.OutBounce);

			spawnParticle.Play();

			StartCoroutine(FollowPlayer());


			Debug.Log("TEST - ClownB - EnterChaseState()");
		}
		
		void EnterAttackState()
		{
			agent.ResetPath();
			agent.isStopped = true;

			attacker.Attack();


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
				case ClownBState.Attack:
					EnterAttackState();
					break;
			}
		}

		Vector3 GetSpawnPosition()
		{
			// Get the player position 
			var playerPosition = playerController.transform.position;
			// Get player forward
			var playerForward = playerController.transform.forward;

			var normPosition = playerPosition / 2f;
			var coords = new Vector2(Mathf.Round(normPosition.x), Mathf.Round(normPosition.z));


			Debug.Log($"TEST - SPAWN - PlayerPosition:{playerPosition}");

			float minDist = 2;

			int tileIndex = MazeBuilder.Instance.GetClosestWalkableTileIndex(playerPosition);

			// Where is the player looking?
			var lookDirection = GetPlayerLookDirection(playerForward);

			// Get all walkable positions
			var walkables = MazeBuilder.Instance.GetWalkableTilePositions();

			Vector3 spawnPosition = new Vector3(coords.x, 0, coords.y);
			switch (lookDirection)
			{
				case 0:
					spawnPosition.z += minDist;
					break;
				case 1:
					spawnPosition.x += minDist;
					break;
				case 2:
					spawnPosition.z -= minDist;
					break;
				case 3:
					spawnPosition.x -= minDist;
					break;
			}

			spawnPosition *= MazeBuilder.CellSize;

			Debug.Log($"TEST - SPAWN - SpawnPosition:{spawnPosition}");

			bool exists = walkables.Exists(w => w == spawnPosition);

			Debug.Log($"TEST - SPAWN - SpawnPosition Exists:{exists}");

			if (!exists)
			{
				// Get any position close enough
				spawnPosition = walkables.Where(w => Vector3.Distance(w, spawnPosition) > minDist).OrderBy(w => Vector3.Distance(w, spawnPosition)).ToList()[0];
				Debug.Log($"TEST - SPAWN - New SpawnPosition:{spawnPosition}");
			}


			return spawnPosition;
		}
		
		int GetPlayerLookDirection(Vector3 playerForward)
		{
			int lookDirection = 0;
			if (Mathf.Abs(playerForward.z) > Mathf.Abs(playerForward.x))
			{
				// Looking forward or backward
				if (playerForward.z > 0)
					lookDirection = 0;
				else
					lookDirection = 2;

			}
			else
			{
				// Looking right or left
				if (playerForward.x > 0)
					lookDirection = 1;
				else
					lookDirection = 2;
			}
			return lookDirection;
		}

	}
}
