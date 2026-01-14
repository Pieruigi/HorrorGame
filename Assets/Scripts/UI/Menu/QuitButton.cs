using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMM.UI
{
	public class QuitButton : MonoBehaviour
	{
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
			{
				if ("gamescene".Equals(SceneManager.GetActiveScene().name.ToLower()))
				{
					//GameManager.Instance.UnpauseGame();
					GameManager.Instance.LoadMainMenu();
                }
				else
				{
                    Application.Quit();
                }
				
			});
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		
	}
}
