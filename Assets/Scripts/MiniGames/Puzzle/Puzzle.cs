
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class Puzzle : MiniGame
	{
	 
		[SerializeField]
		List<GameObject> tiles;

		[SerializeField]
		float tiling = .125f;

		[SerializeField]
		AudioSource swooshAudioSource;

		[SerializeField]
		List<AudioClip> swooshClips;

		List<Vector3> originalPositions = new List<Vector3>();

		//List<int> order = new List<int>();

		int[] selections = new int[] { -1, -1 };

		float defaultZ = 0f;
		float selectedZ;

		GameObject over = null;

        int jumpscareScore = -1;

		Vector3[] selectionStartingPositions = new Vector3[2];



#if UNITY_EDITOR

		protected override void Awake()
		{
			base.Awake();

			// Initialize tiles
			InitializeTiles();

			// Shuffle
			ShuffleTiles();

		}
#endif


		protected override void Update()
        {
            base.Update();

			
			if (IsActive)
			{
				// Raycast
				var origin = Camera.main.transform.position;
				var direction = Camera.main.transform.forward;
				GameObject tmpOver = null;
				if (Physics.Raycast(origin, direction, out var ray, 4f, LayerMask.GetMask(new string[] { "Interactable" })))
				{
					if ("tile".Equals(ray.collider.gameObject.name.ToLower()))
					    tmpOver = ray.collider.gameObject;
                    	
				}
			



				// Check mouse over
				if (tmpOver || over)
				{
				
					if (over != tmpOver)
					{
						if (over != null)
						{
							ClearOver();
						}

						if (tmpOver)
						{
                            int tileIndex = tiles.IndexOf(ray.collider.gameObject);
                            if (!selections.Contains(tileIndex))
                            {
                                over = tmpOver;
                                over.transform.DOShakeRotation(.1f, over.transform.forward * 10).SetLoops(-1, LoopType.Restart);
                            }
                        }

						

					}
				}

				// Check mouse button
				if (over)
				{
					if (Input.GetMouseButtonDown(0))
					{
						// Clear over
						ClearOver();

                        int tileIndex = tiles.IndexOf(ray.collider.gameObject);
                        if (selections[0] < 0)
						{
							selections[0] = tileIndex;
							selectionStartingPositions[0] = tiles[tileIndex].transform.position;
							Select(tileIndex);
						}
						else
						{
							selections[1] = tileIndex;
                            selectionStartingPositions[1] = tiles[tileIndex].transform.position;
                            Select(tileIndex);
						}
					}
				}
			}
        }

        protected override void OnEnable()
        {
            base.OnEnable();

			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

        private void HandleOnMazeCreated()
        {
            // Initialize tiles
            InitializeTiles();

            // Shuffle
            ShuffleTiles();
        }

        void Select(int index)
		{
			if(jumpscareScore > 0)
			{
				jumpscareScore--;
				if (jumpscareScore == 0)
					MiniJumpscare.Play();
			}

			PlaySwoosh();

			var tile = tiles[index];
			var dest = tile.transform.localPosition;
			dest.z = selectedZ;
			tile.transform.DOLocalMoveZ(selectedZ, .1f).SetEase(Ease.OutBack).OnComplete(() => 
			{ 
				tile.transform.localPosition = dest;

				if (selections[0]  >= 0 && selections[1] >= 0)
				{
					// Switch tiles
					Debug.Log($"TEST - Switching tiles [{selections[0]},{selections[1]}]");
					var tile1 = tiles[selections[0]];
                    var tile2 = tiles[selections[1]];
					var dest1 = tile2.transform.position;
					var dest2 = tile1.transform.position;
                    var seq = DOTween.Sequence();
					seq.Append(tile1.transform.DOMove(dest1, .2f).SetEase(Ease.OutBack));
                    seq.Join(tile2.transform.DOMove(dest2, .2f).SetEase(Ease.OutBack));
					seq.AppendCallback(() =>
					{
						var d = tile1.transform.localPosition;
						d.z = defaultZ;
						tile1.transform.localPosition = d;
                        d = tile2.transform.localPosition;
                        d.z = defaultZ;
                        tile2.transform.localPosition = d;
						selections[0] = selections[1] = -1;

						// Check if puzzle has been solved
						if(CheckBeaten())
							ReportBeaten();
                    });
                }
			});
		}

		void PlaySwoosh()
		{
			swooshAudioSource.clip = swooshClips[Random.Range(0, swooshClips.Count)];
			swooshAudioSource.Play();
		}

        private bool CheckBeaten()
        {
            for(int i=0; i<tiles.Count; i++)
			{
				if (Vector3.Distance(tiles[i].transform.position, originalPositions[i]) > 0.0001f)
					return false;
			}

			return true;
        }

        void ClearOver()
		{
			if (!over) return;
            over.transform.DOKill();
            over.transform.localEulerAngles = Vector3.forward * 180;
            over = null;
        }

        private void ShuffleTiles()
        {
			int size = (int)Mathf.Sqrt(tiles.Count);

            //var tmpPositions = originalPositions.ToList();
			List<Vector3> tmpPositions = new List<Vector3>();
			for (int i = 0; i < originalPositions.Count; i++)
			{
				if(i % size != 0)
					tmpPositions.Add(originalPositions[i]);
			}

			foreach (var tile in tiles)
			{
				int index = tiles.IndexOf(tile);
				
                if (index % size != 0)
                {
                    var newPos = tmpPositions[Random.Range(0, tmpPositions.Count)];
                    tmpPositions.Remove(newPos);
                    tile.transform.position = newPos;
                }
				
			}
        }

        void InitializeTiles()
		{
			// Set materials
			int size = (int)Mathf.Sqrt(tiles.Count);

			int rows = size;
			int cols = size;

			var mat = tiles[0].GetComponent<Renderer>().material;

            for (int i = 0; i < rows; i++) // Row
			{
				for(int j = 0; j < cols; j++) // Column
				{
                    var rend = tiles[i*size + j].GetComponent<Renderer>();
			        
                    var newMat = new Material(mat);
					newMat.SetTextureScale("_BaseMap", new Vector2(tiling, tiling));
					newMat.SetTextureOffset("_BaseMap", new Vector2(tiling * j,1f - (tiling * (i+1))));

                    rend.material = newMat;
                }
			}

			// Store original positions
			foreach (var tile in tiles)
			{
				originalPositions.Add(tile.transform.position);
			}

			defaultZ = tiles[0].transform.localPosition.z;
			selectedZ = defaultZ - .1f;

		}

        public override void DoChildActivation()
        {
            base.DoChildActivation();
        }

        public override void DoChildDeactivation()
        {
            base.DoChildDeactivation();

            //for (int i = 0; i < selections.Length; i++)
            //{
            //	if (selections[i] < 0) continue;

            //	Deselect(selections[i]);
            //	selections[i] = -1;
            //}
            ClearOver();

            if (selections[0] >= 0)
			{
				tiles[selections[0]].transform.DOKill();
				tiles[selections[0]].transform.position = selectionStartingPositions[0];
				selections[0] = -1;
			}

            if (selections[1] >= 0)
            {
                tiles[selections[1]].transform.DOKill();
                tiles[selections[1]].transform.position = selectionStartingPositions[1];
                selections[1] = -1;
            }

            
        }

		void Deselect(int selection)
		{
			if (selection < 0) return;

			var dest = tiles[selection].transform.localPosition;
			dest.z = defaultZ;

			var tile = tiles[selection];
			tile.transform.DOKill();
			tile.transform.DOLocalMoveZ(selectedZ, .1f).SetEase(Ease.OutBack).OnComplete(() => { tile.transform.localPosition = dest; });

		}


        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {
            Debug.Log("TEST - Minijumpscare initialization");

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific score
            jumpscareScore = Random.Range(5, 12);
        }

    }
}
