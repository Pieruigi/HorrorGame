using DG.Tweening;
using RetroShadersPro.URP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;

namespace TMM
{
	public class OutOfBreathDebuff : TimedBuffDebuff
	{
        public static OutOfBreathDebuff Instance { get; private set; }

        bool value = false;
        public bool Value
        {
            get { return value; }
        }

        Volume volume;
        Vignette vignette;

        Tween tween;

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
            value = true;

           
            //vignette.intensity.value = 0.75f;
        }

        protected override void DoExpire()
        {
            value = false;

            
            //vignette.intensity.value = 0f;
        }
    }
}
