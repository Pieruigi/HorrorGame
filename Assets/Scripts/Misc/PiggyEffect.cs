using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TMM
{
	public class PiggyEffect : MonoBehaviour
	{
		[SerializeField]
		GameObject coin;

		FirstPersonController player;

		FloorTrigger floorTrigger;

		bool triggered = false;

		Rigidbody coinRB;

		Transform coinRoot;

		Sequence seq;

        void Awake()
        {
			floorTrigger = transform.GetComponentInParent<FloorTrigger>();
			coinRB = coin.GetComponent<Rigidbody>();
			coinRoot = coin.transform.parent;
        }

        // Start is called before the first frame update
        void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();
			ResetCoin();
			StartNotTriggeredTween();
	    }

		// Update is called once per frame
		void Update()
		{
			if (floorTrigger.Triggered)
			{
				if (!triggered)
				{
					triggered = true;
					// Play triggered fx
					transform.DOKill();
					ResetCoin();
					StartTriggeredTween();
				}
			}
			else
			{
				if (triggered)
				{
					triggered = false;
					// Play not triggered fx
					transform.DOKill();
					ResetCoin();
					StartNotTriggeredTween();
				}
			}

		}

		void LateUpdate()
		{
			
			// Look at the player
			var dir = Vector3.ProjectOnPlane(player.transform.position - transform.parent.position, Vector3.up);
			dir.Normalize();

			transform.parent.forward = Vector3.MoveTowards(transform.parent.forward, dir, 5f * Time.deltaTime);
		}

		void StartTriggeredTween()
		{
			seq?.Kill();
			//transform.parent.localEulerAngles = new Vector3(0, eulers.y, 0);
			float time = .5f;
			seq = DOTween.Sequence();
			seq.Append(transform.DOLocalRotate(new Vector3(90, 180, 0), time).SetEase(Ease.OutBounce));
			seq.Join(transform.DOLocalMoveY(0.45f, time).SetEase(Ease.OutBounce));

		}

		void StartNotTriggeredTween()
		{
			seq?.Kill();
			float time = .5f;
			seq = DOTween.Sequence();
			seq.Append(transform.DOLocalRotate(new Vector3(-90, 0, 0), time).SetEase(Ease.OutBounce));
			seq.Join(transform.DOLocalMoveY(0f, time).SetEase(Ease.OutBounce));
			seq.OnComplete(() =>
			{
				seq = DOTween.Sequence();
				seq.AppendInterval(2f);
				seq.AppendCallback(() => { ResetCoin(); });
				seq.Append(transform.DOShakeRotation(1f).OnComplete(()=> { ThrowCoin(); }));
				seq.SetLoops(-1);
			});

			
		}

		void ResetCoin()
		{
			coinRB.isKinematic = true;
			coinRB.interpolation = RigidbodyInterpolation.None;
			coin.transform.localPosition = Vector3.zero;
			coin.transform.localRotation = Quaternion.identity;
		}
		
		void ThrowCoin()
		{
			coinRB.isKinematic = false;
			coinRB.interpolation = RigidbodyInterpolation.Interpolate;
			var dirErr = .5f;
			var dir = Vector3.up + Random.Range(-dirErr, dirErr) * Vector3.right + Random.Range(-dirErr, dirErr) * Vector3.forward;
			coinRB.AddForce(dir * 5, ForceMode.VelocityChange);
			coinRB.AddTorque(Vector3.up * Random.Range(-360f, 360f)+Vector3.right * Random.Range(-360f, 360f)+Vector3.forward * Random.Range(-360f, 360f));
		}
		
    }
}
