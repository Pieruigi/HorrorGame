using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class PlayerSpaceship : MonoBehaviour
	{
		[SerializeField]
		SpaceBullet bulletPrefab;

		[SerializeField]
		Transform shootPoint;

		[SerializeField]
		ParticleSystem explosionFx;

		[SerializeField]
		ParticleSystem creationFx;

		[SerializeField]
		AudioSource shotAudioSource;

		[SerializeField]
		AudioSource hitAudioSource;

		[SerializeField]
		AudioSource creationAudioSource;

		bool activated = false;

		float maxDistance = .625f;

		float maxSpeed = 1f;
		float speedChange = 50f;

		float targetSpeed = 0;
		float speed = 0;

		float shootCooldown = .5f;
		float shootElapsed = 0;

		Collider _collider;

		bool destroyed;

		List<Renderer> renderers;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        renderers = GetComponentsInChildren<Renderer>().ToList();
	    }

	    // Update is called once per frame
	    void Update()
	    {
			if (!activated || destroyed) return;



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
				bullet.SetSpeed(1.5f);
			
				// Avoid collision with player
				Physics.IgnoreCollision(_collider, bullet.GetComponent<Collider>(), true);

				// Play sound
				shotAudioSource.Play();

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
			destroyed = false;
		}

		public void Deactivate()
		{
			activated = false;
		}

        private void OnCollisionEnter(Collision collision)
        {
			if (destroyed) return;


			StartCoroutine(DoDestroy());


			IEnumerator DoDestroy()
			{
				if (destroyed) yield break;

                destroyed = true;

                // Play sound
                hitAudioSource.Play();
                creationAudioSource.PlayDelayed(.8f);

                var fx = Instantiate(explosionFx, transform);
				fx.transform.localPosition = Vector3.zero;
				fx.transform.localEulerAngles = -90f * Vector3.right;
				fx.transform.localScale = Vector3.one * .7f;
				var main = fx.main;
				main.gravityModifierMultiplier = 0;
				fx.Play();

				Destroy(fx.gameObject, 5f);

				yield return new WaitForSeconds(.2f);

                foreach (var r in renderers)
					r.enabled = false;


                
                // Create
                fx = Instantiate(creationFx, transform);
                fx.transform.localPosition = Vector3.zero;
                fx.transform.localEulerAngles = Vector3.zero;
                fx.transform.localScale = Vector3.one * .7f;
                fx.Play();
                Destroy(fx.gameObject, 5f);

				

                yield return new WaitForSeconds(.8f);

                
                foreach (var r in renderers)
                    r.enabled = true;

				destroyed = false;

				

				
            }
        }

		
    }
}
