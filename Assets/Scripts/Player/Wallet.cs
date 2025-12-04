using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class Wallet : SingletonPersistent<Wallet>
	{
		int balance = 0;


		public void AddCoins(int amount)
		{
			balance += amount;
		}

		public bool HasEnoughCoins(int amount)
		{
			return balance >= amount;
		}

		public bool TryUseCoins(int amount)
		{
			if (balance < amount) return false;

			balance -= amount;

			return true;
		}
		
		public void ClearWallet()
        {
			balance = 0;
        }
	}
}
