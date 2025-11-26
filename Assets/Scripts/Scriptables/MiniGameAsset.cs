using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.Scriptables
{
	public class MiniGameAsset : WallBlockAsset
	{
		public new const string ResourceFolder = "MiniGames";


		[SerializeField]
		int minLevel = -1;
		public int MinLevel
        {
            get{ return minLevel; }
        }

		[SerializeField]
		int maxLevel = -1;	
		public int MaxLevel
        {
            get{ return maxLevel; }
        }
		
	}
}
