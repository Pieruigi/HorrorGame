using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class SpaceBullet : MonoBehaviour
	{
        
		float speed = 1.5f;

		Rigidbody rb;

        float lifeTime = 5;

  

        private void Awake()
        {
        
        }

        // Start is called before the first frame update
        void Start()
	    {
            rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.up * speed;
        }

        private void Update()
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0) Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }
    }
}
