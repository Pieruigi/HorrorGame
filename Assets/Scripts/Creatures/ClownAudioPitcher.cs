using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class ClownAudioPitcher : MonoBehaviour
	{
        [SerializeField]
        AudioSource audioSource;

        float pitchDefault;

        private void Awake()
        {
            pitchDefault = audioSource.pitch;
        }

        // Start is called before the first frame update
        void Start()
	    {
            if (StupidClownBuff.Instance.IsActive)
            {
                audioSource.pitch = StupidClownBuff.Instance.LaughPitchMultiplier * pitchDefault;
            }
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            TimedBuffDebuff.OnApplied += HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired += HandleOnDeBuffExpired;
        }

        private void OnDisable()
        {
            TimedBuffDebuff.OnApplied -= HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired -= HandleOnDeBuffExpired;
        }

        private void HandleOnDeBuffApplied(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                audioSource.pitch = (arg0 as StupidClownBuff).LaughPitchMultiplier * pitchDefault;
                return;
            }
        }

        private void HandleOnDeBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                audioSource.pitch = pitchDefault;
                return;
            }
        }
    }
}
