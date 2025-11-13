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
			WorkShiftTrigger.OnEquipmentReturnedBack += HandleOnEquipmentReturnedBack;
		}

        void OnDisable()
        {
			WorkShiftButton.OnButtonHit -= HandleOnWorkShiftButtonHit;
			WorkShiftTrigger.OnEquipmentReturnedBack -= HandleOnEquipmentReturnedBack;
        }

        private void HandleOnEquipmentReturnedBack()
        {
			MoveDown();
        }

        private void HandleOnWorkShiftButtonHit()
        {
			MoveUp();
        }

        public void MoveDown()
		{
			transform.DOLocalMoveY(downY, time).SetEase(Ease.InOutQuad).OnComplete(()=> { Vector3 v = transform.localPosition; v.y = downY; transform.localPosition = v; OnMovedDown?.Invoke(); });
			pillarAudioSource.Play();
		}
		
		public void MoveUp()
        {
			transform.DOLocalMoveY(upY, time).SetEase(Ease.InOutQuad).OnComplete(() => { Vector3 v = transform.localPosition; v.y = upY; transform.localPosition = v; OnMovedUp?.Invoke(); });
			pillarAudioSource.Play();
        }
	}
}
