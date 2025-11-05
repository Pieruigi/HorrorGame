using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace TMM
{
	public class ActivationTrigger : MonoBehaviour
	{
		public delegate void EnterDelegate(Collider other);
		public delegate void ExitDelegate(Collider other);
		public EnterDelegate OnEnter;
		public ExitDelegate OnExit;

		
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnTriggerEnter(Collider other)
		{
			OnEnter?.Invoke(other);
		}

		void OnTriggerExit(Collider other)
		{
			OnExit?.Invoke(other);
		}
		
		
    }
}
