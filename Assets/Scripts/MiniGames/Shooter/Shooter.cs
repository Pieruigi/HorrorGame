using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace TMM
{
    public class Shooter : MiniGame
    {

        [SerializeField]
        Gun gun;

        [SerializeField]
        List<GameObject> targetListA = new List<GameObject>();

        [SerializeField]
        List<GameObject> targetListB = new List<GameObject>();


        [SerializeField]
        CommonTrigger listA_Trigger;

        [SerializeField]
        CommonTrigger listB_Trigger;

        [SerializeField]
        TMP_Text counterUI;


        float targetDistance = .5f;

        float targetSpeed = .8f;

        int count = 50;

        int jumpscareScore = -1;

        protected override void Awake()
        {
            base.Awake();

            targetDistance = Mathf.Abs(targetListA[0].transform.localPosition.z - targetListA[1].transform.localPosition.z);
            //count = targetListA.Count;
            //count += targetListB.Count;

            // Adjust target height
            //AdjustTargetHeight();

            counterUI.text = count.ToString("00");

            
        }

        protected override void Start()
        {
            base.Start();
            ChooseRandomTargets();
        }

        protected override void Update()
        {
            base.Update();

            //if (!IsActive) return;

            MoveTargets();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CommonTrigger.OnEnter += HandleOnTriggerEnter;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CommonTrigger.OnEnter -= HandleOnTriggerEnter;
        }

        void ChooseRandomTargets()
        {
            int remCount = targetListA.Count - Mathf.RoundToInt((float)targetListA.Count * .30f);
            //List<GameObject> toRemove = new List<GameObject>();
            for (int i=0; i<remCount; i++)
            {
                GameObject toRemove = targetListA[Random.Range(0, targetListA.Count)];
                //targetListA.Remove(toRemove);
                //Destroy(toRemove);
                toRemove.GetComponent<Target>().Hide();

                toRemove = targetListB[Random.Range(0, targetListB.Count)];
                //targetListB.Remove(toRemove);
                //Destroy(toRemove);
                toRemove.GetComponent<Target>().Hide();
            }
        }

        void AdjustTargetHeight()
        {
            for (int i = 0; i < targetListA.Count; i++)
            {
                if (i % 2 == 0) continue;

                targetListA[i].transform.localPosition += Vector3.up * .075f;
                targetListB[i].transform.localPosition += Vector3.up * .075f;

            }
        }

        private void HandleOnTriggerEnter(CommonTrigger trigger, Collider collider)
        {

            if (trigger == listA_Trigger && targetListA.Exists(t => t == collider.gameObject))
            {
                // Get last target
                var last = targetListA.FindLast(_ => true);
                // Move this one behind the last
                collider.transform.position = last.transform.position - last.transform.forward * targetDistance;
                targetListA.Remove(collider.gameObject);
                targetListA.Add(collider.gameObject);
                collider.GetComponent<Target>().Reset();
            }


            if (trigger == listB_Trigger && targetListB.Exists(t => t == collider.gameObject))
            {
                // Get last target
                var last = targetListB.FindLast(_ => true);
                // Move this one behind the last
                collider.transform.position = last.transform.position + last.transform.forward * targetDistance;
                targetListB.Remove(collider.gameObject);
                targetListB.Add(collider.gameObject);
                collider.GetComponent<Target>().Reset();
            }


        }



        public override void DoChildActivation()
        {

            gun.Activate(true);
        }

        public override void DoChildDeactivation()
        {

            gun.Activate(false);
        }

        void MoveTargets()
        {
            foreach (var target in targetListA)
                target.transform.position += target.transform.forward * targetSpeed * Time.deltaTime;

            foreach (var target in targetListB)
                target.transform.position -= target.transform.forward * targetSpeed * Time.deltaTime;

        }

        public void ReportTargetHit(GameObject target)
        {
            count--;
            counterUI.text = count.ToString("00");
            if (count == 0)
                ReportBeaten();

            if (count == jumpscareScore)
                MiniJumpscare.Play();
        }

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {
            Debug.Log("TEST - Minijumpscare initialization");

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific move
            jumpscareScore = Random.Range(10, count-10);
        }

    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using DG.Tweening;
//using Unity.VisualScripting;
//using UnityEngine;

//namespace TMM
//{
//	public class Shooter : MiniGame
//	{

//		[SerializeField]
//		Gun gun;

//		[SerializeField]
//		List<GameObject> targetListA = new List<GameObject>();

//		[SerializeField]
//		List<GameObject> targetListB = new List<GameObject>();


//		[SerializeField]
//		CommonTrigger listA_Trigger;

//		[SerializeField]
//		CommonTrigger listB_Trigger;


//		float targetDistance = .5f;

//		float targetSpeed = .8f;

//		int count = 0;

//		protected override void Awake()
//		{
//			base.Awake();

//			targetDistance = Mathf.Abs(targetListA[0].transform.localPosition.z - targetListA[1].transform.localPosition.z);
//			count = targetListA.Count;
//			count += targetListB.Count;

//			// Adjust target height
//			AdjustTargetHeight();
//		}

//		protected override void Update()
//		{
//			base.Update();

//			//if (!IsActive) return;

//			MoveTargets();
//		}

//		protected override void OnEnable()
//		{
//			base.OnEnable();
//			CommonTrigger.OnEnter += HandleOnTriggerEnter;
//		}

//		protected override void OnDisable()
//		{
//			base.OnDisable();
//			CommonTrigger.OnEnter -= HandleOnTriggerEnter;
//		}

//		void AdjustTargetHeight()
//        {
//            for(int i=0; i<targetListA.Count; i++)
//            {
//				if (i % 2 == 0) continue;

//				targetListA[i].transform.localPosition += Vector3.up * .075f;
//				targetListB[i].transform.localPosition += Vector3.up * .075f;

//            }
//        }

//		private void HandleOnTriggerEnter(CommonTrigger trigger, Collider collider)
//		{

//			if (trigger == listA_Trigger && targetListA.Exists(t => t == collider.gameObject))
//			{
//				// Get last target
//				var last = targetListA.FindLast(_ => true);
//				// Move this one behind the last
//				collider.transform.position = last.transform.position - last.transform.forward * targetDistance;
//				targetListA.Remove(collider.gameObject);
//				targetListA.Add(collider.gameObject);

//			}


//			if (trigger == listB_Trigger && targetListB.Exists(t => t == collider.gameObject))
//			{
//				// Get last target
//				var last = targetListB.FindLast(_ => true);
//				// Move this one behind the last
//				collider.transform.position = last.transform.position + last.transform.forward * targetDistance;
//				targetListB.Remove(collider.gameObject);
//				targetListB.Add(collider.gameObject);

//			}


//		}



//		public override void DoChildActivation()
//		{

//			gun.Activate(true);
//		}

//		public override void DoChildDeactivation()
//		{

//			gun.Activate(false);
//		}

//		void MoveTargets()
//		{
//			foreach (var target in targetListA)
//				target.transform.position += target.transform.forward * targetSpeed * Time.deltaTime;

//			foreach (var target in targetListB)
//				target.transform.position -= target.transform.forward * targetSpeed * Time.deltaTime;

//		}

//		public void ReportTargetHit(GameObject target)
//        {
//			count--;
//			if (count == 0)
//				ReportBeaten();
//        }

//	}
//}
