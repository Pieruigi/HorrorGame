using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class PutDownEffect : MonoBehaviour
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
			Debug.Log("TEST - PlayEffect - " + gameObject);
			gameObject.SetActive(true);
			var originalPosition = transform.position;
			var originalRotation = transform.rotation;
			var targetPosition = player.transform.position + Vector3.up;
			transform.position = targetPosition;
			
			StartCoroutine(DoPlayEffect(originalPosition, originalRotation));
		}
		
		IEnumerator DoPlayEffect(Vector3 position, Quaternion rotation)
		{
			transform.DOKill();
			yield return new WaitForSeconds(.5f);
			if (audioSource) audioSource.Play();

			// Move to the original position
			float duration = .25f;
			Debug.Log("TEST - Position:" + position);
			transform.rotation = rotation;
			transform.DOMove(position, duration);
			//transform.DORotate(Vector3.up * 180f, duration); 

        }
	}
}
