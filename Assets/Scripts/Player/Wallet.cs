using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class Wallet : SingletonPersistent<Wallet>
	{
		public delegate void BalanceUpdatedDelegate(int amount);
		public static BalanceUpdatedDelegate OnBalanceUpdated;

		[SerializeField]
		AudioSource addCoinAudioSource;

		[SerializeField]
		AudioSource removeCoinAudioSource;

		int balance = 0;
		public int Balance
        {
            get{ return balance; }
        }

        void Update()
        {
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.X))
			{
				Wallet.Instance.AddCoins(3);
			}
#endif
        }

        void OnEnable()
		{
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
		}

        void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.name == "GameScene")
            {
                if(GameManager.Instance.GameStage == 1)
                {
					ClearWallet();
                }
            }
        }

        public void AddCoins(int amount)
		{
			balance += amount;
			addCoinAudioSource.Play();
			OnBalanceUpdated?.Invoke(amount);
		}

		public bool HasEnoughCoins(int amount)
		{
			return balance >= amount;
		}

		public bool TryUseCoins(int amount)
		{
			if (balance < amount) return false;

			balance -= amount;
			removeCoinAudioSource.Play();

			OnBalanceUpdated?.Invoke(-amount);

			return true;
		}
		
		void ClearWallet()
		{
			balance = 0;
	    }
	}
}
