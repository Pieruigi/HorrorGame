using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMM.Interfaces;
using UnityEditor;
using UnityEngine;

namespace TMM
{
	public class Target : MonoBehaviour, ITarget
	{
		Transform modelRoot;

		Shooter shooter;

		bool hit = false;
		bool hidden = false;

		Collider _collider;

        void Awake()
        {
			shooter = transform.root.GetComponentInChildren<Shooter>();
			modelRoot = transform.GetChild(0);
			_collider = GetComponent<Collider>();
        }

        // Start is called before the first frame update
        void Start()
		{
			//var origin = transform.localPosition.y;
			//float target;
			//if (origin == 0)
			//	target = 0.16f;
			//else
			//	target = 0f;
			//transform.DOLocalMoveY(target, .5f).SetLoops(-1, LoopType.Yoyo);
	    }

		// Update is called once per frame
		void Update()
		{

		}

        void OnCollisionEnter(Collision collision)
        {
			Hit(collision.gameObject);
        }

        public void Hit(GameObject hitter)
        {
            if (hit) return;

			//if (hitter.GetComponent<Bullet>())
			{
				Debug.Log("HIT + " + gameObject.name);
				hit = true;
				//modelRoot.DOLocalRotate(Vector3.up * 90, .25f).SetEase(Ease.OutBounce);
				modelRoot.gameObject.SetActive(false);
				_collider.isTrigger = true;	
                shooter.ReportTargetHit(gameObject);
			}
        }

		public void Hide()
		{
			modelRoot.gameObject.SetActive(false);
			_collider.isTrigger = true;
			hidden = true;
        }

        public void Reset()
        {
			if (!hit || hidden) return;

			modelRoot.gameObject.SetActive(true);
			_collider.isTrigger = false;
			hit = false;
        }
    }
}
