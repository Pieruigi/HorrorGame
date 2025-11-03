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

		StarterAssetsInputs input;

		[SerializeField]
		InteractionTrigger trigger;

		

        void Awake()
        {
			_renderer.material = notInteractableMaterial;
			trigger.SetInteractable(false);
			ShowOutline(false);
        }

		// Start is called before the first frame update
		void Start()
		{
			input = FindAnyObjectByType<StarterAssetsInputs>();
		}

        void Update()
        {
            
        }


        void OnEnable()
		{
			GameplayManager.OnNextShiftReady += HandleOnNextShiftReady;
			trigger.OnEnter += HandleOnTriggerEnter;
			trigger.OnExit += HandleOnTriggerExit;
			trigger.OnInteraction += HandleOnInteraction;
		}

        void OnDisable()
        {
			GameplayManager.OnNextShiftReady -= HandleOnNextShiftReady;
			trigger.OnEnter -= HandleOnTriggerEnter;
			trigger.OnExit -= HandleOnTriggerExit;
			trigger.OnInteraction -= HandleOnInteraction;
        }

        private void HandleOnInteraction()
        {
			Interact();
        }

        private void HandleOnTriggerExit()
        {
			ShowOutline(false);
        }

        private void HandleOnTriggerEnter()
        {
			ShowOutline(true);
        }

        private void HandleOnNextShiftReady()
		{
			trigger.SetInteractable(true);
			_renderer.material = interactableMaterial;
		}

		void Interact()
		{
			_renderer.material = notInteractableMaterial;
			trigger.SetInteractable(false);
			GameplayManager.Instance.StartWorkShift();
			OnButtonHit?.Invoke();
		}
		
		void ShowOutline(bool value)
		{
			outline.SetActive(value);
        }
    }
}
