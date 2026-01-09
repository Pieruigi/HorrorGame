using System.Collections;
using System.Collections.Generic;
using TMM.AI;
using UnityEngine;
using UnityEngine.Animations;

namespace TMM
{
	public class CreatureAnimator : MonoBehaviour
	{
		ClownA creature;

		Animator animator;

		ClownAState state;

        void Awake()
        {
			creature = GetComponentInParent<ClownA>();
			animator = GetComponent<Animator>();
        }


        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			ClownA.OnStateChanged += HandleOnStateChanged;
		}

        void OnDisable()
        {
            ClownA.OnStateChanged -= HandleOnStateChanged;
        }



		void LateUpdate()
		{
			UpdateAnimatorSpeed();
		}
		
		private void HandleOnStateChanged(ClownA creature, ClownAState oldState, ClownAState newState)
		{
			state = newState;
            switch (newState)
            {
				case ClownAState.Idle:
					animator.SetFloat("SpeedMul", 1);
					animator.SetTrigger("Idle");
					break;
				case ClownAState.Patrol:
				case ClownAState.Search:
				case ClownAState.Chase:
					if (oldState == ClownAState.Idle)
					{
						animator.SetInteger("WalkType", 0);
						animator.SetTrigger("Walk");
					}
					if(newState == ClownAState.Patrol)
                    {
						animator.SetFloat("SpeedMul", 1);
                    }
					break;
				case ClownAState.Attack:
					animator.SetFloat("SpeedMul", 1);
					animator.SetTrigger("Idle");
					break;
            }
        }

		void UpdateAnimatorSpeed()
        {
            switch (state)
            {
				case ClownAState.Chase:
				case ClownAState.Search:
					//animator.speed = creature.RunSpeed / creature.WalkSpeed;
					animator.SetFloat("SpeedMul", creature.RunSpeed / creature.WalkSpeed);
					
					break;
		    }
        }
	}
}
