using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.Scriptables
{
	public class FloorTriggerAsset : FloorAsset
	{
		public new const string ResourceFolder = "FloorTriggers";

		[SerializeField]
		int minStage = 1;
		public int MinStage
		{
			get{ return minStage; }
		}

		// [SerializeField]
		// int maxStage = -1;
		// public int MaxStage
		// {
		// 	get{ return maxStage; }
		// }

		// [SerializeField]
		// int minCount = 1;
		// public int MinCount
		// {
		// 	get{ return minCount; }
		// }

		// [SerializeField]
		// int maxCount = 3;
		// public int MaxCount
		// {
		// 	get{ return maxCount; }
		// }

		[SerializeField]
		int weight = 10;
		public int Weight
		{
			get{ return weight; }
		}
	}
}
