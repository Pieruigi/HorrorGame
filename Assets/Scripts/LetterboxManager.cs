using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class LetterboxManager : Singleton<LetterboxManager>
	{
		List<Letterbox> letterboxes;

		public IList<Letterbox> Letterboxes
        {
            get{ return letterboxes.AsReadOnly(); }
        }

	    // Start is called before the first frame update
	    void Start()
	    {
	        letterboxes = FindObjectsByType<Letterbox>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void InitShift(int workingDay, bool isNightShift)
        {
			if (isNightShift) return;

			foreach (var lb in letterboxes)
				lb.Reset();

			

        }
	}
}
