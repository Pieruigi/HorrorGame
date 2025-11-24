using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM.Scriptables
{
	public class WallBlockAsset : ScriptableObject
	{
		public const string ResourceFolder = "WallBlocks";

		[SerializeField]
		List<GameObject> prefabs;
		public IList<GameObject> Prefabs
        {
            get{ return prefabs; }
        }

		[SerializeField]
		bool createFlippedVariant = false;
		public bool CreateFlippedVariant
        {
			get { return createFlippedVariant; }
	    }

		[SerializeField]
		int min;
		public int Min
        {
            get{ return min; }
        }

		[SerializeField]
		int weight;
		public int Weight
        {
            get{ return weight; }
        }

		// Return tiles from the first prefab
		public List<Vector2> GetTiles()
		{
			var l = prefabs[0].GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Floor"));

			List<Vector2> tiles = new List<Vector2>();
			foreach (var e in l)
				tiles.Add(new Vector2(e.localPosition.x, e.localPosition.z) / MazeBuilder.CellSize);

			return tiles;

		}
		
	}
}
