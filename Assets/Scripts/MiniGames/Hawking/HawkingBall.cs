using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class HawkingBall : MonoBehaviour
	{
        [SerializeField]
        GameObject model;

        [SerializeField]
        ParticleSystem particles;

        [SerializeField]
        AudioSource popSource;

        [SerializeField]
        List<AudioClip> popClips;

        Rigidbody rb;

        float lifeTime = 5f;

        HawkingGame miniGame;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>(); 
        }

        // Start is called before the first frame update
        void Start()
	    {
#if UNITY_EDITOR
            miniGame = FindFirstObjectByType<HawkingGame>();
#endif

            // Randomize force
            var dir = transform.forward * Random.Range(.5f, .7f);
            dir.Normalize();

            var mag = Random.Range(1.8f, 2.4f);


            rb.AddForce(dir * mag, ForceMode.VelocityChange);

            // Add torque
            var min = 40;
            var max = 90;
            var torque = Random.Range(min, max) * Vector3.right + Random.Range(min, max) * Vector3.up + Random.Range(min, max) * Vector3.forward;
            rb.AddTorque(torque, ForceMode.VelocityChange);

            Destroy(gameObject, lifeTime);
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
        }

        private void OnEnable()
        {
            MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
            MiniGame.OnStopPlaying += HandleOnStopPlaying;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
            MiniGame.OnStopPlaying -= HandleOnStopPlaying;
        }

        private void HandleOnStopPlaying()
        {
            Destroy(gameObject);
        }

        private void HandleOnMazeCreated()
        {
            miniGame = FindFirstObjectByType<HawkingGame>();    
        }

        private void FixedUpdate()
        {
            rb.AddForce(Vector3.down * .7f, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.GetComponent<Bullet>()) return;

            miniGame.ReportHit();

            particles.transform.parent = null;
            particles.transform.localScale = Vector3.one * .4f;
            particles.Play();
            
            Destroy(particles.gameObject, 3);
            //model.SetActive(false); 

            popSource.clip = popClips[Random.Range(0, popClips.Count)];
            popSource.Play();

            GetComponent<Collider>().enabled = false;   
            Destroy(gameObject);
        }

    }
}
