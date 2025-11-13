using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMM
{
    public class ArmsController : Singleton<ArmsController>
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        float speed;

        [SerializeField]
        Animator animator;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void LateUpdate()
        {
            var rot = target.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed * Time.deltaTime);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, speed * Time.deltaTime);
        }

        public void PlayLeftHint()
        {
            // if (animator.GetCurrentAnimatorStateInfo(2).IsName("Hint") || (animator.IsInTransition(2) && animator.GetNextAnimatorStateInfo(2).IsName("Hint")))
            //     animator.ResetTrigger("Hint");
            // else
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Interaction");
            animator.SetTrigger("Hint"); 

        }

        public void PlayLeftIdle()
        {
            // if (animator.GetCurrentAnimatorStateInfo(2).IsName("Idle") || (animator.IsInTransition(2) && animator.GetNextAnimatorStateInfo(2).IsName("Idle")))
            // {
            //     animator.ResetTrigger("Idle");
            // }
            // else
            // {
            animator.ResetTrigger("Hint");
            animator.ResetTrigger("Interaction");
            animator.SetTrigger("Idle");    
            //}
            
        }

        public void PlayLeftInteraction()
        {
            // if (animator.GetCurrentAnimatorStateInfo(2).IsName("L_Interact") || (animator.IsInTransition(2) && animator.GetNextAnimatorStateInfo(2).IsName("L_Interact")))
            //     animator.ResetTrigger("Interaction");
            // else
            animator.ResetTrigger("Hint");
            animator.ResetTrigger("Idle");
                animator.SetTrigger("Interaction");
            //animator.SetBool("Hint", false);
        }
        

    }
    
}
