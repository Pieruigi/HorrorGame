using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

namespace TMM
{
	public class BreakoutBall : MonoBehaviour
	{
		[SerializeField]
		GameObject mesh;

		[SerializeField]
		ParticleSystem spawnParticle;

		[SerializeField]
		ParticleSystem destroyParticle;

		Rigidbody rb;

        Vector3 ballPositionDefault;

		Transform plane;

		float speed = 1.1f;

		Breakout minigame;
		Vector3 velocity;

		Sequence spawnTween;
		
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            ballPositionDefault = transform.localPosition;
			plane = transform.parent;
			minigame = transform.root.GetComponentInChildren<Breakout>();
			Hide();
        }


        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
			
	    }

        private void FixedUpdate()
        {
			if (!minigame.IsActive) return;

			if(!minigame.Paused)
				rb.velocity = velocity;
			else
				rb.velocity = Vector3.zero;
        }

        void Launch()
		{
			// Compute direction
			var x = Random.Range(-.5f, .5f);
			var y =  -1f;
			var dir = plane.transform.right * x + plane.transform.up * y;
			
			dir.Normalize();
			
			// Apply velocity
			velocity = dir * speed;
			rb.velocity = velocity;
			
		}

        private void OnCollisionEnter(Collision collision)
        {
			var vel = velocity.normalized;
            Debug.Log($"TEST - ----------------------- {collision.collider.transform.parent.gameObject.name}/{collision.collider.transform.gameObject.name} -----------------------------------");
			
            Debug.Log("TEST - Vel:" + vel);
            var normal = collision.contacts[0].normal;
			Debug.Log("TEST - Normal:" + normal);

			if (Vector3.Dot(vel, normal) > 0) return;

            vel = Vector3.Reflect(vel, normal);


			if (collision.collider.CompareTag("Ship"))
			{
				// Depending on the ship move direction we modify the ball direction
				if (minigame.ShipDirection != 0)
				{
					var v = plane.InverseTransformDirection(vel);

					if (minigame.ShipDirection < 0) // Ship is moving left
					{
						// Adjust ball direction to right
						v.x += .5f;
					}
					else
					{
						// Adjust ball direction to right
						v.x -= .5f;
					}
					vel = plane.TransformDirection(v);
					
				}
			}
			

				vel.Normalize();
            Debug.Log("TEST - ReflectedVel:" + vel);
            vel *= speed;
			velocity = vel;
			//rb.velocity = velocity;

			
        }

        private void OnCollisionExit(Collision collision)
        {
            
        }

        public void Show()
		{
			transform.localPosition = ballPositionDefault;

			spawnTween.Kill();

			spawnTween = DOTween.Sequence();
            spawnTween.AppendCallback(()=>spawnParticle.Play());
            spawnTween.AppendInterval(.5f);
            spawnTween.OnComplete(() => 
			{
                mesh.SetActive(true);
                Launch();
            });
			
			
			
		}

		public void Hide()
		{
			spawnTween.Kill();

			velocity = Vector3.zero;
			rb.velocity = velocity;
			mesh.SetActive(false);
			//transform.localPosition = ballPositionDefault;
			
			
		}

		public void DestroyBall()
		{
			destroyParticle.Play();
			Hide();
		}
	}
}
