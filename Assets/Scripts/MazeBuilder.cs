//#define USE_HELPERS
#define USE_WEIGHT
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarterAssets;
using TMM.Scriptables;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TMM
{
	public class MazeBuilder : Singleton<MazeBuilder>
	{
		public static UnityAction OnMazeCreated;

		public const float CellSize = 2;

		[System.Serializable]
		class WallBlockData
		{
			public List<Vector2> tiles;

			public bool createFlippedVariant;

			public int min;

#if USE_WEIGHT
			public int weight;
#else
			public int max;
#endif


			public int count;

			public List<GameObject> prefabs = new List<GameObject>();

			public int blockType = 0; // 0: common, 1: minigame, 2: vending machine


		}




		[System.Serializable]
		class Tile
		{
			public Vector2 coords;

			public int type; // 0: floor, 1: inside block

			public GameObject mainObject;

			public Light light;

			public CoinPicker coin;

			public FloorAsset asset; // Only available for triggers

			public bool AvailableForSpawn()
			{
				return coin == null;
			}

		}

		[System.Serializable]
		class WallBlock
		{
			public WallBlockData data;

			public int rotationType;

			public List<Tile> tiles; // Keeping track of all tiles belonging to this block

			public Tile origin;

			public GameObject mainObject;
		}



		[SerializeField]
		GameObject wallHelperPrefab;

		[SerializeField]
		GameObject floorHelperPrefab;

		[SerializeField]
		GameObject inHelperPrefab;

		[SerializeField]
		GameObject outHelperPrefab;

		[SerializeField]
		GameObject borderHelperPrefab;

		[SerializeField]
		List<WallBlockData> availableBlocks = new List<WallBlockData>();

		[SerializeField]
		GameObject floorPrefab;

		[SerializeField]
		GameObject inPrefab;

		[SerializeField]
		GameObject outPrefab;

		[SerializeField]
		GameObject monsterPrefab;

		[SerializeField]
		GameObject floorLightPrefab;

		[SerializeField]
		GameObject coinPickerPrefab;

		int wallMax = 17; // 14

		[SerializeField]
		List<Tile> tiles = new List<Tile>();

		int nextBorderDirection = 0;

		[SerializeField]
		List<WallBlock> blocks = new List<WallBlock>();

		Tile inTile, outTile;

		int minigameBlockIndex;

		int doubleCreatureStage = 4;
		int tripleCreatureStage = 7;

		float doubleMultiplier = 1.5f;
		float tripleMultiplier = 2f;

		public int TileCount
		{
			get { return tiles.Count; }
		}

		public int BlockCount
		{
			get{ return blocks.Count; }
		}


		// Start is called before the first frame update
		void Start()
		{
			LoadFromResources();

			CreateFlippedVariants();

			ChooseBlocks();

			ChooseMiniGame(); // Choosing mini game after any other block, we can directly add it to the available blocks list

			CreateMaze();

			AddInAndOut();

			AddFloorTriggers();

			InstantiateWallsAndFloors();

			AddCoins();

			AddLights();

			BuildNavMesh();

			SpawnMonster();

			OnMazeCreated?.Invoke();
		}

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Z))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

#endif
		}


		void AddFloorTriggers()
		{

			// How many pressure plates?
			//
			int tileCount = tiles.Count(t => t.type == 0); // Count floors
			float mul = .05f;
			int stage = GameManager.Instance.GameStage;
			// Increase pressure plates when stage increases
			if (stage >= doubleCreatureStage && stage < tripleCreatureStage)
			{
				mul = 0.06f;
			}
			else if (stage >= tripleCreatureStage)
			{
				mul = 0.07f;
			}

			int count = Mathf.CeilToInt((float)mul * (float)tileCount); // Max number of pressure plates to add

			Debug.Log("TileCount:" + tileCount);
			Debug.Log("PressCount:" + count);

			// Load all assets
			var all = Resources.LoadAll<FloorTriggerAsset>(FloorTriggerAsset.ResourceFolder).Where(r => (r.MinStage < 0 || r.MinStage <= stage)).ToList();
			// Create a list of candidates depending of their weight
			List<FloorTriggerAsset> triggers = new List<FloorTriggerAsset>();
			foreach (var fta in all)
			{
				for (int i = 0; i < count * fta.Weight; i++)
					triggers.Add(fta);
			}
			// Filter tiles to be used
			var floors = tiles.Where(t => t.type == 0).ToList();
			floors.Remove(inTile);

			// Remove minigames adjacent tiles (where you stand to play)
			var minigames = blocks.Where(b => b.data.blockType == 1);
			foreach(var mg in minigames)
			{
				Tile tile = null;
				// Get adjacent
				switch (mg.rotationType)
				{
					case 0:
						tile = tiles.Find(t => t.coords == mg.origin.coords - Vector2.up);
						break;
					case 1:
						tile = tiles.Find(t => t.coords == mg.origin.coords - Vector2.right);
						break;
					case 2:
						tile = tiles.Find(t => t.coords == mg.origin.coords + Vector2.up);
						break;
					case 3:
						tile = tiles.Find(t => t.coords == mg.origin.coords + Vector2.right);
						break;
				}
				Debug.Log("Removing tile index:" + tiles.IndexOf(tile));
				floors.Remove(tile);
		
			}


			bool noRoom = false;
			while(count > 0 && noRoom == false)
			{
				// Choose a random tile
				var tile = floors[Random.Range(0, floors.Count)];

				// Remove current tile and closest ones
				floors.RemoveAll(t => t == tile || Vector3.Distance(t.coords, tile.coords) < 3);

				// Choose a random trigger
				var asset = triggers[Random.Range(0, triggers.Count)];

				// Remove trigger 
				triggers.Remove(asset);

				// Add the asset
				tile.asset = asset;

				// Decrease counter
				count--;
			}

			// // Get resources
			// int stage = GameManager.Instance.GameStage;
			// var all = Resources.LoadAll<FloorTriggerAsset>(FloorTriggerAsset.ResourceFolder).Where(r => (r.MinStage < 0 || r.MinStage <= stage) && (r.MaxStage < 0 || r.MaxStage >= stage)).ToList();
			// Debug.Log($"FloorTriggers.Count:{all.Count}");

			// // Get all floors
			// var floors = tiles.Where(t => t.type == 0).ToList();

			// // Remove in tile from the list (we don't want to spawn on some trap or alarm)
			// floors.Remove(inTile);

			// // Add tiles
			// foreach (var asset in all)
			// {
			// 	int count = Random.Range(asset.MinCount, asset.MaxCount + 1);
			// 	while (count > 0 && floors.Count > 0)
			// 	{
			// 		// Get a random floor tile
			// 		var tile = floors[Random.Range(0, floors.Count)];
			// 		// Set trigger tile 
			// 		tile.asset = asset;
			// 		// Remove current tile and closest ones from floor tiles
			// 		floors.RemoveAll(t => t == tile || Vector3.Distance(t.coords, tile.coords) < 3);
			// 		// Update count
			// 		count--;
			// 	}
			// }


		}

		void AddCoins()
		{
			// Get tiles at a minimum given distance from in and out tiles
			List<Tile> availables = tiles.Where(t => t.type == 0 && t.AvailableForSpawn()).ToList();
			availables.RemoveAll(t => t == inTile || Vector3.Distance(t.coords, inTile.coords) < 3);
			availables.RemoveAll(t => t == outTile || Vector3.Distance(t.coords, outTile.coords) < 3);

			int maxCoins = Random.Range(1, 4);
			int count = 0;

			while (availables.Count > 0 && count < maxCoins)
			{
				// get a random tile
				var tile = availables[Random.Range(0, availables.Count)];

				tile.coin = InstantiateObject(coinPickerPrefab, tile.coords).GetComponent<CoinPicker>();

				availables.RemoveAll(t => t == tile || Vector2.Distance(t.coords, tile.coords) < 3);

				count++;
			}

		}

		void AddLights()
		{
			// We start by setting the minigame light
			var wb = blocks.Find(b => b.data.blockType == 1);
			var ml = wb.mainObject.GetComponentInChildren<MiniGame>().MainLight;
			if (ml)
				GetTileTowardsBlockOrigin(wb).light = ml;

			// Add other lights
			int maxLights = (int)((float)tiles.Count * .05f);
			int count = 0;

			List<Tile> availables = tiles.Where(t => t.type == 0 && t.light == null).ToList();
			availables.RemoveAll(t => t == inTile || Vector3.Distance(t.coords, inTile.coords) < 3);
			availables.RemoveAll(t => t == outTile || Vector3.Distance(t.coords, outTile.coords) < 3);


			while (availables.Count > 0 && count < maxLights)
			{
				var tile = availables[Random.Range(0, availables.Count)];

				var light = InstantiateObject(floorLightPrefab, tile.coords);

				tile.light = light.GetComponentInChildren<Light>();

				// Remove current tile and other too closed
				availables.RemoveAll(t => t == tile || Vector2.Distance(t.coords, tile.coords) < 3);
				count++;
			}

		}

		Tile GetTileTowardsBlockOrigin(WallBlock block)
		{
			var origin = block.origin;
			Tile ret = null;
			switch (block.rotationType)
			{
				case 0:
					ret = tiles.Find(t => t.coords.x == origin.coords.x && t.coords.y == origin.coords.y - 1);
					break;
				case 1:
					ret = tiles.Find(t => t.coords.x == origin.coords.x - 1 && t.coords.y == origin.coords.y);
					break;
				case 2:
					ret = tiles.Find(t => t.coords.x == origin.coords.x && t.coords.y == origin.coords.y + 1);
					break;
				case 3:
					ret = tiles.Find(t => t.coords.x == origin.coords.x + 1 && t.coords.y == origin.coords.y);
					break;
			}

			return ret;
		}

		void SpawnMonster()
		{
			//return;
			int stage = GameManager.Instance.GameStage;
			int creatureCount = 1;
			if (stage >= doubleCreatureStage)
			{
				creatureCount++;
			}
			if (stage >= tripleCreatureStage)
			{
				creatureCount++;
			}

			for (int i = 0; i < creatureCount; i++)
			{
				// Choose a floor tile (type = 0) which is at a minimum distance the palayer spawn point
				float minDistance = 10f / CellSize;
				List<Tile> candidates = tiles.Where(t => t.type == 0 && Vector3.Distance(inTile.coords, t.coords) > minDistance).ToList();
				if (candidates.Count == 0) // Just to be sure
					candidates = tiles.Where(t => t.type == 0 && Vector3.Distance(inTile.coords, t.coords) > minDistance / 2f).ToList();

				// Get a random point
				var spawnTile = candidates[Random.Range(0, candidates.Count)];
				// Remove current and closest tiles
				candidates.RemoveAll(t => t == spawnTile || Vector3.Distance(t.coords, spawnTile.coords) < 12);

				// Instantiate the monster gameobject
				var monster = Instantiate(monsterPrefab);
				monster.name = monsterPrefab.name; // To avoid having (Clone) at the end of the name
				monster.GetComponent<NavMeshAgent>().enabled = false;
				monster.transform.position = new Vector3(spawnTile.coords.x, 0, spawnTile.coords.y) * CellSize;
				monster.GetComponent<NavMeshAgent>().enabled = true;
			}



		}

		public void BuildNavMesh()
		{
			var nms = tiles.Find(t => t == inTile).mainObject.GetComponentInChildren<NavMeshSurface>();
			nms.BuildNavMesh();
		}

		void ChooseMiniGame()
		{
			int level = 1;
			var miniGame = MiniGameManager.Instance.ChooseMiniGame(level);

			WallBlockData wbd = new WallBlockData();
			wbd.prefabs = miniGame.Prefabs.ToList();
			wbd.count = 1;
			wbd.createFlippedVariant = false;
			wbd.tiles = miniGame.GetTiles();
			wbd.blockType = 1; // Minigame

			availableBlocks.Add(wbd);

			minigameBlockIndex = availableBlocks.Count - 1;
		}

		void InstantiateWallsAndFloors()
		{

			// Instantiate floor
			foreach (Tile t in tiles)
			{

				if (t.type == 0)
				{
					if (t != inTile && t != outTile)
					{
						if (!t.asset)
						{
							t.mainObject = InstantiateObject(floorPrefab, t.coords);
						}
						else
						{
							var obj = InstantiateObject(t.asset.Prefab, t.coords);
							t.mainObject = obj.transform.GetChild(0).gameObject;
						}

					}
					else
					{
						if (t == inTile)
							t.mainObject = InstantiateObject(inPrefab, t.coords);
						else
							t.mainObject = InstantiateObject(outPrefab, t.coords);
					}

					CheckBorders(t);

					FloorTrigger ft = t.mainObject.GetComponentInChildren<FloorTrigger>();
					if (ft)
						CheckTriggerStepsAndWalls(ft);

				}

			}

			// Instantiate wall blocks
			foreach (var b in blocks)
			{
				// Instantiate object
				b.mainObject = InstantiateObject(b.data.prefabs[Random.Range(0, b.data.prefabs.Count)], b.origin.coords, b.rotationType);
			}



		}

		void CheckTriggerStepsAndWalls(FloorTrigger floorTrigger)
		{
			// Check walls 
			for (int i = 0; i < 4; i++)
			{
				// For floor triggers we move walls in the floor parent to avoid moving them up and down when player walk on the tile
				floorTrigger.transform.GetChild(0).parent = floorTrigger.transform.parent;
			}

			// Steps
			// Get tile
			var tile = tiles.First(t => t.mainObject == floorTrigger.gameObject);
			floorTrigger.SetStepDirection(0, GetTile(tile.coords.x, tile.coords.y + 1) == 0); // North
			floorTrigger.SetStepDirection(1, GetTile(tile.coords.x + 1, tile.coords.y) == 0); // East
			floorTrigger.SetStepDirection(2, GetTile(tile.coords.x, tile.coords.y - 1) == 0); // South
			floorTrigger.SetStepDirection(3, GetTile(tile.coords.x - 1, tile.coords.y) == 0); // West
		}

		void CheckBorders(Tile tile)
		{
			bool[] dirs = new bool[4];
			dirs[0] = tile != outTile && GetTile(tile.coords.x, tile.coords.y + 1) < 0;
			dirs[1] = GetTile(tile.coords.x + 1, tile.coords.y) < 0;
			dirs[2] = /*tile != inTile && */GetTile(tile.coords.x, tile.coords.y - 1) < 0;
			dirs[3] = GetTile(tile.coords.x - 1, tile.coords.y) < 0;


			Transform root = tile.mainObject.transform;
			if (tile == inTile || tile == outTile)
				root = root.GetChild(0);

			// FloorTrigger ft = root.GetComponent<FloorTrigger>();
			// Debug.Log("FT:" + ft);

			for (int i = 0; i < dirs.Length; i++)
			{
				if (dirs[i])
					root.GetChild(i).gameObject.SetActive(true);
				else
					root.GetChild(i).gameObject.SetActive(false);



			}




		}

		GameObject InstantiateObject(GameObject prefab, Vector2 coords, int rotationType = 0)
		{
			var go = Instantiate(prefab);
			// Apply rotation
			go.transform.localEulerAngles = Vector3.up * rotationType * 90;
			// Move to position
			go.transform.position = new Vector3(coords.x, 0, coords.y) * CellSize;

			return go;
		}

		void LoadFromResources()
		{
			string theme = "Default";

			// Load floors
			var floors = Resources.LoadAll<FloorAsset>($"{FloorAsset.ResourceFolder}/{theme}");
			floorPrefab = floors[0].Prefab;

			// // Load wall blocks
			var blocks = Resources.LoadAll<WallBlockAsset>($"{WallBlockAsset.ResourceFolder}/{theme}");

			// Clear the available block list
			availableBlocks.Clear();
			// Fill the list
			foreach (var block in blocks)
			{
				WallBlockData wbd = new WallBlockData();
				wbd.createFlippedVariant = block.CreateFlippedVariant;
				wbd.min = block.Min;
				wbd.weight = block.Weight;
				wbd.count = 0;
				wbd.prefabs = block.Prefabs.ToList();
				wbd.tiles = block.GetTiles();
				availableBlocks.Add(wbd);
			}

			// Check for no trigger tiles floor block
			var stage = GameManager.Instance.GameStage;
			bool hasTriggerTiles = Resources.LoadAll<FloorTriggerAsset>(FloorTriggerAsset.ResourceFolder).ToList().Exists(r => (r.MinStage < 0 || r.MinStage <= stage));
			if (hasTriggerTiles)
			{
				var nttb = Resources.LoadAll<NoTriggerTilesBlockAsset>($"{NoTriggerTilesBlockAsset.ResourceFolder}/{theme}");
				var vb = nttb[0];

				WallBlockData wbd = new WallBlockData();
				wbd.createFlippedVariant = false;
				wbd.min = 1;
				wbd.weight = 0;
				wbd.count = 0;
				wbd.prefabs = new List<GameObject>() { vb.Prefab };
				wbd.tiles = vb.GetTiles();
				wbd.blockType = 2; // Vending machine
				availableBlocks.Add(wbd);

			}

			// Check common vending machines
			var vendBlocks = Resources.LoadAll<VendingMachineBlockAsset>($"{VendingMachineBlockAsset.ResourceFolder}/{theme}");
			foreach(var vb in vendBlocks)
			{
				WallBlockData wbd = new WallBlockData();
				wbd.createFlippedVariant = false;
				wbd.min = 1;
				wbd.weight = 0;
				wbd.count = 0;
				wbd.prefabs = new List<GameObject>() { vb.Prefab };
				wbd.tiles = vb.GetTiles();
				wbd.blockType = 2; // Vending machine
				availableBlocks.Add(wbd);
			}

		}



		void AddInAndOut()
		{


			// Add the entrance to the south and the exit to the north
			var candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.down) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y < t.coords.y)).ToList();
			// Remove blocks which are to close to the minigame block
			var specialBlocks = blocks.Where(b => b.data.blockType == 1 || b.data.blockType == 2); // Mini-game and vending machines
			foreach (var sb in specialBlocks)
			{
				candidates.RemoveAll(t => Vector3.Distance(t.coords, sb.origin.coords) < 8);
				if(candidates.Count == 0)
					candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.down) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y < t.coords.y)).ToList();
			}

			inTile = candidates[Random.Range(0, candidates.Count)];

			// Out tile
			candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.up) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y > t.coords.y)).ToList();
			foreach (var sb in specialBlocks)
			{
				candidates.RemoveAll(t => Vector3.Distance(t.coords, sb.origin.coords) < 8);
				if(candidates.Count == 0)
					candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.up) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y > t.coords.y)).ToList();
			}

			outTile = candidates[Random.Range(0, candidates.Count)];

			var fpc = FindFirstObjectByType<FirstPersonController>();
			fpc.transform.root.position = new Vector3(inTile.coords.x, 5f, inTile.coords.y) * CellSize;
			fpc.transform.root.rotation = Quaternion.identity;
			fpc.GetComponent<CharacterController>().enabled = false;
			fpc.transform.localPosition = Vector3.zero;
			fpc.transform.localRotation = Quaternion.identity;
			fpc.GetComponent<CharacterController>().enabled = true;


		}

		void CreateFlippedVariants()
		{

			List<WallBlockData> toAdd = new List<WallBlockData>();
			foreach (var b in availableBlocks)
			{
				if (!b.createFlippedVariant) continue;

				WallBlockData wbd = new WallBlockData();
				wbd.min = b.min;
#if USE_WEIGHT
				wbd.weight = b.weight;
#else
				wbd.max = b.max;
#endif
				wbd.count = 0;
				wbd.tiles = new List<Vector2>();
				wbd.prefabs = b.prefabs;

				foreach (var tile in b.tiles)
				{
					Vector2 t = tile;
					t.x *= -1;
					wbd.tiles.Add(t);
				}

				toAdd.Add(wbd);
			}

			availableBlocks.AddRange(toAdd);
		}

		void CreateMaze()
		{
			for (int i = 0; i < wallMax; i++)
			{


				if (i == 0)
				{
					// Choose a random block
					var blocks = availableBlocks.Where(b => b.count > 0).ToList();
					var block = blocks[Random.Range(0, blocks.Count)];
					block.count--;

					var rotType = Random.Range(0, 4);
					List<Vector2> tiles = RotateTiles(block.tiles, rotType);

					// Add tiles
					AddTiles(tiles, 1);

					AddToWallBlockList(block, tiles, rotType);

					BorderWithFloor(tiles);

				}
				else
				{
					//if (i > 3) return;
					AddAnotherBlock();

				}
			}
		}


		void AddToWallBlockList(WallBlockData data, List<Vector2> tileCoords, int rotationType)
		{
			// Get tiles
			var t = tiles.Where(t => tileCoords.Contains(t.coords)).ToList();

			// Create new block and add to list
			blocks.Add(new WallBlock() { data = data, origin = t[0], rotationType = rotationType, tiles = t });
		}

		Tile GetClosestBorderToTheOrigin()
		{
			var borders = GetBorderTileAll();

			Tile tile = null;
			float minDist = 0;

			foreach (var border in borders)
			{
				var dist = border.coords.magnitude;
				if (tile == null || dist < minDist)
				{
					tile = border;
					minDist = dist;
				}
			}

			return tile;
		}

		List<Tile> GetBorderTileAll()
		{
			return tiles.Where(t => GetTile(t.coords.x, t.coords.y + 1) < 0 || GetTile(t.coords.x + 1, t.coords.y) < 0 ||
										  GetTile(t.coords.x, t.coords.y - 1) < 0 || GetTile(t.coords.x - 1, t.coords.y) < 0).ToList();
		}

		void AddAnotherBlock()
		{

			// Get borders
			List<Tile>[] borders = new List<Tile>[4];
			borders[0] = tiles.Where(t => GetTile(t.coords.x, t.coords.y + 1) < 0).OrderBy(t => t.coords.y).ToList(); // North
			borders[1] = tiles.Where(t => GetTile(t.coords.x + 1, t.coords.y) < 0).OrderBy(t => t.coords.x).ToList(); // East
			borders[2] = tiles.Where(t => GetTile(t.coords.x, t.coords.y - 1) < 0).OrderBy(t => t.coords.y).ToList(); // South
			borders[3] = tiles.Where(t => GetTile(t.coords.x - 1, t.coords.y) < 0).OrderBy(t => t.coords.x).ToList(); // West


			//int borderType = availableBorderTypes[Random.Range(0, availableBorderTypes.Count)];// 3; // Top
			int borderType = nextBorderDirection;
			nextBorderDirection = (nextBorderDirection + 1) % 4;
#if UNITY_EDITOR
			//int borderType = 1;
#endif
			var borderDirs = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

			bool done = false;

			// Get one element for each available block
			var candidates = new List<WallBlockData>();
			foreach (WallBlockData block in availableBlocks)
			{
				if (block.count <= 0 || candidates.Contains(block)) continue;

				candidates.Add(block);
			}



			while (candidates.Count > 0 && !done)
			{
				// Choose the next candidate
				var candidate = candidates[Random.Range(0, candidates.Count)];
				candidates.Remove(candidate);
#if UNITY_EDITOR
				//if (candidate != availableBlocks[0]) continue;
#endif

				// Create the rotation type array
				List<int> rotTypes = new List<int>() { 0, 1, 2, 3 };

				while (rotTypes.Count > 0 && !done)
				{

					// Choose a random rotation
					int rotType = rotTypes[Random.Range(0, rotTypes.Count)];
					rotTypes.Remove(rotType);

					// Rotate tiles
					var rotatedTiles = RotateTiles(candidate.tiles, rotType);

					// Loop through each border to check if the candidate can be positioned rotated this way starting from a specific position
					foreach (var borderTile in borders[borderType])
					{

						done = true;
						var offset = borderTile.coords + borderDirs[borderType];

						foreach (var rotatedTile in rotatedTiles)
						{
							var placedTile = rotatedTile + offset; // Move the tile in place

							if (GetTile(placedTile.x, placedTile.y) >= 0)
							{
								done = false;
								break;
							}

							foreach (var borderDir in borderDirs)
							{
								if (!rotatedTiles.Exists(t => new Vector2(t.x, t.y) == new Vector2(rotatedTile.x, rotatedTile.y) - borderDir) && GetTile(placedTile - borderDir) < 0 && GetTile(placedTile - 2 * borderDir) == 0)
								{
									done = false;
									break;
								}
							}

							if (!done) break;

							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x - 1) && !rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x - 1) && GetTile(placedTile.x - 1, placedTile.y - 1) < 0 && GetTile(placedTile.x - 1, placedTile.y - 2) == 0)
								done = false;
							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x + 1) && !rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x + 1) && GetTile(placedTile.x + 1, placedTile.y - 1) < 0 && GetTile(placedTile.x + 1, placedTile.y - 2) == 0)
								done = false;


							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x) && !rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x - 1) && GetTile(placedTile.x - 1, placedTile.y + 1) < 0 && GetTile(placedTile.x - 2, placedTile.y + 1) == 0)
								done = false;
							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x) && !rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x - 1) && GetTile(placedTile.x - 1, placedTile.y - 1) < 0 && GetTile(placedTile.x - 2, placedTile.y - 1) == 0)
								done = false;


							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x - 1) && !rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x - 1) && GetTile(placedTile.x - 1, placedTile.y + 1) < 0 && GetTile(placedTile.x - 1, placedTile.y + 2) == 0)
								done = false;
							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x + 1) && !rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x + 1) && GetTile(placedTile.x + 1, placedTile.y + 1) < 0 && GetTile(placedTile.x + 1, placedTile.y + 2) == 0)
								done = false;


							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x) && !rotatedTiles.Exists(t => t.y == rotatedTile.y + 1 && t.x == rotatedTile.x + 1) && GetTile(placedTile.x + 1, placedTile.y + 1) < 0 && GetTile(placedTile.x + 2, placedTile.y + 1) == 0)
								done = false;
							if (!rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x) && !rotatedTiles.Exists(t => t.y == rotatedTile.y - 1 && t.x == rotatedTile.x + 1) && GetTile(placedTile.x + 1, placedTile.y - 1) < 0 && GetTile(placedTile.x + 2, placedTile.y - 1) == 0)
								done = false;


							if (!done) break;

						}

						if (done)
						{
							// Update position and place tiles
							for (int i = 0; i < rotatedTiles.Count; i++)
							{
								rotatedTiles[i] += offset;
								AddTile(rotatedTiles[i], 1);
							}
							// // We can place all tiles
							// foreach (var rotatedTile in rotatedTiles)
							// {
							// 	AddTile(rotatedTile + offset, 1);
							// }
							// Update wall block data counter
							candidate.count--;

							// Add to the wall block list
							AddToWallBlockList(candidate, rotatedTiles, rotType);

							// Border with floor
							BorderWithFloor(rotatedTiles);

							return;
						}
					}
				}
			}




		}

		void AddTiles(List<Vector2> coords, int type)
		{
			foreach (var coord in coords)
				AddTile(coord, type);
		}

		void AddTile(Vector2 coords, int type)
		{
			if (!tiles.Exists(t => t.coords == coords && t.type == type))
			{
				tiles.Add(new Tile() { coords = coords, type = type });
			}
		}

		void CreateFloorTile(Vector2 coords)
		{
			GameObject prefab = floorPrefab;
			GameObject tile = Instantiate(prefab);
			Vector3 pos = new Vector3(coords.x, 0, coords.y) * CellSize;
			tile.transform.position = pos;
		}

		void CreateHelperTile(Vector2 coords, int type)
		{

			GameObject tile = Instantiate(type == 0 ? floorHelperPrefab : wallHelperPrefab);

			Vector3 pos = new Vector3(coords.x, 0, coords.y) * CellSize;
			tile.transform.position = pos;
		}

		void BorderWithFloor(List<Vector2> walls)
		{
			foreach (var wall in walls)
			{
				if (GetTile(wall.x, wall.y + 1) < 0) // North
					AddTile(new Vector2(wall.x, wall.y + 1), 0);

				if (GetTile(wall.x + 1, wall.y + 1) < 0) // North east
					AddTile(new Vector2(wall.x + 1, wall.y + 1), 0);

				if (GetTile(wall.x + 1, wall.y) < 0) // East
					AddTile(new Vector2(wall.x + 1, wall.y), 0);

				if (GetTile(wall.x + 1, wall.y - 1) < 0)
					AddTile(new Vector2(wall.x + 1, wall.y - 1), 0);

				if (GetTile(wall.x, wall.y - 1) < 0)
					AddTile(new Vector2(wall.x, wall.y - 1), 0);

				if (GetTile(wall.x - 1, wall.y - 1) < 0)
					AddTile(new Vector2(wall.x - 1, wall.y - 1), 0);

				if (GetTile(wall.x - 1, wall.y) < 0)
					AddTile(new Vector2(wall.x - 1, wall.y), 0);

				if (GetTile(wall.x - 1, wall.y + 1) < 0)
					AddTile(new Vector2(wall.x - 1, wall.y + 1), 0);
			}
		}

		/// <summary>
		/// -1 is empty
		/// 0 is floor
		/// 1 is wall
		/// </summary>
		/// <param name="x"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public int GetTile(float x, float y)
		{
			if (!tiles.Exists(t => t.coords.x == x && t.coords.y == y))
				return -1;

			return tiles.Find(t => t.coords.x == x && t.coords.y == y).type;
		}

		public int GetTile(Vector2 coords)
		{
			return GetTile(coords.x, coords.y);
		}

		List<Vector2> RotateTiles(List<Vector2> tiles, int rotationType)
		{
			List<Vector2> ret = new List<Vector2>();
			switch (rotationType)
			{
				case 0:
					foreach (var tile in tiles)
						ret.Add(new Vector2(tile.x, tile.y));
					break;
				case 1:
					foreach (var tile in tiles)
						ret.Add(new Vector2(tile.y, -tile.x));
					break;
				case 2:
					foreach (var tile in tiles)
						ret.Add(new Vector2(-tile.x, -tile.y));
					break;
				case 3:
					foreach (var tile in tiles)
						ret.Add(new Vector2(-tile.y, tile.x));
					break;
			}

			return ret;


		}

		void ChooseBlocks()
		{
			int stage = GameManager.Instance.GameStage;
			if (stage >= doubleCreatureStage && stage < tripleCreatureStage)
				wallMax = Mathf.CeilToInt(wallMax * doubleMultiplier);
			else if (stage >= tripleCreatureStage)
				wallMax = Mathf.CeilToInt(wallMax * tripleMultiplier);

#if USE_WEIGHT
			// Minumum
			int count = 1; // Reservation for minigame that will be choose further in the script
			foreach (var bp in availableBlocks)
			{
				bp.count = bp.min;
				count += bp.min;
			}

			if (wallMax < count) return; // Enough

			// Fill a temp list depending on the weights
			List<WallBlockData> tmp = new List<WallBlockData>();
			foreach (var wb in availableBlocks)
			{
				for (int i = 0; i < wb.weight; i++)
					tmp.Add(wb);
			}

			int left = wallMax - count;
			for (int i = 0; i < left; i++)
			{
				// Choose a random block
				var wbd = tmp[Random.Range(0, tmp.Count)];
				// Remove the block
				tmp.Remove(wbd);

				// Increase the counter
				wbd.count++;
			}

		
#else
			// Minumum
			int count = 0;
			foreach (var bp in availableBlocks)
			{
				bp.count = bp.min;
				count += bp.min;
			}

			if (wallMax < count) return; // Enough

			// Keep filling the list

			int left = wallMax - count;
			for (int i = 0; i < left; i++)
			{
				List<WallBlockData> tmp = new List<WallBlockData>();
				foreach (var bp in availableBlocks)
				{
					if (bp.max == 0) continue;

					if (bp.max > 0 && bp.count >= bp.max) continue; // Maximum reached

					// Add prefab
					tmp.Add(bp);

				}

				// Choose the next one
				var wbd = tmp[Random.Range(0, tmp.Count)];
				wbd.count++;
				
			}
#endif


		}

		public List<Vector3> GetWalkableTilePositions()
		{
			List<Vector3> ret = new List<Vector3>();
			var l = tiles.Where(t => t.type == 0);
			foreach (var t in l)
				ret.Add(new Vector3(t.coords.x, 0, t.coords.y) * CellSize);

			return ret;
		}

		public Vector2 GetTileCoords(int index)
		{
			return tiles[index].coords;

		}

		public int GetTileType(int index)
		{
			return tiles[index].type;
		}

		public int GetTileIndex(Vector2 coords)
		{
			return tiles.FindIndex(t => t.coords == coords);

		}

		public bool IsEnterTile(int index)
		{
			return tiles[index] == inTile;
		}

		public bool IsExitTile(int index)
		{
			return tiles[index] == outTile;
		}

		public bool IsMiniGameController(int index)
		{
			var tile = tiles[index];
			return blocks.Exists(b => b.origin == tile && b.data.blockType == 1);
		}

		public bool IsTriggerTile(int index)
		{
			var tile = tiles[index];
			return tile.asset && tile.asset.GetType() == typeof(FloorTriggerAsset);
		}

		public bool TileHasCoin(int index)
		{
			return tiles[index].coin != null;
		}

		public int GetTileIndex(CoinPicker coinPicker)
		{
			return tiles.FindIndex(t => t.coin == coinPicker);
		}
		
		public int GetTileIndex(GameObject mainObject)
		{
			return tiles.FindIndex(t => t.mainObject == mainObject);
		}

		public Vector2 GetBlockCoords(int index)
		{
			return blocks[index].origin.coords;
		}

		public int GetBlockRotationType(int index)
		{
			return blocks[index].rotationType;
		}	

		public int GetBlockType(int index)
		{
			return blocks[index].data.blockType;
		}

	}
}
