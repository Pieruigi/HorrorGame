using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMM.UI
{
	public class BuyTimeButton : MonoBehaviour
	{
		[SerializeField]
		List<TMP_Text> textFields;

		[SerializeField]
		Color walletFullColor;

		[SerializeField]
		Color walletEmptyColor;

		// Start is called before the first frame update
		void Start()
	    {
			HandleOnBalanceUpdated(0);
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			Wallet.OnBalanceUpdated += HandleOnBalanceUpdated;
		}

        void OnDisable()
        {
            Wallet.OnBalanceUpdated -= HandleOnBalanceUpdated;
        }

        private void HandleOnBalanceUpdated(int amount)
        {
            
			foreach (var t in textFields)
				t.color = Wallet.Instance.Balance > 0 ? walletFullColor : walletEmptyColor;
				
        }
    }
}
