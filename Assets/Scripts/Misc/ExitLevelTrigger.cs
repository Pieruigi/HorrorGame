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

			//Time.timeScale = 0;
			//_PrototypeMessage.Instance.Show();

			//yield return new WaitForSeconds(.1f); // Just to stop the coroutine after the proto message has opened
			// GameManager.Instance.IncreaseGameStage();	
			// SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			GameManager.Instance.StartNextStage();
        }

    }
}
