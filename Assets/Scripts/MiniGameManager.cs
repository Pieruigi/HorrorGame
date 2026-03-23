using System.Collections.Generic;
using System.Linq;
using TMM.Scriptables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class MiniGameManager : SingletonPersistent<MiniGameManager>
	{
		//[System.Serializable]
		//public class SpawnedMiniGame
  //      {
		//	public MiniGameAsset asset;

		//	public int count = 0; // How many times we played this minigame since we launched the game

		//	public SpawnedMiniGame(MiniGameAsset asset)
  //          {
		//		this.asset = asset;
  //          }
  //      }

		//List<SpawnedMiniGame> spawnedMiniGames = new List<SpawnedMiniGame>();

		List<MiniGameAsset> miniGames;

		//MiniGameAsset lastChosen;
		List<MiniGameAsset> lastChosenAssets = new List<MiniGameAsset>();

        protected override void Awake()
        {
			base.Awake();

			LoadMiniGameFromResourcesAll();
        }

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
			if ("gamescene".Equals(arg0.name.ToLower()))
			{
				if (GameManager.Instance.GameStage == 1)
					lastChosenAssets.Clear();
            }
        }

        void LoadMiniGameFromResourcesAll()
		{
			if (miniGames != null) return;
            string theme = "Default";
			miniGames = Resources.LoadAll<MiniGameAsset>($"{MiniGameAsset.ResourceFolder}/{theme}").ToList();
        }

		public MiniGameAsset ChooseMiniGame(int level)
		{
			LoadMiniGameFromResourcesAll();
#if DEMO
			var availables = miniGames.Where(m => "tetris".Equals(m.name.ToLower()) || "memory".Equals(m.name.ToLower())).ToList();
#else
			var availables = miniGames.Where(m => (m.MinLevel < 0 || m.MinLevel <= level) && (m.MaxLevel < 0 || m.MaxLevel >= level)).ToList();
#endif


            foreach (var last in lastChosenAssets)
				availables.Remove(last);
        
    
			var lastChosen = availables[Random.Range(0, availables.Count)];
			lastChosenAssets.Add(lastChosen);

#if UNITY_EDITOR
            //lastChosen = availables.Find(m => "hawking".Equals(m.name.ToLower())); 
#endif

            //var smg = spawnedMiniGames.Find(m => m.asset == lastChosen);
            //if (smg == null)
            //             spawnedMiniGames.Add(smg = new SpawnedMiniGame(lastChosen));

            //smg.count++;

            return lastChosen;

		}
		
		
    }
}
