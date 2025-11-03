using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PSXShadersPro.URP.Demo;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TMM
{
	public class WorkShiftButton : MonoBehaviour
	{
		public static UnityAction OnButtonHit;

		[SerializeField]
		Material interactableMaterial;

		[SerializeField]
		Material notInteractableMaterial;

		[SerializeField]
		Renderer _renderer;

		[SerializeField]
		GameObject outline;

		

		bool interactable = false;

		StarterAssetsInputs input;

		

        void Awake()
        {
			_renderer.material = notInteractableMaterial;
			ShowOutline(false);
        }

        // Start is called before the first frame update
        void Start()
	    {
			input = FindAnyObjectByType<StarterAssetsInputs>();
	    }

		// Update is called once per frame
		void Update()
		{
			if (!interactable) return;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });

			bool showOutline = false;

			if (Physics.Raycast(ray, out hit, FirstPersonController.InteractionDistance, mask))
			{
				Debug.Log("TEST - Collider:" + hit.collider.gameObject);	
				if (hit.collider.gameObject == gameObject)
				{
					// Show outline
					showOutline = true;
				}
				else
				{
					showOutline = false;

				}
				if (input.action)
				{
					Interact();
				}
			}
			else
			{
				Debug.Log("TEST - No Collider");	
				showOutline = false;
			}

			Debug.Log("TEST - Outline:" + showOutline);
			ShowOutline(showOutline && interactable);
		}

		void OnEnable()
		{
			GameplayManager.OnNextShiftReady += HandleOnNextShiftReady;
		}

        void OnDisable()
        {
            GameplayManager.OnNextShiftReady -= HandleOnNextShiftReady;
        }

		private void HandleOnNextShiftReady()
		{
			interactable = true;
			_renderer.material = interactableMaterial;
		}

		void Interact()
		{
			interactable = false;
			_renderer.material = notInteractableMaterial;
			
			GameplayManager.Instance.StartWorkShift();


			OnButtonHit?.Invoke();
		}
		
		void ShowOutline(bool value)
		{
			outline.SetActive(value);
        }
    }
}
