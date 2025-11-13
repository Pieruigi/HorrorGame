using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class MusicManager : SingletonPersistent<MusicManager>
	{
		[SerializeField]
		AudioSource daylightAudioSource;

		[SerializeField]
		AudioSource nightAudioSource;

		[SerializeField]
		AudioSource preShiftSource;

		float daylightVolume;
		float preShiftVolume;

        protected override void Awake()
        {
			base.Awake();

			daylightVolume = daylightAudioSource.volume;
			preShiftVolume = preShiftSource.volume;
        }

        // Start is called before the first frame update
        void Start()
	    {
			PlayPreShiftMusic();
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void PlayDaylightMusic(float delay = 0)
		{
			//StopPreShiftMusic(delay);

			Debug.Log("TEST - Playing music");
			daylightAudioSource.volume = 0;
			daylightAudioSource.DOFade(daylightVolume, 1f).SetDelay(delay);
			daylightAudioSource.PlayDelayed(delay);
		}

		public void StopDaylightMusic(float delay=0)
		{
			if (!daylightAudioSource.isPlaying) return;
			daylightAudioSource.DOFade(0, 1f).SetDelay(delay).OnComplete(() => { daylightAudioSource.Stop(); });
		}
		
		public void StopNightMusic(float delay)
		{
			if (!nightAudioSource.isPlaying) return;
			nightAudioSource.DOFade(0, 1f).SetDelay(delay).OnComplete(()=> { nightAudioSource.Stop(); });
        }

		public void PlayNightMusic(float delay = 0)
		{
			PlayDaylightMusic(delay); // TODO: change audio source
		}

		public void PlayPreShiftMusic(float delay = 0)
		{
			// StopDaylightMusic();
			// StopNightMusic();
			preShiftSource.volume = 0;
			preShiftSource.DOFade(preShiftVolume, 1f).SetDelay(delay);
			preShiftSource.PlayDelayed(delay);
		}
		
		public void StopPreShiftMusic(float delay = 0)
        {
			preShiftSource.DOFade(0, 1f).SetDelay(delay).OnComplete(() => { preShiftSource.Stop(); });
        }
	}
}
