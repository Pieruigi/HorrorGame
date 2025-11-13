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

		float time = 1f;


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
			GameplayManager.OnTaskCompleted += HandleOnTaskCompleted;
			//trigger.OnExit += HandleOnTriggerExit;
		}

        void OnDisable()
        {
			trigger.OnEnter -= HandleOnTriggerEnter;
			GameplayManager.OnTaskCompleted -= HandleOnTaskCompleted;
			//trigger.OnExit -= HandleOnTriggerExit;
        }

        private void HandleOnTaskCompleted()
		{
			// Open the door
			Open();
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
			leftDoor.transform.DOLocalMoveX(0, time).SetEase(Ease.OutBounce).OnComplete(()=> { Vector3 v = leftDoor.transform.localPosition; v.x = 0; leftDoor.transform.localPosition = v; });
			rightDoor.transform.DOLocalMoveX(0, time).SetEase(Ease.OutBounce).OnComplete(()=> { Vector3 v = rightDoor.transform.localPosition; v.x = 0; rightDoor.transform.localPosition = v; });
		}

		public void Open()
		{
			open = true;
			audioSource.Play();
			leftDoor.transform.DOKill();
			rightDoor.transform.DOKill();
			leftDoor.transform.DOLocalMoveX(1.06f, time).SetEase(Ease.OutBounce);
			rightDoor.transform.DOLocalMoveX(-1.06f, time).SetEase(Ease.OutBounce);
		}
		
		public void SetLocked(bool value)
        {
			locked = value;

			if (locked && open) Close();
        }
	}
}
