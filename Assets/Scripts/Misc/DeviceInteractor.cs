using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class DeviceInteractor : MonoBehaviour
	{
		public delegate void InteractionDelegate(DeviceInteractor deviceInteractor);
		public static InteractionDelegate OnInteraction;

		[SerializeField]
		ActivationTrigger activationTrigger;

		[SerializeField]
		Collider interactionCollider;

		bool inside = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
            if (inside)
            {
            	RaycastHit hit;
				LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });
				if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, FirstPersonController.InteractionDistance, mask))
				{
				    if (hit.collider == interactionCollider && Input.GetMouseButtonDown(0))
                    {
						OnInteraction?.Invoke(this);
                    }
                }    
            }

			
		}

		void OnEnable()
		{
			activationTrigger.OnEnter += HandleOnEnter;
			activationTrigger.OnExit += HandleOnExit;
		}

        void OnDisable()
        {
			activationTrigger.OnEnter -= HandleOnEnter;
			activationTrigger.OnExit -= HandleOnExit;
        }

        private void HandleOnEnter(Collider other)
        {
			inside = true;
        }

		private void HandleOnExit(Collider other)
		{
			inside = false;
		}
		
		public void SetEnable(bool value)
        {
			interactionCollider.enabled = value;
        }
    }
}
