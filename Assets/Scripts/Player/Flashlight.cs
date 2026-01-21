using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

namespace TMM
{
	public class Flashlight : MonoBehaviour
	{
		[SerializeField]
		Light _light;

		[SerializeField]
		bool available = false;

		[SerializeField]
		AudioSource audioSource;

		[SerializeField]
		AudioClip onClip;

		[SerializeField]
		AudioClip offClip;

		bool isOn = false;


		Animator animator;

        void Awake()
        {
			animator = GetComponentInParent<Animator>();
			_light.gameObject.SetActive(false);

#if UNITY_EDITOR
			//SetAvailable(true);
#endif
        }

        // Start is called before the first frame update
        void Start()
	    {
			// if (!available || !isOn)
			// {
			// 	_light.gameObject.SetActive(false);
			// }

			SetOn(true);
	    }

		// Update is called once per frame
		void Update()
		{
			if (!available) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
				SetOn(!isOn);
            }

		}

		public void SetAvailable(bool available)
		{
			this.available = available;

			if (!available)
			{
				if (isOn)
				{
					isOn = false;
					StartCoroutine(TurnLightOnOff());
					animator.SetBool("LightOn", isOn);
				}
			}

		}

		public bool IsAvailable()
        {
			return available;
        }

		public void SetOn(bool value)
		{
			if (!available) return;

			isOn = value;
			StartCoroutine(TurnLightOnOff());
			animator.SetBool("LightOn", isOn);

		}

		public bool IsOn()
        {
			return isOn;
        }
		
		IEnumerator TurnLightOnOff()
		{
			if (audioSource)
            {
				audioSource.clip = isOn ? onClip : offClip;
				audioSource.Play();
            }

			if(!isOn)
				yield return new WaitForSeconds(.5f);
			_light.gameObject.SetActive(isOn);
            
        }
	}
}
