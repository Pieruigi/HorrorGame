using System;
using Cinemachine;
using DG.Tweening;
using TMM;
using TMM.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		public static UnityAction OnOutOfBreath;

		public const float InteractionDistance = 1.5f;

		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		public float CrouchSpeed = 2.0f;


		float speedDebuff = 1f;

		float walkNoiseRange = 3f;

		float runNoiseRange = 6f;

		float noiseRange;
		public float NoiseRange
        {
			get{ return noiseRange; }
        }



		[SerializeField]
		float staminaMax = 1.0f;
		public float MaxStamina
        {
            get{ return staminaMax; }
        }
		float stamina;
		public float Stamina
        {
            get{ return stamina; }
        }

		float staminaRechargeSpeed = .5f;
		float staminaDepleteSpeed = .4f;

		float staminaRecheargeDelayMax = 1f;
		float staminaRecheargeDelay;

		

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;
		public bool CanJump = false;

		[Space(10)]
		public float CrouchHeight = 0.5f;
		[Tooltip("How much time it will take to crouch or stand back")]
		public float CrouchTime = .25f;
		public bool CanCrouch = false;
		

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		public Transform CameraRoot;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;

        private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		float _cameraRootHeightDefault;

		float _playerHeight;

		float mouseSensitivity = 1f;
		bool verticalMouse  = false;	


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		CinemachineVirtualCamera _virtualCamera;

		bool isDead = false;

		public bool IsRunning
		{
			get { return Grounded && !_input.crouch && _input.sprint; }
		}
		
		public bool IsCrouching
        {
            get{ return Grounded && _input.crouch; }
        }

		private bool IsCurrentDeviceMouse
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
			}
		}

		
		public bool InputDisabled { get; set; }
		
		
        public bool AimingDisabled { get; set; }
		
		

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			stamina = staminaMax;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			

            //InputDisabled = true;
        }

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			_cameraRootHeightDefault = CameraRoot.localPosition.y;
			_playerHeight = _controller.height;

			speedDebuff = PlayerSpeedDebuff.Instance.Value;

            UpdateMouseSensitivity();
            UpdateVerticalMouse();
        }

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.P))
				Time.timeScale = Time.timeScale == 1 ? 0 : 1;
#endif
			if (isDead) return;

            GroundedCheck();
            JumpAndGravity();
            CrouchCheck();
			CheckStamina();
			Move();
			ComputeNoiseRange();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		void OnEnable()
		{
			PlayerSpeedDebuff.OnApplied += HandleOnSpeedDebuffApplied;
			PlayerSpeedDebuff.OnExpired += HandleOnSpeedDebuffExpired;
			OptionsManager.OnOptionsChanged += HandleOnOptionsChnaged;
		}

        void OnDisable()
        {
			PlayerSpeedDebuff.OnApplied -= HandleOnSpeedDebuffApplied;
			PlayerSpeedDebuff.OnExpired -= HandleOnSpeedDebuffExpired;
            OptionsManager.OnOptionsChanged -= HandleOnOptionsChnaged;
        }

        private void HandleOnOptionsChnaged()
        {
			UpdateMouseSensitivity();
			UpdateVerticalMouse();
        }

        private void HandleOnSpeedDebuffApplied(TimedBuffDebuff arg)
		{
			if (arg.GetType() != typeof(PlayerSpeedDebuff)) return;
			speedDebuff = PlayerSpeedDebuff.Instance.Value;
        }

        private void HandleOnSpeedDebuffExpired(TimedBuffDebuff arg)
		{
			if (arg.GetType() != typeof(PlayerSpeedDebuff)) return;
			speedDebuff = 1;
        }

		void UpdateMouseSensitivity()
		{
            mouseSensitivity = .5f + OptionsManager.Instance.MouseSpeed / OptionsManager.MouseSpeedOptionMax;
        }

		void UpdateVerticalMouse()
		{
			verticalMouse = OptionsManager.Instance.VerticalMouse;
        }

        void ComputeNoiseRange()
        {
			if (_speed < 0.01f)
			{
				noiseRange = 0;
			}
            else
            {
				noiseRange = IsRunning ? runNoiseRange : walkNoiseRange;
            }
        }

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			//Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
			Grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.1f, GroundLayers, QueryTriggerInteraction.Ignore);
        }

		private void CameraRotation()
		{
			if(AimingDisabled || isDead)
            {
				_input.look = Vector2.zero;
            }
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				//float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_cinemachineTargetPitch += _input.look.y * RotationSpeed * mouseSensitivity * (verticalMouse ? -1f : 1f);//  * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * mouseSensitivity * (DeathTrapDebuff.Instance.Value ? -1f : 1f);// * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);

				//Debug.Log("Pitch:" + _cinemachineTargetPitch);
			}
		}

		void CrouchCheck()
		{
			var height = CameraRoot.localPosition.y;
			if (InputDisabled) _input.crouch = false;
			if (_input.crouch && CanCrouch)
			{
				
				if (height > CrouchHeight)
				{
					var heightDiff = Mathf.Abs(CrouchHeight - _cameraRootHeightDefault);
					var speed = heightDiff / CrouchTime;
					// Camera
					height = Mathf.MoveTowards(height, CrouchHeight, speed * Time.deltaTime);
					if (height < CrouchHeight) height = CrouchHeight;
					CameraRoot.localPosition = Vector3.up * height;

					// Character
					var pHeight = Mathf.MoveTowards(_controller.height, _controller.height - heightDiff, speed * Time.deltaTime);
					if (pHeight < _controller.height - heightDiff) pHeight = _controller.height - heightDiff;
					_controller.height = pHeight;

				}
			}
			else
			{
				if (height < _cameraRootHeightDefault)
				{
					var heightDiff = Mathf.Abs(CrouchHeight - _cameraRootHeightDefault);
					var speed = heightDiff / CrouchTime;
					// Camera
					height = Mathf.MoveTowards(height, _cameraRootHeightDefault, speed * Time.deltaTime);
					if (height > _cameraRootHeightDefault) height = _cameraRootHeightDefault;
					CameraRoot.localPosition = Vector3.up * height;

					// Character
					var pHeight = Mathf.MoveTowards(_controller.height, _playerHeight, speed * Time.deltaTime);
					if (pHeight > _playerHeight) pHeight = _playerHeight;
					_controller.height = pHeight;
				}
			}
		}

		
		private void CheckStamina()
        {
			if (_input.sprint)
			{
				// Check stamina
				if (stamina > 0 && !OutOfBreathDebuff.Instance.Value)
				{
					staminaRecheargeDelay = staminaRecheargeDelayMax;
					stamina -= staminaDepleteSpeed * Time.deltaTime;
					if (stamina < 0) stamina = 0;
				}
				else
				{
					if (OutOfBreathDebuff.Instance.Value)
						OnOutOfBreath?.Invoke();
					
					_input.sprint = false;
				}
			}
			
			if(stamina < staminaMax)
            {
				staminaRecheargeDelay -= Time.deltaTime;
				if(staminaRecheargeDelay < 0)
                {
					stamina += staminaRechargeSpeed * Time.deltaTime;
					if (stamina > staminaMax) stamina = staminaMax;	
                }
            }
        }

		private void Move()
		{


			if (InputDisabled || isDead)
			{
				_input.crouch = false;
				//_input.sprint = false;
				//_input.move = Vector3.zero;
			}
			

            if (!Grounded)
            {
                _input.crouch = false;
				_input.sprint = false;
				//_input.move = Vector3.zero; //TODO: disable movement when falling down
            }

			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			if (_input.crouch) targetSpeed = CrouchSpeed;

			targetSpeed *= speedDebuff;

			if (InputDisabled || isDead) targetSpeed = 0;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			// if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			// {
			// 	// creates curved result rather than a linear one giving a more organic speed change
			// 	// note T in Lerp is clamped, so we don't need to clamp our speed
			// 	_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);


			// 	// round speed to 3 decimal places
			// 	_speed = Mathf.Round(_speed * 1000f) / 1000f;
			// }
			// else
			// {
			// 	_speed = targetSpeed;
			// }

			_speed = Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * SpeedChangeRate);


			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * _speed * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			
			//if(_verticalVelocity != 0f)
			//	 transform.position += _verticalVelocity * Time.deltaTime * Vector3.up;

            // To fix a strange behaviour on edge collision
            // if (Grounded)
            // 	transform.position = new Vector3(transform.position.x, 0, transform.position.z);


        }

		private void JumpAndGravity()
		{

			if (Grounded)
			{
				if (_verticalVelocity < 0.0f)
					_verticalVelocity = 0.0f;

				if (_jumpTimeoutDelta > 0.0f)
				{
					_input.jump = false;
					_jumpTimeoutDelta -= Time.deltaTime;
				}



				if (_input.jump && CanJump && _jumpTimeoutDelta <= 0)
				{
					_input.jump = false;
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

			}
			else
			{
				_jumpTimeoutDelta = JumpTimeout;
				_verticalVelocity += Gravity * Time.deltaTime;
			}

			//return;
			
			//if (Grounded)
			//{
				
			//	// reset the fall timeout timer
			//	_fallTimeoutDelta = FallTimeout;

			//	// stop our velocity dropping infinitely when grounded
			//	if (_verticalVelocity < 0.0f)
			//	{
			//		_verticalVelocity = -2f;
			//	}

				
			//	// Jump
			//	if (_input.jump && _jumpTimeoutDelta <= 0.0f && CanJump)
			//	{
					
			//		// the square root of H * -2 * G = how much velocity needed to reach desired height
			//		_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					
			//	}

               

   //             // jump timeout
   //             if (_jumpTimeoutDelta >= 0.0f)
			//	{
			//		_jumpTimeoutDelta -= Time.deltaTime;
			//	}
			//}
			//else
			//{
				
			//	// reset the jump timeout timer
			//	_jumpTimeoutDelta = JumpTimeout;

			//	// fall timeout
			//	if (_fallTimeoutDelta >= 0.0f)
			//	{
			//		_fallTimeoutDelta -= Time.deltaTime;
			//	}

			//	// if we are not grounded, do not jump
			//	_input.jump = false;

   //             // if vertical speed is positive then decrease it
			//	//if(_verticalVelocity > 0f)
			//	{
			//		_verticalVelocity += Gravity * Time.deltaTime;
   //             }
				
   //         }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            //if (_verticalVelocity < _terminalVelocity)
            //{
            //	_verticalVelocity += Gravity * Time.deltaTime;
            //}
        }

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		public void Die()
		{
			isDead = true;
			_cinemachineTargetPitch = 0;
			CinemachineCameraTarget.transform.DOLocalRotateQuaternion(Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f), .2f) ;
		}

		public float GetSpeed()
		{
			return _speed;
		}

		public void SetTargetPitch(float value)
		{
			_cinemachineTargetPitch = value;
		}

		public float GetTargetPitch()
		{
			return _cinemachineTargetPitch;
		}

		

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}


	}
}