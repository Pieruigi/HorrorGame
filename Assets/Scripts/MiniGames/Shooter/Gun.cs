using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

namespace TMM
{
	public class Gun : MonoBehaviour
	{
		[SerializeField]
		GameObject bulletPrefab;

		[SerializeField]
		Transform bulletTarget;

		[SerializeField]
		Renderer lightRenderer;

		[SerializeField]
		Material greenMaterial;

		[SerializeField]
		Material redMaterial;

		[SerializeField]
		float rate = 1;

		[SerializeField]
		AudioSource shotAudioSource;

		[SerializeField]
		List<AudioClip> shotClips;

		[SerializeField]
		AudioSource readyAudioSource;

		float cooldown = 0;

		bool active = false;

		Transform parentDefault;

		GameObject gunRoot;

		CameraShake shake;

        void Awake()
        {
			parentDefault = transform.parent;
			gunRoot = transform.GetChild(0).gameObject;
			gunRoot.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
	    {
			shake = FindFirstObjectByType<FirstPersonController>().GetComponentInChildren<CameraShake>();
	    }

		// Update is called once per frame
		void Update()
		{
			if (!active) return;

			if (cooldown > 0)
			{
				cooldown -= Time.deltaTime;
				if (cooldown <= 0)
                {
					lightRenderer.material = greenMaterial;
					readyAudioSource.Play();
                }
					
			}




			if (Input.GetMouseButton(0))
			{
				if (cooldown <= 0)
				{
					cooldown = 1 / rate;
					// Shoot
					var bullet = Instantiate(bulletPrefab);
					bullet.GetComponent<Rigidbody>().position = bulletTarget.position;
					bullet.transform.rotation = bulletTarget.rotation;
					bullet.GetComponent<Rigidbody>().AddForce(bulletTarget.forward * 20, ForceMode.VelocityChange);

					// Set red light
					lightRenderer.material = redMaterial;
					//Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>(), true);

					// Play light shake
					StartCoroutine(ShakeCamera());

					PlayShotAudio();
				}
			}
		}

		IEnumerator ShakeCamera()
		{
			yield return new WaitForSeconds(4f * Time.fixedDeltaTime);
			shake.PlayLightShootShake();
		}
		
		void PlayShotAudio()
        {
			shotAudioSource.clip = shotClips[Random.Range(0, shotClips.Count)];
			shotAudioSource.Play();
        }
		
		public void Activate(bool value)
        {
			active = value;
			cooldown = 999; //0.25f;
			lightRenderer.material = redMaterial;

			float moveTime = .25f;

			if (!value)
			{
				transform.DOLocalMoveZ(0, moveTime).OnComplete(() =>
                {
                	transform.parent = parentDefault;
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
					gunRoot.SetActive(false);
                });

				
			}
            else
			{
				transform.parent = Camera.main.transform;
				transform.localPosition = Vector3.down * .136f;
				transform.localRotation = Quaternion.identity;
				gunRoot.SetActive(true);

				transform.DOLocalMoveZ(.321f, moveTime).OnComplete(()=> { cooldown = 1f / rate; });
				//transform.localPosition = Vector3.down * .136f + Vector3.forward * .321f;
				
            }
        }
	}
}
