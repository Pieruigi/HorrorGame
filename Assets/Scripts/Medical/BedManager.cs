using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class BedManager : Singleton<BedManager>
	{
		List<Bed> beds;

	    // Start is called before the first frame update
	    void Start()
	    {
			beds = FindObjectsByType<Bed>(FindObjectsSortMode.None).ToList(); 
		}

		// Update is called once per frame
		void Update()
		{

		}

		public Bed GetRandomFreeBed()
		{
			var filtered = beds.Where(b => b.IsFree).ToList();
			if (filtered.Count == 0) return null;
			else  return filtered[UnityEngine.Random.Range(0, filtered.Count)];
		}

		
		
	}
}
