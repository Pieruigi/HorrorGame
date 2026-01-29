using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class MainPig : MonoBehaviour
	{
		[SerializeField]
		Transform startPoint;

		AudioSource audioSource;


		float time = 5f;
		float currentTime = 0;
		float err = .2f;

		bool playing = false;
		float speed = 2f;
		
        private void Awake()
        {
			err *= time;
			audioSource = GetComponent<AudioSource>();
			GetComponentInChildren<Animator>().SetTrigger("Walk");
        }

        // Start is called before the first frame update
        void Start()
	    {
            RandomizeRoot();
            currentTime = Random.Range(time - err, time + err);
	    }

	    // Update is called once per frame
	    void Update()
	    {
			if (!playing)
			{
				currentTime -= Time.deltaTime;

				if(currentTime < 0)
				{
					playing = true;
					
					StartPig();
				}
			}

	    }

		void StartPig()
		{
			StopAllCoroutines();
			StartCoroutine(Move());
			StartCoroutine(Talk());
		}

		IEnumerator Move()
		{
			// Reset position
			transform.position = startPoint.position;
			transform.rotation = startPoint.rotation;
			float speed = Random.Range(0.4f, .8f);

			while (playing)
			{
				transform.position += transform.forward * speed * Time.deltaTime;
				yield return null;
			}
		}

		IEnumerator Talk()
		{
			while (playing)
			{
                yield return new WaitForSeconds(Random.Range(2f, 3f));

				audioSource.Play();
            }
		}

		public void StopPlaying()
		{
			playing = false;
            currentTime = Random.Range(time - err, time + err);
            StopAllCoroutines();

			RandomizeRoot();
        }

		void RandomizeRoot()
		{
            var root = startPoint.parent;
            root.transform.localEulerAngles += Vector3.up * 180f;
            root.transform.localPosition = Vector3.forward * Random.Range(-.55f, .55f);
        }
	}
}
