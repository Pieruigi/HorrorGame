using System.Collections.Generic;
using TMM.AI;
using UnityEngine;

namespace TMM
{
	public class CreatureGrowl : MonoBehaviour
	{
		[SerializeField]
		List<AudioClip> clips;

		AudioSource audioSource;

		Creature creature;

		float timer = 5;

		float currentTimer = 0;

		bool _spotted = false;

        void Awake()
        {
			creature = GetComponentInParent<Creature>();
			audioSource = GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			if (!_spotted) return;

			currentTimer -= Time.deltaTime;
			if(currentTimer < 0)
            {
				PlayRandomGrowl();
				InitTimer();
            }
		}

		void OnEnable()
		{
			Creature.OnPlayerSpotted += HandleOnPlayerSpotted;
		}

        void OnDisable()
        {
            Creature.OnPlayerSpotted -= HandleOnPlayerSpotted;
        }

		private void HandleOnPlayerSpotted(Creature creature, bool spotted)
		{
			if (creature != this.creature) return;
			if (_spotted == spotted) return;

			_spotted = spotted;

			if (spotted)
			{
				if (!audioSource.isPlaying) PlayRandomGrowl();
				InitTimer();
			}
		}

		void PlayRandomGrowl()
		{
			audioSource.clip = clips[Random.Range(0, clips.Count)];
			audioSource.Play();
		}
		
		void InitTimer()
        {
            currentTimer = Random.Range(timer * .8f, timer * 1.2f);
        }
    }
}
