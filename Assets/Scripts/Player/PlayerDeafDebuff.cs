using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TMM
{
    public class PlayerDeafDebuff : TimedBuffDebuff
    {
        public static PlayerDeafDebuff Instance { get; private set; }

        [SerializeField]
        AudioMixer mixer;

        [SerializeField]
        AudioSource deafSource;

        [SerializeField]
        AudioSource jumpSource;

        protected override void Awake()
        {
            base.Awake();

            if (!Instance)
            {
                Instance = this;
                mixer.SetFloat("DeafVolume", -80f);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected override void DoApply()
        {
            mixer.SetFloat("FXVolume", -80f);
            mixer.SetFloat("DeafVolume", 0f);
            jumpSource.Play();
            deafSource.Play();
        }

        protected override void DoExpire()
        {
            mixer.SetFloat("FXVolume", 0f);
            mixer.SetFloat("DeafVolume", -80f);
            deafSource.Stop();
        }
    }
}
