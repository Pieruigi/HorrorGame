using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class AlarmManager : Singleton<AlarmManager>
	{
		public static UnityAction OnActivated;
		public static UnityAction OnDeactivated;

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
				OnActivated?.Invoke();
			}
		}

		public void ReportTriggerDeactivated(GameObject trigger)
		{
			activeTriggers.Remove(trigger);
			if (activeTriggers.Count == 0)
			{
				active = false;
				alarmAudioSource.Stop();
				OnDeactivated?.Invoke();
			}
		}
		
		public bool IsActive()
        {
			return active;
        }
	}
}
