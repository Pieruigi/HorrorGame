using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class WinnerLoserClown : MonoBehaviour
	{

        [SerializeField]
        AudioSource jumpscareAudioSource;

        [SerializeField]
        AudioSource partyAudioSource;

        Animator animator;

        ClownAttacker attacker;

        bool isAttacking = false;

        private void Awake()
        {
           attacker = GetComponent<ClownAttacker>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
            if (!isAttacking)
            {
                if(attacker != null && attacker.CanAttackPlayer())
                {
                    isAttacking = true;
                    animator.speed = 0;
                    attacker.Attack();
                    jumpscareAudioSource.Play();
                    partyAudioSource.Stop();
                }
            }
	    }

        public void SetType(int type)
        {
           // type = 17;
            animator = GetComponent<Animator>();
            animator.SetFloat("Type", type);

            if(type == 17)
            {
                transform.position += transform.right * .5f;
            }
        }

        //private void OnEnable()
        //{
        //    SceneManager.sceneLoaded += HandleOnSceneLoaded;
        //}

        //private void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        //}

        //private void HandleOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        //{
        //    if ("loserscene".Equals(arg0.name.ToLower()))
        //    {
        //        animator = GetComponent<Animator>();

        //        animator.SetFloat("Type", Random.Range(0, 18));
        //    }
            
        //}
    }
}
