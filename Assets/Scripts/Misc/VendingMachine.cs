using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMM
{
	/// <summary>
	/// NoTriggerTiles only disables trigger tiles, neither switchs the alarm off nor remove debuff cause by an already triggered trap
	/// </summary>
	public enum VendingMachineType { NoTriggerTiles, Map }

	public class VendingMachine : MonoBehaviour
	{

		[SerializeField]
		VendingMachineType type = VendingMachineType.NoTriggerTiles;

		[SerializeField]
		float timer = 60;

		[SerializeField]
		int cost = 1;

		[SerializeField]
		TMP_Text buttonField;

		[SerializeField]
		TMP_Text descriptionField;

		[SerializeField]
		Color enabledColor;

		[SerializeField]
		Color disabledColor;

		[SerializeField]
		DeviceInteractor deviceInteractor;

		[SerializeField]
		AudioSource switchAudioSource;

		[SerializeField]
		List<AudioClip> switchClips;

		
        void Awake()
        {
		
        }

        // Start is called before the first frame update
        void Start()
	    {
			Init();
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			DeviceInteractor.OnInteraction += HandleOnInteraction;


			TriggerTileManager.OnChanged += HandleOnTriggerTileManagerChanged;
		}

        void OnDisable()
        {
			DeviceInteractor.OnInteraction -= HandleOnInteraction;

			TriggerTileManager.OnChanged -= HandleOnTriggerTileManagerChanged;
        }

        private void HandleOnTriggerTileManagerChanged()
        {
			if (type != VendingMachineType.NoTriggerTiles) return;

			StartCoroutine(Switch());
        }

		private void HandleOnInteraction(DeviceInteractor deviceInteractor)
		{
			if (this.deviceInteractor != deviceInteractor) return;

			switch (type)
			{
				case VendingMachineType.NoTriggerTiles:
					if (TriggerTileManager.Instance.TriggerTilesDisabled) return; // Already disabled (unless we want to give the player the change to buy more time)
					if (Wallet.Instance.TryUseCoins(cost))
						TriggerTileManager.Instance.DisableTriggers(timer);
					break;
				case VendingMachineType.Map:
					if (Wallet.Instance.TryUseCoins(cost))
					{
						Map.Instance.SetTimer(timer);
						StartCoroutine(SwitchAndForceOff());	
					}
					break;
			}

		}

		IEnumerator SwitchAndForceOff()
		{
			yield return Switch();
			InitButton(true);
			InitDescription(true);
		}
		
		IEnumerator Switch()
		{
			yield return new WaitForSeconds(.5f);
			
			switchAudioSource.clip = switchClips[Random.Range(0, switchClips.Count)];
			switchAudioSource.Play();
			yield return new WaitForSeconds(.1f);
			Init();
		}
		
        void Init()
		{
			switch (type)
			{
				case VendingMachineType.NoTriggerTiles:
					InitButton(TriggerTileManager.Instance.TriggerTilesDisabled);
					InitDescription(TriggerTileManager.Instance.TriggerTilesDisabled);
					break;
				case VendingMachineType.Map:
					InitButton(false);
					InitDescription(false);
					break;
			}
		}

		void InitButton(bool disabled)
		{
			buttonField.color = disabled ? disabledColor : enabledColor;
		}

		void InitDescription(bool disabled)
		{
			descriptionField.color = disabled ? enabledColor : disabledColor;
		}

		// public void SetType(VendingMachineType type)
		// {
		// 	this.type = type;
		// 	Init();
		// }

	}
}
