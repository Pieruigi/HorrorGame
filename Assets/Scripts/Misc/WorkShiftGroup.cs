using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class WorkShiftGroup : MonoBehaviour
	{
		float downY = 0;
		float upY = 2;

		float time = 3;

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
		}
		
		public void MoveUp()
        {
            transform.DOMoveY(upY, time).SetEase(Ease.InOutQuad);
        }
	}
}
