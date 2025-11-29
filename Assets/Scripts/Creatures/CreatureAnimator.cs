using System.Collections;
using System.Collections.Generic;
using TMM.AI;
using UnityEngine;
using UnityEngine.Animations;

namespace TMM
{
	public class CreatureAnimator : MonoBehaviour
	{
		Creature creature;

		Animator animator;

		CreatureState state;

        void Awake()
        {
			creature = GetComponentInParent<Creature>();
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
			Creature.OnStateChanged += HandleOnStateChanged;
		}

        void OnDisable()
        {
            Creature.OnStateChanged -= HandleOnStateChanged;
        }



		void LateUpdate()
		{
			UpdateAnimatorSpeed();

		}
		
		private void HandleOnStateChanged(Creature creature, CreatureState oldState, CreatureState newState)
		{
			state = newState;
            switch (newState)
            {
				case CreatureState.Idle:
					animator.speed = 1;
					animator.SetTrigger("Idle");
					break;
				case CreatureState.Patrol:
				case CreatureState.Search:
				case CreatureState.Chase:
					if (oldState == CreatureState.Idle)
					{
						animator.SetInteger("WalkType", 0);
						animator.SetTrigger("Walk");
					}
					if(newState == CreatureState.Patrol)
                    {
						animator.speed = 1;
                    }
					break;
            }
        }

		void UpdateAnimatorSpeed()
        {
            switch (state)
            {
				case CreatureState.Chase:
				case CreatureState.Search:
					animator.speed = creature.RunSpeed / creature.WalkSpeed;
					break;
		    }
        }
	}
}
