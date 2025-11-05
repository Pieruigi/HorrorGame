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

		[SerializeField]
		AudioSource audioSource;

		bool locked = true;

		bool open = false;

		float time = 1.75f;


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
			if (Input.GetKeyDown(KeyCode.Z))
				if (open) Close(); else Open();

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
				trigger.SetEnabled(false);
				SetLocked(true);
	        } 
        }

        public void Close()
		{
			open = false;
			audioSource.Play();
			leftDoor.transform.DOKill();
			rightDoor.transform.DOKill();
			leftDoor.transform.DOMoveX(0, time).SetEase(Ease.OutBounce);
			rightDoor.transform.DOMoveX(0, time).SetEase(Ease.OutBounce);
		}

		public void Open()
		{
			open = true;
			audioSource.Play();
			leftDoor.transform.DOKill();
			rightDoor.transform.DOKill();
			leftDoor.transform.DOMoveX(-1.06f, time).SetEase(Ease.OutBounce);
			rightDoor.transform.DOMoveX(1.06f, time).SetEase(Ease.OutBounce);
		}
		
		public void SetLocked(bool value)
        {
			locked = value;

			if (locked && open) Close();
        }
	}
}
