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

		

		//bool interactable = false;
		[SerializeField]
		InteractionTrigger trigger;

        void Awake()
		{
			trigger.SetInteractable(false);
			flashlight.SetActive(false);
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
		}

        void OnDisable()
        {
			WorkShiftGroup.OnMovedUp -= HandleOnGroupMovedUp;
			trigger.OnInteraction -= HandleOnInteraction;
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
