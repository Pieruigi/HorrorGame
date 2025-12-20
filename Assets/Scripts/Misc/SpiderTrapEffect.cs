using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class SpiderTrapEffect : MonoBehaviour
	{

		[SerializeField]
		FloorTrigger floorTrigger;

		[SerializeField]
		List<GameObject> spiderPrefabs;

		[SerializeField]
		AudioSource audioSource;

		[SerializeField]
		List<AudioClip> clips;

		

		// Start is called before the first frame update
		void Start()
	    {
		
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			floorTrigger.OnTriggered += HandleOnTriggered;
		}

        void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
        }

		private void HandleOnTriggered()
		{
			ApplySpiderEffect();
		}
		
		void ApplySpiderEffect()
		{
			Transform cameraRoot = Camera.main.transform;
			int spiderCount = 3;

			List<GameObject> spiders = new List<GameObject>();

			var seq = DOTween.Sequence();
			
			for (int i = 0; i < spiderCount; i++)
			{
				var spider = Instantiate(spiderPrefabs[Random.Range(0, spiderPrefabs.Count)]);
				spiders.Add(spider);

				spider.transform.parent = cameraRoot;
				spider.transform.localPosition = Vector3.forward * .5f;

				spider.transform.localEulerAngles = new Vector3(Random.Range(0, 360), 90, 90);

				spider.transform.position += spider.transform.forward * .8f;

				spider.transform.localEulerAngles += Vector3.right * Random.Range(-15f, 15f);

				// Move to inizial position
				//spider.transform.localPosition = Vector3.up * .6f + Vector3.forward * .6f + cameraRoot.right * Random.Range(-.9f, .9f);
				var targetPos = spider.transform.position - spider.transform.forward * 1.6f;
				targetPos = cameraRoot.InverseTransformPoint(targetPos);
				seq.Join(spider.transform.DOLocalMove(targetPos, Random.Range(1.5f,2f)));

			}

			seq.OnComplete(() =>
			{
				foreach (var spider in spiders)
					Destroy(spider);

			});

			seq.SetDelay(.5f);

			// Play audio
			audioSource.clip = clips[Random.Range(0, clips.Count)];
			audioSource.PlayDelayed(.5f);

		}
	}
}
