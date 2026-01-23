using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TMM.AI
{
	public enum PigState
	{
		Idle,
		Chasing,
    }

    public class Pig : MonoBehaviour
	{
		PigState state = PigState.Idle;
		public PigState State => state;

        NavMeshAgent agent;

		FirstPersonController player;

		float range = 4f;

		[SerializeField]
		Animator animator;

		[SerializeField]
		AudioSource gruntAudioSource;

		[SerializeField]
		AudioSource oinkAudioSource;

		float idleSpeed = 1;

		float chaseSpeed = 3;

        ClownA clownA;

		void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			agent.speed = chaseSpeed;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        player = FindFirstObjectByType<FirstPersonController>();

			EnterIdleState();
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            MazeBuilder.OnMazeCreated += MazeBuilder_OnMazeCreated;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= MazeBuilder_OnMazeCreated;
        }

        private void MazeBuilder_OnMazeCreated()
        {
            clownA = FindFirstObjectByType<ClownA>();
        }

        void SetState(PigState newState)
		{
			if(state == newState) return;

			var oldState = state;
			state = newState;

			switch (state)
			{
				case PigState.Idle:
					EnterIdleState();
					break;
				case PigState.Chasing:
					EnterChasingState();
					break;
            }
        }

        private void EnterChasingState()
        {
            StopAllCoroutines();

			StartCoroutine(UpdateChasingState());

			StartCoroutine(PlayOinkSound());
        }

        private void EnterIdleState()
        {
			StopAllCoroutines();

			agent.ResetPath();

            StartCoroutine(UpdateIdleState());

            StartCoroutine(PlayGruntSound());
        }

		IEnumerator PlayOinkSound()
		{
			while (state == PigState.Chasing)
			{
				oinkAudioSource.Play();
				yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));
            }
        }

        IEnumerator PlayGruntSound()
        {
            while (state == PigState.Idle)
            {
                gruntAudioSource.Play();
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));
            }
        }

        IEnumerator UpdateChasingState()
		{

            

            animator.SetTrigger("Walk");

            while (state == PigState.Chasing)
			{
                // Check player distance	
                var playerDistance = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).magnitude;
                if (playerDistance > range * 1.2f)
                {
                    SetState(PigState.Idle); // Reset pig state to idle
                    yield break;
                }

                // Move towards player
                agent.SetDestination(player.transform.position);

                // Move clownA towards pig
                clownA.ForcePatrol(transform.position); 

                // Wait a bit before next update
                yield return new WaitForSeconds(.2f);
            }
        }


        IEnumerator UpdateIdleState()
		{
			if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                animator.SetTrigger("Idle");

			float minTime = 10f;
			float maxTime = 20f;
            float idleTimer = UnityEngine.Random.Range(minTime, maxTime);
			float elapsedTime = 0f;
			float waitTime = .5f;

            while (state == PigState.Idle)
			{
                // Check player distace
				var playerDistance = Vector3.ProjectOnPlane(player.transform.position-transform.position, Vector3.up).magnitude;
				Debug.Log("TEST - PIG - PlayerDistance:" + playerDistance);
                if (playerDistance < range)
                {
					SetState(PigState.Chasing); // Set pig state to chasing
                    yield break;
                }

				elapsedTime += waitTime;
				if(elapsedTime >= idleTimer)
				{

					elapsedTime = 0f;
                }


                // Wait a bit before next update
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
