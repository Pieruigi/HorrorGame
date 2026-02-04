using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class ExitLevelTrigger : MonoBehaviour
	{
		[SerializeField]
		GameObject tile;

		[SerializeField]
		AudioSource screamAudioSource;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			//if (Input.GetKeyDown(KeyCode.X))
			//{
   //             screamAudioSource.PlayDelayed(.7f);
   //             StartCoroutine(LoadNextLevelDelayed());
   //         }
            
#endif
        }

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				tile.SetActive(false);
				screamAudioSource.PlayDelayed(.7f);
                StartCoroutine(LoadNextLevelDelayed());
			}
		}
		
		IEnumerator LoadNextLevelDelayed()
        {

			yield return new WaitForSeconds(2f);

			if("gamescene" == SceneManager.GetActiveScene().name.ToLower())
			{
#if UNITY_EDITOR
				//GameManager.Instance.YouWin();
				//yield break;
#endif

				if (GameManager.Instance.GameStage < 5)
					GameManager.Instance.StartNextStage();
				else
					GameManager.Instance.YouWin();
            }
			else
			{
				if("loserscene" == SceneManager.GetActiveScene().name.ToLower())
				{
					GameManager.Instance.StartNewGame();
                }
			}
			
        }

    }
}
