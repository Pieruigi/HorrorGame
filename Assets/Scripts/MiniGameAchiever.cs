using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class MiniGameAchiever : SingletonPersistent<MiniGameAchiever>
    {
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            MiniGame.OnMiniGameBeaten += HandleOnMiniGameBeaten;
        }

        private void OnDisable()
        {
            MiniGame.OnMiniGameBeaten -= HandleOnMiniGameBeaten;
        }

        private void HandleOnMiniGameBeaten(MiniGame miniGame)
        {
            string s = GetAchievementName(miniGame);

            if(!string.IsNullOrEmpty(s))
            {
                SteamAchievementManager.Instance.UnlockAchievement(s);
            }
        }

        string GetAchievementName(MiniGame miniGame)
        {
            var type = miniGame.GetType();

            string achName = "BEAT_";
            if (type == typeof(Colors))
            {
                return achName + "PIPES";
            }
            if (type == typeof(Tetris))
            {
                return achName + "TETRIS";
            }
            if (type == typeof(Memory))
            {
                return achName + "MEMORY";
            }
            if (type == typeof(Breakout))
            {
                return achName + "ARKANOID";
            }

            return "";
        }

    }
}
