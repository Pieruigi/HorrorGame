using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using StarterAssets;
using TMM.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


namespace TMM
{
	public abstract class MiniGame : MonoBehaviour
	{
		public delegate void MiniGameBeatenDelegate(MiniGame miniGame);
		public static MiniGameBeatenDelegate OnMiniGameBeaten;

		[SerializeField]
		Transform playerTarget;

		[SerializeField]
		bool activateDot;

		[SerializeField]
		DeviceInteractor deviceInteractor;

		//float attempts = 10;
		[SerializeField]
		float timer = 30;

		[SerializeField]
		AudioSource beatenAudioSource;

		[SerializeField]
		Light mainLight;

		public Light MainLight
        {
            get{ return mainLight; }
        }
		
		float timeLeft = 30;
		public float TimeLeft
        {
            get{ return timeLeft; }
        }


		FirstPersonController player;

		//Transform cameraRoot;

		bool activated = false;
		public bool IsActive
        {
            get{ return activated; }
        }

		float moveTime = .25f;
		

		bool beaten = false;

		Vector3 lastPlayerPosition;
		Quaternion lastPlayerRotation;

		Flashlight flashlight;

		bool activateFlashlightOnExit = false;

		float recheargeTime = 20;
		float recheargeElapsed = 0;


		protected virtual void Awake()
        {
			timeLeft = timer;
        }

	    // Start is called before the first frame update
	    protected virtual void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();
			//cameraRoot = player.GetComponent<CameraShake>().transform;
			flashlight = player.transform.parent.GetComponentInChildren<Flashlight>();
	    }

		// Update is called once per frame
		protected virtual void Update()
		{
#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.Z))
			// {
			// 	if (!activated)
			// 		Activate();
			// 	else
			// 		Deactivate();
			// }
#endif
			if (activated)
			{
				if (Input.GetMouseButtonDown(1))
				{
					Deactivate();
					return;
				}

				timeLeft -= Time.deltaTime;
				if (timeLeft < 0)
				{
					timeLeft = 0;
					recheargeElapsed = 0;
					Deactivate();
					return;
				}

				//DoUpdate();
			}
            else
			{
				if(timeLeft == 0)
                {
					recheargeElapsed += Time.deltaTime;
					if(recheargeElapsed > recheargeTime)
					{
						timeLeft = timer;	
					}    
                }
				
            }
           
		}

		protected virtual void OnEnable()
		{
			DeviceInteractor.OnInteraction += HandleOnDeviceInteraction;
			PlayerDeath.OnPlayerDead += HandleOnPlayerDead;
		}

        protected virtual void OnDisable()
        {
			DeviceInteractor.OnInteraction -= HandleOnDeviceInteraction;
			PlayerDeath.OnPlayerDead -= HandleOnPlayerDead;
        }

        private void HandleOnPlayerDead()
		{
			// Kill any running tween
			player.transform.DOKill();

			activated = false;

			DotCanvas.Instance.Hide();
        }

        private void HandleOnDeviceInteraction(DeviceInteractor deviceInteractor)
		{
			if (this.deviceInteractor != deviceInteractor) return;

            if(timeLeft>0)
           		Activate();
            
        }

        public void Activate()
		{
			if (activated || timeLeft <= 0 || beaten) return;

			if (flashlight.IsOn())
            {
				flashlight.SetOn(false);
				activateFlashlightOnExit = true;
            }
				

			// Deactivate the device interactor
			deviceInteractor.SetEnable(false);

			// Kill any possible running tween
			player.transform.DOKill();

			// Stop player from moving
			player.InputDisabled = true;

			// Store last player position and rotation
			lastPlayerPosition = player.transform.position;
			lastPlayerRotation = player.transform.rotation;

			// Move the controller to the target position
			Sequence seq = DOTween.Sequence();
			seq.Append(player.transform.DOMove(playerTarget.position, moveTime));
			seq.Join(player.transform.DORotateQuaternion(playerTarget.rotation, moveTime));
			seq.OnComplete(() => { activated = true; if (activateDot) DotCanvas.Instance.Show(); });

			DoChildActivation();
		}

		public void Deactivate()
		{
			if (!activated) return;

			// Deactivate the device interactor back
			deviceInteractor.SetEnable(true);

			// Kill any possible running tween
			player.transform.DOKill();

			activated = false;

			if (activateDot) DotCanvas.Instance.Hide();

			Sequence seq = DOTween.Sequence();
			seq.Append(player.transform.DOMove(lastPlayerPosition, moveTime));
			seq.Join(player.transform.DORotateQuaternion(lastPlayerRotation, moveTime));
			seq.OnComplete(() =>
			{
				player.InputDisabled = false;
				if (activateFlashlightOnExit)
				{
					activateFlashlightOnExit = false;
					flashlight.SetOn(true);
				}
			});

			DoChildDeactivation();

		}

		public virtual void DoChildActivation()
		{

		}
		
		public virtual void DoChildDeactivation()
		{

		}

		/// <summary>
		/// Call by the children
		/// </summary>
		protected void ReportBeaten()
		{
			beaten = true;
			Deactivate();

			beatenAudioSource.Play();

			OnMiniGameBeaten?.Invoke(this);
		}
		
		public float GetCooldownLeft()
        {
			return Mathf.Max(0, recheargeTime - recheargeElapsed);
        }
		
	}
}
