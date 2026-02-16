using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class URLCaller : MonoBehaviour
	{
		[SerializeField]
		string url;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void Call()
		{
			Application.OpenURL(url);
		}
	}
}
