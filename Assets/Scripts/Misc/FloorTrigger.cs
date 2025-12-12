using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TMM
{
	public class FloorTrigger : MonoBehaviour
	{
		//public delegate void TriggeredDelegate();
		public UnityAction OnTriggered;

		[SerializeField]
		ActivationTrigger activationTrigger;

		[SerializeField]
		List<GameObject> steps;

		float height;


		bool inside = false;

		CharacterController characterController;

		bool triggered = false;

		bool[] stepDirections = new bool[4];

        void Awake()
        {
			height = transform.position.y;
			HideStepAll();
			
        }

        // Start is called before the first frame update
        void Start()
	    {
			characterController = FindFirstObjectByType<CharacterController>();
			transform.localPosition = Vector3.up * height;
			ShowSteps();
	    }

		// Update is called once per frame
		void Update()
		{
			if (!inside || triggered) return;

			var center = characterController.transform.position + characterController.center;
			var distance = characterController.height / 2f + .05f;
			RaycastHit hit;
			Debug.Log("Center:" + center);
			Debug.Log("Distance:" + distance);
			if(Physics.Raycast(center, Vector3.down, out hit, distance, LayerMask.GetMask(new string[] { "Floor" })))
			{
				Debug.Log("Hit:" + hit.collider.gameObject);
				if(hit.collider.gameObject == gameObject)
				{
					triggered = true;
					MoveDown();
					OnTriggered?.Invoke();
				}
			}
		}

		void OnEnable()
		{
			activationTrigger.OnEnter += HandleOnEnter;
			activationTrigger.OnExit += HandleOnExit;
		}

        void OnDisable()
        {
			activationTrigger.OnEnter -= HandleOnEnter;
			activationTrigger.OnExit -= HandleOnExit;
        }

        private void HandleOnEnter(Collider other)
        {
			inside = true;
        }

		private void HandleOnExit(Collider other)
		{
			inside = false;
		}

		void HideStepAll()
		{
			foreach (var step in steps)
				step.SetActive(false);
		}

		void ShowSteps()
		{
			for(int i=0; i<stepDirections.Length; i++)
				steps[i].SetActive(stepDirections[i]);
			
		}

		void MoveDown()
		{
			transform.DOKill();
			transform.DOMoveY(0, .5f).OnComplete(()=> { HideStepAll(); });
		}

		public void ResetTrigger()
		{
			if (!triggered) return;

			ShowSteps();

			transform.DOKill();
			transform.DOMoveY(height, .5f).OnComplete(() => { triggered = false; });
		}
		
		public void SetStepDirection(int index, bool visible)
		{
			stepDirections[index] = visible;
		}
    }
}
