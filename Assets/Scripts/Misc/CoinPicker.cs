using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class CoinPicker : MonoBehaviour
	{
		[SerializeField]
		ActivationTrigger trigger;

		[SerializeField]
		int amount = 1;

		[SerializeField]
		GameObject model;

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
			trigger.OnEnter += HandleOnTriggerEnter;
		}

		private void HandleOnTriggerEnter(Collider other)
		{
			trigger.SetEnabled(false);
			Wallet.Instance.AddCoins(amount);

			// Do some effect
			PlayFX();

			Destroy(gameObject, 1);
		}

		void PlayFX()
		{
			GetComponentInChildren<PickableObject>().PickUp();
        }





	}
}
