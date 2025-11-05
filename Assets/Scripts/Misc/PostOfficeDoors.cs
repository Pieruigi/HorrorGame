using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class PostOfficeDoors : MonoBehaviour
	{
		[SerializeField]
		GameObject leftDoor, rightDoor;

		[SerializeField]
		ActivationTrigger trigger;

		bool locked = true;

		bool open = false;


        void Awake()
        {
		}

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.Z))
			// 	if (open) Close(); else Open();

#endif
		}

		void OnEnable()
		{
			trigger.OnEnter += HandleOnTriggerEnter;
			//trigger.OnExit += HandleOnTriggerExit;
		}

        void OnDisable()
        {
            trigger.OnEnter -= HandleOnTriggerEnter;
			//trigger.OnExit -= HandleOnTriggerExit;
        }

        // private void HandleOnTriggerEnter(Collider other)
        // {
		// 	if (locked || open) return;

		// 	Open();
        // }

        private void HandleOnTriggerEnter(Collider other)
        {
			if (other.CompareTag("Player"))
			{
				SetLocked(true);  
	        } 
        }

        public void Close()
		{
			open = false;
			leftDoor.transform.DOKill();
			rightDoor.transform.DOKill();
			leftDoor.transform.DOMoveX(0, 1f).SetEase(Ease.OutBounce);
			rightDoor.transform.DOMoveX(0, 1f).SetEase(Ease.OutBounce);
		}

		public void Open()
		{
			open = true;
			leftDoor.transform.DOKill();
			rightDoor.transform.DOKill();
			leftDoor.transform.DOMoveX(-1.06f, 1f).SetEase(Ease.OutBounce);
			rightDoor.transform.DOMoveX(1.06f, 1f).SetEase(Ease.OutBounce);
		}
		
		public void SetLocked(bool value)
        {
			locked = value;

			if (locked && open) Close();
        }
	}
}
