using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMM
{
	public class WebCanvasScaler : MonoBehaviour
	{
		[SerializeField]
		RawImage rawImage;

		// Start is called before the first frame update
		void Start()
		{
#if UNITY_WEBGL
			
#else
			Destroy(this);
#endif

		}

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
