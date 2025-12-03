using System.Collections;
using System.Collections.Generic;
using TMM.Interfaces;
using UnityEngine;

namespace TMM
{
	public class Bullet : MonoBehaviour
	{
		[SerializeField]
		AudioSource thudAudioSource;

		[SerializeField]
		List<AudioClip> thudClips;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnCollisionEnter(Collision collision)
		{

			//GetComponent<Collider>().enabled = false;
			// ITarget iT = collision.gameObject.GetComponent<ITarget>();
			// if (iT != null)
			// {
			// 	Debug.Log("HIT - Bullet - IT");
			// 	iT.Hit(gameObject);

			// }
			// else
			// {
			//     Debug.Log("HIT - Bullet - " + collision.gameObject);
			// }

			GetComponent<Collider>().enabled = false;
			transform.GetChild(0).gameObject.SetActive(false);
			PlayThudAudio();
			Destroy(gameObject, 1);

		}
		
		void PlayThudAudio()
        {
			thudAudioSource.clip = thudClips[Random.Range(0, thudClips.Count)];
			thudAudioSource.Play();
        }
    }
}
