using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

		[SerializeField]
		PostOfficeDoors doors;

	

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
			GameplayManager.OnTaskCompleted += HandleOnTaskCompleted;
			// trigger.OnEnter += HandleOnTriggerEnter;
			// trigger.OnExit += HandleOnTriggerExit;
		}

        void OnDisable()
        {
			WorkShiftGroup.OnMovedUp -= HandleOnGroupMovedUp;
			trigger.OnInteraction -= HandleOnInteraction;
			GameplayManager.OnTaskCompleted -= HandleOnTaskCompleted;
			// trigger.OnEnter -= HandleOnTriggerEnter;
			// trigger.OnExit -= HandleOnTriggerExit;
        }

        private void HandleOnTaskCompleted()
		{
			// Activate the trigger back
			trigger.SetInteractable(true);
        }

        // private void HandleOnTriggerExit()
        // {
        // 	// bagOutline.SetActive(false);
        // 	// mapOutline.SetActive(false);
        // 	// flashlightOutline.SetActive(false);
        // 	bag.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
        // 	bag.transform.GetChild(0).GetComponent<InteractionEffect>().EnableInteractionEffect(false);
        // 	map.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
        // 	flashlight.GetComponent<InteractionEffect>().EnableInteractionEffect(false);
        // }

        // private void HandleOnTriggerEnter()
        // {
        // 	bag.GetComponent<InteractionEffect>().EnableInteractionEffect(true);
        // 	bag.transform.GetChild(0).GetComponent<InteractionEffect>().EnableInteractionEffect(true);
        // 	map.GetComponent<InteractionEffect>().EnableInteractionEffect(true);
        // 	flashlight.GetComponent<InteractionEffect>().EnableInteractionEffect(false);;
        // }

        private void HandleOnInteraction()
		{
            //if(GameplayManager.Instance.)
            // Work shift always starts when you hit the button and stop when you return back the equipment after you completed your task
            if (!GameplayManager.Instance.NightShift) // Daylight
            {
				if (GameplayManager.Instance.TaskCompleted) // Start collect mail
					StartCoroutine(StopCollectingMail()); 
				else // All mails collected from mailboxes
					StartCoroutine(StartCollectingMail()); 
            }
            else // Night shift
            {
				if (GameplayManager.Instance.TaskCompleted) // We delivered all the mails
					StartCoroutine(StopDeliveringMail());
				else
					StartCoroutine(StartDeliveringMail());
				
            }
			

		}

		IEnumerator StartDeliveringMail()
		{
			yield return null;
		}
		
		IEnumerator StopDeliveringMail()
        {
            yield return null;
        }

		IEnumerator StopCollectingMail()
		{
			trigger.SetInteractable(false);
			// Check flashlight
			var fl = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponentInChildren<Flashlight>();
			if (fl.IsAvailable())
				fl.SetAvailable(false);

			// Move equipment to the pillar
			bag.GetComponent<PutDownEffect>().PlayEffect();
			map.GetComponent<PutDownEffect>().PlayEffect();
			if (GameplayManager.Instance.NightShift)
				flashlight.GetComponent<PutDownEffect>().PlayEffect();

			yield return new WaitForSeconds(1f);
			
			// Close doors
			doors.SetLocked(true);
			
        }

		IEnumerator StartCollectingMail()
		{
			trigger.SetInteractable(false);

			// bag.SetActive(false);
			// map.SetActive(false);
			bag.GetComponent<PickUpEffect>().PlayEffect();
			map.GetComponent<PickUpEffect>().PlayEffect();
			if (flashlight.activeSelf) flashlight.GetComponent<PickUpEffect>().PlayEffect();

			// Play music
			if (!DayNightManager.Instance.IsNight)
				MusicManager.Instance.PlayDaylightMusic(1f);

			yield return new WaitForSeconds(2f);

			doors.SetLocked(false);
			doors.Open();

	
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
