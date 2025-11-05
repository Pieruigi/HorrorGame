using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class InteractionTrigger : MonoBehaviour
	{
		public UnityAction OnInteraction;
		public UnityAction OnEnter;
		public UnityAction OnExit;

		[SerializeField]
		Collider _collider;
		StarterAssetsInputs input;

		bool isHover = false;

        void Awake()
        {
			//_collider = GetComponent<Collider>();
        }

        // Start is called before the first frame update
        void Start()
	    {
			input = FindAnyObjectByType<StarterAssetsInputs>();
	    }

		// Update is called once per frame
		void Update()
		{
			if (_collider.enabled == false) return;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });

			
			if (Physics.Raycast(ray, out hit, FirstPersonController.InteractionDistance, mask))
			{
				Debug.Log("TEST - Collider:" + hit.collider.gameObject);	
				if (hit.collider.gameObject == gameObject)
				{
                    // Show outline
                    if (!isHover)
                    {
						isHover = true;
						OnEnter?.Invoke();
                    }
				}
				else
				{
                    //showOutline = false;
                    if (isHover)
                    {
						isHover = false;
						OnExit?.Invoke();
                    }

				}
				if (input.action)
				{
					OnInteraction?.Invoke();
					//SetInteractable(false);
				}
			}
			else
			{
                if (isHover)
                {
					isHover = false;
					OnExit?.Invoke();
                }
			}
		}

		
		public void SetInteractable(bool value)
		{
			_collider.enabled = value;

			if (!value)
			{
				if (isHover)
				{
					isHover = false;
					OnExit?.Invoke();
				}
			}
		}

		public bool IsInteractable()
		{
			return _collider.enabled = true;
		}
		

		
		
	}
}
