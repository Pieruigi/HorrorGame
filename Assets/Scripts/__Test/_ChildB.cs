using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TMM
{
	public class _ChildB : _Parent
	{
	    // Start is called before the first frame update
	    async void Start()
	    {
			await Task.Delay(8000);
			InvokeOnTest();
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
