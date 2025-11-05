using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class WorkShiftTrigger : MonoBehaviour
	{
		[SerializeField]
		GameObject bag;
		
		[SerializeField]
		GameObject map;

		[SerializeField]
		GameObject flashlight;

		// [SerializeField]
		// GameObject bagOutline;

		// [SerializeField]
		// GameObject flashlightOutline;

		// [SerializeField]
		// GameObject mapOutline;

		

		//bool interactable = false;
		[SerializeField]
		InteractionTrigger trigger;

        void Awake()
		{
			trigger.SetInteractable(false);
			flashlight.SetActive(false);

			// Hide outlines
			// mapOutline.SetActive(false);
			// flashlightOutline.SetActive(false);
			// bagOutline.SetActive(false);
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
			WorkShiftGroup.OnMovedUp += HandleOnGroupMovedUp;
			trigger.OnInteraction += HandleOnInteraction;
			trigger.OnEnter += HandleOnTriggerEnter;
			trigger.OnExit += HandleOnTriggerExit;
		}

        void OnDisable()
        {
			WorkShiftGroup.OnMovedUp -= HandleOnGroupMovedUp;
			trigger.OnInteraction -= HandleOnInteraction;
			trigger.OnEnter -= HandleOnTriggerEnter;
			trigger.OnExit -= HandleOnTriggerExit;
        }

        private void HandleOnTriggerExit()
		{
			// bagOutline.SetActive(false);
			// mapOutline.SetActive(false);
			// flashlightOutline.SetActive(false);
			bag.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
			bag.transform.GetChild(0).GetComponent<InteractionEffect>().EnableInteractionEffect(false);
			map.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
			flashlight.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
        }

        private void HandleOnTriggerEnter()
		{
			bag.GetComponent<InteractionEffect>().EnableInteractionEffect(true);
			bag.transform.GetChild(0).GetComponent<InteractionEffect>().EnableInteractionEffect(true);
			map.GetComponent<InteractionEffect>().EnableInteractionEffect(true);
			flashlight.GetComponent<InteractionEffect>().EnableInteractionEffect(false);;
        }

        private void HandleOnInteraction()
		{
			trigger.SetInteractable(false);
			bag.SetActive(false);
			map.SetActive(false);
        }

        private void HandleOnGroupMovedUp()
		{
			StartCoroutine(SetInteractabledDelayed(.5f));
		}

        private IEnumerator SetInteractabledDelayed(float delay)
        {
			yield return new WaitForSeconds(delay);

			trigger.SetInteractable(true);
        }
    }
}
