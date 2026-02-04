using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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

		[SerializeField]
		Image nextBlockImage;

		[SerializeField]
		List<Sprite> sprites;

		[SerializeField]
		TMP_Text scoreField;

		[SerializeField]
		int score = 10;

		[SerializeField]
		AudioSource placeBlockAudioSource;

		[SerializeField]
		List<AudioClip> placeBlockClips;

		[SerializeField]
		AudioSource clearRowAudioSource;

		[SerializeField]
		List<AudioClip> clearRowClips;

		[SerializeField]
		AudioSource failedBlockAudioSource;

		[SerializeField]
		List<AudioClip> failedBlockClips;

		[SerializeField]
		AudioSource rotateAudioSource;

		[SerializeField]
		List<AudioClip> rotateClips;

		GameObject currentBlock;

		GameObject nextBlockPrefab;

		Material emptyMaterial;

		bool[,] gridCells;

		bool alignBlockToView;

		bool blockBusy = false;

		float rotationTime = .2f;

		Rect borders = new Rect();

		CanvasGroup miniCanvas;

		int jumpscareScore = -1;

		protected override void Awake()
		{
			base.Awake();
			miniCanvas = scoreField.transform.parent.GetComponent<CanvasGroup>();
			scoreField.text = score.ToString("00");
		}

		protected override void Start()
		{
			base.Start();

			// Create cell grid
			int rows = cellRows.Count;
			int cols = cellRows[0].transform.childCount;
			gridCells = new bool[rows, cols];

			// Add cells to grid
			bool top = false, bottom = false, left = false, right = false;
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					gridCells[i, j] = cellRows[i].transform.GetChild(j);
					var child = cellRows[i].transform.GetChild(j);
					child.gameObject.name = "E"; // E=empty, F=full

					var relPos = currentBlockRoot.InverseTransformPoint(child.position);

					if (!top || relPos.y > borders.yMax)
					{
						top = true;
						borders.yMax = relPos.y;
					}
					if (!right || relPos.x > borders.xMax)
					{
						right = true;
						borders.xMax = relPos.x;
					}
					if (!bottom || relPos.y < borders.yMin)
					{
						bottom = true;
						borders.yMin = relPos.y;
					}
					if (!left || relPos.x < borders.xMin)
					{
						left = true;
						borders.xMin = relPos.x;
					}
				}

			}

			Debug.Log($"Borders:{borders}");

			emptyMaterial = cellRows[0].transform.GetChild(0).GetComponent<Renderer>().material;


		}

		protected override void Update()
		{
			base.Update();

			if (IsActive)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					ClearBoard();
				}

				var origin = Camera.main.transform.position;
				var direction = Camera.main.transform.forward;

				var distance = 3f;
				RaycastHit hitInfo;
				alignBlockToView = false;
				if (Physics.Raycast(origin, direction, out hitInfo, distance, LayerMask.GetMask(new string[] { "Interactable" })))
				{
					//Debug.Log($"Tetris hit:{hitInfo.collider.transform.parent.gameObject.name}/{hitInfo.collider.gameObject.name}");

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

						// Play sound
						rotateAudioSource.clip = rotateClips[Random.Range(0, rotateClips.Count)];
						rotateAudioSource.Play();
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
					var orig = Vector3.Project(currentBlockRoot.position - Camera.main.transform.position, currentBlockRoot.forward);
					var cos = Mathf.Cos(Vector3.Angle(orig, Camera.main.transform.forward)*Mathf.Deg2Rad);
					// Distance along camera forward
					var dist = orig.magnitude / cos;
					// Get target position
					var targetPos = Camera.main.transform.position + dist * Camera.main.transform.forward;
					
					targetPos = ClampCurrentBlockPosition(targetPos);
					
					// Move
					currentBlock.transform.position = targetPos;// Vector3.MoveTowards(currentBlock.transform.position, targetPos, 5f * Time.deltaTime);
					

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
			Destroy(currentBlock);
			currentBlock = null;
			nextBlockPrefab = null;

		}
		
		Vector3 ClampCurrentBlockPosition(Vector3 position)
		{
			Vector3 clamped = currentBlockRoot.InverseTransformPoint(position);

			Vector3 oldPosition = currentBlock.transform.position;
			currentBlock.transform.position = position;

			int count = currentBlock.transform.childCount;

			List<Transform> children = new List<Transform>();
			for (int i = 0; i < count; i++)
			{
				children.Add(currentBlock.transform.GetChild(i));
			}
			var tmp = children.OrderBy(c => currentBlockRoot.InverseTransformPoint(c.position).x);
			Transform left = tmp.First();
			Transform right = tmp.Last();
			tmp = children.OrderBy(c => currentBlockRoot.InverseTransformPoint(c.position).y);
			Transform top = tmp.Last();
			Transform bottom = tmp.First();

			float xMin = currentBlockRoot.InverseTransformPoint(left.position).x;
			float xMax = currentBlockRoot.InverseTransformPoint(right.position).x;
			float yMin = currentBlockRoot.InverseTransformPoint(bottom.position).y;
			float yMax = currentBlockRoot.InverseTransformPoint(top.position).y;

			if (xMin < borders.xMin)
			{
				clamped.x = borders.xMin + Mathf.Abs(currentBlockRoot.InverseTransformPoint(currentBlock.transform.position).x - xMin);
			}

			if (xMax > borders.xMax)
			{
				clamped.x = borders.xMax - Mathf.Abs(currentBlockRoot.InverseTransformPoint(currentBlock.transform.position).x - xMax);// - 0.01f;
			}

			if (yMin < borders.yMin)
			{
				clamped.y = borders.yMin + Mathf.Abs(currentBlockRoot.InverseTransformPoint(currentBlock.transform.position).y - yMin);// + 0.01f;
			}

			if (yMax > borders.yMax)
			{
				clamped.y = borders.yMax - Mathf.Abs(currentBlockRoot.InverseTransformPoint(currentBlock.transform.position).y - yMax);// - 0.01f;
			}


			currentBlock.transform.position = oldPosition;

			return currentBlockRoot.TransformPoint(clamped);
		}

		void ClearBoard()
		{
			if (!cellRows.Exists(r => r.GetComponentsInChildren<Transform>().ToList().Exists(c => "f".Equals(c.gameObject.name.ToLower())))) return;
			if (blockBusy) return;

			blockBusy = true;
			float time = .2f;

			foreach (var row in cellRows)
			{
				int count = row.transform.childCount;
				for (int i = 0; i < count; i++)
				{
					var child = row.transform.GetChild(i);
					if ("e".Equals(child.gameObject.name.ToLower())) continue;

					child.gameObject.name = "E";
					//child.GetComponent<Renderer>().material = emptyMaterial;
					var seq = DOTween.Sequence();
					//seq.AppendInterval(time / 2f).OnComplete(() => { child.GetComponent<Renderer>().material = emptyMaterial; });
					StartCoroutine(SetCellFreeMaterialDelayed(child.gameObject, time / 2f));
					seq.Append(child.DOShakeRotation(time).SetEase(Ease.OutBounce).OnComplete(() => { child.localEulerAngles = Vector3.left * 90f; }));
					seq.OnComplete(() => { blockBusy = false; });
				}
			}
			
			clearRowAudioSource.clip = clearRowClips[Random.Range(0, clearRowClips.Count)];
			clearRowAudioSource.Play();
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
				
					if ("e".Equals(hitInfo.collider.gameObject.name.ToLower()))
						cells.Add(hitInfo.collider.gameObject);
					else
						failed = true;
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
				blockBusy = true;
				var eulers = currentBlock.transform.localEulerAngles;
				currentBlock.transform.DOShakeRotation(.1f).OnComplete(() => { currentBlock.transform.localEulerAngles = eulers; blockBusy = false; });

				// Play audio
				failedBlockAudioSource.clip = failedBlockClips[Random.Range(0, failedBlockClips.Count)];
				failedBlockAudioSource.Play();
			}
			else // Place the block
			{
				blockBusy = true;
				Material mat = currentBlock.transform.GetChild(0).GetComponent<Renderer>().material;
				foreach (var cell in cells)
					cell.name = "F";

				float time = .2f;
				StartCoroutine(SetCellsMaterialAndCheckRows(mat, cells, time / 2f));
				var seq = DOTween.Sequence();
				seq.Append(currentBlock.transform.DOPunchScale(Vector3.one * 1.2f, time));
				seq.Append(currentBlock.transform.DOScale(0, time).SetEase(Ease.OutBounce).OnComplete(() =>
				{
					Destroy(currentBlock);
					currentBlock = null;
					UpdateBlocks();
				}));

				placeBlockAudioSource.clip = placeBlockClips[Random.Range(0, placeBlockClips.Count)];
				placeBlockAudioSource.Play();


			}


		}
		
		IEnumerator SetCellsMaterialAndCheckRows(Material mat, List<GameObject> cells, float time)
		{
			yield return new WaitForSeconds(time);

			foreach (var cell in cells)
				cell.GetComponent<Renderer>().material = mat;

			CheckRows(cells, time);
		}

		void CheckRows(List<GameObject> cells, float time)
		{
			var playSound = false;
			foreach (var cell in cells)
			{
				// Get row
				var row = cell.transform.parent;
				bool setFree = true;
				int count = row.childCount;
				for (int i = 0; i < count && setFree; i++)
				{
					if ("e".Equals(row.GetChild(i).gameObject.name.ToLower()))
						setFree = false;
				}

				if (setFree)
				{
					playSound = true;
					for (int i = 0; i < count && setFree; i++)
					{
						var seq = DOTween.Sequence();
						var child = row.GetChild(i);
						child.gameObject.name = "E";
						StartCoroutine(SetCellFreeMaterialDelayed(child.gameObject, time / 2f));
						seq.Append(child.DOShakeRotation(time).OnComplete(() => { child.localEulerAngles = Vector3.left * 90f; }));
					}

					// Update score 
					score--;
					if (score < 0) score = 0;
					scoreField.text = score.ToString("00");

					if (jumpscareScore > 0)
						MiniJumpscare.Play();

				}
			}

			if(playSound)
			{
				clearRowAudioSource.clip = clearRowClips[Random.Range(0, clearRowClips.Count)];
				clearRowAudioSource.Play();
			}
			
			if (score <= 0)
				ReportBeaten();
		}
		
		IEnumerator SetCellFreeMaterialDelayed(GameObject cell, float delay)
		{
			yield return new WaitForSeconds(delay);
			cell.GetComponent<Renderer>().material = emptyMaterial;
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

			// Do scale
			blockBusy = true;
			float time = .2f;
			currentBlock.transform.localScale = Vector3.zero;
			var seq = DOTween.Sequence();
			seq.Append(currentBlock.transform.DOScale(1, time).SetEase(Ease.OutBounce));
			seq.Join(currentBlock.transform.DOShakePosition(time));
			seq.OnComplete(() => { blockBusy = false; });

			// Set sprite

			var sprite = sprites.Find(s => s.name.ToLower().EndsWith(nextBlockPrefab.name.Substring(nextBlockPrefab.name.Length - 2).ToLower()));
			
			nextBlockImage.sprite = sprite;

		}

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {
            Debug.Log("TEST - Minijumpscare initialization");

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific score
            jumpscareScore = Random.Range(2, score-2);
        }
    }
}
