using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class PickUpEffect : MonoBehaviour
	{

		[SerializeField]
		AudioSource audioSource;

		GameObject player;

	    // Start is called before the first frame update
	    void Start()
	    {
			player = GameObject.FindGameObjectWithTag("Player");
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void PlayEffect()
		{
			StartCoroutine(DoPlayEffect());
		}
		
		IEnumerator DoPlayEffect()
		{
			transform.DOKill();
			yield return new WaitForSeconds(.5f);
			if(audioSource) audioSource.Play();
			// Store original position and rotation
			var originalPosition = transform.position;
			var originalRotation = transform.rotation;
			// Move the object towards the player
			var targetPosition = player.transform.position + Vector3.up;
			float duration = .25f;
			transform.DOMove(targetPosition, duration);
			//transform.DORotate(Vector3.up * 180f, duration); 

			yield return new WaitForSeconds(duration);
			gameObject.SetActive(false);
			// Reset original position and rotation
			transform.position = originalPosition;
			transform.rotation = originalRotation;
        }
	}
}
