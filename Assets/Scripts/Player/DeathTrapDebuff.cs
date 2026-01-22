using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
    public class DeathTrapDebuff : TimedBuffDebuff
    {
        public static DeathTrapDebuff Instance { get; private set; }

        bool value = false;
        public bool Value
        {
            get { return value; }
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

        protected override void DoApply()
        {
            value = true;
        }

        protected override void DoExpire()
        {
            value = false;
        }
    }
}
