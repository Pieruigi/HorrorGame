using System.Collections.Generic;
using DG.Tweening;
using TMM.AI;
using UnityEngine;

namespace TMM
{
	public class CreatureAudio : MonoBehaviour
	{
		[SerializeField]
		AudioSource laughAudioSource;

		[SerializeField]
		List<AudioClip> laughClips;

		[SerializeField]
		AudioSource chaseAudioSource;

		float laughElapsed = 0;

		float laughTime = 4;

		Creature creature;

		bool chasePlaying = false;

		float chaseVolume;

        void Awake()
        {
			creature = GetComponent<Creature>();
			chaseVolume = chaseAudioSource.volume;
			chaseAudioSource.volume = 0;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			laughElapsed += Time.deltaTime;
			if (laughElapsed > laughTime)
            {
				laughElapsed = Random.Range(-1f, 1f);
				Laugh();
            }

			if ((creature.State == CreatureState.Chase || creature.State == CreatureState.Search) && creature.IsPlayerTarget())
			{

				//if (!chaseAudioSource.isPlaying) chaseAudioSource.Play();
				if (!chasePlaying)
				{
					chasePlaying = true;
					chaseAudioSource.DOKill();
                    chaseAudioSource.DOFade(chaseVolume, 1f);
                }
					
			}
            else
			{
                //if (chaseAudioSource.isPlaying) chaseAudioSource.Stop();
                if (chasePlaying)
                {
					chasePlaying = false;
					chaseAudioSource.DOKill();
					chaseAudioSource.DOFade(0, 1f);
                }
            }
		}

        private void Laugh()
        {
			laughAudioSource.clip = laughClips[Random.Range(0, laughClips.Count)];
			laughAudioSource.Play();
        }
    }
}
