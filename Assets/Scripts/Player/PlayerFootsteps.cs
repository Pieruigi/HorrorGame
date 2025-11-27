using System.Collections;
using System.Collections.Generic;
using System.Threading;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class PlayerFootsteps : MonoBehaviour
	{
		[SerializeField]
		List<AudioClip> clips;

		[SerializeField]
		AudioSource audioSource;

		FirstPersonController player;

		float baseTime = 2f;

		float currentTime;
		float elapsed;

        void Awake()
        {
			player = GetComponent<FirstPersonController>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        audioSource.clip = clips[Random.Range(0, clips.Count)];
			audioSource.PlayDelayed(1.5f);
	    }

		// Update is called once per frame
		void Update()
		{
			if (player.GetSpeed() > 0)
			{
				currentTime = baseTime / player.GetSpeed();

				elapsed += Time.deltaTime;
				if(elapsed > currentTime)
                {
					elapsed -= currentTime;
					audioSource.clip = clips[Random.Range(0, clips.Count)];
					audioSource.Play();
                }
			}
            else
            {
				elapsed = 0;
            }
		}
		
		
	}
}
