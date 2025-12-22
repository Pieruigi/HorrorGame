using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMM.AI;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class JumpscareManager : MonoBehaviour
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
			MazeBuilder.OnMazeCreated += HandleOnMazeCrated;
		}

        void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCrated;
        }

		private void HandleOnMazeCrated()
		{
		
		}

		
		
    }
}
