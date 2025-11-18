using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using Unity.Mathematics;
using UnityEngine;

namespace TMM
{
	public class LetterboxEffect : MonoBehaviour
	{

		[SerializeField]
		GameObject door;

		[SerializeField]
		GameObject wing;

		[SerializeField]
		GameObject envelopePrefab;

		[SerializeField]
		Transform envelopeTarget;

		[SerializeField]
		AudioSource doorOpenAudioSource;

		[SerializeField]
		AudioSource doorCloseAudioSource;

		[SerializeField]
		AudioSource mailAudioSource;

		[SerializeField]
		AudioSource wrongChoiceAudioSource;

		[SerializeField]
		GameObject fish;


        void Awake()
        {
			fish.SetActive(false);
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
			// {
			// 	//PlayDeliverEffect();
			// 	PlayWrongChoiceEffect();
            // }
#endif
		}

		public void PlayDeliverEffect()
		{
			// Stop player
			FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
			fpc.InputDisabled = true;

			// Play open door audio 
			doorOpenAudioSource.Play();

			// Open the door
			door.transform.DOLocalRotate(Vector3.right * 90f, .5f).SetEase(Ease.OutBounce).onComplete += () =>
			{
				// Create envelope
				var envelope = Instantiate(envelopePrefab);

				// Set position below the camera
				envelope.transform.position = Camera.main.transform.position - Vector3.up * .5f;
				envelope.transform.rotation = envelopeTarget.rotation;

				// Play envelope audio
				mailAudioSource.Play();

				// Move the envelope to the letterbox
				envelope.transform.DOMove(envelopeTarget.position, .2f).onComplete += () =>
				{
					// We can release player at this point
					fpc.InputDisabled = false;

					// Play close audio
					doorCloseAudioSource.Play();

					// Close door and move wing
					Sequence seq = DOTween.Sequence();
					seq.Join(door.transform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBounce));
					seq.Append(wing.transform.DOLocalRotate(Vector3.right * 90f, .5f).SetEase(Ease.OutBounce));

					seq.onComplete += () => { Destroy(envelope); };
				};
			};
		}

		public void PlayWrongChoiceEffect()
		{
			wrongChoiceAudioSource.Play();

			// Play jump scare camera shake
			Camera.main.transform.root.GetComponentInChildren<CameraShake>().PlayLetterboxJumpScare();

			// Get collider
			var collider = GetComponent<Collider>();

			// Store original position and rotation
			var originalPos = transform.position;
			var originalRot = transform.rotation;

			// Move and rotate the letterbox to the camera
			var targetPos = Camera.main.transform.position + Camera.main.transform.forward * 1.142f;
			var targetRot = Quaternion.LookRotation(-Camera.main.transform.forward);

			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOMove(targetPos, 0));
			seq.Join(transform.DOLocalRotateQuaternion(targetRot, 0));
			seq.Join(door.transform.DOLocalRotate(Vector3.right * 100, 0));

			seq.AppendInterval(1f);
			seq.Append(transform.DOMove(originalPos, 0));
			seq.Join(transform.DOLocalRotateQuaternion(originalRot, 0));
			seq.Join(door.transform.DOLocalRotate(Vector3.zero, 0));

			seq.OnStart(() => { collider.enabled = false; fish.SetActive(true); });
			seq.OnComplete(() => { collider.enabled = true; fish.SetActive(false);});
			
		}
		


        public void Reset()
        {
			wing.transform.localRotation = Quaternion.identity;
        }
    }
}
