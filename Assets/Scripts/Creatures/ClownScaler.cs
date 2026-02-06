using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class ClownScaler : MonoBehaviour
	{
        float scaleDefault = 1f;

        private void Awake()
        {
            scaleDefault = transform.localScale.x;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            TimedBuffDebuff.OnApplied += HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired += HandleOnDeBuffExpired;
            StupidClownBuff.OnForcedExpired += HandleOnStupidForcedExpired;
        }

        private void OnDisable()
        {
            TimedBuffDebuff.OnApplied -= HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired -= HandleOnDeBuffExpired;
            StupidClownBuff.OnForcedExpired -= HandleOnStupidForcedExpired;
        }

        private void HandleOnStupidForcedExpired()
        {
            transform.DOKill();
            transform.localScale = Vector3.one * scaleDefault;
        }

        private void HandleOnDeBuffApplied(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                transform.DOKill();
                transform.DOScale((arg0 as StupidClownBuff).ScaleMultiplier * scaleDefault, 0.2f).SetEase(Ease.OutBack);
                return;
            }
        }

        private void HandleOnDeBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff))
            {
                transform.DOKill();
                transform.DOScale(scaleDefault, 0.2f).SetEase(Ease.OutBack);
                return;
            }
        }
    }
}
