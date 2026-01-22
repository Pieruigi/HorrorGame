using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering;

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

		[SerializeField]
		List<GameObject> tileSpiders;

		List<Vector3> tileSpiderOriginalPositions = new List<Vector3>();


        private void Awake()
        {
            foreach(var s in tileSpiders)
			{
				tileSpiderOriginalPositions.Add(s.transform.localPosition);
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
			if(!floorTrigger.Triggered)
				StartJumping();
			else
				KillJumpingSpiders();

        }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			floorTrigger.OnTriggered += HandleOnTriggered;
			floorTrigger.OnUnTriggered += HandleOnUnTriggered; 

        }

        void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
			floorTrigger.OnUnTriggered -= HandleOnUnTriggered; 
        }

        private void HandleOnUnTriggered()
        {
            StartJumping();
        }

        private void HandleOnTriggered()
		{
			ApplySpiderEffect();
			KillJumpingSpiders();
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

		void KillJumpingSpiders()
		{
			foreach(var s in tileSpiders)
			{
				s.transform.DOKill();
				var index = tileSpiders.IndexOf(s);
				s.transform.DOLocalMoveY(tileSpiderOriginalPositions[index].y, .25f).OnComplete(() => { s.transform.localPosition = tileSpiderOriginalPositions[index]; });
				Animator animator = s.GetComponent<Animator>();
				//if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
				s.GetComponent<Animator>().SetTrigger("Dead");
            }
        }

		void StartJumping()
		{
			foreach(var s in tileSpiders)
			{
				s.transform.DOKill();
				s.transform.DOLocalMoveY(0.5f, .5f).SetDelay(Random.Range(.1f, 1f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
				Animator animator = s.GetComponent<Animator>();
				if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                    s.GetComponent<Animator>().SetTrigger("Jump");
            }
		}
	}
}
