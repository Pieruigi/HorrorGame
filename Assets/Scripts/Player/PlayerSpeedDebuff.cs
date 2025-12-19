using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class PlayerSpeedDebuff : TimedBuffDebuff
	{

		public static PlayerSpeedDebuff Instance { get; private set; }

		float value = 1;
		public float Value
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

        // Start is called before the first frame update
        void Start()
		{

		}

		protected override void DoApply()
		{
			value = .75f;

		}

        protected override void DoExpire()
        {
			value = 1f;
        }
		
    }
}
