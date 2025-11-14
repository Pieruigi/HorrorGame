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
		// public delegate void MailboxIsEmptyDelegate(Mailbox mailBox);
		// public static MailboxIsEmptyDelegate OnMailboxIsEmpty;

		// public delegate void NotAllowedOnNightShiftDelegate(Mailbox mailbox);
		// public static NotAllowedOnNightShiftDelegate NotAllowedOnNightShift;

		

		[SerializeField]
		ActivationTrigger proximityTrigger;

		[SerializeField]
		InteractionTrigger interactionTrigger;

		[SerializeField]
		MailboxEffect mailboxEffect;


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

		/// <summary>
        /// Deactivate interaction trigger when we move away
        /// </summary>
        /// <param name="other"></param>
		private void HandleOnProximityExit(Collider other)
		{
			if (!other.CompareTag("Player")) return;
			if(interactionTrigger.IsInteractable())
				interactionTrigger.SetInteractable(false);
		}

		/// <summary>
		/// Activate interaction trigger when we are close enough
		/// </summary>
		/// <param name="other"></param>
		private void HandleOnProximityEnter(Collider other)
		{
			if (!other.CompareTag("Player")) return;
			if(mails.Count > 0)
				interactionTrigger.SetInteractable(true);
		}

        public void CollectMailAll()
		{
            if (GameplayManager.Instance.NightShift)
			{
				//NotAllowedOnNightShift?.Invoke(this);
				return;
            }

			if(mails.Count == 0)
            {
				//OnMailboxIsEmpty?.Invoke(this);
				return;
            }
			// Set interaction disabled
			//interactionTrigger.SetInteractable(false);
			
			// Set collected flags
			foreach (var l in mails)
				l.SetCollected();

			mailboxEffect.PlayEffect(mails.Count);

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

		public void Reset()
        {
			mailboxEffect.Reset();
        }
    }
}
