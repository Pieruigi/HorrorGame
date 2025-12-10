using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace TMM.UI
{
	public class MessageUI : Singleton<MessageUI>
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		TMP_Text messageField;

		bool visible = false;

		float fadeTime = .1f;

        protected override void Awake()
		{
			base.Awake();
		

			canvasGroup.alpha = 0;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void ShowMessage(string message)
        {
			if (visible) return;

			visible = true;

			messageField.text = message;

			var seq = DOTween.Sequence();
			seq.Append(canvasGroup.DOFade(1, fadeTime));
			seq.AppendInterval(2f);
			seq.Append(canvasGroup.DOFade(0, fadeTime));


        }

	}
}
