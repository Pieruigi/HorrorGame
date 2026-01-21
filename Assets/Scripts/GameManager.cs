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
#if UNITY_EDITOR
		int gameStage = 1;
#else
		int gameStage = 1;
#endif
		public int GameStage
		{
			get { return gameStage; }
		}
		


	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
#if !UNITY_WEBGL
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
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


			StartCoroutine(DoLoadGameScene());
			//SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
		}
		
		public void StartNextStage()
        {
			gameStage++;

			StartCoroutine(DoLoadGameScene());
        }

		IEnumerator DoLoadGameScene()
        {
			CameraFade.Instance.FadeOut();
			yield return new WaitForSeconds(1f);
			SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
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
