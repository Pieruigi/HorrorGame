using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class SpaceBullet : MonoBehaviour
	{
		float speed = 2f;

		Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
			rb.velocity = Vector3.up * speed;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }

    }
}
