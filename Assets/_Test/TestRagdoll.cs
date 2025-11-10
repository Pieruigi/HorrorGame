using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class TestRagdoll : MonoBehaviour
	{
		[SerializeField]
		GameObject target;

		[SerializeField]
		Transform leftForeArm;

		[SerializeField]
		Transform head;

		[SerializeField]
		Transform point;

		[SerializeField]
		Transform point2;

		[SerializeField]
		List<Rigidbody> rigidbodies;

		bool move = false;
	    // Start is called before the first frame update
	    void Start()
	    {
			rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
	    }

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				
				move = true;
			}
			if (Input.GetKeyDown(KeyCode.X))
			{
				
				move = false;
			}
		}


        void FixedUpdate()
        {
            if(move)
            {
                // head.GetComponent<Rigidbody>().position = point2.position;
				 head.GetComponent<Rigidbody>().rotation = point2.rotation;

				
				//leftForeArm.GetComponent<Rigidbody>().position = point.position;
            }
        }

    }
}
