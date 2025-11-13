using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class Mailbox : MonoBehaviour
	{
		public delegate void MailboxIsEmptyDelegate(Mailbox mailBox);
		public static MailboxIsEmptyDelegate OnMailboxIsEmpty;

		public delegate void NotAllowedOnNightShiftDelegate(Mailbox mailbox);
		public static NotAllowedOnNightShiftDelegate NotAllowedOnNightShift;

		

		[SerializeField]
		ActivationTrigger proximityTrigger;

		[SerializeField]
		InteractionTrigger interactionTrigger;

		[SerializeField]
		GameObject door;

		[SerializeField]
		GameObject mailPrefab;

		[SerializeField]
		GameObject mailSpawnPoint;

		List<Mail> mails = new List<Mail>();

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
			proximityTrigger.OnEnter += HandleOnProximityEnter;
			proximityTrigger.OnExit += HandleOnProximityExit;
			interactionTrigger.OnInteraction += HandleOnInteraction;
		}

        void OnDisable()
        {
			proximityTrigger.OnEnter -= HandleOnProximityEnter;
			proximityTrigger.OnExit += HandleOnProximityExit;
			interactionTrigger.OnInteraction -= HandleOnInteraction;
        }

        private void HandleOnInteraction()
		{
			CollectMailAll();
			interactionTrigger.SetInteractable(false); // Just for now (if you exit and enter the activation trigger the interaction becomes available again)
        }

        private void HandleOnProximityExit(Collider other)
        {
			if (!other.CompareTag("Player")) return;
			//if(interactionTrigger.IsInteractable())
			interactionTrigger.SetInteractable(false);
        }

        private void HandleOnProximityEnter(Collider other)
        {
			if (!other.CompareTag("Player")) return;
			//if(letters.Count > 0)
			interactionTrigger.SetInteractable(true);
        }

        public void CollectMailAll()
		{
            if (GameplayManager.Instance.NightShift)
			{
				NotAllowedOnNightShift?.Invoke(this);
				return;
            }

			if(mails.Count == 0)
            {
				OnMailboxIsEmpty?.Invoke(this);
				return;
            }
			// Set interaction disabled
			//interactionTrigger.SetInteractable(false);
			
			// Set collected flags
			foreach (var l in mails)
				l.SetCollected();

			PlayEffect(mails.Count);

			// Clear mailbox list
			mails.Clear();

			// Play effect
			
		}

		public void AddMail(Mail mail)
		{
			mails.Add(mail);

			// Set interaction enabled
			// if(!interactionTrigger.IsInteractable())
			// 	interactionTrigger.SetInteractable(true);
		}



		void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Player")) return;
			interactionTrigger.SetInteractable(false);
		}

		void PlayEffect(int count)
		{
			

			// Open door
			door.transform.DOLocalRotate(Vector3.forward * 24f, .2f);

			StartCoroutine(DoCollectEnvelopesEffect(count));
		}

		IEnumerator DoCollectEnvelopesEffect(int count)
		{
			FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
			fpc.InputDisabled = true;
			// Create envelopes
			List<GameObject> envelopes = new List<GameObject>();
			for (int i = 0; i < count; i++)
			{
				var env = Instantiate(mailPrefab);
				env.transform.position = mailSpawnPoint.transform.position;
				env.transform.rotation = mailSpawnPoint.transform.rotation;
				envelopes.Add(env);
			}

			yield return new WaitForSeconds(.5f);

			// Collect envelopes
			foreach (var env in envelopes)
			{
				env.transform.DOMove(GetTargetPosition(), .2f).OnComplete(() => { Destroy(env); });
				//tweener.OnUpdate(()=> { tweener.ChangeEndValue(GetTargetPosition(), .2f); });
				yield return new WaitForSeconds(.5f);
			}

			fpc.InputDisabled = false;
		}
		
		Vector3 GetTargetPosition()
        {
			return Camera.main.transform.position - Vector3.up * .5f;
        }

		public void Reset()
        {
			door.transform.localRotation = Quaternion.identity;
        }
    }
}
