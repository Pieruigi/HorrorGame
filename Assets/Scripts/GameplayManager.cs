using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class GameplayManager : Singleton<GameplayManager>
	{
		public static UnityAction OnNextShiftReady;

		public delegate void WorkShiftStartedDelegate(int day, bool isNightShift);
		public static WorkShiftStartedDelegate OnWorkShiftStarted;

		public static UnityAction OnTaskCompleted;

		

		int workingDay = 1;

		bool nightShift = false;
		public bool NightShift
        {
            get{ return nightShift; }
        }

		bool workShiftRunning = false;
	
		int workingDayMax = 5;

		bool ready = false;

		bool taskCompleted = false;
		public bool TaskCompleted
        {
            get{ return taskCompleted; }
        }


	    // Start is called before the first frame update
	    void Start()
	    {
			StartCoroutine(SetNextShiftReady());
	    }

		// Update is called once per frame
		void Update()
		{

		}

	
		public void ReportMailCollectedAll()
		{
			taskCompleted = true;
			OnTaskCompleted?.Invoke();
		}
		
		public void ReportMailDeliveredAll()
        {
            taskCompleted = true;
			OnTaskCompleted?.Invoke();
        }

		IEnumerator SetNextShiftReady()
		{
			
			yield return new WaitForSeconds(5);

			ready = true;
			OnNextShiftReady?.Invoke();
        }

		/// <summary>
		/// Every time you hit the button you start the next work shift
		/// </summary>
		public void StartWorkShift()
		{
			if (!ready) return;
			if (nightShift)
			{
				
			}
            else
			{
		       
            }

			if (workingDay == workingDayMax + 1)
			{
				// Let the player free to go (lying, you leave the base but you get killed by some creature)
			}
			else
			{
				taskCompleted = false;

				// Start a new day of work
				workShiftRunning = true;

				LetterboxManager.Instance.InitShift(workingDay, nightShift);
				MailboxManager.Instance.InitShift(workingDay, nightShift);
				MailManager.Instance.InitShift(workingDay, nightShift);

				OnWorkShiftStarted?.Invoke(workingDay, nightShift);
				
			}

			
		}

		public void StopWorkShift()
		{
			workShiftRunning = false;
			ready = false;
			if (workingDay == workingDayMax)
			{
				// Do something
			}
            else
			{
				workingDay++;
				nightShift = !nightShift;
				//if(!DayNightManager.Instance.IsNight)
				DayNightManager.Instance.Switch();
            	StartCoroutine(SetNextShiftReady());    
            }
						

		}
		
	

	}
}
