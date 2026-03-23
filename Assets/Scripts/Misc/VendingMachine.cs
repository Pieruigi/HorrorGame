using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	/// <summary>
	/// NoTriggerTiles only disables trigger tiles, neither switchs the alarm off nor remove debuff cause by an already triggered trap
	/// </summary>
	public enum VendingMachineType { NoTriggerTiles, Map, CuteClown }

	public class VendingMachine : MonoBehaviour
	{
		public static UnityAction<VendingMachineType> OnPurchased;

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
		TMP_Text timeField;

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

		int messageId = -1;


        void Awake()
        {
			messageId = deviceInteractor.MessageId;
        }

        // Start is called before the first frame update
        void Start()
	    {
			Init();
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.X))
			// {
			// 	if(type == VendingMachineType.Map)
			// 	{
			// 		Map.Instance.SetTimer(60);
			// 	}
			// }

#endif
		}

		void OnEnable()
		{
			DeviceInteractor.OnInteraction += HandleOnInteraction;


			TriggerTileManager.OnChanged += HandleOnTriggerTileManagerChanged;
			AlarmManager.OnActivated += HandleOnAlarmActivated;
			AlarmManager.OnDeactivated += HandleOnAlarmDeactivated;

			TimedBuffDebuff.OnApplied += HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired += HandleOnDeBuffExpired;

			Map.OnExpired += HandleOnMapExpired;
        }

        void OnDisable()
        {
			DeviceInteractor.OnInteraction -= HandleOnInteraction;

			TriggerTileManager.OnChanged -= HandleOnTriggerTileManagerChanged;
            AlarmManager.OnActivated -= HandleOnAlarmActivated;
			AlarmManager.OnDeactivated -= HandleOnAlarmDeactivated;

            TimedBuffDebuff.OnApplied -= HandleOnDeBuffApplied;
            TimedBuffDebuff.OnExpired -= HandleOnDeBuffExpired;

            Map.OnExpired -= HandleOnMapExpired;
        }

        private void HandleOnMapExpired()
        {
            if(type == VendingMachineType.Map)
			{
				InitButton(false);
				InitDescription(false);
			}
        }

        private void HandleOnDeBuffApplied(TimedBuffDebuff arg0)
        {
			if (arg0.GetType() == typeof(StupidClownBuff) && type == VendingMachineType.CuteClown) 
			{
				InitButton(true);
				InitDescription(true);
				return;
			}
        }

        private void HandleOnDeBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() == typeof(StupidClownBuff) && type == VendingMachineType.CuteClown)
            {
                InitButton(false);
                InitDescription(false);
                return;
            }
        }

        private void HandleOnAlarmActivated()
        {
            InitButton(true);
        }

        private void HandleOnAlarmDeactivated()
        {
            if(type != VendingMachineType.NoTriggerTiles || !TriggerTileManager.Instance.TriggerTilesDisabled)
				InitButton(false);
        }

        private void HandleOnTriggerTileManagerChanged()
        {
			if (type != VendingMachineType.NoTriggerTiles) return;

			StartCoroutine(Switch());
        }

		private void HandleOnInteraction(DeviceInteractor deviceInteractor)
		{
			if (this.deviceInteractor != deviceInteractor) return;

			if (AlarmManager.Instance.IsActive()) return; // Cannot use while alarm is active

            switch (type)
			{
				case VendingMachineType.NoTriggerTiles:
					if (TriggerTileManager.Instance.TriggerTilesDisabled) return; // Already disabled (unless we want to give the player the change to buy more time)
					if (Wallet.Instance.TryUseCoins(cost))
					{
                        TriggerTileManager.Instance.DisableTriggers(timer);
						OnPurchased?.Invoke(VendingMachineType.NoTriggerTiles);
                    }
						
					break;
				case VendingMachineType.Map:
					if (Wallet.Instance.TryUseCoins(cost))
					{
						Map.Instance.SetTimer(timer);
						StartCoroutine(SwitchAndForceOff());
						OnPurchased?.Invoke(VendingMachineType.Map);
					}
					break;
				case VendingMachineType.CuteClown:
					if (Wallet.Instance.TryUseCoins(cost))
					{
						//StupidClownBuff.Instance.Timer = timer;
						StupidClownBuff.Instance.Apply();
						OnPurchased?.Invoke(VendingMachineType.CuteClown);
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
					InitButton(Map.Instance.Available);
					InitDescription(Map.Instance.Available);
					break;
				case VendingMachineType.CuteClown:
					InitButton(StupidClownBuff.Instance.IsActive);
					InitDescription(StupidClownBuff.Instance.IsActive);
					InitTime(timer);
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
			deviceInteractor.MessageId = disabled ? -1 : messageId;
        }

		void InitTime(float time)
		{
			timeField.text = $"{time.ToString("00")} sec";
		}

		// public void SetType(VendingMachineType type)
		// {
		// 	this.type = type;
		// 	Init();
		// }

	}
}
