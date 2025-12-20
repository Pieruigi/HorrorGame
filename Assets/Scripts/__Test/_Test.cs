using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class _Test : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

        void OnEnable()
        {
			_ChildA.OnTest += (a) => { Debug.Log("TTTTTTTTTTTTTTTTTTTT - A:"+a.GetType()); };
			_ChildB.OnTest += (a) => { Debug.Log("TTTTTTTTTTTTTTTTTTTT - B:" + a.GetType()); };
        }
    }
}
