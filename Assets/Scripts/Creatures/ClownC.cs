using StarterAssets;
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

        public delegate void StateChangedDelegate(ClownCState oldState, ClownCState newState);
        public static StateChangedDelegate OnStateChanged;

        [SerializeField]
        GameObject model;

        ClownCState state;
        public ClownCState State { get { return state; } }

		FirstPersonController player;
        PlayerChased playerChased;
        ClownA clownA;
        ClownB clownB;

       
        float elapsed;
       
        float chaseCheckTimer = 2f;

        NavMeshAgent agent;

        ClownAttacker attacker;
        CharacterController characterController;

        float speedDefault;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            attacker = GetComponent<ClownAttacker>();
            
            state = ClownCState.Hidden;

            chaseCheckTimer -= (chaseCheckTimer * .1f * GameManager.Instance.Level);

            agent.speed += (agent.speed * .05f * GameManager.Instance.Level);
            speedDefault = agent.speed;


        }

        // Start is called before the first frame update
        void Start()
	    {
            // Check speed
            agent.speed = speedDefault * (StupidClownBuff.Instance.IsActive ? StupidClownBuff.Instance.SpeedMultiplier : 1f);

            EnterHiddenState();
        }

	    // Update is called once per frame
	    void Update()
	    {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    //GetSpawnPosition();
            //    Time.timeScale = 1;
            //}
#endif

            UpdateState();
	    }


        private void OnEnable()
        {
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
            AlarmManager.OnActivated += HandleOnAlarmActivated;
            AlarmManager.OnDeactivated += HandleOnAlarmDeactivated;
            TimedBuffDebuff.OnApplied += HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired += HandleOnDeBuffExpired;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
            AlarmManager.OnActivated -= HandleOnAlarmActivated;
            AlarmManager.OnDeactivated -= HandleOnAlarmDeactivated;
            TimedBuffDebuff.OnApplied -= HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired -= HandleOnDeBuffExpired;
        }

        private void HandleOnDeBuffApplied(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                agent.speed = speedDefault * (arg0 as StupidClownBuff).SpeedMultiplier;
                return;
            }
        }

        private void HandleOnDeBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                agent.speed = speedDefault;
                return;
            }
        }

        private void HandleOnAlarmActivated()
        {
            chaseCheckTimer = 2;// 8;
        }

        private void HandleOnAlarmDeactivated()
        {
            chaseCheckTimer = 2;
        }

        private void HandleOnMazeCreated()
        {
            player = FindFirstObjectByType<FirstPersonController>();
            characterController = player.GetComponent<CharacterController>();
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
            if(clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search) // ClownA only  // || clownB?.State == ClownBState.Chase)
            {
                elapsed += Time.deltaTime;
                if(elapsed >= chaseCheckTimer)
                {
                    elapsed = 0;
                    if (Random.Range(0, 4) == 0)
                        SetState(ClownCState.Chase);
                }
                
                    
            }
            else
            {
                elapsed = 0;
            }
        }

        void UpdateChaseState()
        {
            if (attacker.CanAttackPlayer())
            {
                SetState(ClownCState.Attack);
                return;
            }

          
            
        }

        void EnterHiddenState()
        {
            StopAllCoroutines();
            // Disable ClownC's model to hide it
            model.SetActive(false);

            agent.ResetPath();
            agent.isStopped = true;
            


            elapsed = 0;

        }

        void EnterAttackState()
        {
            StopAllCoroutines();
            agent.ResetPath();
            agent.isStopped = true;
            attacker.Attack();
        }

        void EnterChaseState()
        {
            StopAllCoroutines();

            model.SetActive(true);

            float scale = 1f;
            if (StupidClownBuff.Instance.IsActive)
                scale *= StupidClownBuff.Instance.ScaleMultiplier;

            model.transform.localScale = Vector3.one * scale;

            elapsed = 0;


            // Move ClownC to spawn position
            if(TryGetSpawnPosition(out Vector3 spawnPosition))
            {
                transform.position = spawnPosition;
                transform.forward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized;

                agent.ResetPath();
                agent.isStopped = false;

               

                StartCoroutine(ChasePlayer());
#if UNITY_EDITOR
                //Time.timeScale = 0;
#endif

            }
            else
            {
                SetState(ClownCState.Hidden);
            }

        }

        bool TryGetSpawnPosition(out Vector3 spawnPosition)
        {
            spawnPosition = Vector3.zero;
            // Get other clowns' positions
            // Let's try with clownA
            //Vector3 otherPosition = Vector3.zero;
            //if (clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search) // ClownA is chasing the player
            //{
            //    otherPosition = clownA.transform.position;
            //}
            //else
            //{
            //    if (clownB.State == ClownBState.Chase) // ClownB is chasing the player
            //    {
            //        otherPosition = clownB.transform.position;
            //    }
            //    else
            //    {
            //        // No other clown is chasing the player
            //        return false;
            //    }
            //}

            // Get player position
            var playerPosition = player.transform.position;

            int playerTileIndex = MazeBuilder.Instance.GetClosestWalkableTileIndex(playerPosition);
            // No tile found
            if (playerTileIndex < 0)
                return false;

           
            // Get the player move direction
            var moveDirection = characterController.velocity;
            
            
            moveDirection.y = 0;
            if(moveDirection.magnitude < 0.01f)
                return false;
            
            moveDirection.Normalize();

         
            //if (Mathf.Abs(moveDirection.z) - Mathf.Abs(moveDirection.x) < 0.2f)
            //    return false; // Diagonal movement, skip

            
            int dir = 0;
            if(Mathf.Abs(moveDirection.z) > Mathf.Abs(moveDirection.x)) // North or south
            {
                if (moveDirection.z > 0)
                    dir = 0;
                else
                    dir = 2;
            }
            else // East or west
            {
                if(moveDirection.x > 0)
                    dir = 1;
                else
                    dir = 3;
            }

            // Try get a spawn tile
            var playerTileCoords = MazeBuilder.Instance.GetTileCoords(playerTileIndex);
            List<Vector2> newCoords = new List<Vector2>();
            

            switch (dir)
            {
                case 0:
                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 3) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2) != 0))
                        newCoords.Add(playerTileCoords + Vector2.up * 3);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 3 + Vector2.right) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up + Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2 + Vector2.right) != 0))
                        newCoords.Add(playerTileCoords + Vector2.up * 3 + Vector2.right);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 3 - Vector2.right) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up - Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2 - Vector2.right) != 0))
                        newCoords.Add(playerTileCoords + Vector2.up * 3 - Vector2.right);


                    break;
                case 1:
                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 3) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2) != 0))
                        newCoords.Add(playerTileCoords + Vector2.right * 3);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 3 + Vector2.up) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right + Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2 + Vector2.up) != 0))
                        newCoords.Add(playerTileCoords + Vector2.right * 3 + Vector2.up);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 3 - Vector2.up) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right - Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2 - Vector2.up) != 0))
                        newCoords.Add(playerTileCoords + Vector2.right * 3 - Vector2.up);
                    break;
                case 2:
                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 3) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2) != 0))
                        newCoords.Add(playerTileCoords - Vector2.up * 3);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 3 + Vector2.right) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up + Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2 + Vector2.right) != 0))
                        newCoords.Add(playerTileCoords - Vector2.up * 3 + Vector2.right);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 3 - Vector2.right) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up - Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2 - Vector2.right) != 0))
                        newCoords.Add(playerTileCoords - Vector2.up * 3 - Vector2.right);
                    break;
                case 3:
                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 3) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2) != 0))
                        newCoords.Add(playerTileCoords - Vector2.right * 3);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 3 + Vector2.up) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right + Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2 + Vector2.up) != 0))
                        newCoords.Add(playerTileCoords - Vector2.right * 3 + Vector2.up);

                    if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 3 - Vector2.up) == 0 && (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right - Vector2.up) != 0 || MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2 - Vector2.up) != 0))
                        newCoords.Add(playerTileCoords - Vector2.right * 3 - Vector2.up);
                    break;

            }

            //if(newCoords.Count == 0)
            //{
            //    switch (dir)
            //    {
            //        case 0:
            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.up * 2);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2 + Vector2.right) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up + Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.up * 2 + Vector2.right);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up * 2 - Vector2.right) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.up - Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.up * 2 - Vector2.right);


            //            break;
            //        case 1:
            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.right * 2);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2 + Vector2.up) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right + Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.right * 2 + Vector2.up);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right * 2 - Vector2.up) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords + Vector2.right - Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords + Vector2.right * 2 - Vector2.up);
            //            break;
            //        case 2:
            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.up * 2);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2 + Vector2.right) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up + Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.up * 2 + Vector2.right);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up * 2 - Vector2.right) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.up - Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.up * 2 - Vector2.right);
            //            break;
            //        case 3:
            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.right * 2);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2 + Vector2.up) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right + Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.right * 2 + Vector2.up);

            //            if (MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right * 2 - Vector2.up) == 0 && MazeBuilder.Instance.GetTileType(playerTileCoords - Vector2.right - Vector2.up) != 0)
            //                newCoords.Add(playerTileCoords - Vector2.right * 2 - Vector2.up);
            //            break;

            //    }
            //}

            bool found = false;
            int spawnTileIndex = -1;
           
            while (newCoords.Count > 0 && !found)
            {
                var coords = newCoords[Random.Range(0, newCoords.Count)];
                newCoords.Remove(coords);
                spawnTileIndex = MazeBuilder.Instance.GetTileIndex(coords);
                if(spawnTileIndex >= 0)
                    found = true;
            }

            if (!found)
                return false;
         
         
            var spawnCoords = MazeBuilder.Instance.GetTileCoords(spawnTileIndex);
            spawnPosition = new Vector3(spawnCoords.x, 0, spawnCoords.y) * MazeBuilder.CellSize;
     
            return true;
        }

      

        IEnumerator ChasePlayer()
        {
            int counter = 10;
            while(state == ClownCState.Chase)
            {
                agent.SetDestination(player.transform.position);

                yield return new WaitForSeconds(0.5f);

                counter--;
                if(counter< 0) 
                    SetState(ClownCState.Hidden);


            }
        }

        void SetState(ClownCState newState)
        {
            if (newState == state) return;

         
            var oldState = state;
            state = newState;

            switch (state)
            {
                case ClownCState.Hidden:
                    EnterHiddenState();
                    break;
                case ClownCState.Chase:
                    EnterChaseState();
                    break;
                case ClownCState.Attack:
                    EnterAttackState();
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);
        }
    }
}
