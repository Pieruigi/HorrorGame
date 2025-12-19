using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

namespace TMM
{
	public abstract class TimedBuffDebuff : MonoBehaviour
	{
	 	public static UnityAction OnApplied;
		public static UnityAction OnExpired;

		float timer = 0;

		float timerDefault = 60;
		
		public float Timer
		{
			get { return timer; }
		}

		protected abstract void DoApply();
		protected abstract void DoExpire();

        protected virtual void Awake()
        {
            
        }

        // Update is called once per frame
        void Update()
		{
			if(timer > 0)
			{
				timer -= Time.deltaTime;
				if(timer <= 0)
				{
					timer = 0;
					DoExpire();
					OnExpired?.Invoke();
				}
			}
		}

		void OnEnable()
		{
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
		}

        void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

		private void HandleOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			if ("GameScene".Equals(arg0.name))
			{
				if (GameManager.Instance.GameStage == 1)
					ResetAll();

			}
		}
		
		protected virtual void ResetAll()
		{
			timer = 0;
		}

		public void Apply()
		{
			timer = timerDefault;
			DoApply();
			OnApplied?.Invoke();
		}
	}
}
