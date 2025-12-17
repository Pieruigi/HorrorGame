using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
	public class PinMap : MonoBehaviour
	{
		CanvasGroup canvasGroup;

		Image image;

		float fadeTime = .25f;

		bool visible = false;

        void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			image = GetComponent<Image>();
			canvasGroup.alpha = 0;
        }

        public void Show()
		{
			if (visible) return;
			visible = true;
			canvasGroup.DOKill();
			canvasGroup.DOFade(1, fadeTime);
		}

		public void Hide()
		{
			if (!visible) return;
			visible = false;
			canvasGroup.DOKill();
			canvasGroup.DOFade(0, fadeTime);
		}

		public void SetGoodPin()
		{
			image.color = Color.green;
		}
		
		public void SetBadPin()
		{
			image.color = Color.red;
		}
	}
}
