using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
    
    public class DayNightManager : Singleton<DayNightManager>
    {
        public delegate void DayNightSwitchDelegate(bool isNight);
        public static DayNightSwitchDelegate OnDayNightSwitch;

        bool isNight = true;
        public bool IsNight
        {
            get { return isNight; }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Switch()
        {

            isNight = !isNight;

            OnDayNightSwitch?.Invoke(isNight);
        }



    }
    
}
