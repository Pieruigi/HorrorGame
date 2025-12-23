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
			if (Vector3.Angle(FirstPersonController.transform.forward, t.transform.forward) < 40f) return;

			int index = triggers.IndexOf(t);

			Debug.Log($"Jumpscare triggered - {transform.root.gameObject.name} - {index}");



			
		}

        public override void ReportUsed()
		{
			// Instantiate a clown
			clown = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);


		}

		protected override bool CheckPlay()
		{
			return false;
		}

        public override bool CheckForDestroy()
        {
            // Get the root tile
			var tile = MazeBuilder.Instance.GetTileIndex(transform.root.gameObject);

			// Get coords
			var coords = MazeBuilder.Instance.GetTileCoords(tile);

			// Get others
			var n = MazeBuilder.Instance.GetTileIndex(coords + Vector2.up);
			var e = MazeBuilder.Instance.GetTileIndex(coords + Vector2.right);
			var s = MazeBuilder.Instance.GetTileIndex(coords - Vector2.up);
			var w = MazeBuilder.Instance.GetTileIndex(coords - Vector2.right);

			// Get types
			n = n < 0 ? -1 : MazeBuilder.Instance.GetTileType(n);
			e = e < 0 ? -1 : MazeBuilder.Instance.GetTileType(e);
			s = s < 0 ? -1 : MazeBuilder.Instance.GetTileType(s);
			w = w < 0 ? -1 : MazeBuilder.Instance.GetTileType(w);

			triggers[0].gameObject.SetActive(n == 0);
			triggers[1].gameObject.SetActive(e == 0);
			triggers[2].gameObject.SetActive(s == 0);
			triggers[3].gameObject.SetActive(w == 0);


			// Check coords
			if (!((n == 0 || s == 0) && (e == 0 || w == 0)))
				return true;

			// Check valid triggers (only trigger with another tile behind, just like if you walk through a corridor)


			return false;
        }
	}
}
