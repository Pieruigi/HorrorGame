using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM.UI
{
	public class CameraFade : SingletonPersistent<CameraFade>
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		float fadeTime = .5f;

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

		public void FadeIn()
		{
			canvasGroup.DOKill();
			canvasGroup.DOFade(0, fadeTime);
		}
		
		public void FadeOut()
        {
			canvasGroup.DOKill();
			canvasGroup.DOFade(1, fadeTime);
        }
	}
}
