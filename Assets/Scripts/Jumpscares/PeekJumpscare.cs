using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace TMM
{
	public class PeekJumpscare : Jumpscare
	{

		[SerializeField]
		List<GameObject> prefabs;

		GameObject clown;

		bool[] directions = new bool[4]; 

		public override void ReportUsed()
		{
			// Instantiate a clown
			clown = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);

			// Get floor object
			var floor = transform.root;

			// Get the corresponding tile from builder
			var tileIndex = MazeBuilder.Instance.GetTileIndex(floor.gameObject);

			var coords = MazeBuilder.Instance.GetTileCoords(tileIndex);

			for(int i=0; i<4; i++)
			{
				var otherCoords = Vector2.zero;
				switch (i)
				{
					case 0:
						otherCoords = coords + Vector2.up;

						break;
				}

				if (MazeBuilder.Instance.GetTileType(MazeBuilder.Instance.GetTileIndex(otherCoords)) == 0)
					directions[i] = true;
			}

		}

        protected override bool CheckPlay()
        {
			return false;
        }
	}
}
