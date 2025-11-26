using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TMM.AI
{
	public enum CreatureState { Idle, Patrol, Chase }

	public class Creature : MonoBehaviour
	{
		public delegate void StateChangedDelegate(Creature creature);
		public static StateChangedDelegate OnStateChanged;

		[SerializeField]
		float sightRange;

		[SerializeField]
		float hearRange;

		NavMeshAgent agent;

		CreatureState state = CreatureState.Patrol;

        protected virtual void Awake()
        {
			agent = GetComponent<NavMeshAgent>();

        }

        
		// Update is called once per frame
		protected virtual void Update()
		{
			UpdateState();
		}

        void UpdateState()
        {
            switch (state)
            {
				case CreatureState.Patrol:
					UpdatePatrolState();
					break;
				case CreatureState.Chase:
					UpdateChaseState();
					break;
            }
        }

        public virtual void SetState(CreatureState state)
        {
			this.state = state;
			switch (state)
			{
				case CreatureState.Patrol:
					EnterPatrolState();
					break;
				case CreatureState.Chase:
					EnterChaseState();
					break;
			}

			OnStateChanged?.Invoke(this);
        }

        protected virtual void EnterChaseState()
        {
            throw new NotImplementedException();
        }

		protected virtual void EnterPatrolState()
		{
			if (agent.isStopped) agent.isStopped = false;
		}

		protected virtual void UpdatePatrolState()
		{
			
		}
		
		protected virtual void UpdateChaseState()
        {
            
        }
    }
}
