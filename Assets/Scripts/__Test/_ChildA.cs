using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class _ChildA : SingletonPersistent<_ChildA>
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
			PlayerSpeedDebuff.OnApplied += HandleOnApplied;
		}
        void OnDisable()
        {
            PlayerSpeedDebuff.OnApplied -= HandleOnApplied;
        }

        private void HandleOnApplied()
        {
            throw new NotImplementedException();
        }
    }
}
