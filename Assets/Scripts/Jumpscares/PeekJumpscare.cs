using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using TMM.AI;
using UnityEngine;

namespace TMM
{
	public class PeekJumpscare : Jumpscare
	{

		[SerializeField]
		List<GameObject> clownPrefabs;

		/// <summary>
		/// 0: north
		/// 1: east
		/// 2: south
		/// 3: west
		/// </summary>
		[SerializeField]
		List<ActivationTrigger> triggers;



		GameObject clown;

		List<ClownA> creatures;

		ClownA clownA;
		ClownB clownB;
		ClownC clownC;

        protected override void Start()
        {
			base.Start();

			creatures = FindObjectsByType<ClownA>(FindObjectsSortMode.None).ToList();

			clownA = FindFirstObjectByType<ClownA>();
			clownB = FindFirstObjectByType<ClownB>();
			clownC= FindFirstObjectByType<ClownC>();
        }


		protected override void OnEnable()
		{
			base.OnEnable();

			foreach (var t in triggers)
				t.OnEnter += (c) => { HandleOnTriggerEnter(t, c); };
		}

	
		
		void HandleOnTriggerEnter(ActivationTrigger t, Collider other)
		{
			if (!other.CompareTag("Player")) return;
			if (Triggered) return;

			// Only if you are not chased or searched for
			//if (creatures.Exists(c => c.State == ClownAState.Chase || c.State == ClownAState.Search)) return;
			if (clownA.State == ClownAState.Chase || clownA.State == ClownAState.Search) return;
			if (clownB?.State == ClownBState.Chase) return;
			if (clownC?.State == ClownCState.Chase) return;

			
			// The player must be looking along the trigger's forward axis
			float signedAngle = Vector3.SignedAngle(FirstPersonController.transform.forward, t.transform.forward, Vector3.up);
			if (Mathf.Abs(signedAngle) > 40f) return;

			int index = triggers.IndexOf(t);

			Debug.Log($"Jumpscare triggered - {transform.root.gameObject.name} - {index}, pitch:{FirstPersonController.GetTargetPitch()}, angle:{signedAngle}");

			float pitch = 20;
			if (FirstPersonController.GetTargetPitch() > -pitch && FirstPersonController.GetTargetPitch() < pitch)
				Play(signedAngle, index);


		}

		bool IsRightAvailable(int triggerIndex)
		{
			var builder = MazeBuilder.Instance;
			var coords = builder.GetTileCoords(builder.GetTileIndex(transform.parent.gameObject));
			bool ret = false;
			switch (triggerIndex)
			{
				case 0:
					ret = builder.GetTileType(coords - Vector2.right) == 0;
					break;
				case 1:
					ret = builder.GetTileType(coords + Vector2.up) == 0;
					break;

				case 2:
					ret = builder.GetTileType(coords + Vector2.right) == 0;
					break;
				case 3:
					ret = builder.GetTileType(coords - Vector2.up) == 0;
					break;
			}

			return ret;
		}

		bool IsLeftAvailable(int triggerIndex)
		{
			var builder = MazeBuilder.Instance;
			var coords = builder.GetTileCoords(builder.GetTileIndex(transform.parent.gameObject));
			bool ret = false;
			switch (triggerIndex)
			{
				case 0:
					ret = builder.GetTileType(coords + Vector2.right) == 0;
					break;
				case 1:
					ret = builder.GetTileType(coords - Vector2.up) == 0;
					break;

				case 2:
					ret = builder.GetTileType(coords - Vector2.right) == 0;
					break;
				case 3:
					ret = builder.GetTileType(coords + Vector2.up) == 0;
					break;
			}

			return ret;
		}

		void Play(float signedAngle, int triggerIndex)
        {
			
			// Spawn clown
			clown = Instantiate(clownPrefabs[Random.Range(0, clownPrefabs.Count)]);
			clown.transform.parent = transform;

			// Check side 
			int side = 0; // 0: right, 1: left
			if (IsLeftAvailable(triggerIndex) && IsRightAvailable(triggerIndex))
			{
				side = Random.Range(0, 2);
			}
			else
			{
				if (IsLeftAvailable(triggerIndex))
					side = 1;
			}

			// Get spawn point
			var spawn = side == 0 ? triggers[triggerIndex].transform.Find("R") : triggers[triggerIndex].transform.Find("L");

			var lookAt = new GameObject("LookAt");
			lookAt.transform.parent = transform;
			lookAt.transform.position = spawn.position + Vector3.up * 1.5f + (side == 0 ? spawn.right : -spawn.right) * .75f;
			SetLookAt(lookAt.transform);

			clown.transform.position = spawn.position;
			clown.transform.rotation = spawn.rotation;

			// Get animator
			var animator = clown.GetComponentInChildren<Animator>();

			animator.SetInteger("Side", side);
			animator.SetTrigger("Peek");

			var seq = DOTween.Sequence();
			seq.AppendInterval(.5f).OnComplete(() => { animator.SetTrigger("Idle"); });
			seq.AppendInterval(.5f).OnComplete(() => { Destroy(clown); });
			

			Play();
        }

		public override void ReportUsed()
		{
			// Instantiate a clown
			//clown = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);


		}

		// protected override bool CheckPlay()
		// {
		// 	return false;
		// }

        public override bool Validate()
		{
			var builder = MazeBuilder.Instance;

            // Get the root tile
			var tile = MazeBuilder.Instance.GetTileIndex(transform.root.gameObject);

			// Get coords
			var coords = MazeBuilder.Instance.GetTileCoords(tile);

		

			// Get types
			triggers[0].gameObject.SetActive(builder.GetTileType(coords + Vector2.up) == 0 &&
											 builder.GetTileType(coords + Vector2.up * 2) == 0 &&
											 builder.GetTileType(coords + Vector2.up * 3) == 0 &&
											 (builder.GetTileType(coords + Vector2.right) == 0 || builder.GetTileType(coords - Vector2.right) == 0));

			triggers[1].gameObject.SetActive(builder.GetTileType(coords + Vector2.right) == 0 &&
											 builder.GetTileType(coords + Vector2.right * 2) == 0 &&
											 builder.GetTileType(coords + Vector2.right * 3) == 0 &&
											 (builder.GetTileType(coords + Vector2.up) == 0 || builder.GetTileType(coords - Vector2.up) == 0));

			triggers[2].gameObject.SetActive(builder.GetTileType(coords - Vector2.up) == 0 &&
											 builder.GetTileType(coords - Vector2.up * 2) == 0 &&
											 builder.GetTileType(coords - Vector2.up * 3) == 0 && 
											 (builder.GetTileType(coords + Vector2.right) == 0 || builder.GetTileType(coords - Vector2.right) == 0));

			triggers[3].gameObject.SetActive(builder.GetTileType(coords - Vector2.right) == 0 &&
											 builder.GetTileType(coords - Vector2.right * 2) == 0 &&
											 builder.GetTileType(coords - Vector2.right * 3) == 0 &&
											 (builder.GetTileType(coords + Vector2.up) == 0 || builder.GetTileType(coords - Vector2.up) == 0));

			// Check coords
			if (!((triggers[0].gameObject.activeSelf || triggers[2].gameObject.activeSelf) && (triggers[1].gameObject.activeSelf || triggers[3].gameObject.activeSelf)))
				return true;

			
			return false;
        }
	}
}
