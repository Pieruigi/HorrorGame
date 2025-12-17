using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM.UI
{
	public class MapUI : MonoBehaviour
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		GameObject mapWallPrefab;

		[SerializeField]
		GameObject mapFloorPrefab;

		[SerializeField]
		Transform mapRoot;

		bool open = false;

		float fadeTime = .25f;

		float cellSize = 100;

		

	    // Start is called before the first frame update
	    void Start()
	    {
		
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

			if (open)
			{
				
			}
		}

		void OnEnable()
		{
			MazeBuilder.OnMazeCreated += Create;
		}

        void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= Create;
        }

        void Open()
		{
			if (open) return;
			open = false;
			canvasGroup.DOKill();
			canvasGroup.DOFade(1, fadeTime);
		}
		
		void Close()
		{
			if (!open) return;
			open = true;
			canvasGroup.DOKill();
			canvasGroup.DOFade(0, fadeTime);
		}
		
		void Create()
		{
			var builder = MazeBuilder.Instance;
			// Get the number of tiles
			int count = builder.TileCount;

			Debug.Log($"MapUI - Tile count:{count}");
			
			// First simply add floor tiles
			for(int i=0; i<count; i++)
			{
				// Tile type
				var type = builder.GetTileType(i);
				// Return if tile is not a floor tile
				if (type != 0) continue;

				// Get coordinates
				var coords = builder.GetTileCoords(i);
				// Create a new map floor and add to the root
				var mf = Instantiate(mapFloorPrefab, mapRoot);
				mf.name = $"T-{i.ToString("000")}";
				mf.transform.localPosition = coords * cellSize;
				mf.transform.localRotation = Quaternion.identity;
				
			}	
		}
	}
}
