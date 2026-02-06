using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class WineEffect : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> objects;

		[SerializeField]
		FloorTrigger trigger;

		List<Quaternion> rotations = new List<Quaternion>();

        private void Awake()
        {
            foreach (var obj in objects)
            {
                rotations.Add(obj.transform.localRotation);
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
	        StartUntriggeredEffect();
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
			trigger.OnTriggered += HandleOnTriggered;
			trigger.OnUnTriggered += HandleOnUntriggered;
        }

        private void OnDisable()
        {
            trigger.OnTriggered -= HandleOnTriggered;
            trigger.OnUnTriggered -= HandleOnUntriggered;
        }

        private void HandleOnTriggered()
        {
            
            StartTriggeredEffect();
        }

        private void HandleOnUntriggered()
        {
            StartUntriggeredEffect();

        }

		void StartUntriggeredEffect()
		{
			for(int i = 0; i < objects.Count; i++) 
			{
                int index = i;
                objects[index].transform.DOKill();
                //objects[i].transform.DOShakeRotation(1f).SetLoops(-1, LoopType.Restart);
                objects[index].transform.DOLocalRotate(rotations[index].eulerAngles, .2f).SetEase(Ease.OutBack).OnComplete(()=> { objects[index].transform.DOShakeRotation(1f).SetLoops(-1, LoopType.Restart); });
     		}
		}

        void StartTriggeredEffect()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].transform.DOKill();
                var eulers = rotations[i].eulerAngles;
                eulers.x = 90;
                objects[i].transform.DOLocalRotate(eulers, .25f).SetEase(Ease.OutBack);
            }
        }
    }
}
