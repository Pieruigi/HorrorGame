using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class DemoUI : MonoBehaviour
	{
#if DEMO
		static bool show = false;
#endif

		private void Awake()
        {
#if !DEMO
			Destroy(gameObject); 
#else
			if (!show)
			{
				show = true;
				transform.GetChild(0).gameObject.SetActive(false);
			}

#endif
		}

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void Wishlist()
		{
			Application.OpenURL("https://store.steampowered.com/app/4318370/MAZY");
		}
	}
}
