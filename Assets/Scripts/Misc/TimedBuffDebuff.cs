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
	 	public static UnityAction<TimedBuffDebuff> OnApplied;
		public static UnityAction<TimedBuffDebuff> OnExpired;

		float timer = 0;

		[SerializeField]
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
					OnExpired?.Invoke(this);
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
			else if("loserscene".Equals(arg0.name.ToLower()) || "winnerscene".Equals(arg0.name.ToLower()))
			{
				ResetAll();
            };
		}
		
		protected virtual void ResetAll()
		{
			timer = 0;
			DoExpire();
		}

		public void Apply()
		{
			timer = timerDefault;
			DoApply();
			OnApplied?.Invoke(this);
		}
	}
}
