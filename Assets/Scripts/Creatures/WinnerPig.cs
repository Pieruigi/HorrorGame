using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class WinnerPig : MonoBehaviour
	{
		AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
	    {
			StartCoroutine(PlayGrunt());
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		IEnumerator PlayGrunt()
		{
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(2f, 4f));

				audioSource.Play();
            }
		}
	}
}
