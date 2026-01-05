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
	        if (TriggerTileManager.Instance.TriggerTilesDisabled)
				floorTrigger.SwitchOff();
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
				//floorTrigger.ResetTrigger();
			}
		}

		void OnEnable()
		{
			floorTrigger.OnTriggered += HandleOnTriggered;
			AlarmManager.OnActivated += HandleOnAlarmaActivated;
			AlarmManager.OnDeactivated += HandleOnAlarmDeactivated;
			TriggerTileManager.OnChanged += HandleOnTriggerTileManagerChanged;
		}

        void OnDisable()
        {
			floorTrigger.OnTriggered -= HandleOnTriggered;
			AlarmManager.OnActivated -= HandleOnAlarmaActivated;
			AlarmManager.OnDeactivated -= HandleOnAlarmDeactivated;
			TriggerTileManager.OnChanged -= HandleOnTriggerTileManagerChanged;
        }

        private void HandleOnTriggerTileManagerChanged()
        {
			if (TriggerTileManager.Instance.TriggerTilesDisabled)
				floorTrigger.SwitchOff();
			else if (!AlarmManager.Instance.IsActive())
				floorTrigger.ResetTrigger();
			
        }

        private void HandleOnAlarmaActivated()
        {
			floorTrigger.SwitchOff();
        }

        private void HandleOnAlarmDeactivated()
		{
			if (TriggerTileManager.Instance.TriggerTilesDisabled) return;
			 
			floorTrigger.ResetTrigger();
        }

		private void HandleOnTriggered()
		{
			StartCoroutine(ReportTriggerActivated());
			elapsed = 0;
			activated = true;
		}
		
		IEnumerator ReportTriggerActivated()
		{
			yield return new WaitForSeconds(.5f);
			AlarmManager.Instance.ReportTriggerActivated(gameObject);
		}
    }
}
