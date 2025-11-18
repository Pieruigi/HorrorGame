using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace TMM
{
	public class NatureRandomizer : MonoBehaviour
	{
		[SerializeField]
		float scaleMax = 1.05f;

		[SerializeField]
		float scaleMin = .95f;

		[SerializeField]
		bool rotate = false;

        void Awake()
        {
			if (scaleMin != 1f || scaleMax != 1f)
			{
				var s = Random.Range(scaleMin, scaleMax);
				transform.localScale = Vector3.one * s;
			}

            if (rotate)
            {
				var r = Random.Range(0, 359);
				transform.eulerAngles = Vector3.up * r;
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
