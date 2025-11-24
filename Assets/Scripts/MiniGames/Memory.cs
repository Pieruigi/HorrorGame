using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace TMM
{
	public class Memory : MiniGame
	{
		[SerializeField]
		List<GameObject> tiles;

		Vector3[] originalPositions;

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

        protected override void Start()
		{
			base.Start();



			// Shuffle

		}


		protected override void DoUpdate()
		{

		}
		
		
	}
}
