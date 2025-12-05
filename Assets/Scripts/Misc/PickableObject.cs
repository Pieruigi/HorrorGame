using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class PickableObject : MonoBehaviour
	{
		
		// Start is called before the first frame update
		void Start()
	    {
			Float();
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		void Float()
		{
			float targetY = .5f;
			var seq = DOTween.Sequence();
			seq.Append(transform.DOLocalMoveY(targetY, .5f).SetLoops(-1, LoopType.Yoyo));
			seq.Join(transform.DOLocalRotate(Vector3.up * 90f, .5f).SetLoops(-1, LoopType.Yoyo));
			
        }
	}
}
