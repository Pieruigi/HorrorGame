using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class _ChildB : SingletonPersistent<_ChildB>
	{
	    // Start is called before the first frame update
	    void Start()
	    {
			Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDDDDDD:"+(_ChildA.Instance == _ChildB.Instance));
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
