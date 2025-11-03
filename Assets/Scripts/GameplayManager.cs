using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class GameplayManager : Singleton<GameplayManager>
	{
		public static UnityAction OnNextShiftReady;

		public static UnityAction OnWorkShiftStarted;
		public static UnityAction OnWorkShiftCompleted;

		

		int workingDay = 1;

		bool nightShift = false;

		bool workShiftStarted = false;
		public bool WorkShiftStarted
		{
			get { return workShiftStarted; }
		}

		int workingDayMax = 5;

		bool ready = false;


	    // Start is called before the first frame update
	    void Start()
	    {
			StartCoroutine(SetNextShiftReady());
	    }

		// Update is called once per frame
		void Update()
		{

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
				if(!DayNightManager.Instance.IsNight)
					DayNightManager.Instance.Switch();
				workingDay++;
			}
            else
            {
                if(DayNightManager.Instance.IsNight)
					DayNightManager.Instance.Switch();
            }

			if (workingDay == workingDayMax + 1)
			{
				// Let the player free to go (lying, you leave the base but you get killed by some creature)
			}
			else
			{
				// Start a new day of work
				OnWorkShiftStarted?.Invoke();
			}

			
		}

		public void EndWorkShift()
		{
			workShiftStarted = false;

			if (workingDay == workingDayMax)
			{
				// Do wmoething
			}

			nightShift = !nightShift;
			workingDay++;

			ready = false;
			StartCoroutine(SetNextShiftReady());

			OnWorkShiftCompleted?.Invoke();



		}
		
	

	}
}
