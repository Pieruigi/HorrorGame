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
		public delegate void NoMailForThisLetterboxDelegate(Letterbox letterbox);
		public static NoMailForThisLetterboxDelegate OnNoMailForThisLetterbox;

		public delegate void ThisLetterboxIsAlreadyFullDelegate(Letterbox letterbox);
		public static ThisLetterboxIsAlreadyFullDelegate OnThisLetterboxIsAlreadyFull;

		public delegate void WrongMailForThisLetterboxDelegate(Letterbox letterbox);
		public static WrongMailForThisLetterboxDelegate OnWrongMailForThisLetterbox;

		public delegate void OnlyAllowedOnNightShiftDelegate(Letterbox letterbox);
		public static OnlyAllowedOnNightShiftDelegate OnlyAllowedOnNightShift;

		

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
			Debug.Log("TEST - Trying delive mail");
			// Letterbox interaction is only allowed on night shift
			if(!GameplayManager.Instance.NightShift)
            {
				OnlyAllowedOnNightShift?.Invoke(this);
				return;
            }

			// We open a UI to let the player choose the mail to deliver, but first we want to check if the mail has already been delivered or if there is no mail to deliver at all
			Mail mail = MailManager.Instance.Mails.ToList().Find(l => l.Address == address);

			// If mail is null send an event to scare the player
			if (mail == null)
			{
				OnNoMailForThisLetterbox?.Invoke(this);
				return;
			}

			// If the mail has already been delivered send an event to scare the player
			if (mail.Delivered)
			{
				OnThisLetterboxIsAlreadyFull?.Invoke(this);
				return;
			}
			Debug.Log("TEST - Set delivered");
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
			if(!full)
				interactionTrigger.SetInteractable(true);
        }

		public void DeliverMail(Mail mail)
		{
			if (mail.Address != address)
			{
				OnWrongMailForThisLetterbox?.Invoke(this);
				return;
			}

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
