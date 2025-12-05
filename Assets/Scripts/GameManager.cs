using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class GameManager : Singleton<GameManager>
	{

		int gameStage = 1;
		public int GameStage
        {
            get{ return gameStage; }
        }

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

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

			}
            else
            {
                
            }
        }

        public void StartNewGame()
		{
			gameStage = 1;

			SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
		}
		

	}
}
