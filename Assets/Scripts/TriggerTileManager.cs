using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class TriggerTileManager : SingletonPersistent<TriggerTileManager>
	{
		public static UnityAction OnChanged;

		bool triggersDisabled = false;
		public bool TriggerTilesDisabled
		{
			get{ return triggersDisabled; }
		}

		float elapsed = 0;

		float timer = 0;

        void Update()
        {
			if (!triggersDisabled) return;

			elapsed += Time.deltaTime;
			if(elapsed > timer)
			{
				triggersDisabled = false;
				OnChanged?.Invoke();
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
				{
					triggersDisabled = false;
				}
			}
		}
		
		public void DisableTriggers(float timer)
		{
			triggersDisabled = true;
			this.timer = timer;
			elapsed = 0;

			OnChanged?.Invoke();
		}
    }
}
