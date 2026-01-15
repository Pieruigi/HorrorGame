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

		public delegate void EnterDelegate(DeviceInteractor deviceInteractor);
		public static EnterDelegate OnEnter;

		public delegate void ExitDelegate(DeviceInteractor deviceInteractor);
		public static ExitDelegate OnExit;

        [SerializeField]
		ActivationTrigger activationTrigger;

		[SerializeField]
		Collider interactionCollider;

		[SerializeField]
		bool mouseButton0 = true;

		[SerializeField]
		KeyCode key = KeyCode.None;

		[SerializeField]
		int messageId = -1;

		bool inside = false;

		bool lastShowMessage = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			bool showMessage = false;
			if (inside)
			{
				RaycastHit hit;
				LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });
				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, FirstPersonController.InteractionDistance, mask))
				{
					
					if (hit.collider == interactionCollider)
					{
						// Show message if any
						showMessage = true; 
						// Check interaction
						if((mouseButton0 && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(key))
							OnInteraction?.Invoke(this);	
					}
					
				}
			}
			

			if(showMessage != lastShowMessage)
			{
				if (showMessage)
				{
                    MessageManager.Instance.ShowCustomMessage(messageId, true);
                    OnEnter?.Invoke(this);	
                }
				else
				{
                    MessageManager.Instance.HideMessage();
					OnExit?.Invoke(this);
                }
					
			}

			lastShowMessage = showMessage;
			
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
		
		public void SetInteractionCollider(Collider collider)
		{
			interactionCollider = collider;
		}
    }
}
