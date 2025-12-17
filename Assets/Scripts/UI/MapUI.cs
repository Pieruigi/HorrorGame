using System;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
	public class MapUI : MonoBehaviour
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		GameObject wallPrefab;

		[SerializeField]
		GameObject floorPrefab;

		[SerializeField]
		GameObject miniGamePrefab;

		[SerializeField]
		Transform mapRoot;

		[SerializeField]
		GameObject pinPrefab;

		bool open = false;

		float fadeTime = .25f;

		float cellSize = 100;

		FirstPersonController fpc;


		int enterTIleIndex;

		List<GameObject> floors = new List<GameObject>();

		Vector3 playerStartingPosition;
		Vector3 rootStartingPosition;

		float mapRatio;

		List<GameObject> pointsOfInterest = new List<GameObject>();

		// Start is called before the first frame update
		void Start()
	    {
			fpc = FindFirstObjectByType<FirstPersonController>();
			playerStartingPosition = fpc.transform.position;
			canvasGroup.alpha = 0;
	    }

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (open)
					Close();
				else
					Open();
			}

			
		}

        void LateUpdate()
		{
			UpdatePositionAndRotation();

			UpdatePointsOfInterest();
        }

        void OnEnable()
		{
			MazeBuilder.OnMazeCreated += Create;
		}

        void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= Create;
        }

		void UpdatePositionAndRotation()
		{
			var playerPos = fpc.transform.position;
			var diff = playerPos - playerStartingPosition;
			// Move map
			var move = Vector3.zero;
			move.x = diff.x * mapRatio;
			move.y = diff.z * mapRatio;
			mapRoot.transform.localPosition = rootStartingPosition - move;

			var yaw = fpc.transform.eulerAngles.y;
			mapRoot.parent.localEulerAngles = Vector3.forward * yaw;
		}

		void UpdatePointsOfInterest()
		{
			float width = (canvasGroup.transform as RectTransform).rect.width;
			float height = (canvasGroup.transform as RectTransform).rect.height;
			//var rect = (mapRoot as RectTransform).rect;
			var playerPos = (mapRoot.parent as RectTransform).anchoredPosition;
			var rootPos = (mapRoot as RectTransform).anchoredPosition - playerPos;

			var yaw = fpc.transform.eulerAngles.y;

			Debug.Log($"Rect:{playerPos}");
			foreach(var poi in pointsOfInterest)
			{
				var poiPos = (poi.transform as RectTransform).anchoredPosition + rootPos;

				var atanPoi = Mathf.Atan(poiPos.y / poiPos.x) * Mathf.Deg2Rad;
				var angle = atanPoi - yaw;
				angle *= Mathf.Deg2Rad;
				var poiX = poiPos.x * Mathf.Cos(angle) + poiPos.y * Mathf.Sin(angle);
				var poiY = -poiPos.x * Mathf.Sin(angle) + poiPos.y * Mathf.Cos(angle);
				Debug.Log($"POI(X,Y):{poiX},{poiY}");
				
				Debug.Log($"POI.Rect:{poiPos}");
				if (Mathf.Abs(poiX) < width/2f && Mathf.Abs(poiY) < height/2f)
					Debug.Log($"POI is visibile");
				else
				 	Debug.Log($"POI is NOT visibile");
			}
		}

		void Open()
		{
			if (open) return;
			open = true;
			canvasGroup.DOKill();
			canvasGroup.DOFade(1, fadeTime);
		}
		
		void Close()
		{
			if (!open) return;
			open = false;
			canvasGroup.DOKill();
			canvasGroup.DOFade(0, fadeTime);
		}

		void Create()
		{
			mapRatio = cellSize / MazeBuilder.CellSize;

			var builder = MazeBuilder.Instance;
			// Get the number of tiles
			int count = builder.TileCount;

			Debug.Log($"MapUI - Tile count:{count}");

			// First simply add floor tiles
			for (int i = 0; i < count; i++)
			{
				if (builder.IsEnterTile(i))
					enterTIleIndex = i;

				var blockType = -1;
				if (builder.IsMiniGameController(i))
				{
					blockType = 1;
				}
				// Tile type
				var type = builder.GetTileType(i);
				// Return if tile is not a floor tile
				if (type != 0 && blockType < 0) continue;

				// Get coordinates
				var coords = builder.GetTileCoords(i);
				// Get prefab
				var prefab = floorPrefab;
				if (blockType == 1)
					prefab = miniGamePrefab;
				// Create a new map floor and add to the root
				var mf = Instantiate(prefab, mapRoot);
				mf.name = $"T-{i.ToString("000")}";
				mf.transform.localPosition = coords * cellSize;
				mf.transform.localRotation = Quaternion.identity;

				floors.Add(mf);

				CheckBlocks();

				if (blockType >= 0)
					pointsOfInterest.Add(mf);

				

				// Hide floor
				//mf.GetComponent<Image>().enabled = false;

				if (blockType < 0)
					CreateWalls(coords, mf.transform);


			}

			// Move to starting position
			MoveToStartingPosition();


		}
		
		void CheckBlocks()
		{
			var builder = MazeBuilder.Instance;
			int count = builder.BlockCount;
			
			for(int i=0; i<count; i++)
			{
				var coords = builder.GetBlockCoords(i);
				var rotType = builder.GetBlockRotationType(i);
				var blockType = builder.GetBlockType(i);
				var otherCoords = Vector2.down;
				switch (rotType)
				{
					case 1:
						otherCoords = Vector2.left;
						break;
					case 2:
						otherCoords = Vector2.up;
						break;
					case 3:
						otherCoords = Vector2.right;
						break;
				}

				GameObject prefab = null;
				switch (blockType)
				{
					case 1:
						prefab = miniGamePrefab;
						break;
				}

				var mf = Instantiate(prefab, mapRoot);
				mf.name = $"T-{i.ToString("000")}-{blockType}";
				mf.transform.localPosition = coords * cellSize;
				mf.transform.localRotation = Quaternion.identity;
			}
		}

		void MoveToStartingPosition()
		{
			var builder = MazeBuilder.Instance;
			var enterCoords = builder.GetTileCoords(enterTIleIndex);
			mapRoot.transform.localPosition = -enterCoords * cellSize;
			rootStartingPosition = mapRoot.transform.localPosition;
		}

		void CreateWalls(Vector2 coords, Transform parent)
		{
			var builder = MazeBuilder.Instance;
			for (int j = 0; j < 4; j++)
				{
					var otherCoords = coords + Vector2.up;
					if (j == 1)
						otherCoords = coords + Vector2.right;
					else if (j == 2)
						otherCoords = coords - Vector2.up;
					else if (j == 3)
						otherCoords = coords - Vector2.right;

					var otherIndex = builder.GetTileIndex(otherCoords);
					if (otherIndex < 0 || builder.GetTileType(otherIndex) != 0)
					{
						var wl = Instantiate(wallPrefab, parent);
						wl.transform.localPosition = Vector3.zero;
						wl.transform.localEulerAngles = Vector3.forward * -90f * j;		
					}
		
					// Get the other type
					// var otherType = builder.GetTileType(otherIndex);
					// if (otherType == 0) continue;

					// Create wall
					//CreateWall(mf.transform, j);
				}
		}
	}
}
