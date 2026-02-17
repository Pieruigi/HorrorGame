using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMM
{
	public class HawkingGame : MiniGame
	{
		[SerializeField]
		Gun gun;

        [SerializeField]
        Transform[] spawners;

        [SerializeField]
        GameObject ballPrefab;

		[SerializeField]
		TMP_Text counterUI;

        [SerializeField]
        AudioSource whipSource;

        [SerializeField]
        List<AudioClip> whipClips;

        int count = 30;

        int jumpscareScore = -1;

        int nextSpawner = 0;

        float spawnTime = .8f;

        float spawnElapsed = 0;

        protected override void Awake()
        {
            base.Awake();

            counterUI.text = count.ToString("00");  
        }

        protected override void Update()
        {
            base.Update();

            if (!IsActive) return;

            // Check for spawning
            spawnElapsed += Time.deltaTime;
            if(spawnElapsed >= spawnTime)
            {
                spawnElapsed = 0;

                var spawner = spawners[nextSpawner];
                nextSpawner = (nextSpawner + 1) % spawners.Length;

                // Spawn
                var ball = Instantiate(ballPrefab, spawner.position, spawner.rotation);

                whipSource.clip = whipClips[Random.Range(0, whipClips.Count)];
                whipSource.PlayDelayed(.5f);
                //var vel = spawner.rotation.eulerAngles;
                //vel.y *= 4f;// Random.Range(.8f, 1.2f);
                //vel = vel.normalized * 6;// Random.Range(3f, 5f);
                //ball.GetComponent<Rigidbody>().velocity = vel;
            }
        }

        public override void DoChildActivation()
        {
            spawnElapsed = 0.5f; // Delay

            gun.Activate(true);
        }

        public override void DoChildDeactivation()
        {

            gun.Activate(false);
        }

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific move
            jumpscareScore = Random.Range(5, count - 5);
        }

        public void ReportHit()
        {
            count--;

            counterUI.text = count.ToString("00");

            if (count <= 0)
                ReportBeaten();

            if (jumpscareScore > 0 && count == jumpscareScore)
            {
                jumpscareScore = -1;
                MiniJumpscare.Play();
            }
                
        }
    }
}
