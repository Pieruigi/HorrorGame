using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMM
{
	public class Colors : MiniGame
	{

		[SerializeField]
		GameObject pillarPrefab;

		[SerializeField]
		Transform pillarRoot;

		[SerializeField]
		AudioSource swooshAudioSource;

		[SerializeField]
		List<AudioClip> swooshClips;

		[SerializeField]
		AudioSource lockAudioSource;

		Pillar[] pillars;
		
		float pillarDistance = .15f;

        int numOfColumns = 9;
        
        int numOfRows = 6;
		int numOfColors = 4;

		float loadFactor = .8f;

		bool horizontalSymmetry = false;
		bool verticalSymmetry = false;

		Pillar selected = null;

		bool busy;

		protected override void Awake()
		{
			base.Awake();

			// Create
			Create();

			pillarRoot.eulerAngles = Vector3.right * -90;
			horizontalSymmetry = Random.Range(0, 2) == 0 ? true : false;
			verticalSymmetry = Random.Range(0, 2) == 0 ? true : false;
		}


        protected override void Update()
        {
			base.Update();

#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.X))
				CheckCompleted();
#endif

			if (IsActive)
			{
				if (busy) return;

				RaycastHit hit;
				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5))
				{
					Pillar pillar = hit.collider.GetComponent<Pillar>();
					if (pillar != selected)
					{
						if (selected) selected.StopShaking();
						selected = pillar;
						if (selected) selected.StartShaking();
					}
				}
				else
				{
					if (selected) selected.StopShaking();
					selected = null;
				}

				if (selected)
				{
					if (Input.GetMouseButtonDown(0))
					{
						busy = true;
						float angle = selected.transform.localEulerAngles.y + 90f;
						selected.StopShaking();
						//selected.SetPreshakeRotation(Quaternion.Euler(0, angle, 0));
					
						selected.transform.DOLocalRotate(Vector3.up * angle, 0.1f).SetEase(Ease.OutBounce).OnComplete(() => { busy = false;  CheckCompleted(); });
						selected = null;

						PlaySwoosh();
						
					}
				}
			}
        }

		void PlaySwoosh()
        {
			swooshAudioSource.clip = swooshClips[Random.Range(0, swooshClips.Count)];
			swooshAudioSource.Play();
        }

		void Create()
		{
			pillars = new Pillar[numOfRows * numOfColumns];

			// The horizontal starting position
			float startX = 0;
			if (numOfColumns % 2 == 0)
			{
				startX = -pillarDistance / 2f - ((float)numOfColumns / 2f - 1) * pillarDistance;
			}
			else
			{
				startX = -((float)numOfColumns - 1) / 2f * pillarDistance;
			}

			// Vertical starting position
			float startZ = 0;
			if (numOfRows % 2 == 0)
			{
				startZ = pillarDistance / 2f + ((float)numOfRows / 2f - 1) * pillarDistance;
			}
			else
			{
				startZ = ((float)numOfRows - 1) / 2f * pillarDistance;
			}
			float z = startZ;

			//
			// Creates pillars depending on the rows and columns; we don't take into account
			// the load factor yet.
			for (int i = 0; i < numOfRows; i++)
			{
				float x = startX;
				for (int j = 0; j < numOfColumns; j++)
				{
					// Create the game object
					Pillar pillar = GameObject.Instantiate(pillarPrefab).GetComponent<Pillar>();
					pillar.name = $"Pillar_{j}_{i}";
					pillar.transform.parent = pillarRoot;
					pillar.transform.localPosition = new Vector3(x, 0, z);
					x += pillarDistance;
					pillars[Utility.MatrixCoordsToArrayIndex(i, j, numOfColumns)] = pillar;

					// We must check if the pillar is not on the edge before we can set
					// a specific branch.
					// We start by checking the north edge
					if (i > 0)
					{
						// We can add the north branch but we need to check the pillar to the 
						// north first ( in the specific we must check the south branch ).
						// In fact we move from north to south while creating pillars, so we 
						// expect we already have some pillar to the north when we must set
						// color on the north branch of a specific pillar.
						// Get the index of the pillar to the north
						int northIndex = Utility.MatrixCoordsToArrayIndex(i - 1, j, numOfColumns);
						// Set the north color of the current branch as the south color
						// of the north branch
						pillar.AddBranch(0);
						pillar.GetBranch(0).ColorId = pillars[northIndex].GetBranch(2).ColorId;
					}
					// Checking the west edge
					if (j > 0)
					{
						// It works like the north branch; in this case we must check the east
						// branch of the pillar to the west.
						int westIndex = Utility.MatrixCoordsToArrayIndex(i, j - 1, numOfColumns);
						// Set the west color
						pillar.AddBranch(3);
						pillar.GetBranch(3).ColorId = pillars[westIndex].GetBranch(1).ColorId;
						//pillar.colors[3] = pillars[westIndex].colors[1];
					}

					// Checking south
					if (i < numOfRows - 1)
					{
						// South color ( random )
						pillar.AddBranch(2);
						pillar.GetBranch(2).ColorId = Random.Range(0, numOfColors);
						//pillar.colors[2] = Random.Range(0, numOfColors) + 1;
					}
					// Checking east
					if (j < numOfColumns - 1)
					{
						// East color ( random )
						pillar.AddBranch(1);
						pillar.GetBranch(1).ColorId = Random.Range(0, numOfColors);
						//pillar.colors[1] = Random.Range(0, numOfColors) + 1;
					}
				}
				z -= pillarDistance;
			}

			//
			// Remove some pillars depending on the load factor
			int count = pillars.Length - (int)(pillars.Length * loadFactor);

			// If we need simmetry we must cut the number of pillars to remove
			if (horizontalSymmetry)
				count /= 2;
			if (verticalSymmetry)
				count /= 2;

			for (int i = 0; i < count; i++)
			{
				// Availables
				List<Pillar> tmp = new List<Pillar>(pillars).FindAll(p => p.gameObject.activeSelf);
				// Get random pillar to remove
				Pillar toRemove = tmp[Random.Range(0, tmp.Count)];
				RemovePillar(toRemove);

				// If symmetry is on we must remove the corresponding pillar
				if (horizontalSymmetry || verticalSymmetry)
				{
					int toRemoveIndex = new List<Pillar>(pillars).IndexOf(toRemove);
					int toRemoveRow, toRemoveColumn;
					Utility.ArrayIndexToMatrixCoords(toRemoveIndex, numOfColumns, out toRemoveRow, out toRemoveColumn);

					// Horizontal symmetry
					int otherIndexForVertical = -1;
					if (horizontalSymmetry)
					{
						if (numOfColumns % 2 == 0 || toRemoveColumn != (numOfColumns - 1) / 2)
						{
							int otherColumn = numOfColumns - 1 - toRemoveColumn;
							int otherRow = toRemoveRow;
							int otherIndex = Utility.MatrixCoordsToArrayIndex(otherRow, otherColumn, numOfColumns);

							otherIndexForVertical = otherIndex;
							RemovePillar(pillars[otherIndex]);
						}
					}

					// Vertical symmetry
					if (verticalSymmetry)
					{
						if (numOfRows % 2 == 0 || toRemoveRow != (numOfRows - 1) / 2)
						{
							int otherRow = numOfRows - 1 - toRemoveRow;
							int otherColumn = toRemoveColumn;
							int otherIndex = Utility.MatrixCoordsToArrayIndex(otherRow, otherColumn, numOfColumns);

							RemovePillar(pillars[otherIndex]);

							// The other in the horizontal step
							if (otherIndexForVertical >= 0)
							{
								int toRemoveRow2, toRemoveColumn2;
								Utility.ArrayIndexToMatrixCoords(otherIndexForVertical, numOfColumns, out toRemoveRow2, out toRemoveColumn2);

								int otherRow2 = numOfRows - 1 - toRemoveRow2;
								int otherColumn2 = toRemoveColumn2;
								int otherIndex2 = Utility.MatrixCoordsToArrayIndex(otherRow2, otherColumn2, numOfColumns);

								RemovePillar(pillars[otherIndex2]);
							}
						}
					}


				}
			}

			//
            // Clear and shuffle; we remove deactivated pillars and randomly rotate the others
            for(int i=0; i<pillars.Length; i++)
            {
                if (!pillars[i].gameObject.activeSelf)
                {
                    DestroyImmediate(pillars[i].gameObject);
                    pillars[i] = null;
                }
                else
                {
                    float angle = Random.Range(0, 4) * 90f;
                    pillars[i].transform.Rotate(Vector3.up, angle);
                }
                    
            }
		}

		void RemovePillar(Pillar toRemove)
		{
			toRemove.gameObject.SetActive(false);

			// Get index
			int index = new List<Pillar>(pillars).IndexOf(toRemove);

			// Get rows and cols
			int row, col;
			Utility.ArrayIndexToMatrixCoords(index, numOfColumns, out row, out col);

			// Remove branches of the neighbour pillars
			// Pillar to the left
			if (col - 1 >= 0)
			{
				pillars[Utility.MatrixCoordsToArrayIndex(row, col - 1, numOfColumns)].RemoveBranch(1);
			}
			// To the right
			if (col + 1 < numOfColumns)
			{
				pillars[Utility.MatrixCoordsToArrayIndex(row, col + 1, numOfColumns)].RemoveBranch(3);
			}
			// North
			if (row - 1 >= 0)
			{
				pillars[Utility.MatrixCoordsToArrayIndex(row - 1, col, numOfColumns)].RemoveBranch(2);
			}
			// South
			if (row + 1 < numOfRows)
			{
				pillars[Utility.MatrixCoordsToArrayIndex(row + 1, col, numOfColumns)].RemoveBranch(0);
			}
		}

		public void CheckCompleted()
		{
			Debug.Log("Colors check -------------------------------------------------");
			for(int i=0; i<pillars.Length; i++)
			{


				if (pillars[i] == null)
				{
					Debug.Log($"Pillar[{i}] is null");
					continue;
				}

				Branch n = pillars[i].GetBranch(0);
				Branch e = pillars[i].GetBranch(1);
				Branch s = pillars[i].GetBranch(2);
				Branch w = pillars[i].GetBranch(3);
				Debug.Log($"Pillar[{i}] - N:{(n ? n.ColorId : -1)}, E:{(e ? e.ColorId : -1)}, S:{(s ? s.ColorId : -1)}, W:{(w ? w.ColorId : -1)},");

				for(int j=0; j<4; j++)
                {
                    if (pillars[i].HasBranch(j))
					{
						Debug.Log($"Checking pillars[{i}].Branch[{j}].ColorId = {pillars[i].GetBranch(j).ColorId}");
						// Get current branch
						Branch currentBranch = pillars[i].GetBranch(j);

						// Get collider
						SphereCollider coll = currentBranch.GetComponentInChildren<SphereCollider>();
						// Collider origin
						var pos = coll.transform.position;
						var radius = coll.radius * coll.transform.lossyScale.x;
						Debug.Log($"Collision pos:{pos}, radius:{radius}");
						// Disable current collider to ovoid overlap
						coll.enabled = false;
						// Overlap
						Collider[] others = Physics.OverlapSphere(pos, radius, LayerMask.GetMask(new string[] { "Overlapper"}));
						// Enable collider back
						coll.enabled = true;
						if (others.Length == 0)
						{
							Debug.Log("No overlap");
							return;
						}
                        else
                        {
							var otherBranch = others[0].GetComponentInParent<Branch>();
							Debug.Log("OtherBranch parent:" + otherBranch.GetComponentInParent<Pillar>());
							Debug.Log("Other branch color id:" + otherBranch.ColorId);

							if (otherBranch.ColorId != currentBranch.ColorId) return;
							
                        }
                    }
                }
            }
			Debug.Log("----------------------------------------------------------");
			
			Debug.Log("Game Has Completed");
			// Disable player 
			//PlayerController.Instance.Disabled = true;

			// Move connectors
			Connect();

			// Play audio
			//GetComponent<AudioSource>().Play();

			//OnGameCompleted?.Invoke();
			ReportBeaten();

		}

		void Connect()
		{
			for (int i = 0; i < pillars.Length; i++)
			{
				if (pillars[i] == null)
					continue;

				for (int j = 0; j < 4; j++)
				{
					if (pillars[i].HasBranch(j))
						pillars[i].GetBranch(j).Connect();
				}
			}

			lockAudioSource.Play();
		}

        public override void DoChildDeactivation()
        {
			base.DoChildDeactivation();

            if (selected)
            {
				selected.StopShaking();
				selected = null;
            }
        }
	}
}
