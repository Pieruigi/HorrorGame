using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class TrapAlarmEffect : MonoBehaviour
	{
		[SerializeField]
		Transform megaphoneContainer;

		[SerializeField]
		FloorTrigger floorTrigger;

		float triggeredY = -.175f;

	    // Start is called before the first frame update
	    void Start()
	    {
			if (floorTrigger.Triggered)
				megaphoneContainer.localPosition = Vector3.up * triggeredY;
            
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
			floorTrigger.OnTriggered += HandleOnTriggered;
			floorTrigger.OnUnTriggered += HandleOnUnTriggered;
        }

        private void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
			floorTrigger.OnUnTriggered -= HandleOnUnTriggered;
        }

        private void HandleOnTriggered()
        {
            megaphoneContainer.DOKill();
			megaphoneContainer.DOLocalMoveY(triggeredY, .2f).SetEase(Ease.OutBack);
        }

        private void HandleOnUnTriggered()
        {
            megaphoneContainer.DOKill();
            megaphoneContainer.DOLocalMoveY(0, .2f).SetEase(Ease.OutBack).OnComplete(() => { megaphoneContainer.localPosition = Vector3.zero; });
        }
    }
}
