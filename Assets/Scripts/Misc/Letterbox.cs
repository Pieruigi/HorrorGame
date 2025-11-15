using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class Letterbox : MonoBehaviour
	{
		

		[SerializeField]
		Address address;

		public Address Address
		{
			get { return address; }
		}

		[SerializeField]
		ActivationTrigger activationTrigger;

		[SerializeField]
		InteractionTrigger interactionTrigger;

		[SerializeField]
		LetterboxEffect letterboxEffect;

		bool full = false;

        void Awake()
        {
			interactionTrigger.SetInteractable(false);
        }

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
			activationTrigger.OnEnter += HandleOnActivationEnter;
			activationTrigger.OnExit += HandleOnActivationExit;
			interactionTrigger.OnInteraction += HandleOnInteraction;
		}

        void OnDisable()
        {
            activationTrigger.OnEnter -= HandleOnActivationEnter;
			activationTrigger.OnExit -= HandleOnActivationExit;
			interactionTrigger.OnInteraction -= HandleOnInteraction;
        }

        private void HandleOnInteraction()
		{
			if (full) return;

			Debug.Log("TEST - Trying delive mail");
		
			// We open a UI to let the player choose the mail to deliver, but first we want to check if the mail has already been delivered or if there is no mail to deliver at all
			Mail mail = MailManager.Instance.Mails.ToList().Find(l => l.Address == address);

			// If mail is null send an event to scare the player
			if (mail == null)
			{
				letterboxEffect.PlayWrongChoiceEffect();
				return;
			}

			
			// Ok, lets open a UI to let the player choose the mail to deliver (for now we just deliver the mail)
			//mail.SetDelivered();
			DeliverMail(mail);
        }

        private void HandleOnActivationExit(Collider other)
		{
			if(interactionTrigger.IsInteractable())
				interactionTrigger.SetInteractable(false);
        }

        private void HandleOnActivationEnter(Collider other)
		{
			if(!full && GameplayManager.Instance.NightShift)
				interactionTrigger.SetInteractable(true);
        }

		public void DeliverMail(Mail mail)
		{
			mail.SetDelivered();

			full = true;

			letterboxEffect.PlayDeliverEffect();

		}
		
		public void Reset()
        {
			full = false;
			letterboxEffect.Reset();
        }
	}
}
