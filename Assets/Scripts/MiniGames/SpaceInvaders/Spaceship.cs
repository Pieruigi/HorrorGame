using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace TMM
{
	public class Spaceship : MonoBehaviour
	{
		[SerializeField]
		SpaceBullet bulletPrefab;

		[SerializeField]
		ParticleSystem explosionParticlePrefab;

		[SerializeField]
		AudioSource destroyAudioSource;

        [SerializeField]
        AudioSource shootAudioSource;

        bool destroyed = false;
		public bool Destroyed { get { return destroyed; } }

		Collider _collider;
		
		List<Renderer> renderers;

		SpaceInvaders miniGame;

		float fireTime = 5f;

		float fireElapsed = 0;

		List<Collider> otherColliders = new List<Collider>();
		

        private void Awake()
        {
            _collider = GetComponent<Collider>();	
			fireElapsed = Random.Range(0, fireTime);
        }

        // Start is called before the first frame update
        void Start()
	    {
	        renderers = GetComponentsInChildren<Renderer>().ToList();
			miniGame = transform.root.GetComponentInChildren<SpaceInvaders>();

			var l = FindObjectsByType<Spaceship>(FindObjectsSortMode.None).ToList();
			l.Remove(this);
			foreach (var s in l)
				otherColliders.Add(s.GetComponent<Collider>());
	    }

	    // Update is called once per frame
	    void Update()
	    {
			if (Destroyed) return;

            if (miniGame.IsActive)
			{
                fireElapsed += Time.deltaTime;

                if (fireElapsed > fireTime)
                {
                    fireElapsed -= fireTime;
                    // Shoot
                    TryShoot();

                }
            }

            

	    }

        private void OnEnable()
        {
			MiniGame.OnStartPlaying += HandleOnStartPlaying;
        }

        private void OnDisable()
        {
            MiniGame.OnStartPlaying -= HandleOnStartPlaying;
        }

        private void HandleOnStartPlaying()
        {
			fireElapsed -= 1f;
        }

        private void TryShoot()
        {
			// check if another enemy is blocking the sight
			if(Physics.Raycast(transform.position, Vector3.down, out var hit, 1f))
			{
				if (hit.collider.GetComponent<Spaceship>()) return;
			}

			shootAudioSource.Play();

            var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
			// Set speed
			bullet.SetSpeed(-1.5f);
			// Disable collision with the shooter
			var bulletColl = bullet.GetComponent<Collider>();
            Physics.IgnoreCollision(_collider, bulletColl, true);
			// Ignore collisions with the other enemied
			foreach(var coll in otherColliders)
                Physics.IgnoreCollision(coll, bulletColl, true);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(destroyed) return;

			SpaceBullet bullet = collision.collider.GetComponent<SpaceBullet>();

			if(bullet == null) return;
			
			destroyed = true;
			_collider.enabled = false;


			

			miniGame.ReportSpaceshipDestroyed();

			StartCoroutine(PlayExplosionFX());

		
        }

		
		IEnumerator PlayExplosionFX()
		{
			destroyAudioSource.Play();

			var particle = Instantiate(explosionParticlePrefab, transform);
			particle.transform.localPosition = Vector3.zero;
			particle.transform.localEulerAngles = -90f * Vector3.right;
			particle.transform.localScale = Vector3.one * .6f;
			var main = particle.main;
			main.gravityModifierMultiplier = 0;
			

			particle.Play();

			yield return new WaitForSeconds(.2f);
            foreach (Renderer r in renderers)
                r.enabled = false;

            yield return new WaitForSeconds(3);

			Destroy(particle.gameObject);
		}
    }
}
