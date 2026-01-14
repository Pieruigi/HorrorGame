using System;
using System.Collections;
using System.Collections.Generic;
using TMM.AI;
using UnityEngine;
using UnityEngine.Animations;

namespace TMM
{
	public class ClownAnimationManager : MonoBehaviour
	{
		ClownA clownA;
		ClownB clownB;
        ClownC clownC;

        Animator animator;

		ClownAState clownAState;


        void Awake()
        {
			clownA = GetComponentInParent<ClownA>();
			clownB = GetComponentInParent<ClownB>();
			clownC = GetComponentInParent<ClownC>();
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

        void LateUpdate()
        {
            UpdateAnimatorSpeed();
        }

        void OnEnable()
		{
			if(clownA)
				ClownA.OnStateChanged += HandleOnClownAStateChanged;

			if(clownC)
				ClownC.OnStateChanged += HandleOnClownCStateChanged;
        }

        void OnDisable()
        {
			if(clownA)
				ClownA.OnStateChanged -= HandleOnClownAStateChanged;

            if (clownC)
                ClownC.OnStateChanged -= HandleOnClownCStateChanged;
        }

		private void HandleOnClownCStateChanged(ClownCState oldState, ClownCState newState)
		{
			switch (newState)
			{
				case ClownCState.Hidden:
					//animator.SetFloat("SpeedMul", 1);
					//animator.SetTrigger("Idle");
					break;
				case ClownCState.Chase:
					animator.SetInteger("WalkType", 0);
                    animator.SetFloat("SpeedMul", 1.5f);
                    animator.SetTrigger("Walk");
                    
                    break;
				case ClownCState.Attack:
					animator.SetFloat("SpeedMul", 1);
					animator.SetTrigger("Idle");
					break;
            }

		}

        
		
		private void HandleOnClownAStateChanged(ClownA creature, ClownAState oldState, ClownAState newState)
		{
			clownAState = newState;
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
			if (clownA)
			{
                switch (clownAState)
                {
                    case ClownAState.Chase:
                    case ClownAState.Search:
                        //animator.speed = creature.RunSpeed / creature.WalkSpeed;
                        animator.SetFloat("SpeedMul", clownA.RunSpeed / clownA.WalkSpeed);

                        break;
                }

				return;
            }
            
        }
	}
}
