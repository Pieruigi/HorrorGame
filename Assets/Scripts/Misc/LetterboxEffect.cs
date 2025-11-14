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

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayDeliverEffect();
            }
#endif
		}
		
		public void PlayDeliverEffect()
		{
			// Stop player
			FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
			fpc.InputDisabled = true;

			// Open the door
			door.transform.DOLocalRotate(Vector3.right * 90f, .5f).SetEase(Ease.OutBounce).onComplete += () =>
			{
				// Create envelope
				var envelope = Instantiate(envelopePrefab);

				// Set position below the camera
				envelope.transform.position = Camera.main.transform.position - Vector3.up * .5f;
				envelope.transform.rotation = envelopeTarget.rotation;

				// Move the envelope to the letterbox
				envelope.transform.DOMove(envelopeTarget.position, .5f).onComplete += () =>
				{
					// We can release player at this point
					fpc.InputDisabled = false;

					// Close door and move wing
					Sequence seq = DOTween.Sequence();
					seq.Join(door.transform.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBounce));
					seq.Join(wing.transform.DOLocalRotate(Vector3.right * 90f, .5f).SetEase(Ease.OutBounce));

					seq.onComplete += () => { Destroy(envelope); };
                };
            };
        }

        public void Reset()
        {
			wing.transform.localRotation = Quaternion.identity;
        }
    }
}
