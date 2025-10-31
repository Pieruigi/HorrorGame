using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
    public class CarAlarm : MonoBehaviour
    {
        [SerializeField]
        List<Light> lights;

        [SerializeField]
        float lightIntensity;

        [SerializeField]
        List<Renderer> emissiveRenderers;

        //[SerializeField]
        List<Material> emissiveMaterials;

        [SerializeField]
        float emissiveIntensity;

        //[SerializeField]
        Color emissiveColor;



        //[SerializeField]
        float lightSpeed = 2f;

        [SerializeField]
        AudioSource _audio;

        float time = 2.5f;
        float elapsed = 0;

        float lightDelay = 0;

        //Vector4 emissiveColorDefault = 

        
        

        void Awake()
        {
            foreach (var light in lights)
            {
                light.intensity = 0;
                light.enabled = false;
            }
            emissiveMaterials = new List<Material>();
            foreach(var rend in emissiveRenderers)
            {
                rend.enabled = false;
                emissiveMaterials.Add(rend.material);
                emissiveColor = rend.material.GetColor("_BaseColor");
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            // if (Input.GetKeyDown(KeyCode.Z))
            //     StartAlarm();
#endif

        }

        public void StartAlarm()
        {
            elapsed = 0;

            

            // Reset light intensity
            foreach (var light in lights)
            {
                light.intensity = 0;
                light.enabled = true;
            }

            foreach (var rend in emissiveRenderers)
                rend.enabled = true;

            StartCoroutine(PlayAlarm());

            foreach(Material mat in emissiveMaterials)
                mat.SetColor("_BaseColor", emissiveColor);
            

        }

        IEnumerator PlayAlarm()
        {
            _audio.pitch = 1;
            _audio.Play();

            yield return new WaitForSeconds(lightDelay);

            //elapsed += Time.deltaTime;
            int dir = 1;
            float dirT = 0;
            var t = 1f / lightSpeed;
            while (elapsed < time)
            {
                float l1 = 0;
                float l2 = lightIntensity;
                foreach (var light in lights)
                {

                    light.intensity = Mathf.Lerp(dir > 0 ? l1 : l2, dir > 0 ? l2 : l1, dirT / t);

                }

                Vector4 e1 = emissiveColor;
                Vector4 e2 = emissiveColor * emissiveIntensity;
                foreach (var emissive in emissiveMaterials)
                {
                    var c = Vector4.Lerp(dir > 0 ? e1 : e2, dir > 0 ? e2 : e1, dirT / t);
                    emissive.SetVector("_BaseColor", c);
                }

                yield return null;
                dirT += Time.deltaTime;
                if (dirT > t)
                {
                    dirT -= t;
                    dir *= -1;
                }

                elapsed += Time.deltaTime;
            }

            // Shut down alarm
            float shutTime = 2.5f;
            float minPitch = 0;

            elapsed = 0;
            while (elapsed < shutTime)
            {
                float pitch = Mathf.Lerp(1, minPitch, elapsed / shutTime);
                _audio.pitch = pitch;

                float l1 = 0;
                float l2 = lightIntensity;
                foreach (var light in lights)
                {

                    light.intensity = Mathf.Lerp(l2, l1, elapsed / shutTime);

                }

                Vector4 e1 = emissiveColor;
                Vector4 e2 = emissiveColor * emissiveIntensity;
                foreach (var emissive in emissiveMaterials)
                {
                    var c = Vector4.Lerp(e2, e1, elapsed / shutTime);
                    emissive.SetVector("_BaseColor", c);
                }

                yield return null;
                elapsed += Time.deltaTime;

            }

            foreach (var light in lights)
                light.enabled = false;
            foreach (var rend in emissiveRenderers)
                rend.enabled = false;


        }
        
    }
}