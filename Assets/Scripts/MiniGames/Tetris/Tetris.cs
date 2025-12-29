using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class Tetris : MiniGame
	{

		[SerializeField]
		List<GameObject> blockPrefabs;

		[SerializeField]
		List<GameObject> cellRows;

		[SerializeField]
		Transform currentBlockRoot;

		GameObject currentBlock;

		GameObject nextBlockPrefab;

		bool[,] gridCells;

		bool alignBlockToView;

		bool blockBusy = false;

		float rotationTime = .2f;

		protected override void Awake()
		{
			base.Awake();

		}

		protected override void Start()
		{
			base.Start();

			// Create cell grid
			int rows = cellRows.Count;
			int cols = cellRows[0].transform.childCount;
			gridCells = new bool[rows, cols];

			// Add cells to grid
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					gridCells[i, j] = cellRows[i].transform.GetChild(j);
					cellRows[i].transform.GetChild(j).gameObject.name = "E"; // E=empty, F=full
				}
					
			}


		}

		protected override void Update()
		{
			base.Update();

			if (IsActive)
			{
				var origin = Camera.main.transform.position;
				var direction = Camera.main.transform.forward;

				var distance = 3f;
				RaycastHit hitInfo;
				alignBlockToView = false;
				if (Physics.Raycast(origin, direction, out hitInfo, distance, LayerMask.GetMask(new string[] { "Interactable" })))
				{
					Debug.Log($"Tetris hit:{hitInfo.collider.transform.parent.gameObject.name}/{hitInfo.collider.gameObject.name}");

					// Check if the player is trying to insert the block
					if (Input.GetMouseButtonDown(0))
					{
						
						TryInsertCurrentBlock();
					}

					if (!blockBusy && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))) // Rotate left
					{
						blockBusy = true;
						var angle = currentBlock.transform.localEulerAngles;
						var rotAngle = Input.GetKeyDown(KeyCode.A) ? 90f : -90f;
						currentBlock.transform.DOLocalRotate(angle + Vector3.forward * rotAngle, rotationTime).SetEase(Ease.OutBounce).OnComplete(() =>
						{
							blockBusy = false;
							currentBlock.transform.localEulerAngles = angle + Vector3.forward * rotAngle;
						});
					}

					if (!blockBusy)
					{
						alignBlockToView = true;
					}

				}
				else
				{
					// We should hide the block
				}


				// Try align the block
				if (alignBlockToView)
				{
					// Vector from camera to current block root
					var orig = currentBlockRoot.position - Camera.main.transform.position;
					var cos = Mathf.Cos(Vector3.Angle(orig, Camera.main.transform.forward)*Mathf.Deg2Rad);
					// Distance along camera forward
					var dist = orig.magnitude / cos;
					// Get target position
					var targetPos = Camera.main.transform.position + dist * Camera.main.transform.forward;
					// Move
					currentBlock.transform.position = Vector3.MoveTowards(currentBlock.transform.position, targetPos, 5f * Time.deltaTime);
					

				}

			}
		}

		public override void DoChildActivation()
		{
			base.DoChildActivation();

			// Choose current and next blocks
			UpdateBlocks();

			alignBlockToView = false;
			blockBusy = false;
		}

		public override void DoChildDeactivation()
		{
			base.DoChildDeactivation();

			// Clear current and next blocks
			currentBlock = null;
			nextBlockPrefab = null;

		}
		
		void TryInsertCurrentBlock()
		{
			// Raycast from each piece of the block to the panel
			List<GameObject> cells = new List<GameObject>();

			bool failed = false;
			var count = currentBlock.transform.childCount;
			float rayDist = .25f;
			for (int i = 0; i < count && !failed; i++)
			{
				var child = currentBlock.transform.GetChild(i);
				var orig = child.position;
				var dir = -child.up;
				RaycastHit hitInfo;
				if (Physics.Raycast(orig, dir, out hitInfo, rayDist, LayerMask.GetMask(new string[] { "Interactable" })))
				{
					if("e".Equals(hitInfo.collider.gameObject.name.ToLower()))
						cells.Add(hitInfo.collider.gameObject);
				}
				else
				{
					failed = true;
				}
			}

			// If failed just shake the block
			if (failed)
			{
				// Do shake
			}
			else // Place the block
			{
				Material mat = currentBlock.transform.GetChild(0).GetComponent<Renderer>().material;
				foreach(var cell in cells)
				{
					cell.GetComponent<Renderer>().material = mat;
					cell.name = "F";
				}
			}


		}

		void UpdateBlocks()
		{
			var prefab = nextBlockPrefab;
			if (!prefab)
				prefab = blockPrefabs[Random.Range(0, blockPrefabs.Count)];

			// Instantiate current block
			currentBlock = Instantiate(prefab);
			currentBlock.transform.parent = currentBlockRoot;
			currentBlock.transform.localPosition = Vector3.zero;
			currentBlock.transform.localRotation = Quaternion.identity;
			
			// Choose the next block prefab
			var candidates = blockPrefabs.Where(b => b != prefab).ToList();
			nextBlockPrefab = candidates[Random.Range(0, candidates.Count)];

		}
    }
}
