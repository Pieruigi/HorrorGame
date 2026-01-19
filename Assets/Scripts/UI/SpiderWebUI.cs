using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM.UI
{
	public class SpiderWebUI : MonoBehaviour
	{
		[SerializeField]
		CanvasGroup canvasGroup;

        [SerializeField]
        AudioSource removeAudioSource;

        float maxAlpha = 0.25f;

        // Start is called before the first frame update
        void Start()
        {
            canvasGroup.alpha = PlayerSpeedDebuff.Instance.Timer > 0 ? maxAlpha : 0;
        }
        private void OnEnable()
        {
            PlayerSpeedDebuff.OnApplied += HandleOnApplied;
            PlayerSpeedDebuff.OnExpired += HandleOnExpired;
        }

        private void OnDisable()
        {
            PlayerSpeedDebuff.OnApplied -= HandleOnApplied;
            PlayerSpeedDebuff.OnExpired -= HandleOnExpired;
        }

        private void HandleOnExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(PlayerSpeedDebuff))
            {
                canvasGroup.DOFade(0f, 1f);
                removeAudioSource.Play();
            }
        }

        private void HandleOnApplied(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() == typeof(PlayerSpeedDebuff))
            {
                canvasGroup.DOFade(maxAlpha, 1f).SetDelay(1f);
            }
        }
    }
}
