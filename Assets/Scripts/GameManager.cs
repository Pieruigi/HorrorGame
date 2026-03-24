using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMM.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class GameManager : SingletonPersistent<GameManager>
	{
		public readonly int MaxLevel = 2;

#if UNITY_EDITOR
		int gameStage = 1;
#else
		int gameStage = 1;
#endif
		public int GameStage
		{
			get { return gameStage; }
		}

#if UNITY_EDITOR
		int level = 2;
#else
		int level = 0;
#endif
        public int Level
		{
			get { return  level; }	
			set { level = value; }
		}


       

        // Start is called before the first frame update
        void Start()
	    {
			if (SteamStatsManager.Instance.GetGameLevel(out var l))
				Debug.Log("TEST - Last level:" + l);
	    }

		// Update is called once per frame
		void Update()
		{
#if !UNITY_WEBGL
            //if (Input.GetKeyDown(KeyCode.Escape))
            //	Application.Quit();


#endif


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
			if (arg0.name == "GameScene")
			{
				CameraFade.Instance.FadeIn();
			}
            else
            {
                CameraFade.Instance.FadeIn();
            }
        }

		public void StartNewGame()
		{
            gameStage = 1;
#if UNITY_EDITOR
            //gameStage = 4;
#endif


			StartCoroutine(DoLoadGameScene("GameScene"));
			//SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
		}

		public void RestartGame()
		{
            gameStage = 1;
		    SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        }

		public void YouLose()
		{
            SceneManager.LoadSceneAsync("LoserScene", LoadSceneMode.Single);
        }

		public void YouWin()
		{
            SteamAchievementManager.Instance.UnlockAchievement("STAGE_" + gameStage + "_COMPLETED");

			// Check level
			if(SteamStatsManager.Instance.GetGameLevel(out var l))
			{
				if (l < MaxLevel && l <= level)
				{
                    SteamStatsManager.Instance.SetGameLevel(l + 1);
					level++;
                }
					
			}

            StartCoroutine(DoLoadGameScene("WinnerScene"));
        }

        public void StartNextStage()
        {
			// Check steam achievement
			
			SteamAchievementManager.Instance.UnlockAchievement("STAGE_" + gameStage + "_COMPLETED");
			
            gameStage++;

			StartCoroutine(DoLoadGameScene("GameScene"));
        }

		IEnumerator DoLoadGameScene(string sceneName)
        {
			CameraFade.Instance.FadeOut();
			yield return new WaitForSeconds(1f);
			SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

		public void LoadMainMenu()
		{
			if("mainscene".Equals(SceneManager.GetActiveScene().name.ToLower()))
				return;

            // In case we are in the game menu which pauses the game
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
			Time.timeScale = 1f;
            // Fade and load main menu
            CameraFade.Instance.FadeOut();
            SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
        }

		public void PauseGame()
		{
			if (!"gamescene".Equals(SceneManager.GetActiveScene().name.ToLower())) return;

			var player = FindFirstObjectByType<FirstPersonController>();
			player.InputDisabled = true;
			player.AimingDisabled = true;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Time.timeScale = 0f;
        }

		public void UnpauseGame()
		{
			if (!"gamescene".Equals(SceneManager.GetActiveScene().name.ToLower())) return;
			
            var player = FindFirstObjectByType<FirstPersonController>();
            player.InputDisabled = false;
            player.AimingDisabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
	}
}
