using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using NUnit.Framework.Internal;
using UnityEngine;

namespace TMM
{
	public class PeekJumpscare : Jumpscare
	{

		[SerializeField]
		List<GameObject> prefabs;

		/// <summary>
		/// 0: north-east
		/// 1: south-east
		/// 2: south-west
		/// 3: north-west
		/// </summary>
		[SerializeField]
		List<ActivationTrigger> triggers;

		GameObject clown;

		void OnEnable()
		{
			foreach (var t in triggers)
				t.OnEnter += (c) => { Test(t, c); }; 
		}

		void OnDisable()
		{

		}
		
		void Test(ActivationTrigger t, Collider other)
		{
			if (!other.CompareTag("Player")) return;
			if (Triggered) return;

			int index = triggers.IndexOf(t);

			
		}

        public override void ReportUsed()
		{
			// Instantiate a clown
			clown = Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform);


		}

        protected override bool CheckPlay()
        {
			return false;
        }
	}
}
