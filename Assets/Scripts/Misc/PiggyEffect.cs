using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class PiggyEffect : MonoBehaviour
	{
		FirstPersonController player;

		FloorTrigger floorTrigger;

		bool triggered = false;


        void Awake()
        {
			floorTrigger = transform.root.GetComponentInChildren<FloorTrigger>();
        }

        // Start is called before the first frame update
        void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();

			transform.DOShakeRotation(1f).SetDelay(2f).SetLoops(-1);
	    }

		// Update is called once per frame
		void Update()
		{
			if (floorTrigger.Triggered)
			{
				if (!triggered)
				{
					triggered = true;
					// Play triggered fx
					transform.DOKill();
					//transform.
				}
			}
			else
			{
				if (triggered)
				{
					triggered = false;
					// Play not triggered fx
					transform.DOKill();
					transform.DOShakeRotation(1f).SetLoops(-1);
				}
			}

		}

		void LateUpdate()
		{
			// Look at the player
			var dir = Vector3.ProjectOnPlane(player.transform.position - transform.parent.position, Vector3.up);

			transform.parent.forward = dir;
		}
		
		
    }
}
