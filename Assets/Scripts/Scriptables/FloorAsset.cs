using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.Scriptables
{
	public class FloorAsset : ScriptableObject
	{
		public const string ResourceFolder = "Floors";


		[SerializeField]
		GameObject prefab;

		public GameObject Prefab
        {
            get{ return prefab; }
        }

	    
	}
}
