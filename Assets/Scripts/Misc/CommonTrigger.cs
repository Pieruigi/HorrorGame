using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class CommonTrigger : MonoBehaviour
	{
		public delegate void EnterDelegate(CommonTrigger trigger, Collider collider);
		public static EnterDelegate OnEnter;

		public delegate void ExitDelegate(CommonTrigger trigger, Collider collider);
		public static ExitDelegate OnExit;


		void OnTriggerEnter(Collider other)
		{
			OnEnter?.Invoke(this, other);
		}

        void OnTriggerExit(Collider other)
        {
			OnExit?.Invoke(this, other);
        }
    }
}
