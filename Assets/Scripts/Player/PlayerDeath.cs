using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	public class PlayerDeath : MonoBehaviour
	{
		public static UnityAction OnPlayerDead;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void Die(GameObject killer)
        {
			Debug.Log("YOU ARE DEAD.................................");
			GetComponent<FirstPersonController>().Die();
			transform.root.GetComponentInChildren<Flashlight>().SetOn(false);

			

			OnPlayerDead?.Invoke();
        }
	}
}
