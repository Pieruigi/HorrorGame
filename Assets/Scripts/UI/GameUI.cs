using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RetroShadersPro.URP;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMM
{
	public class GameUI : Singleton<GameUI>
	{
		[SerializeField]
		GameObject root;

		[SerializeField]
		GameObject mapGroup;

		[SerializeField]
		GameObject mailGroup;

		bool mapOpen = false;

		bool mailSelectorOpen = false;

		bool busy = false;

		CRTSettings crtSettings;

		int pixelSizeDefault;
		int scanlineSizeDefault;
		float distortionStrengthDefault;

		bool mapAvailable = false;

		float transitionTime = .25f;

		CanvasGroup canvasGroup;

        protected override void Awake()
		{
			base.Awake();
			canvasGroup = root.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;

			mapGroup.SetActive(false);
			mailGroup.SetActive(false);
        }

		// Start is called before the first frame update
		void Start()
		{
			Volume volume = FindFirstObjectByType<Volume>();

			volume.profile.TryGet(out crtSettings);

			pixelSizeDefault = crtSettings.pixelSize.value;
			scanlineSizeDefault = crtSettings.scanlineSize.value;
			distortionStrengthDefault = crtSettings.distortionStrength.value;

#if UNITY_EDITOR
			//available = true;
#endif
		}

		// Update is called once per frame
		void Update()
		{
			if (busy || !mapAvailable || mailSelectorOpen) return;

			if (Input.GetKeyDown(KeyCode.M))
			{
				if (!mapOpen) OpenMapGroup(); else CloseMapGroup();
			}

		}

		void OpenMapGroup()
		{
			mapOpen = true;
			DoOpen();
			
		}

		void CloseMapGroup()
		{
			DoClose();
			mapOpen = false;
		}

		public void OpenMailGroup()
		{

		}
		
		public void CloseMailGroup()
        {
            
        }

        private void DoClose()
		{
			busy = true;
			var seq = DOTween.Sequence();
			seq.Join(DOTween.To(() => crtSettings.pixelSize.value, x => crtSettings.pixelSize.value = x, pixelSizeDefault, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.scanlineSize.value, x => crtSettings.scanlineSize.value = x, scanlineSizeDefault, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.distortionStrength.value, x => crtSettings.distortionStrength.value = x, distortionStrengthDefault, transitionTime));
			seq.Join(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, transitionTime));
			seq.OnComplete(() =>
			{
				busy = false;
				if (mapOpen)
					mapGroup.SetActive(false);
				else
					mailGroup.SetActive(false);
			});
        }

		void DoOpen()
		{
			busy = true;
			if (mapOpen)
				mapGroup.SetActive(true);
			else
				mailGroup.SetActive(true);

			var seq = DOTween.Sequence();
			seq.Join(DOTween.To(() => crtSettings.pixelSize.value, x => crtSettings.pixelSize.value = x, 2, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.scanlineSize.value, x => crtSettings.scanlineSize.value = x, 2, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.distortionStrength.value, x => crtSettings.distortionStrength.value = x, 0.04f, transitionTime));
			seq.Join(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, transitionTime));
			seq.OnComplete(() => { busy = false; });

		}
		
		

		public void SetMapAvailable(bool value)
        {
			mapAvailable = value;
        }
		
	}
}
