using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class NoObstacleAvoidance : MonoBehaviour
	{
        private void Awake()
        {
            var navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (navMeshAgent != null)
			{
				navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
