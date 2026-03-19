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

		[SerializeField]
		TMP_Text escField;

		bool visible = false;
		public bool Visible
		{
			get{ return visible; }
		}

		float fadeTime = .1f;

        protected override void Awake()
		{
			base.Awake();


			canvasGroup.alpha = 0;

			escField.gameObject.SetActive(false);

#if UNITY_WEBGL
			escField.text = "";
#endif
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void ShowMessage(string message, bool keepOn = false)
		{
			if (visible) return;

			Debug.Log("Showwwwwww " + message);

			canvasGroup.DOKill();

			visible = true;

			messageField.text = message;

			var seq = DOTween.Sequence();
			seq.Append(canvasGroup.DOFade(1, fadeTime));
			if (!keepOn)
			{
				seq.AppendInterval(2f);
				seq.Append(canvasGroup.DOFade(0, fadeTime));
				seq.OnComplete(() => { visible = false; });
			}

		}
		
		public void HideMessage()
		{
			if (!visible) return;

			visible = false;

			canvasGroup.DOKill();

			canvasGroup.DOFade(0, fadeTime);
		}

	}
}
