using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMM.UI
{
    public class ButtonFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {

        Vector3 originalPosition;


        void Awake()
        {
            originalPosition = (transform as RectTransform).anchoredPosition;

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            (transform as RectTransform).DOShakeAnchorPos(.5f, 10, 10).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            UIAudioManager.Instance.PlayEnter();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIAudioManager.Instance.PlayClick();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            (transform as RectTransform).DOKill();
            (transform as RectTransform).anchoredPosition = originalPosition;
            UIAudioManager.Instance.PlayExit();
        }

        private void OnDisable()
        {
            (transform as RectTransform).DOKill();
            (transform as RectTransform).anchoredPosition = originalPosition;
        }
    }
}
