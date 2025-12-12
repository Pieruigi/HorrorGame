using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class FloorTrapAlarm : MonoBehaviour
	{
		[SerializeField]
		FloorTrigger floorTrigger;

		[SerializeField]
		float duration = 20;

		float elapsed = 0;

		bool activated = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			if (!activated) return;

			elapsed += Time.deltaTime;
			if(elapsed >= duration)
			{
				activated = false;
				AlarmManager.Instance.ReportTriggerDeactivated(gameObject);
				floorTrigger.ResetTrigger();
			}
		}

		void OnEnable()
		{
			floorTrigger.OnTriggered += HandleOnTriggered;
		}

        void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
        }

        private void HandleOnTriggered()
        {
			AlarmManager.Instance.ReportTriggerActivated(gameObject);
			elapsed = 0;
			activated = true;
        }
    }
}
