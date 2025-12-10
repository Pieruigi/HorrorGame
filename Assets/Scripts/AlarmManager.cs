using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class AlarmManager : Singleton<AlarmManager>
	{
		[SerializeField]
		AudioSource alarmAudioSource;

		List<GameObject> activeTriggers = new List<GameObject>();

		bool active = false;
		

	    

		public void ReportTriggerActivated(GameObject trigger)
		{
			activeTriggers.Add(trigger);

			if (!active)
			{
				active = true;
				alarmAudioSource.Play();
			}
		}

		public void ReportTriggerDeactivated(GameObject trigger)
		{
			activeTriggers.Remove(trigger);
			if (activeTriggers.Count == 0)
			{
				active = false;
				alarmAudioSource.Stop();
			}
		}
		
		public bool IsActive()
        {
			return active;
        }
	}
}
