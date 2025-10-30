using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
    public class DayNightMaterial : MonoBehaviour
    {
        [SerializeField]
        Material dayMaterial;

        [SerializeField]
        Material nightMaterial;

        //Renderer _renderer;

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
            var rend = GetComponent<Renderer>();
            rend.material = isNight ? nightMaterial : dayMaterial;
        }
    }
    
}
