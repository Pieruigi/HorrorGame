using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using TMM.UI;
using UnityEngine;

namespace TMM
{
	public class ClownAttacker : MonoBehaviour
	{

		[SerializeField]
		float attackRange;

		[SerializeField]
		[Range(0, 180)]
		float attackAngle;

		[SerializeField]
		Transform playerDeadTarget;


		FirstPersonController player;

	    // Start is called before the first frame update
	    void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public bool CanAttackPlayer()
		{
			// Check distance
			if (Vector3.Distance(player.transform.position, transform.position) > attackRange)
				return false;

			// Compute direction
			var pDir = player.transform.position - transform.position;
			if (Vector3.Angle(transform.forward, pDir) > attackAngle)
				return false;



			return true;
		}

		public void Attack()
		{
			player.GetComponent<PlayerDeath>().Die(gameObject);

			player.transform.parent = playerDeadTarget;

			StartCoroutine(PlayJumpScare());

			var seq = DOTween.Sequence();
			seq.Append(player.transform.DOLocalMove(Vector3.zero, .5f));
			seq.Join(player.transform.DOLocalRotateQuaternion(Quaternion.identity, .5f));
			seq.AppendCallback(() => { CameraFade.Instance.FadeOut(); });
			seq.AppendInterval(2f);

            seq.OnComplete(() =>
			{
				
				// Fade and restart	
				//GameManager.Instance.RestartGame();
				GameManager.Instance.YouLose();
            });

		}
		
		IEnumerator PlayJumpScare()
		{
			yield return new WaitForSeconds(.25f);
			player.transform.root.GetComponentInChildren<CameraShake>().PlayLetterboxJumpScare();
			GetComponent<CreatureAudio>()?.PlayPlayerDeath();
		}

	}
}
