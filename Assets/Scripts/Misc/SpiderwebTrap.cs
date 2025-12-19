using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace TMM
{
	public class SpiderwebTrap : MonoBehaviour
	{
		[SerializeField]
		FloorTrigger floorTrigger;

		[SerializeField]
		float duration;

		float elapsed = 0;

		bool triggered = false;

	
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			if (!triggered) return;

			elapsed += Time.deltaTime;
			if(elapsed >= duration)
			{
				triggered = false;
				floorTrigger.ResetTrigger();
			}
		}

		void OnEnable()
		{
			floorTrigger.OnTriggered += HandleOnTriggered;
		}

        void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
        }

        private void HandleOnTriggered()
		{
			elapsed = 0;
			triggered = true;
			PlayerSpeedDebuff.Instance.Apply();
        }
    }
}
