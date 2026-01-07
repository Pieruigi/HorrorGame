using System.Collections.Generic;
using System.Linq;
using TMM.Scriptables;
using UnityEngine;

namespace TMM
{
	public class MiniGameManager : SingletonPersistent<MiniGameManager>
	{
		[System.Serializable]
		public class SpawnedMiniGame
        {
			public MiniGameAsset asset;

			public int count = 0; // How many times we played this minigame since we launched the game

			public SpawnedMiniGame(MiniGameAsset asset)
            {
				this.asset = asset;
            }
        }

		List<SpawnedMiniGame> spawnedMiniGames = new List<SpawnedMiniGame>();

		List<MiniGameAsset> miniGames;

		MiniGameAsset lastChosen;

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

		void LoadMiniGameFromResourcesAll()
		{
			if (miniGames != null) return;
            string theme = "Default";
			miniGames = Resources.LoadAll<MiniGameAsset>($"{MiniGameAsset.ResourceFolder}/{theme}").ToList();
        }

		public MiniGameAsset ChooseMiniGame(int level)
		{
			LoadMiniGameFromResourcesAll();
			var availables = miniGames.Where(m => (m.MinLevel < 0 || m.MinLevel <= level) && (m.MaxLevel < 0 || m.MaxLevel >= level)).ToList();

			if (lastChosen)
				availables.Remove(lastChosen);

			lastChosen = availables[Random.Range(0, availables.Count)];
			

#if UNITY_EDITOR
			//lastChosen = availables.Find(m => "memory".Equals(m.name.ToLower()));
#endif

			var smg = spawnedMiniGames.Find(m => m.asset == lastChosen);
			if (smg == null)
                spawnedMiniGames.Add(smg = new SpawnedMiniGame(lastChosen));
           
			smg.count++;

			return lastChosen;

		}
		
		
    }
}
