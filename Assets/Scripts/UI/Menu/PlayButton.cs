using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TMM.UI
{
	public class PlayButton : MonoBehaviour
	{
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if ("gamescene".Equals(SceneManager.GetActiveScene().name.ToLower()))
                {
                    GetComponentInParent<MenuUI>().HideAll();
                    GameManager.Instance.UnpauseGame();
                }
                else
                {
                    GameManager.Instance.StartNewGame();
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
