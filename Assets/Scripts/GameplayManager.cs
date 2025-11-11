using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class GameplayManager : Singleton<GameplayManager>
	{

		public static UnityAction OnNightStarted;
		public static UnityAction OnDayStarted;

		public static UnityAction OnNightComing;

		public static UnityAction OnDayComing;

		int days = 0;

		float time = 0;

		bool isNight = false;

		float dayDuration = 240f;

		float nightDuration = 120f;

		float elapsed = 0;

		float alertTime = 5;
		bool alert = false;
		

		void Start()
		{
			time = dayDuration;
		}

		void Update()
		{
			elapsed += Time.deltaTime;

			if (elapsed > time)
			{
				elapsed -= time;

				if (isNight)
				{
					days++;
					isNight = false;
					alert = false;
					time = dayDuration;
					OnDayStarted?.Invoke();
				}
				else
				{
					isNight = true;
					alert = false;
					time = nightDuration;
					OnNightStarted?.Invoke();
				}

			}
			else // Elapsed < time
			{
				if (!alert && elapsed > time - alertTime)
				{
					alert = true;
					if (!isNight)
						OnNightComing?.Invoke();
					else
						OnDayComing?.Invoke();
				}
			}

		}
		
		public bool IsFirstDay()
        {
			return days == 0;
        }


    }
}
