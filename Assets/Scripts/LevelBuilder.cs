using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace TMM
{
	public class LevelBuilder : MonoBehaviour
	{
		[System.Serializable]
		class WallBlockData
		{
			public GameObject prefab;

			public int min = 1;

			public int max = 3;

			public int count = 0;

		}



		[SerializeField]
		GameObject floorPrefab;

		[SerializeField]
		List<WallBlockData> blockPrefabs;

		[SerializeField]
		List<GameObject> blocks;


		[SerializeField]
		List<GameObject> floors;

		[SerializeField]
		int cellSize = 2;

		float epsilon = 0.01f;



		int wallMax = 20;

		// Start is called before the first frame update
		void Start()
		{
			// Choose blocks to use
			ChooseBlocks();

			// Create the maze
			CreateMaze();
		}

		// Update is called once per frame
		void Update()
		{

		}

		void ChooseBlocks()
		{
			// Minumum
			int count = 0;
			foreach (var bp in blockPrefabs)
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
				foreach (var bp in blockPrefabs)
				{
					if (bp.max > 0 && bp.count >= bp.max) continue; // Maximum reached

					// Add prefab
					tmp.Add(bp);

				}

				// Choose the next one
				var wbd = tmp[Random.Range(0, tmp.Count)];
				wbd.count++;
				
			}



		}

		void CreateMaze()
		{
			for (int i = 0; i < wallMax; i++)
			{
				// Get a random block
				var bl = blockPrefabs.Where(b => b.count > 0).ToList();
				var block = bl[Random.Range(0, bl.Count)];
				block.count--;

				// Create the game object
				GameObject wall = Instantiate(block.prefab);

				if (i == 0)
				{


					// Apply a random rotation
					wall.transform.eulerAngles = Vector3.up * 90f * Random.Range(0, 4);

					// Add to the wall list
					blocks.Add(wall);

					// Set position
					wall.transform.position = Vector3.zero;

					// Place floor all around
					PlaceFloorAllAround(wall);

				}
				else
				{
					if (i > 1) return;
					if (!TryAddWall(wall))
					{
						// Destroy wall
						Destroy(wall);
						block.count++; // Increase back the counter

						Debug.LogError("Can't add wall!!!");
						break;
					}
                    else
                    {
                        // Place floor all around
						//PlaceFloorAllAround(wall);
                    }
				}
			}
		}

		int GetTile(float x, float z)
		{
			Debug.Log($"GetTile({x},{z})");
			foreach (var block in blocks)
			{
				var walls = block.GetComponentsInChildren<Transform>();
				foreach (var wall in walls)
				{
					if (wall.position.x == x && wall.position.z == z)
                    {
						Debug.Log($"GetTile({x},{z}) => 1");
							return 1;
                    }
						
                    	
				}
			}

			foreach (var floor in floors)
			{
				if (floor.transform.position.x == x && floor.transform.position.z == z)
                {
					Debug.Log($"GetTile({x},{z}) => 0");
					return 0;
                }
					
			}
					Debug.Log($"GetTile({x},{z}) => -1");
			return -1;
        }

		bool TryAddWall(GameObject wall)
		{
			// Get boundaries
			//var top = floors.Where(f => !floors.Exists(f1 => f1.transform.position.x == f.transform.position.x && f1.transform.position.z == f.transform.position.z + cellSize) && !blocks.Exists(w => w.transform.position.x == f.transform.position.x && w.transform.position.z == f.transform.position.z + cellSize)).ToList();

			var top = floors.Where(f => GetTile(f.transform.position.x, f.transform.position.z + cellSize) < 0).ToList();

			var right = floors.Where(f => !floors.Exists(f1 => f1.transform.position.x == f.transform.position.x + cellSize)).ToList();
			var bottom = floors.Where(f => !floors.Exists(f1 => f1.transform.position.z == f.transform.position.z - cellSize)).ToList();
			var left = floors.Where(f => !floors.Exists(f1 => f1.transform.position.x == f.transform.position.x - cellSize)).ToList();

			Debug.Log("TopCOunt:" + top.Count);
			

			List<GameObject>[] tot = new List<GameObject>[4];
			tot[0] = top;
			tot[1] = right;
			tot[2] = bottom;
			tot[3] = left;
						
			int count = top.Count + right.Count + bottom.Count + left.Count;

			for(int i=0; i<count; i++)
			{
				List<int> dirs = new List<int> { 0, 1, 2, 3 };
				if (top.Count == 0) dirs.Remove(0);
				if (right.Count == 0) dirs.Remove(1);
				if (bottom.Count == 0) dirs.Remove(2);
				if (left.Count == 0) dirs.Remove(3);
				int dir = dirs[Random.Range(0, dirs.Count)];
#if UNITY_EDITOR			
				dir = 0;
				// if (tot[dir].Count == 0)
				// 	dir = 1;

#endif

				var floor = tot[dir][Random.Range(0, tot[dir].Count)];
				tot[dir].Remove(floor);

				Debug.Log("Floor:" + floor.transform.position);

				// Get all the cells of the wall
				List<Transform> cells = wall.GetComponentsInChildren<Transform>().ToList();

                // Place the cell 
                switch (dir)
                {
					case 0:
						wall.transform.position = floor.transform.position + Vector3.forward * cellSize;
						var c = cells.Min(c => c.transform.position.z);
						wall.transform.position += Mathf.Abs(wall.transform.position.z - c) * Vector3.forward;
						break;
					case 1:
						wall.transform.position = floor.transform.position + Vector3.right * cellSize;
						c = cells.Min(c => c.transform.position.x);
						wall.transform.position += Mathf.Abs(wall.transform.position.x - c) * Vector3.right;
						break;
					case 2:
						wall.transform.position = floor.transform.position + Vector3.back * cellSize;
						c = cells.Max(c => c.transform.position.z);
						wall.transform.position += Mathf.Abs(wall.transform.position.z - c) * Vector3.back;
						break;
					case 3:
						wall.transform.position = floor.transform.position + Vector3.left * cellSize;
						c = cells.Max(c => c.transform.position.x);
						wall.transform.position += Mathf.Abs(wall.transform.position.x - c) * Vector3.left;
						break;
                }

				// Check every cell in the wall block we have chosen
				bool found = true;
				foreach (var cell in cells)
				{
					switch (dir)
					{
						case 0:
							if (!cells.Exists(c => c.transform.position.z == cell.transform.position.z - cellSize)) // South border (no other cell below)
							{
								Debug.Log($"Tile name:{cell.name}");

								// If there is a hole and then a floor under the cell fail
								if (!floors.Exists(f => f.transform.position.z == cell.transform.position.z - cellSize && f.transform.position.x == cell.transform.position.x) &&
									 floors.Exists(f => f.transform.position.z == cell.transform.position.z - 2f * cellSize && f.transform.position.x == cell.transform.position.x)
									)
								{
									Debug.Log($"Tile name:{cell.name} found is false");
									found = false;
								}
							}
							
							// No cell to the right
							// if(!cells.Exists(c=>c.transform.position.x == cell.transform.position.x + cellSize))
							// {
                            //     // Check bottom right coord
							// 	if(!floors.Exists(f => f.transform.position.x == cell.transform.position.x + cellSize && f.transform.position.z == cell.transform.position.z - cellSize) &&
							// 	    floors.Exists(f=> f.transform.position.x == cell.transform.position.x + cellSize && f.transform.position.z == cell.transform.position.z - 2 + cellSize))
                            //    	{
							// 		found = false;
                            // 	}
                            // }

							// if (!cells.Exists(c => Mathf.Abs(c.transform.position.x - cell.transform.position.x - cellSize) < epsilon)) // South east border
							// {
							// 	// If there is a hole and then a floor under the cell fail
							// 	if (!floors.Exists(f => Mathf.Abs(f.transform.position.z - cell.transform.position.z + cellSize) < epsilon &&
							// 							Mathf.Abs(f.transform.position.x - cell.transform.position.x - cellSize) < epsilon) &&
							// 		floors.Exists(f => Mathf.Abs(f.transform.position.z - cell.transform.position.z + 2f * cellSize) < epsilon &&
							// 						   Mathf.Abs(f.transform.position.x - cell.transform.position.x - cellSize) < epsilon))
							// 	{
							// 		found = false;
							// 	}
							// }
							// if(!cells.Exists(c => Mathf.Abs(c.transform.position.x - cell.transform.position.x + cellSize) < epsilon)) // South west border
							// {
							// 	// If there is a hole and then a floor under the cell fail
							// 	if (!floors.Exists(f => Mathf.Abs(f.transform.position.z - cell.transform.position.z + cellSize) < epsilon && 
							// 	                        Mathf.Abs(f.transform.position.x - cell.transform.position.x + cellSize) < epsilon) && 
							// 		floors.Exists(f => Mathf.Abs(f.transform.position.z - cell.transform.position.z + 2f * cellSize) < epsilon &&
							// 						   Mathf.Abs(f.transform.position.x - cell.transform.position.x + cellSize) < epsilon))
							// 	{
							// 		found = false;
							// 	}    
                            // }
							break;
						case 1:
							if (!cells.Exists(c => Mathf.Abs(c.transform.position.x - cell.transform.position.x + cellSize) < epsilon)) // West border
							{
								// If there is a hole and then a floor under the cell fail
								if (!floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + cellSize) < epsilon) &&
									floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + 2f * cellSize) < epsilon))
								{
									found = false;
								}
							}
							if (!cells.Exists(c => Mathf.Abs(c.transform.position.x - cell.transform.position.x + cellSize) < epsilon &&
												Mathf.Abs(c.transform.position.z - cell.transform.position.z - cellSize) < epsilon)) // North west border
							{
								// If there is a hole and then a floor under the cell fail
								if (!floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + cellSize) < epsilon &&
														Mathf.Abs(f.transform.position.z - cell.transform.position.z - cellSize) < epsilon) &&
									floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + 2f * cellSize) < epsilon &&
													   Mathf.Abs(f.transform.position.z - cell.transform.position.z - cellSize) < epsilon))
								{
									found = false;
								}
							}
							// if(!cells.Exists(c=>Mathf.Abs(c.transform.position.x - cell.transform.position.x + cellSize) < epsilon &&
							//     			    Mathf.Abs(c.transform.position.z - cell.transform.position.z + cellSize) < epsilon)) // South west border
							// {
							// 	// If there is a hole and then a floor under the cell fail
							// 	if (!floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + cellSize) < epsilon && 
							// 	                        Mathf.Abs(f.transform.position.z - cell.transform.position.z + cellSize) < epsilon) && 
							// 		floors.Exists(f => Mathf.Abs(f.transform.position.x - cell.transform.position.x + 2f * cellSize) < epsilon &&
							// 						   Mathf.Abs(f.transform.position.z - cell.transform.position.z + cellSize) < epsilon))
							// 	{
							// 		found = false;
							// 	}    
                            // }
							break;	
					}

					if (!found)
						break;

				}

				if (found)
					return true;
            }

			return false;
        }

		void PlaceFloorAllAround(GameObject wall)
		{
			var l = wall.GetComponentsInChildren<Transform>().ToList();
			Debug.Log("L:" + l.Count);

			foreach (var cell in l)
			{
				AddTopFloor(cell, l);
				AddTopRightFloor(cell, l);
				AddRightFloor(cell, l);
				AddBottomRightFloor(cell, l);
				AddBottomFloor(cell, l);
				AddBottomLeftFloor(cell, l);
				AddLeftFloor(cell, l);
				AddTopLeftFloor(cell, l);
			}
		}

		void AddTopFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x;
			var z = cell.transform.position.z + cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c=>c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddTopRightFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x + cellSize;
			var z = cell.transform.position.z + cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddRightFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x + cellSize;
			var z = cell.transform.position.z;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddBottomRightFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x + cellSize;
			var z = cell.transform.position.z - cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddBottomFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x;
			var z = cell.transform.position.z - cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddBottomLeftFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x - cellSize;
			var z = cell.transform.position.z - cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

		void AddLeftFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x - cellSize;
			var z = cell.transform.position.z;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c => c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}
		
		void AddTopLeftFloor(Transform cell, List<Transform> cells)
		{
			var x = cell.transform.position.x - cellSize;
			var z = cell.transform.position.z + cellSize;
			var pos = new Vector3(x, 0, z);
			if (!floors.Exists(f => f.transform.position == pos) && !cells.Exists(c=>c.transform.position == pos))
			{
				var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
				floors.Add(floor);
			}
		}

	}
}
