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
			if(other.CompareTag("Player"))
				OnEnter?.Invoke(other);
		}

		void OnTriggerExit(Collider other)
		{
			if(other.CompareTag("Player"))
				OnExit?.Invoke(other);
		}
		
		public void SetEnabled(bool value)
        {
			//disabled = !value;
			GetComponent<Collider>().enabled = value;
        }
    }
}
