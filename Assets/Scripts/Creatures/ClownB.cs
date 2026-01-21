using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

namespace TMM
{
	public enum ClownBState { Hidden, Chase, Attack, PreChase }
	
	public class ClownB : MonoBehaviour
	{
        public delegate void StateChangedDelegate(ClownBState oldState, ClownBState newState);
        public static StateChangedDelegate OnStateChanged;

        [SerializeField]
		GameObject model;

		[SerializeField]
		ParticleSystem spawnParticle;

		[SerializeField]
		AudioSource spawnAudioSource;

		[SerializeField]
		AudioSource laughAudioSource;

		ClownAttacker attacker;

		float checkIdleTime = 20f;

		float playerDistance = 4; // Number of tiles

		float elapsed = 0;

		float chaseTime = 10f;

		Vector3 spawnPosition;

		

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

		MiniGame miniGame;



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
            miniGame = FindFirstObjectByType<MiniGame>();
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
				if(Random.Range(0, randomMax) == 0)
				{
					SetState(ClownBState.PreChase);
				}
				else
				{
					if (randomMax > 1) randomMax--;
				}
		
			}

		}

		void UpdateChaseState()
		{
			if(model.activeSelf == false) // Not spawned yet
			{
				return;
            }	

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
			yield return new WaitForSeconds(.5f);

			while (state == ClownBState.Chase)
			{
				agent.SetDestination(playerController.transform.position);

				yield return new WaitForSeconds(.5f);
			}
		}

		void EnterHiddenState()
		{
			StopAllCoroutines();

			elapsed = 0;

			agent.isStopped = true;
			agent.enabled = false;
			randomMax = randomMaxDefault;

			model.transform.DOScale(0.1f, .5f).SetEase(Ease.OutBounce).OnComplete(()=> { model.SetActive(false); });
			
			spawnParticle.Play();
			Debug.Log("TEST - ClownB - EnterHiddeState()");
		}

		void EnterChaseState()
		{
            
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

            spawnAudioSource.Play();

			laughAudioSource.Play();

            StartCoroutine(FollowPlayer());


            Debug.Log("TEST - ClownB - EnterChaseState()");
        }

		void EnterPreChaseState()
		{
            elapsed = 0;
            StopAllCoroutines();

            StartCoroutine(DoGetSpawnPosition());
        }

		IEnumerator DoGetSpawnPosition()
		{
			bool spawnPositionFound = TryGetSpawnPosition(out spawnPosition);
			while(!spawnPositionFound)
			{
				yield return new WaitForSeconds(.5f);

                spawnPositionFound = TryGetSpawnPosition(out spawnPosition);
            }

            // Get spawn position
            SetState(ClownBState.Chase);
        }
		
		bool TryGetSpawnPosition(out Vector3 position)
		{

			position = Vector3.zero;

			if (!miniGame.IsActive)
			{
                // Raycast to find a valid position
                var origin = playerController.transform.position + Vector3.up;
                var direction = playerController.transform.forward;
                var distance = 6f;

                if (!Physics.Raycast(origin, direction, distance))
                {
                    position = origin + direction * distance;
                    position.y = 0;
                    return true;
                }

                return false;
            }
			else
			{
				
				float minDist = 6;
				List<Vector2> validCoords = new List<Vector2>();
                for (int i = 0; i < 3; i++)
				{
					var pos = playerController.transform.position;

                    switch (i)
					{
						case 0: // Right
                            pos += playerController.transform.right * minDist;
							break;
                        case 1: // Back
                            pos -= playerController.transform.forward * minDist;
                            break;
                        case 2: // Left
                            pos -= playerController.transform.right * minDist;
                            break;

                    }

                    var coords = MazeBuilder.Instance.PositionToCoords(pos);
					var type = MazeBuilder.Instance.GetTileType(coords);
                    if(type == 0) // Walkable
                        validCoords.Add(coords);
                }

				if(validCoords.Count > 0)
				{
					var coords = validCoords[Random.Range(0, validCoords.Count)];
                    position = new Vector3(coords.x, 0, coords.y) * MazeBuilder.CellSize;
					return true;
                }

                return false;
            }

                
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
			var oldState = state;
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
				case ClownBState.PreChase:
					EnterPreChaseState();
					break;
			}

			OnStateChanged?.Invoke(oldState, newState);
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
