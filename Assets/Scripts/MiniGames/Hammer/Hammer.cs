using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TMM
{
	public class Hammer : MonoBehaviour
	{
		[SerializeField]
		GameObject model;

		[SerializeField]
		AudioSource hitSource;

		[SerializeField]
		List<AudioClip> hitClips;


		Transform pivot;

		Vector3 position;
		Quaternion rotation;

		

		bool active = false;

		bool ready = false;

        private void Awake()
        {
			

			pivot = model.transform.parent;

            // Set local position and rotation
            position = pivot.localPosition;
            rotation = pivot.localRotation;

            Deactivate();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
#if UNITY_EDITOR
			//if (Input.GetKeyDown(KeyCode.X))
			//{
			//	Time.timeScale = Time.timeScale > 0 ? 0 : 1;
			//}
					
#endif

			if(!active || !ready) return;

			if (Input.GetMouseButtonDown(0))
			{
				Hit();
			}
	    }

        private void LateUpdate()
        {
			if(!active) return;

            transform.position = Camera.main.transform.position;
			transform.rotation = Camera.main.transform.rotation;
        }

		void Hit()
		{
			

			Transform hitPoint = model.transform.parent;

			var eulers = pivot.localEulerAngles;
			var position = pivot.localPosition;

			// Raycast
			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;
			var distance = 5f;
			var mask = LayerMask.GetMask(new string[] { "BasePlain" });
			if(Physics.Raycast(origin, direction, out var hit, distance, mask))
			{
			    ready = false;

				Vector3 targetDirection = -hit.normal;

				var targetRotation = Quaternion.LookRotation(targetDirection, hitPoint.up);

                // Move hit point to collision point
                var seq = DOTween.Sequence();

                seq.Append(hitPoint.DOMove(hit.point, .1f).SetEase(Ease.OutBack));
                seq.Join(hitPoint.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.OutBack));

                seq.OnComplete(() => { hitPoint.localEulerAngles = eulers; hitPoint.localPosition = position; ready = true; });

				// Play audio
				hitSource.clip = hitClips[Random.Range(0, hitClips.Count)];
				hitSource.Play();
            }


			
		}

        public void Activate()
		{
			model.SetActive(true);

			active = true;

			ready = true;
		}

		public void Deactivate()
		{
			model.SetActive(false);

			active = false;

			ready = false;
		}
	}
}
