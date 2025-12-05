using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class CreatureFootsteps : MonoBehaviour
	{
		[SerializeField]
		AudioSource audioSource;

		[SerializeField]
		List<AudioClip> audioClips;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void PlayFootsteps()
        {
			if (audioSource.isPlaying) audioSource.Stop();

			audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
			audioSource.Play();
        }
	}
}
