using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.UI
{
	public class LevelToggleUI : MonoBehaviour
	{
		[SerializeField]
		GameObject lockImage;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void Unlock()
		{
			lockImage.SetActive(false);
		}
	}
}
