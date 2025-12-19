using System;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace TMM.UI
{
	public class MapUI : Singleton<MapUI>
	{
		class PointOfInterest
		{
			public enum Type { MiniGame, VendingMachine, PressurePlate, CoinPicker }
			
			public GameObject mapObject;

			public Type type;

			public PinMap pin;

			public int tileIndex = -1;
		}

		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		GameObject wallPrefab;

		[SerializeField]
		GameObject floorPrefab;

		[SerializeField]
		GameObject miniGamePrefab;

		[SerializeField]
		GameObject pressurePlatePrefab;

		[SerializeField]
		GameObject coinPickerPrefab;

		
		[SerializeField]
		Transform mapRoot;

		[SerializeField]
		GameObject pinPrefab;

		bool open = false;
		public bool IsOpen
		{
			get{ return open; }
		}

		float fadeTime = .25f;

		float cellSize = 100;

		FirstPersonController fpc;


		int enterTIleIndex;

		List<GameObject> floors = new List<GameObject>();

		Vector3 playerStartingPosition;
		Vector3 rootStartingPosition;

		float mapRatio;

		List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

	    protected override void Awake()
		{
			base.Awake();
            mapRatio = cellSize / MazeBuilder.CellSize;
        }

        // Start is called before the first frame update
        void Start()
	    {
			
			
			canvasGroup.alpha = 0;
	    }

		// Update is called once per frame
		void Update()
		{
			
		}

        void LateUpdate()
		{
			UpdatePositionAndRotation();

			UpdatePointsOfInterest();
			
        }

        void OnEnable()
		{
			MazeBuilder.OnMazeCreated += Create;
			CoinPicker.OnCoinPicked += HandleOnCoinPicked;
		}

        void OnDisable()
        {
			MazeBuilder.OnMazeCreated -= Create;
			CoinPicker.OnCoinPicked -= HandleOnCoinPicked;
        }

        private void HandleOnCoinPicked(CoinPicker coinPicker)
        {
			int tileIndex = MazeBuilder.Instance.GetTileIndex(coinPicker);
			var poi = pointsOfInterest.Find(p => p.type == PointOfInterest.Type.CoinPicker && p.tileIndex == tileIndex);
			// Remove poi from the list
			pointsOfInterest.Remove(poi);
			// Remove both map and pin elements
			Destroy(poi.mapObject);
			Destroy(poi.pin);
			
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

			foreach(var poi in pointsOfInterest)
			{
				//if (poi.type != PointOfInterest.Type.MiniGame) continue;
				var poiPos = (poi.mapObject.transform as RectTransform).anchoredPosition + rootPos;

				
				var atanPoi = Mathf.Atan(poiPos.y / poiPos.x) * Mathf.Deg2Rad;
				var angle = atanPoi - yaw;
				angle *= Mathf.Deg2Rad;
				var poiX = poiPos.x * Mathf.Cos(angle) + poiPos.y * Mathf.Sin(angle); // X relative to player screen
				var poiY = -poiPos.x * Mathf.Sin(angle) + poiPos.y * Mathf.Cos(angle); // Y relative to player screen
				
				if (Mathf.Abs(poiX) < width/2f && Mathf.Abs(poiY) < height / 2f)
				{
					poi.pin.Hide();
				}
				else
				{

					var pin = poi.pin;
					pin.Show();
					var h = height / 2f - 20;
					var w = width / 2f - 20;
					float x = 0, y = 0;
					// Check Y
					if (poiY > h || poiY < -h)
					{
						// Horizontal intersection
						y = poiY > h ? h : -h;
						x = y * poiX / poiY;

						if(x > w || x < -w)
						{
							// Vertical
							x = x > w ? w : -w;
							y = x * poiY / poiX;
						}
					

						
					}
					else // Check X
					{
						if(poiX > w || poiX < -w)
						{
							x = poiX > w ? w : -w;
							y = x * poiY / poiX;
						}
					}


					// Move pin out
					var oldParent = pin.transform.parent;
					pin.transform.parent = canvasGroup.transform;
					//pin.transform.SetParent(canvasGroup.transform, false);
					(pin.transform as RectTransform).anchoredPosition = new Vector2(x, y);
					pin.transform.parent = oldParent;
					
				}
				 	
			}
		}

		public void Open()
		{
			if (open) return;
			open = true;
			canvasGroup.DOKill();
			canvasGroup.DOFade(1, fadeTime);
		}
		
		public void Close()
		{
			if (!open) return;
			open = false;
			canvasGroup.DOKill();
			canvasGroup.DOFade(0, fadeTime);
		}

		void Create()
		{
			fpc = FindFirstObjectByType<FirstPersonController>();
			playerStartingPosition = fpc.transform.position;

			CheckFloor();

			CheckBlocks();

			CheckSpawnables();

			CreatePins();

			// Move to starting position
			MoveToStartingPosition();

		}
		
		void CheckSpawnables()
		{
			
		}

		void CreatePins()
		{
			foreach(var poi in pointsOfInterest)
			{
				var pin = Instantiate(pinPrefab, mapRoot);
				poi.pin = pin.GetComponent<PinMap>();

				if (poi.type == PointOfInterest.Type.MiniGame || poi.type == PointOfInterest.Type.VendingMachine)
					poi.pin.SetGoodPin();
				else if (poi.type == PointOfInterest.Type.CoinPicker)
					poi.pin.SetGoldPin();
				else
					poi.pin.SetBadPin();

			}
		}

		void CheckFloor()
		{
			var builder = MazeBuilder.Instance;
			// Get the number of tiles
			int count = builder.TileCount;

			Debug.Log($"MapUI - Tile count:{count}");

			// First simply add floor tiles
			for (int i = 0; i < count; i++)
			{
				if (builder.IsEnterTile(i))
					enterTIleIndex = i;

				// Tile type
				var type = builder.GetTileType(i);
				// Return if tile is not a floor tile
				if (type != 0) continue;

				// Get coordinates
				var coords = builder.GetTileCoords(i);
				// Create a new map floor and add to the root
				var mf = Instantiate(floorPrefab, mapRoot);
				mf.name = $"T-{i.ToString("000")}";
				mf.transform.localPosition = coords * cellSize;
				mf.transform.localRotation = Quaternion.identity;

				floors.Add(mf);

				CreateWalls(coords, mf.transform);


				// Check if is a trigger tile
				if (builder.IsTriggerTile(i))
				{
					// Add the pressure plate prefab
					GameObject pp = Instantiate(pressurePlatePrefab, mapRoot);
					pp.name = $"T-{i.ToString("000")}-B";
					pp.transform.localPosition = mf.transform.localPosition;
					pp.transform.localRotation = Quaternion.identity;

					PointOfInterest poi = new PointOfInterest();
					poi.mapObject = pp;
					poi.type = PointOfInterest.Type.PressurePlate;
					poi.tileIndex = i;
					pointsOfInterest.Add(poi);
				}

				// Check coins
				if (builder.TileHasCoin(i))
				{
					// Add the pressure plate prefab
					GameObject pp = Instantiate(coinPickerPrefab, mapRoot);
					pp.name = $"T-{i.ToString("000")}-G";
					pp.transform.localPosition = mf.transform.localPosition;
					pp.transform.localRotation = Quaternion.identity;

					PointOfInterest poi = new PointOfInterest();
					poi.mapObject = pp;
					poi.type = PointOfInterest.Type.CoinPicker;
					poi.tileIndex = i;
					pointsOfInterest.Add(poi);
				}

				// Check Exit tile
				if (builder.IsExitTile(i))
				{
					
				}

				// Hide floor
				//mf.GetComponent<Image>().enabled = false;

			}
			

		}

		void CheckBlocks()
		{
			var builder = MazeBuilder.Instance;
			int count = builder.BlockCount;
			
			for(int i=0; i<count; i++)
			{
				var blockType = builder.GetBlockType(i);
				if (blockType == 0) continue;

				var coords = builder.GetBlockCoords(i);
				var rotType = builder.GetBlockRotationType(i);
				
				var otherCoords = coords + Vector2.down;
				switch (rotType)
				{
					case 1:
						otherCoords = coords + Vector2.left;
						break;
					case 2:
						otherCoords = coords + Vector2.up;
						break;
					case 3:
						otherCoords = coords + Vector2.right;
						break;
				}

				var poi = new PointOfInterest();
				GameObject prefab = null;
				switch (blockType)
				{
					case 1:
						prefab = miniGamePrefab;
						poi.type = PointOfInterest.Type.MiniGame;
						break;
					case 2:
						prefab = miniGamePrefab;
						poi.type = PointOfInterest.Type.VendingMachine;
						break;
					default:
						prefab = miniGamePrefab;
						break;
				}

				var mf = Instantiate(prefab, mapRoot);
				mf.name = $"T-{i.ToString("000")}-{blockType}";
				mf.transform.localPosition = otherCoords * cellSize;
				mf.transform.localRotation = Quaternion.identity;

				
				poi.mapObject = mf;
				

				pointsOfInterest.Add(poi);
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
		
				
				}
		}
	}
}
