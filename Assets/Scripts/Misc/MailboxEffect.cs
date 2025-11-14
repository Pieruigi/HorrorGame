using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class MailboxEffect : MonoBehaviour
	{

		[SerializeField]
		GameObject door;

		[SerializeField]
		GameObject mailPrefab;

		[SerializeField]
		GameObject mailSpawnPoint;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void PlayEffect(int count)
		{
			

			// Open door
			door.transform.DOLocalRotate(Vector3.forward * 24f, .2f).SetEase(Ease.OutBounce).OnComplete(()=> { StartCoroutine(DoCollectEnvelopesEffect(count)); });

			
		}

		IEnumerator DoCollectEnvelopesEffect(int count)
		{
			FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
			fpc.InputDisabled = true;
			// Create envelopes
			List<GameObject> envelopes = new List<GameObject>();
			for (int i = 0; i < count; i++)
			{
				var env = Instantiate(mailPrefab);
				env.transform.position = mailSpawnPoint.transform.position;
				env.transform.rotation = mailSpawnPoint.transform.rotation;
				envelopes.Add(env);
			}

			yield return new WaitForSeconds(.5f);

			// Collect envelopes
			foreach (var env in envelopes)
			{
				env.transform.DOMove(Camera.main.transform.position - Vector3.up * .5f, .2f).OnComplete(() => { Destroy(env); });
				//tweener.OnUpdate(()=> { tweener.ChangeEndValue(GetTargetPosition(), .2f); });
				yield return new WaitForSeconds(.5f);
			}

			fpc.InputDisabled = false;
		}
		
	
		public void Reset()
        {
			door.transform.localRotation = Quaternion.identity;
        }
	}
}
