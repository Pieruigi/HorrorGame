//#define USE_HELPERS
#define USE_WEIGHT
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using TMM.Scriptables;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;

namespace TMM
{
	public class MazeBuilder : MonoBehaviour
	{
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


		}




		[System.Serializable]
		class Tile
		{
			public Vector2 coords;

			public int type; // 0: floor, 1: wall

			public GameObject mainObject;
		}

		[System.Serializable]
		class WallBlock
        {
			public WallBlockData data;

			public int rotationType;

			public List<Tile> tiles; // Keeping track of all tiles belonging to this block

			public Tile origin;
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

		int wallMax = 14;

		List<Tile> tiles = new List<Tile>();

		int nextBorderDirection = 0;


		List<WallBlock> blocks = new List<WallBlock>();

		Tile inTile, outTile;

		Tile miniGameTile;

		// Start is called before the first frame update
		void Start()
		{
			LoadFromResources();

			CreateFlippedVariants();

			ChooseBlocks();

			ChooseMiniGame(); // Choosing mini game after any other block, we can directly add it to the available blocks list

			CreateMaze();

			AddInAndOut();

			//CloseBorders();

			InstantiateWallsAndFloors();

			BuildNavMesh();

		
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Z))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

#endif
		}

		void BuildNavMesh()
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

			availableBlocks.Add(wbd);
        }

		void InstantiateWallsAndFloors()
		{
#if !USE_HELPER
			// Instantiate floor
			foreach (Tile t in tiles)
			{

				if (t.type == 0)
				{
					if (t != inTile && t != outTile)
					{
						t.mainObject = InstantiateObject(floorPrefab, t.coords);
					}
					else
					{
						if (t == inTile)
							t.mainObject = InstantiateObject(inPrefab, t.coords);
						else
							t.mainObject = InstantiateObject(outPrefab, t.coords);
					}

					CheckBorders(t);

				}

			}

			// Instantiate wall blocks
			foreach (var b in blocks)
			{
				// Instantiate object
				InstantiateObject(b.data.prefabs[Random.Range(0, b.data.prefabs.Count)], b.origin.coords, b.rotationType);
			}


#endif
		}
		
		void CheckBorders(Tile tile)
        {
            bool[] dirs = new bool[4];
			dirs[0] = tile != outTile && GetTile(tile.coords.x, tile.coords.y + 1) < 0;
			dirs[1] = GetTile(tile.coords.x + 1, tile.coords.y) < 0;
			dirs[2] = /*tile != inTile && */GetTile(tile.coords.x, tile.coords.y - 1) < 0;
			dirs[3] = GetTile(tile.coords.x - 1, tile.coords.y) < 0;

			Debug.Log("Tile main object:" + tile.mainObject);

			Transform root = tile.mainObject.transform;
			if (tile == inTile || tile == outTile)
				root = root.GetChild(0);

			for(int i=0; i<dirs.Length; i++)
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
			var blocks= Resources.LoadAll<WallBlockAsset>($"{WallBlockAsset.ResourceFolder}/{theme}");
			Debug.Log($"Found {blocks.Count()} wall blocks");

			// Clear the available block list
			availableBlocks.Clear();
			// Fill the list
			foreach(var block in blocks)
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

			
        }

		void CloseBorders()
		{
            // Loop through each floor and check borders
			foreach(var tile in tiles)
			{
				bool[] dirs = new bool[4];
				dirs[0] = outTile != tile && GetTile(tile.coords.x, tile.coords.y + 1) < 0;
				dirs[1] = GetTile(tile.coords.x + 1, tile.coords.y) < 0;
				dirs[2] = inTile != tile && GetTile(tile.coords.x, tile.coords.y - 1) < 0;
				dirs[3] = GetTile(tile.coords.x - 1, tile.coords.y) < 0;

				if (dirs[0] || dirs[1] || dirs[2] || dirs[3])
				{
#if USE_HELPERS
					var go = Instantiate(borderHelperPrefab);
					go.transform.position = new Vector3(tile.coords.x, 0, tile.coords.y) * CellSize;
					for(int i=0; i<dirs.Length; i++)
                    {
						if (!dirs[i])
							go.transform.GetChild(i).gameObject.SetActive(false);

                    }
#endif

				}
            }
        }

		void AddInAndOut()
		{
			// Add the entrance to the south and the exit to the north
			var candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.down) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y < t.coords.y)).ToList();

			inTile = candidates[Random.Range(0, candidates.Count)];

			candidates = tiles.Where(t => t.type == 0 && GetTile(t.coords + Vector2.up) < 0 && !tiles.Exists(t2 => t2.coords.x == t.coords.x && t2.coords.y > t.coords.y)).ToList();

			outTile = candidates[Random.Range(0, candidates.Count)];

			FindFirstObjectByType<FirstPersonController>().transform.root.position = new Vector3(inTile.coords.x, 5f, inTile.coords.y) * CellSize;


#if USE_HELPERS
			var obj = Instantiate(inHelperPrefab);
			obj.transform.position = new Vector3(inTile.coords.x, 0, inTile.coords.y)  * CellSize;
			obj = Instantiate(outHelperPrefab);
			obj.transform.position = new Vector3(outTile.coords.x, 0, outTile.coords.y) * CellSize;
#endif

		}

		void CreateFlippedVariants()
		{
			Debug.Log("TO ADD Start");
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
			Debug.Log("TO ADD " + toAdd.Count);
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

			Debug.Log($"WallBlock Prefab:{data.prefabs[0].name}, t.count:{t.Count}");

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
			borders[0] = tiles.Where(t => GetTile(t.coords.x, t.coords.y + 1) < 0).OrderBy(t=>t.coords.y).ToList(); // North
			borders[1] = tiles.Where(t => GetTile(t.coords.x + 1, t.coords.y) < 0).OrderBy(t=>t.coords.x).ToList(); // East
			borders[2] = tiles.Where(t => GetTile(t.coords.x, t.coords.y - 1) < 0).OrderBy(t=>t.coords.y).ToList(); // South
			borders[3] = tiles.Where(t => GetTile(t.coords.x - 1, t.coords.y) < 0).OrderBy(t=>t.coords.x).ToList(); // West

			// var closestTile = GetClosestBorderToTheOrigin();
			// List<int> availableBorderTypes = new List<int>();
			// for (int i = 0; i < 4; i++)
			// {
			// 	if (!borders[i].Contains(closestTile)) borders[i].Clear();
			// 	else availableBorderTypes.Add(i);

			// }

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
								if (!rotatedTiles.Exists(t=>new Vector2(t.x,t.y) == new Vector2(rotatedTile.x, rotatedTile.y)-borderDir) && GetTile(placedTile - borderDir) < 0 && GetTile(placedTile - 2 * borderDir) == 0)
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
							for(int i=0; i<rotatedTiles.Count; i++)
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
#if USE_HELPERS
				CreateHelperTile(coords, type);
#endif
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
#if USE_WEIGHT
			// Minumum
			int count = 0;
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
	}
}
