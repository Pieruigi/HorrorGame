using DG.Tweening;
using StarterAssets;
using TMM.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;



namespace TMM
{
	public abstract class MiniGame : MonoBehaviour
	{
		public delegate void MiniGameBeatenDelegate(MiniGame miniGame);
		public static MiniGameBeatenDelegate OnMiniGameBeaten;

		public static UnityAction OnStartPlaying;
        public static UnityAction OnStopPlaying;

        [SerializeField]
		Transform playerTarget;

		[SerializeField]
		bool activateDot;

		[SerializeField]
		DeviceInteractor deviceInteractor;

		[SerializeField]
		DeviceInteractor recharger;

		//float attempts = 10;
		[SerializeField]
		float timer = 30;

		[SerializeField]
		AudioSource beatenAudioSource;

		[SerializeField]
		Light mainLight;

		public Light MainLight
		{
			get { return mainLight; }
		}

		[SerializeField]
		Canvas mainCanvas;

		GameObject wall;
		
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

		float wallHeightDefault;

		CanvasGroup ruleCanvasGroup;

		bool noExit = false;

		bool stopTimer = false;
	


		protected virtual void Awake()
        {
			
			mainCanvas.worldCamera = Camera.main;
			mainCanvas.planeDistance = .1f;
			
        }

	    // Start is called before the first frame update
	    protected virtual void Start()
	    {
            timer = 45 - (GameManager.Instance.GameStage - 1) * 5f;
            timeLeft = timer;
            player = FindFirstObjectByType<FirstPersonController>();
			//cameraRoot = player.GetComponent<CameraShake>().transform;
			flashlight = player.transform.parent.GetComponentInChildren<Flashlight>();
			wall = transform.Find("Wall").gameObject;
			wallHeightDefault = wall.transform.localPosition.y;
			ruleCanvasGroup = transform.Find("RuleCanvas").GetComponentInChildren<CanvasGroup>();
			ruleCanvasGroup.alpha = 0;
			// var rechargeInteractor = transform.Find("RechargeInteractor").gameObject;
			// recharger.SetInteractionCollider(rechargeInteractor.GetComponent<Collider>());
	    }

		// Update is called once per frame
		protected virtual void Update()
		{
#if UNITY_EDITOR
			
			// if (Input.GetKeyDown(KeyCode.X))
            // {
			// 	timeLeft = 0;
            // }

#endif
			if (activated && !noExit)
			{
#if UNITY_EDITOR				
				if (Input.GetKeyDown(KeyCode.E))
#else
				if (Input.GetKeyDown(KeyCode.E))
#endif
				{
					Deactivate();
					return;
				}

				if(!stopTimer)
					timeLeft -= Time.deltaTime;
				
				if (timeLeft < 0)
				{
					timeLeft = 0;
					recheargeElapsed = 0;
					Deactivate();
					return;
				}

                // if (Input.GetKeyDown(KeyCode.E))
                // {
				// 	if (Wallet.Instance.TryUseCoins(1))
				// 		Recharge();
                // }
			}
			else
			{
				if (timeLeft == 0)
				{
					recheargeElapsed += Time.deltaTime;
					if (recheargeElapsed > recheargeTime)
					{
						//timeLeft = timer;	
						Recharge();
					}
				}

			}

           
           
		}

        private void LateUpdate()
        {
			if (IsActive)
			{
                // Stop timer when player is looking away
				// Get looking direction 
				var lookDir = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
				// Get angle between looking direction and this object forward axis
				var angle = Vector3.Angle(lookDir, transform.forward);

				stopTimer = angle > 60f;

            }
        }

        protected virtual void OnEnable()
		{
			DeviceInteractor.OnInteraction += HandleOnDeviceInteraction;
			PlayerDeath.OnPlayerDead += HandleOnPlayerDead;
			DeviceInteractor.OnEnter += HandleOnDeviceEnter;
			DeviceInteractor.OnExit += HandleOnDeviceExit;
        }

        protected virtual void OnDisable()
        {
			DeviceInteractor.OnInteraction -= HandleOnDeviceInteraction;
			PlayerDeath.OnPlayerDead -= HandleOnPlayerDead;
            DeviceInteractor.OnEnter -= HandleOnDeviceEnter;
            DeviceInteractor.OnExit += HandleOnDeviceExit;
        }

		private void HandleOnDeviceEnter(DeviceInteractor deviceInteractor)
		{
			if (deviceInteractor == recharger)
			{
				noExit = true;
			}
		}

        private void HandleOnDeviceExit(DeviceInteractor deviceInteractor)
        {
            if(deviceInteractor == recharger)
			{
				noExit = false;
            }
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
			if (this.deviceInteractor == deviceInteractor && timeLeft > 0)
            {
				Activate();
				return;
            }
				
            if(recharger == deviceInteractor)
            {
				if (Wallet.Instance.TryUseCoins(1))
					Recharge();

				return;
            }
			
        }

        public void Activate()
		{
			if (activated || timeLeft <= 0 || beaten) return;

			if (flashlight.IsOn())
            {
				flashlight.SetOn(false);
				activateFlashlightOnExit = true;
            }
			
			stopTimer = false;

            // Deactivate the device interactor
            deviceInteractor.SetEnable(false);

			// Kill any possible running tween
			player.transform.DOKill();
			wall.transform.DOKill();
			ruleCanvasGroup.DOKill();

			// Stop player from moving
			player.InputDisabled = true;

			// Store last player position and rotation
			lastPlayerPosition = player.transform.position;
			lastPlayerRotation = player.transform.rotation;

			CharacterController cc = player.GetComponent<CharacterController>();
			cc.enabled = false;	

            // Move the controller to the target position
            Sequence seq = DOTween.Sequence();
			seq.Append(player.transform.DOMove(playerTarget.position, moveTime));
			seq.Join(player.transform.DORotateQuaternion(playerTarget.rotation, moveTime));
			seq.Join(wall.transform.DOLocalMoveY(wallHeightDefault * 5f, moveTime));
			seq.Join(ruleCanvasGroup.DOFade(1, moveTime));
			seq.OnComplete(() => { activated = true; if (activateDot) DotCanvas.Instance.Show(); player.transform.position = playerTarget.position; player.transform.rotation = playerTarget.rotation; cc.enabled = true; });

			DoChildActivation();

			OnStartPlaying?.Invoke();
        }

		public void Deactivate()
		{
			if (!activated) return;

			stopTimer = false;

            // Kill any possible running tween
            player.transform.DOKill();
			wall.transform.DOKill();
			ruleCanvasGroup.DOKill();

			activated = false;

			if (activateDot) DotCanvas.Instance.Hide();

            CharacterController cc = player.GetComponent<CharacterController>();
            cc.enabled = false;

            Sequence seq = DOTween.Sequence();
			seq.Append(player.transform.DOMove(lastPlayerPosition, moveTime));
			seq.Join(player.transform.DORotateQuaternion(lastPlayerRotation, moveTime));
			if(!beaten)
				seq.Join(wall.transform.DOLocalMoveY(wallHeightDefault, moveTime));
			seq.Join(ruleCanvasGroup.DOFade(0, moveTime));
			seq.OnComplete(() =>
			{
				player.InputDisabled = false;
				if (activateFlashlightOnExit)
				{
					activateFlashlightOnExit = false;
					flashlight.SetOn(true);
				}
                // Activate the device interactor back
                deviceInteractor.SetEnable(true);
				cc.enabled = true;
            });

			DoChildDeactivation();

			OnStopPlaying?.Invoke();

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

			// Deactivate triggers
			deviceInteractor.SetEnable(false);
			recharger.SetEnable(false);

			beatenAudioSource.Play();

			OnMiniGameBeaten?.Invoke(this);
		}

		public float GetCooldownLeft()
		{
			return Mathf.Max(0, recheargeTime - recheargeElapsed);
		}
		
		public void Recharge()
        {
			timeLeft += 15;// timer / 3f;
        }
	}
}
