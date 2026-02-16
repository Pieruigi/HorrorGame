using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
    public class StupidClownBuff : TimedBuffDebuff
    {
        public static UnityAction OnForcedExpired;

        public static StupidClownBuff Instance { get; private set; }

        [SerializeField]
        float speedMultiplier = .5f;
        public float SpeedMultiplier { get { return speedMultiplier; } }

        [SerializeField]
        float attackRangeMultiplier = .2f;
        public float AttackRangeMultiplier { get {return attackRangeMultiplier; } }

        [SerializeField]
        float scaleMultiplier = .5f;
        public float ScaleMultiplier { get { return scaleMultiplier; } }

        [SerializeField]
        float laughPitchMultiplier;
        public float LaughPitchMultiplier {  get { return laughPitchMultiplier; } }

        public bool IsActive
        {
            get { return Timer > 0; }
        }

        protected override void Awake()
        {
            base.Awake();

            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.X))
            //    Apply();
#endif
        }


        protected override void DoApply()
        {
                        
        }

        protected override void DoExpire()
        {
            
        }

        public void ForceExpire()
        {
            //ResetAll();
            OnForcedExpired?.Invoke();
        }
    }
}
