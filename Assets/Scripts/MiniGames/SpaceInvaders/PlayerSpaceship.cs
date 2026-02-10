using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class PlayerSpaceship : MonoBehaviour
	{
		[SerializeField]
		SpaceBullet bulletPrefab;

		[SerializeField]
		Transform shootPoint;

		bool activated = false;

		float maxDistance = .625f;

		float maxSpeed = 1f;
		float speedChange = 50f;

		float targetSpeed = 0;
		float speed = 0;

		float shootCooldown = .5f;
		float shootElapsed = 0;

		Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
			if (!activated) return;

			Move();
			Shoot();
        }

        private void Shoot()
        {
			shootElapsed += Time.deltaTime;
			if (shootElapsed < shootCooldown) return;

			if (Input.GetMouseButton(0)) 
			{
				// Shoot
				shootElapsed = 0;
				// Create bullet
				var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
				// Avoid collision with player
				Physics.IgnoreCollision(_collider, bullet.GetComponent<Collider>(), true);

			}
        }

        void Move()
		{
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.A))
                    targetSpeed = -maxSpeed;

                if (Input.GetKey(KeyCode.D))
                    targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = 0;
            }

            speed = Mathf.MoveTowards(speed, targetSpeed, speedChange * Time.deltaTime);

            var pos = transform.localPosition;
            pos.x += speed * Time.deltaTime;

            pos.x = Mathf.Clamp(pos.x, -maxDistance, maxDistance);

            transform.localPosition = pos;
        }

		public void Activate()
		{
			activated = true;
			shootElapsed = 0;
		}

		public void Deactivate()
		{
			activated = false;
		}
	}
}
