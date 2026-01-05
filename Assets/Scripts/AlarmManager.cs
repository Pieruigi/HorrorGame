using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RetroShadersPro.URP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

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

		CRTSettings crt;

		

		protected override void Awake()
		{
			base.Awake();

		}

		void Start()
		{
			// Get crt setting 
			FindFirstObjectByType<Volume>().profile.TryGet<CRTSettings>(out crt);
		}
		
		void Update()
		{

#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.X))
			// {
			// 	StartFx();
			// }
			// if (Input.GetKeyDown(KeyCode.C))
			// {
			// 	StopFx();
			// }
#endif
		}

        public void ReportTriggerActivated(GameObject trigger)
		{
			activeTriggers.Add(trigger);

			if (!active)
			{
				active = true;
				alarmAudioSource.Play();

				StartFx();

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
				StopFx();
				OnDeactivated?.Invoke();
			}
		}

		void StartFx()
		{
			// Kill any previous tween
			DOTween.KillAll();

			// Set initial color by default for safety
			crt.tintColor.value = Color.white;

			// Start tween
			DOTween.To(() => crt.tintColor.value, c => crt.tintColor.value = c, Color.red, .25f).SetLoops(-1, LoopType.Yoyo);

		}
		
		void StopFx()
		{
			// Kill any previous tween
			DOTween.KillAll();

			// Reset color
			DOTween.To(() => crt.tintColor.value, c => crt.tintColor.value = c, Color.white, 0.25f);
		}

		public bool IsActive()
        {
			return active;
        }
	}
}
