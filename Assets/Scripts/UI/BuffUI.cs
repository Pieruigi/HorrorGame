using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMM.UI
{
	public class BuffUI : MonoBehaviour
	{
        [SerializeField]
        CanvasGroup canvasGroup;

        [SerializeField]
        bool keepVisible = false;

        [SerializeField]
        Transform container;

        [SerializeField]
        GameObject playerSpeedPrefab;

        [SerializeField]
        Color buffColor, debuffColor;

        [SerializeField]
        TMP_Text messageField;

        bool busy = false;

        float fadeTime = .1f;

        GameObject playerSpeed= null;


        void Awake()
        {
            if (!keepVisible)
                canvasGroup.alpha = 0;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
            //if (Input.GetKeyDown(KeyCode.Tab))
            //    ShowAndHide();
        }

        private void OnEnable()
        {
            PlayerSpeedDebuff.OnApplied += HandleOnBuffApplied;
            PlayerSpeedDebuff.OnExpired += HandleOnBuffExpired;
        }

        private void OnDisable()
        {
            PlayerSpeedDebuff.OnApplied -= HandleOnBuffApplied;
            PlayerSpeedDebuff.OnExpired -= HandleOnBuffExpired;
        }

        private void HandleOnBuffApplied(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() == typeof(PlayerSpeedDebuff))
            {
                //if (!playerSpeed)
                //    playerSpeed = CreateBuff(playerSpeedPrefab);
                messageField.color  = debuffColor;
                messageField.text = $"Speed Reduced for {PlayerSpeedDebuff.Instance.Timer} seconds!";
                ShowAndHide();

            }
        }

        private void HandleOnBuffExpired(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() == typeof(PlayerSpeedDebuff))
            {
                if (playerSpeed)
                {
                    DestroyBuff(playerSpeed);
                    playerSpeed = null;
                }
                
            }
        }

        void ShowAndHide()
        {
            if (keepVisible || busy) return;

            busy = true;
            canvasGroup.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(canvasGroup.DOFade(1, fadeTime));
            seq.AppendInterval(3f);
            seq.Append(canvasGroup.DOFade(0, fadeTime));
            seq.onComplete += () => { messageField.text = ""; busy = false; };
        }

        GameObject CreateBuff(GameObject prefab)
        {
            var ret = Instantiate(playerSpeedPrefab, container);
            ret.transform.localScale = Vector3.zero;
            ret.transform.DOScale(Vector3.one, .5f).SetDelay(.5f).SetEase(Ease.OutBack);
            ShowAndHide();
            return ret;

        }

        void DestroyBuff(GameObject buff)
        {
            buff.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.InBack);
            Destroy(buff, .6f);
            ShowAndHide();
        }
    }
}
