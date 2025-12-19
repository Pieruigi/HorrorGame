using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.UI
{
	public class CanvasSetter : MonoBehaviour
	{
        void Awake()
		{
			Canvas c = GetComponent<Canvas>();

			//  c.renderMode = RenderMode.ScreenSpaceOverlay;
			//  return;

			if (c.renderMode == RenderMode.ScreenSpaceOverlay)
				c.renderMode = RenderMode.ScreenSpaceCamera;
            c.worldCamera = Camera.main;
			c.planeDistance = .1f;
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
