using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TMM.UI
{
	public class WalletUI : MonoBehaviour
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		TMP_Text balanceField;

		[SerializeField]
		bool keepVisible = false;

		bool visible = false;

		float fadeTime = .1f;

		bool busy = false;

		
        void Awake()
        {
			if(!keepVisible)
				canvasGroup.alpha = 0;
			
        }

        // Start is called before the first frame update
        void Start()
	    {
			balanceField.text = Wallet.Instance.Balance.ToString("00");
			//ShowAndHide();

	    }

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Tab))
				ShowAndHide();
		}

		void OnEnable()
		{
			Wallet.OnBalanceUpdated += HandleOnBalanceUpdated;
		}

        void OnDisable()
        {
            Wallet.OnBalanceUpdated -= HandleOnBalanceUpdated;
        }

		private void HandleOnBalanceUpdated(int amount)
		{
			balanceField.text = Wallet.Instance.Balance.ToString("00");
			ShowAndHide();
		}

		void Show(bool value)
		{
			if (value == visible || busy || keepVisible) return;

			visible = value;

			canvasGroup.DOKill();

			canvasGroup.DOFade(visible ? 1 : 0, fadeTime);
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
			seq.onComplete += ()=>{ busy = false; };
		}
		
		
		
    }
}
