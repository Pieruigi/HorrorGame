using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class WinnerLever : MonoBehaviour
	{
		[SerializeField]
		GameObject spiderPrefab;

		[SerializeField]
		Transform spiderTarget;

		[SerializeField]
		Transform leverPivot;

		[SerializeField]
		GameObject floor;

		[SerializeField]
		Collider leverCollider;

		[SerializeField]
		GameObject endMessageCanvas;

		[SerializeField]
		AudioSource screamAudioSource;

		[SerializeField]
		AudioSource clownLaughAudioSource;

		[SerializeField]
		AudioSource leverAudioSource;

		[SerializeField]
		AudioSource musicAudioSource;

        [SerializeField]
        AudioSource stingerAudioSource;

        [SerializeField]
        AudioSource stinger2AudioSource;


        bool inside = false;

	    // Start is called before the first frame update
	    void Start()
	    {
			//MessageManager.Instance.ShowCustomMessage(10, false);
			StartCoroutine(ShowBeFreeMessage());
        }

	    // Update is called once per frame
	    void Update()
	    {
	        if(!inside) return;

			var origin = Camera.main.transform.position;
			var direction = Camera.main.transform.forward;
			var distance = 1;
			bool showMessage = false;
            if (Physics.Raycast(origin, direction, distance, LayerMask.GetMask(new string[] { "Interactable"})))
            {
                showMessage = true;
				if (Input.GetKeyDown(KeyCode.E))
				{
					// Disable collider
					showMessage = false;
					GetComponent<Collider>().enabled = false;
					PullTheLever();
				}
            }
			else
			{
				showMessage = false;
            }

			if (showMessage)
			{
                if(!MessageManager.Instance.IsMessageVisible()) 
					MessageManager.Instance.ShowCustomMessage(6, true);
            }

			else
			{
                if (MessageManager.Instance.IsMessageVisible()) 
					MessageManager.Instance.HideMessage();
            }
                

        }

        private void OnTriggerEnter(Collider other)
        {
			if (!other.CompareTag("Player")) return;

			inside = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			inside = false;
        }

		IEnumerator ShowBeFreeMessage()
		{
            MessageManager.Instance.ShowCustomMessage(10, true);
            yield return new WaitForSeconds(3f);
			MessageManager.Instance.HideMessage();
		}

		void PullTheLever()
		{
			// Stop player 
			//FirstPersonController player = FindFirstObjectByType<FirstPersonController>();
			//player.InputDisabled = true;
			//player.AimingDisabled = true;
			leverCollider.enabled = false;
            if (MessageManager.Instance.IsMessageVisible())
                MessageManager.Instance.HideMessage();

			
            //MessageManager.Instance.ShowCustomMessage(9, false);
            StartCoroutine(ShowMessageDelayed(.5f+.75f));

            clownLaughAudioSource.Play();
			musicAudioSource.Stop();

            // Spawn spider
            var position = Camera.main.transform.position + Camera.main.transform.forward * .5f - Camera.main.transform.right * 2f + Camera.main.transform.up * 2f;
			var rotation = Quaternion.identity;
			var spider = Instantiate(spiderPrefab, position, rotation);
			spider.transform.parent = spiderTarget;

			// Move spider to the lever
			var seq = DOTween.Sequence();
			seq.Append(spider.transform.DOMove(spiderTarget.position, .2f));
			seq.Join(spider.transform.DORotateQuaternion(spiderTarget.rotation, .2f));
			var eulers = leverPivot.localEulerAngles;
            seq.AppendCallback(() => { leverAudioSource.PlayDelayed(.25f); stingerAudioSource.PlayDelayed(.25f+.95f); stinger2AudioSource.PlayDelayed(.25f + .7f); });
            seq.Append(leverPivot.DOLocalRotate(new Vector3(eulers.x, eulers.y, -18f), .2f).SetEase(Ease.OutBack));
			

			seq.AppendInterval(.5f);
			seq.AppendCallback(() => { floor.SetActive(false); screamAudioSource.PlayDelayed(1f); });

			seq.AppendInterval(4f);
			seq.AppendCallback(() => { GameManager.Instance.StartNewGame(); });


			IEnumerator ShowMessageDelayed(float time)
			{
				yield return new WaitForSeconds(time);
                endMessageCanvas.GetComponent<FinalMessage>().PlayFinalSequence();
			
            }
			
		}
    }
}
