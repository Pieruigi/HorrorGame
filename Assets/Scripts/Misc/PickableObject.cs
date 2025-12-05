using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
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
		
		public void PickUp()
        {
			transform.DOKill();

			var target = Camera.main.transform;
			var offset = Vector3.down * 1.75f;
			var time = .2f;
			var elapsed = 0f;
			
			var tweener = transform.DOMove(target.position + offset, time);
			tweener.OnUpdate(() =>
			{
				tweener.ChangeValues(transform.position, target.position + offset);
				elapsed += Time.deltaTime;
				if (elapsed >= time)
					gameObject.SetActive(false);
			});
			
			
        }
	}
}
