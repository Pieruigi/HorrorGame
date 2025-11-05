using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class WorkShiftGroup : MonoBehaviour
	{
		public static UnityAction OnMovedUp;
		public static UnityAction OnMovedDown;

		float downY = 0;
		float upY = 2.5f;

		float time = 1.6f;

		[SerializeField]
		AudioSource pillarAudioSource;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			WorkShiftButton.OnButtonHit += HandleOnWorkShiftButtonHit;
		}

        void OnDisable()
        {
            WorkShiftButton.OnButtonHit -= HandleOnWorkShiftButtonHit;
        }

        private void HandleOnWorkShiftButtonHit()
        {
			MoveUp();
        }

        public void MoveDown()
		{
			transform.DOMoveY(downY, time).SetEase(Ease.InOutQuad);
			pillarAudioSource.Play();
		}
		
		public void MoveUp()
        {
			transform.DOMoveY(upY, time).SetEase(Ease.InOutQuad).OnComplete(() => { OnMovedUp?.Invoke(); });
			pillarAudioSource.Play();
        }
	}
}
