using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMM.AI;
using TMM.Interfaces;
using UnityEngine;

namespace TMM
{
	public class PlayerChased : MonoBehaviour
	{
		[SerializeField]
		AudioSource chaseAudioSource;

		float chaseVolume;

		ClownA clownA;
		ClownB clownB;

		bool chasePlaying = false;

		

        void Awake()
        {
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
			if (clownA?.State == ClownAState.Chase || clownA?.State == ClownAState.Search || clownB?.State == ClownBState.Chase)// && creature.IsPlayerTarget())
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

		void OnEnable()
		{
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
		}

        void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

		private void HandleOnMazeCreated()
		{
			clownA = FindFirstObjectByType<ClownA>();
			clownB = FindFirstObjectByType<ClownB>();
		}
		
		public bool IsPlayerChased(List<GameObject> chasers = null)
		{
			return chasePlaying;
		}
    }
}
