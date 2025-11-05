using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;

namespace TMM
{
    public class ProceduralSkybox : MonoBehaviour
    {
        [SerializeField]
        Material material;

        [SerializeField]
        Material daySkybox;

        [SerializeField]
        Material nightSkybox;

        float speed = 1f;
        float elapsed = 0;

        
        // Start is called before the first frame update
        void Start()
        {
            HandleOnDayNightSwitch(DayNightManager.Instance.IsNight);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            DayNightManager.OnDayNightSwitch += HandleOnDayNightSwitch;
        }

        void OnDisable()
        {
            DayNightManager.OnDayNightSwitch -= HandleOnDayNightSwitch;
        }

        private void HandleOnDayNightSwitch(bool isNight)
        {
            // Get current colors
            Vector4 c0_Start = !isNight ? nightSkybox.GetVector("_Ground_Color") : daySkybox.GetVector("_Ground_Color");
            Vector4 c1_Start = !isNight ? nightSkybox.GetVector("_Sky_Color") : daySkybox.GetVector("_Sky_Color");
            Vector4 c2_Start = !isNight ? nightSkybox.GetVector("_Cloud_Color") : daySkybox.GetVector("_Cloud_Color");

            // Get target colors
            Vector4 c0_End = isNight ? nightSkybox.GetVector("_Ground_Color") : daySkybox.GetVector("_Ground_Color");
            Vector4 c1_End = isNight ? nightSkybox.GetVector("_Sky_Color") : daySkybox.GetVector("_Sky_Color");
            Vector4 c2_End = isNight ? nightSkybox.GetVector("_Cloud_Color") : daySkybox.GetVector("_Cloud_Color");

            // Set rendering setting for the lighting gradient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

            float t = 0;
            float duration = 2;
            DOTween.To(() => t,
                        x =>
                        {
                            t = x;
                            var c0 = Vector4.Lerp(c0_Start, c0_End, t);
                            var c1 = Vector4.Lerp(c1_Start, c1_End, t);
                            var c2 = Vector4.Lerp(c2_Start, c2_End, t);
                            material.SetVector("_Ground_Color", c0);
                            material.SetVector("_Sky_Color", c1);
                            material.SetVector("_Cloud_Color", c2);

                            RenderSettings.ambientGroundColor = c0;
                            RenderSettings.ambientSkyColor = c1;
                            RenderSettings.ambientEquatorColor = isNight ? Vector4.Lerp(c0, c1, 0.3f) : Vector4.Lerp(c0, c1, 0.7f);
                            
                        },
                        1f,
                        duration);


        }

        
    }
}