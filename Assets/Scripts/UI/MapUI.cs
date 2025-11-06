using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RetroShadersPro.URP;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMM
{
	public class MapUI : MonoBehaviour
	{
		[SerializeField]
		GameObject root;

		[SerializeField]
		GameObject dayBlock;

		[SerializeField]
		GameObject nightBlock;

		bool open = false;

		bool busy = false;

		CRTSettings crtSettings;

		int pixelSizeDefault;
		int scanlineSizeDefault;
		float distortionStrengthDefault;

		bool available = false;

		float transitionTime = .25f;

		CanvasGroup canvasGroup;

        void Awake()
        {
			canvasGroup = root.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;

			// Hide day and night blocks
			dayBlock.SetActive(false);
			nightBlock.SetActive(false);
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
			available = true;
#endif
		}

		// Update is called once per frame
		void Update()
		{
			if (busy || !available) return;

			if (Input.GetKeyDown(KeyCode.M))
			{
				if (!open) Open(); else Close();
			}

		}

		void Open()
		{
			open = true;
			DoOpen();
			
		}

		void Close()
		{
			open = false;
			DoClose();
			
		}

        private void DoClose()
		{
			busy = true;
			var seq = DOTween.Sequence();
			seq.Join(DOTween.To(() => crtSettings.pixelSize.value, x => crtSettings.pixelSize.value = x, pixelSizeDefault, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.scanlineSize.value, x => crtSettings.scanlineSize.value = x, scanlineSizeDefault, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.distortionStrength.value, x => crtSettings.distortionStrength.value = x, distortionStrengthDefault, transitionTime));
			seq.Join(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, transitionTime));
			seq.OnComplete(() => { busy = false; dayBlock.SetActive(false); nightBlock.SetActive(false); });
        }

		void DoOpen()
		{
			busy = true;
			if (DayNightManager.Instance.IsNight) nightBlock.SetActive(true); else dayBlock.SetActive(true);
			var seq = DOTween.Sequence();
			seq.Join(DOTween.To(() => crtSettings.pixelSize.value, x => crtSettings.pixelSize.value = x, 2, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.scanlineSize.value, x => crtSettings.scanlineSize.value = x, 2, transitionTime));
			seq.Join(DOTween.To(() => crtSettings.distortionStrength.value, x => crtSettings.distortionStrength.value = x, 0.04f, transitionTime));
			seq.Join(DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1, transitionTime));
			seq.OnComplete(() => { busy = false; });

		}
		
		
		
	}
}
