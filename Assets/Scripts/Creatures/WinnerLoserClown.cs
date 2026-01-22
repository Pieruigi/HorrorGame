using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class WinnerLoserClown : MonoBehaviour
	{
		
		Animator animator;

		
        private void Awake()
        {
           
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if ("loserscene".Equals(arg0.name.ToLower()))
            {
                animator = GetComponent<Animator>();

                animator.SetFloat("Type", Random.Range(0, 19));
            }
            
        }
    }
}
