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

		
        public void MoveDown()
		{
			transform.DOLocalMoveY(downY, time).SetEase(Ease.InOutQuad).OnComplete(()=> { Vector3 v = transform.localPosition; v.y = downY; transform.localPosition = v; });
			pillarAudioSource.Play();
		}
		
		public void MoveUp()
        {
			transform.DOLocalMoveY(upY, time).SetEase(Ease.InOutQuad).OnComplete(() => { Vector3 v = transform.localPosition; v.y = upY; transform.localPosition = v; GetComponentInChildren<WorkShiftTrigger>().Activate(); });
			pillarAudioSource.Play();
        }
	}
}
