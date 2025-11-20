using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class MazeBuilder : MonoBehaviour
	{
		[System.Serializable]
		class WallBlockData
		{
			public List<Vector2> tiles;

			public int min;

			public int max;

			public int count;


		}

		


		[System.Serializable]
		class Tile
        {
			public Vector2 coords;

			public int type; // 0: floor, 1: wall

			
        }
		

		[SerializeField]
		GameObject wallPrefab;

		[SerializeField]
		GameObject floorPrefab;

		[SerializeField]
		List<WallBlockData> availableBlocks = new List<WallBlockData>();

		float cellSize = 2;

		int wallMax = 20;

		List<Tile> tiles = new List<Tile>();

		

		// Start is called before the first frame update
		void Start()
	    {
			ChooseBlocks();

			CreateMaze();
	    }

		// Update is called once per frame
		void Update()
		{

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

					List<Vector2> tiles = RotateTiles(block.tiles, Random.Range(0, 4));

					// Add tiles
					AddTiles(tiles, 1);

					BorderWithFloor(tiles);
				}
                else
				{
					if (i > 4) return;
					AddAnotherBlock();
					
                }
			}
		}

		void AddAnotherBlock()
		{
			// Get borders
			List<Tile>[] borders = new List<Tile>[4];
			borders[0] = tiles.Where(t => GetTile(t.coords.x, t.coords.y + 1) < 0).ToList(); // North
			borders[1] = tiles.Where(t => GetTile(t.coords.x + 1, t.coords.y) < 0).ToList(); // East
			borders[2] = tiles.Where(t => GetTile(t.coords.x, t.coords.y - 1) < 0).ToList(); // South
			borders[3] = tiles.Where(t => GetTile(t.coords.x - 1, t.coords.y) < 0).ToList(); // West

			int borderType = 0; // Top


			var borderDirs = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

			bool done = false;

			// Get one element for each available block
			var candidates = new List<WallBlockData>();
			foreach (WallBlockData block in availableBlocks)
			{
				if (candidates.Contains(block)) continue;

				candidates.Add(block);
			}



			while (candidates.Count > 0 && !done)
			{
				// Choose the next candidate
				var candidate = candidates[Random.Range(0, candidates.Count)];
				candidates.Remove(candidate);

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

							if (GetTile(placedTile - borderDirs[borderType]) < 0 && GetTile(placedTile - 2 * borderDirs[borderType]) == 0)
							{
								done = false;
								break;
							}

							switch (borderType)
							{
								case 0:
									if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x - 1) && GetTile(placedTile.x - 1, placedTile.y - 1) < 0 && GetTile(placedTile.x - 1, placedTile.y - 2) == 0)
										done = false;
									if (!rotatedTiles.Exists(t => t.y == rotatedTile.y && t.x == rotatedTile.x + 1) && GetTile(placedTile.x + 1, placedTile.y - 1) < 0 && GetTile(placedTile.x + 1, placedTile.y - 2) == 0)
										done = false;
									break;
							}

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
				CreateHelperTile(coords, type);
			}
        }
		
		void CreateHelperTile(Vector2 coords, int type)
        {
			GameObject tile = Instantiate(type == 0 ? floorPrefab : wallPrefab);
			Vector3 pos = new Vector3(coords.x, 0, coords.y) * cellSize;
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
					if (bp.max > 0 && bp.count >= bp.max) continue; // Maximum reached

					// Add prefab
					tmp.Add(bp);

				}

				// Choose the next one
				var wbd = tmp[Random.Range(0, tmp.Count)];
				wbd.count++;
				
			}



		}
	}
}
