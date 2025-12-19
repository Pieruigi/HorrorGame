using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM.Scriptables
{
	public class VendingMachineBlockAsset : ScriptableObject
	{
		public const string ResourceFolder = WallBlockAsset.ResourceFolder;

		
		[SerializeField]
		int minStage;
		public int MinStage
		{
			get{ return minStage; }
		}


		[SerializeField]
		GameObject prefab;
		public GameObject Prefab
		{
			get { return prefab; }
		}
		
		public List<Vector2> GetTiles()
		{
			var l = prefab.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("Floor"));

			List<Vector2> tiles = new List<Vector2>();
			foreach (var e in l)
				tiles.Add(new Vector2(e.localPosition.x, e.localPosition.z) / MazeBuilder.CellSize);

			return tiles;

		}
	}
}
