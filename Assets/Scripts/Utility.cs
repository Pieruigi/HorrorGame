using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class Utility
	{
		
		public static List<GameObject> Shuffle(List<GameObject> list)
        {
			List<GameObject> ret = new List<GameObject>();
			List<GameObject> tmp = new List<GameObject>(list);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				var go = tmp[Random.Range(0, tmp.Count)];
				tmp.Remove(go);
				ret.Add(go);
			}

			return ret;
        }
	}
}
