using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class DayNightAlertUI : MonoBehaviour
	{
		[SerializeField]
		GameObject dayAlert, nightAlert;

        void Awake()
        {
			dayAlert.SetActive(false);
			nightAlert.SetActive(false);
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
			GameplayManager.OnDayComing += HandleOnDayComing;
			GameplayManager.OnNightComing += HandleOnNightComing;
			GameplayManager.OnNightStarted += HandleOnNightStarted;
			GameplayManager.OnDayStarted += HandleOnDayStarted;
		}

        void OnDisable()
        {
            GameplayManager.OnDayComing -= HandleOnDayComing;
			GameplayManager.OnNightComing -= HandleOnNightComing;
			GameplayManager.OnNightStarted -= HandleOnNightStarted;
			GameplayManager.OnDayStarted -= HandleOnDayStarted;
        }

        private void HandleOnNightStarted()
        {
			nightAlert.SetActive(false);
        }

        private void HandleOnDayStarted()
        {
			dayAlert.SetActive(false);
        }

        private void HandleOnDayComing()
        {
			dayAlert.SetActive(true);
        }

        private void HandleOnNightComing()
        {
			nightAlert.SetActive(true);
        }
    }
}
