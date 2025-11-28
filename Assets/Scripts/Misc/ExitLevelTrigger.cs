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
				LoadNextLevelDelayed();
			}
		}
		
		IEnumerator LoadNextLevelDelayed()
        {
			yield return new WaitForSeconds(2f);

			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}
