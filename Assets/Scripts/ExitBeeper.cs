using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class ExitBeeper : MonoBehaviour
	{
		[SerializeField]
		AudioSource source;

        Transform target;

        float minSpeed = .5f;
        float maxSpeed = 8f;

        Transform player;

        bool loop = false;

        float elapsed = 0;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void LateUpdate()
        {
            if (!loop) return;

            elapsed += Time.deltaTime;

            if(elapsed > 1f/GetSpeed())
            {
                elapsed = 0;
                source.Play();
            }
        }

        private void OnEnable()
        {
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
			MiniGame.OnMiniGameBeaten += HandleOnMinigameBeaten;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
            MiniGame.OnMiniGameBeaten -= HandleOnMinigameBeaten;
        }

        private void HandleOnMazeCreated()
        {
            target = FindFirstObjectByType<ExitDoor>().transform;
            player = FindFirstObjectByType<FirstPersonController>().transform;
        }

        private void HandleOnMinigameBeaten(MiniGame minigame)
        {

            StartCoroutine(Do());

            IEnumerator Do()
            {
                yield return new WaitForSeconds(2f);

                source.Play();

                loop = true;
                elapsed = 0;
            }
        }

        float GetSpeed()
        {
            var pPos = player.position;
            var tPos = target.position;
            //pPos.y = tPos.y = 0;
            var dist = Vector3.Distance(pPos, tPos);
            var maxDistance = 40f;
            var minDist = 2f;
            if (dist > maxDistance) return minSpeed;
            if(dist < minDist) return maxSpeed;
            return Mathf.Lerp(maxSpeed, minSpeed, dist / maxDistance);
        }
    }
}
