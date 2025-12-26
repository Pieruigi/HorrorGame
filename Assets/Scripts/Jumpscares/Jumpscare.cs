using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using StarterAssets;
using TMM.AI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMM
{
	public abstract class Jumpscare : MonoBehaviour
	{

		[SerializeField]
		bool stopMoving = false;

		[SerializeField]
		bool stopAiming = false;

		// Null if you don't want the player looking at any specific direction
		[SerializeField]
		Transform lookAt;

		[SerializeField]
		float lookTime = .2f;

		[SerializeField]
		float duration = 1;

		bool triggered = false;
		public bool Triggered
		{
			get{ return triggered; }
		}

		FirstPersonController fpc;
		Transform camRoot;
		CameraShake camShake;

		float oldJaw, oldPitch;

		public FirstPersonController FirstPersonController
		{
			get { return fpc; }
		}

		List<Creature> clowns;

		public abstract void ReportUsed(); // Called by the JumpscareManager

		//protected abstract bool CheckPlay();

		public abstract bool Validate();


	    // Start is called before the first frame update
	    void Start()
	    {
			fpc = FindFirstObjectByType<FirstPersonController>();
			camRoot = fpc.transform.Find("PlayerCameraRoot");
			camShake = FindFirstObjectByType<CameraShake>();
	    }

		// Update is called once per frame
		void Update()
		{
			if (triggered) return;

			// if (CheckPlay())
			// 	Play();

#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.X))
			{
				Play();
			}
#endif
		}

		protected virtual void OnEnable()
		{
			clowns = FindObjectsByType<Creature>(FindObjectsSortMode.None).ToList();
		}
		
		protected virtual void OnDisable()
		{
			
		}

		protected virtual void Play()
		{
			if (triggered) return;

			// Only if you are not chased or searched for
			if (clowns.Exists(c => c.State == CreatureState.Chase || c.State == CreatureState.Search)) return;

			triggered = true;

			camShake.PlayLetterboxJumpScare();

			if (stopMoving)
				fpc.InputDisabled = true;

			if (stopAiming)
				fpc.AimingDisabled = true;

			if (lookAt)
				LookAt();

			StartCoroutine(Stop());

#if UNITY_EDITOR
			//StartCoroutine(_Test());
#endif
		}

		IEnumerator Stop()
		{
			if (!triggered) yield break;

			yield return new WaitForSeconds(duration);

			if (stopMoving)
				fpc.InputDisabled = false;

			if (stopAiming)
				fpc.AimingDisabled = false;

			

			Destroy(gameObject, 5f);
		}

		void LookAt()
		{
			// Jaw
			Vector3 dir = lookAt.position - fpc.transform.position;
			dir.y = 0;
			float jawAngle = dir.sqrMagnitude < 0.0001f ? fpc.transform.localEulerAngles.y : Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

			// Pitch
			dir = lookAt.position - camRoot.position;
			dir = fpc.transform.InverseTransformDirection(dir);

			float pitchAngle = Mathf.Atan2(-dir.y, dir.z) * Mathf.Rad2Deg;
			fpc.SetTargetPitch(pitchAngle);

			fpc.transform.localEulerAngles = Vector3.up * jawAngle;
			camRoot.localEulerAngles = Vector3.right * pitchAngle;

		}

		
		public void ReportNotUsed()
		{
			Destroy(gameObject);
		}
	

	}
}
