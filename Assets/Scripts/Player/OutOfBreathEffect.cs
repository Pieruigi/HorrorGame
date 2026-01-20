using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMM.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TMM
{
	public class OutOfBreathEffect : MonoBehaviour
	{
        [SerializeField]
        AudioSource breathAudioSource;

        
        Vignette vignette;

        Tween tween;

        float maxIntensity = .75f;

        StaminaUI staminaUI;

        // Start is called before the first frame update
        void Start()
	    {
            FindFirstObjectByType<Volume>().profile.TryGet<Vignette>(out vignette);
            if(GetComponent<OutOfBreathDebuff>().Value)
            {
                vignette.intensity.value = maxIntensity;
            }
            else
            {
                vignette.intensity.value = 0f;
            };

            staminaUI = FindFirstObjectByType<StaminaUI>();
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            TimedBuffDebuff.OnApplied += OnBuffApplied;
            TimedBuffDebuff.OnExpired += OnBuffExpired;
            FirstPersonController.OnOutOfBreath += HandleOnOutOfBreath;
        }

        private void OnDisable()
        {
            TimedBuffDebuff.OnApplied -= OnBuffApplied;
            TimedBuffDebuff.OnExpired -= OnBuffExpired;
            FirstPersonController.OnOutOfBreath -= HandleOnOutOfBreath;
        }

        private void HandleOnOutOfBreath()
        {
            staminaUI.Shake();
        }

        private void OnBuffApplied(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() != typeof(OutOfBreathDebuff)) return;

            // Apply effect
            tween?.Kill();
            tween = DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, maxIntensity, 0.5f).OnComplete(() => 
            {
                tween = DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, maxIntensity*.5f, 1.272f*.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });

            // Play sound
            breathAudioSource.Play();
        }

        private void OnBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() != typeof(OutOfBreathDebuff)) return;

            // Remove effect
            tween?.Kill();
            DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, 0f, 0.5f);

            // Stop sound
            breathAudioSource.Stop();
        }
    }
}
