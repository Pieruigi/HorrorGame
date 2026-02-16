using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class Memory : MiniGame
	{
		[SerializeField]
		List<GameObject> tiles;

		[SerializeField]
		AudioSource swooshAudioSource;

		[SerializeField]
		List<AudioClip> swooshClips;

		Vector3[] originalPositions;

		GameObject shakingTile;

		GameObject[] selectedTiles = new GameObject[2];

		[SerializeField]
		float raycastDistance = 10;

		bool checkingTiles = false;

		int jumpscareScore = -1;

        protected override void Awake()
        {
			base.Awake();

			// Store original positions
			originalPositions = new Vector3[tiles.Count];

			for (int i = 0; i < tiles.Count; i++)
				originalPositions[i] = tiles[i].transform.position;

			// Shuffle
			var shuffledList = Utility.Shuffle(tiles);
			Debug.Log("ShuffledList.Count:" + shuffledList.Count);
			for(int i=0;  i<shuffledList.Count; i++)
            {
				shuffledList[i].transform.position = originalPositions[i];
            }
        }

		protected override void Update()
		{
			base.Update();

			// Raycast from camera
			var mask = LayerMask.GetMask(new string[] { "Interactable" });
			RaycastHit hit;
			if (IsActive)
			{
				if (!checkingTiles && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, raycastDistance, mask))
				{
					MemoryTile tile = hit.collider.GetComponent<MemoryTile>();

					if (tile)
					{
						if (shakingTile)
                        {
							shakingTile.GetComponent<MemoryTile>().Shake(false);
							shakingTile = null;
                        }
							

                        if (!tile.IsSelected)
                        {
                        	shakingTile = tile.gameObject;
							tile.Shake(true);    
                        }
						
					}
					else
					{
						if (shakingTile)
                        {
							shakingTile.GetComponent<MemoryTile>().Shake(false);
							shakingTile = null;
                        }
							
						
					}
				}
				else
				{
					if (shakingTile)
                    {
						shakingTile.GetComponent<MemoryTile>().Shake(false);
						shakingTile = null;
                    }
						

					
				}

			}
			else
			{
				if (shakingTile)
                {
					shakingTile.GetComponent<MemoryTile>().Shake(false);
					shakingTile = null;
                }
					

			}

			// If there is a shaking 
			if (shakingTile)
			{
				if (Input.GetMouseButtonDown(0))
				{
					if (!selectedTiles[0])
						selectedTiles[0] = shakingTile;
					else
						selectedTiles[1] = shakingTile;

					shakingTile.GetComponent<MemoryTile>().Select(true);
					shakingTile = null;
				}
			}

			if (selectedTiles[0] && selectedTiles[1] && !checkingTiles)
			{
				StartCoroutine(CheckTiles());
			}
		}

		IEnumerator CheckTiles()
		{
			checkingTiles = true;

			if (selectedTiles[0].name != selectedTiles[1].name)
			{
				yield return new WaitForSeconds(1f);
				selectedTiles[0].GetComponent<MemoryTile>().Select(false);
				selectedTiles[1].GetComponent<MemoryTile>().Select(false);
				//yield return new WaitForSeconds(.25f);
			}
			else
			{
				if(jumpscareScore > 0)
				{
					jumpscareScore--;
					if (jumpscareScore == 0)
						MiniJumpscare.Play();
				}
			}

			selectedTiles[0] = null;
			selectedTiles[1] = null;

			checkingTiles = false;

			if (IsBeaten())
	            ReportBeaten();
    			
		}

		bool IsBeaten()
		{
			return tiles.Count(t => !t.GetComponent<MemoryTile>().IsSelected) == 0;
		}
		
		public void PlaySwoosh()
        {
			swooshAudioSource.clip = swooshClips[Random.Range(0, swooshClips.Count)];
			swooshAudioSource.Play();
        }

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {
            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific move
            jumpscareScore = Random.Range(2, 6);
        }

    }
}
