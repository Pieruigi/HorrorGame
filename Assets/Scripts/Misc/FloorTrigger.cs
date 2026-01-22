using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace TMM
{
	public class FloorTrigger : MonoBehaviour
	{
		//public delegate void TriggeredDelegate();
		public UnityAction OnTriggered;
		public UnityAction OnUnTriggered;

		[SerializeField]
		ActivationTrigger activationTrigger;

		[SerializeField]
		List<GameObject> steps;

		[SerializeField]
		AudioSource audioSource;

		float height;


		bool inside = false;

		CharacterController characterController;

		bool triggered = false;
		public bool Triggered
		{
			get{ return triggered; }
		}

		bool[] stepDirections = new bool[4];

		Material tileMaterial;
		Material stepMaterial;

		float materialIntensity = 1f;//6.5f;
		
		
        void Awake()
        {
			height = transform.position.y;
			transform.localPosition = Vector3.up * height;
			HideStepAll();

			// Get tile material
			var rend = GetComponent<Renderer>();
			tileMaterial = new Material(rend.material);
			tileMaterial.SetVector("_BaseColor", new Vector4(1, 1, 1, 1) * materialIntensity);
			rend.material = tileMaterial;

			// Get step material
			rend = steps[0].GetComponent<Renderer>();
			stepMaterial = new Material(rend.material);
			stepMaterial.SetVector("_BaseColor", new Vector4(1, 1, 1, 1) * materialIntensity);
			foreach(var step in steps)
				step.GetComponent<Renderer>().material = stepMaterial;
			
        }

        // Start is called before the first frame update
        void Start()
	    {
			characterController = FindFirstObjectByType<CharacterController>();

			ShowSteps();



	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.X))
			// {
			// 		triggered = true;
			// 		MoveDown();
			// 		OnTriggered?.Invoke();
			// }
#endif

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
			float time = .5f;
			var seq = DOTween.Sequence();
			seq.Append(transform.DOMoveY(0, time));
			// seq.Join(tileMaterial.DOVector(new Vector4(1, 1, 1, 0), "_BaseColor", time));
			// seq.Join(stepMaterial.DOVector(new Vector4(1, 1, 1, 0), "_BaseColor", time));
			seq.OnComplete(() => { HideStepAll(); /*MazeBuilder.Instance.BuildNavMesh();*/ });

			// Play audio
			audioSource.Play();
		}

		public void ResetTrigger()
		{
			if (!triggered) return;

			ShowSteps();

			transform.DOKill();
			float time = .5f;
			var seq = DOTween.Sequence();
			seq.Append(transform.DOMoveY(height, time));
			// seq.Join(tileMaterial.DOVector(new Vector4(1, 1, 1, 1) * materialIntensity, "_BaseColor", time));
			// seq.Join(stepMaterial.DOVector(new Vector4(1, 1, 1, 1) * materialIntensity, "_BaseColor", time));
			seq.OnComplete(() => { triggered = false; OnUnTriggered?.Invoke(); /*MazeBuilder.Instance.BuildNavMesh();*/ });

			// Play audio
			audioSource.Play();
		}

		public void SetStepDirection(int index, bool visible)
		{
			stepDirections[index] = visible;
		}

		public void SwitchOff()
		{
			if (triggered) return;
			triggered = true;
			MoveDown();
		}
		
		
    }
}
