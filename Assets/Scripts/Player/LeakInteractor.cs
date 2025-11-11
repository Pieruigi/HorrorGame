using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class LeakInteractor : MonoBehaviour
	{

		LeakTrigger lastLeakTrigger;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

        void LateUpdate()
		{
			// Raycast from camera
			Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
			RaycastHit hit;
			LayerMask mask = LayerMask.GetMask(new string[] { "Interactable" });
			if (Physics.Raycast(ray, out hit, FirstPersonController.InteractionDistance, mask))
			{
				// Check if the collider belongs to a leak trigger
				LeakTrigger lt = hit.collider.GetComponent<LeakTrigger>();
				if (lt)
				{
					if (lastLeakTrigger != lt)
                    {
						if (lastLeakTrigger)
							lastLeakTrigger.HideMedicine();

						lastLeakTrigger = lt;
						lt.ShowMedicine();
                    }
						
				}
				else
				{
					if (lastLeakTrigger)
					{
						lastLeakTrigger.HideMedicine();
						lastLeakTrigger = null;
					}
				}

			}
			else
			{
				if (lastLeakTrigger)
				{
					lastLeakTrigger.HideMedicine();
					lastLeakTrigger = null;
				}
			}

        }
    }
}
