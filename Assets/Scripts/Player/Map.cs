using System;
using System.Collections;
using System.Collections.Generic;
using TMM.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

namespace TMM
{
	public class Map : SingletonPersistent<Map>
	{
		public static UnityAction OnExpired;

		float timer = 0;

		bool firstTime = true;

		bool showMessage = false;

		public bool Available { get { return GetTimer() > 0; } }

	    // Start is called before the first frame update
	    void Start()
	    {
			MapUI.Instance.Close();
	    }

		// Update is called once per frame
		void Update()
		{
			if(timer <= 0)
			{
                if (Input.GetKeyDown(KeyCode.Q))
					MessageManager.Instance.ShowCustomMessage(5);

                if (MapUI.Instance.IsOpen)
				{
                    Close();
					OnExpired?.Invoke();
                }
					
				return;
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (MapUI.Instance.IsOpen)
					Close();
				else
					Open();
			}

			if (MapUI.Instance.IsOpen)
				timer -= Time.deltaTime;

			if (showMessage && !MessageManager.Instance.IsMessageVisible())
			{
				showMessage = false;
				MessageManager.Instance.ShowCustomMessage(4);
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
				if(GameManager.Instance.GameStage == 1)
				{
					timer = 0;
				}
			}
        }

        void Open()
		{
			if (MapUI.Instance.IsOpen) return;


			MapUI.Instance.Open();
		}

		void Close()
		{
			if (!MapUI.Instance.IsOpen) return;

			MapUI.Instance.Close();
		}

		public void SetTimer(float amount)
		{
			timer = amount;

			if (firstTime)
			{
				firstTime = false;
				showMessage = true;
			}
		}
		
		public float GetTimer()
		{
			return timer;
		}
	}
}
