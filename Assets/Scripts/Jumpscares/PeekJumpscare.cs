using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using NUnit.Framework.Internal;
using StarterAssets;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace TMM
{
	public class PeekJumpscare : Jumpscare
	{

		[SerializeField]
		List<GameObject> prefabs;

		/// <summary>
		/// 0: north
		/// 1: east
		/// 2: south
		/// 3: west
		/// </summary>
		[SerializeField]
		List<ActivationTrigger> triggers;

		GameObject clown;



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

			// The player must be looking along the trigger's forward axis
			if (Vector3.Angle(FirstPersonController.transform.forward, t.transform.forward) > 40f) return;

			int index = triggers.IndexOf(t);

			Debug.Log($"Jumpscare triggered - {transform.root.gameObject.name} - {index}");

			if (FirstPersonController.GetTargetPitch() > -30 && FirstPersonController.GetTargetPitch() < 30)
				Play();

			
		}

        protected override void Play()
        {
			base.Play();
			

        }

		public override void ReportUsed()
		{
			// Instantiate a clown
			clown = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);


		}

		// protected override bool CheckPlay()
		// {
		// 	return false;
		// }

        public override bool Validate()
        {
            // Get the root tile
			var tile = MazeBuilder.Instance.GetTileIndex(transform.root.gameObject);

			// Get coords
			var coords = MazeBuilder.Instance.GetTileCoords(tile);

		

			// Get types
			triggers[0].gameObject.SetActive(MazeBuilder.Instance.GetTileType(coords + Vector2.up) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.up * 2) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.up * 3) == 0 /*&&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.up + Vector2.right) != 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.up - Vector2.right) != 0*/);

			triggers[1].gameObject.SetActive(MazeBuilder.Instance.GetTileType(coords + Vector2.right) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.right * 2) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.right * 3) == 0 /*&&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.right + Vector2.up) != 0 &&
											 MazeBuilder.Instance.GetTileType(coords + Vector2.right - Vector2.up) != 0*/);

			triggers[2].gameObject.SetActive(MazeBuilder.Instance.GetTileType(coords - Vector2.up) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.up * 2) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.up * 3) == 0 /*&&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.up + Vector2.right) != 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.up - Vector2.right) != 0*/);

			triggers[3].gameObject.SetActive(MazeBuilder.Instance.GetTileType(coords - Vector2.right) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.right * 2) == 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.right * 3) == 0 /*&&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.right + Vector2.up) != 0 &&
											 MazeBuilder.Instance.GetTileType(coords - Vector2.right - Vector2.up) != 0*/);

			// Check coords
			if (!((triggers[0].gameObject.activeSelf || triggers[2].gameObject.activeSelf) && (triggers[1].gameObject.activeSelf || triggers[3].gameObject.activeSelf)))
				return true;

			
			return false;
        }
	}
}
