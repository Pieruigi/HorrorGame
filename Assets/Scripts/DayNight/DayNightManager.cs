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

        


        bool isNight = false;
        public bool IsNight
        {
            get { return isNight; }
        }

        // Start is called before the first frame update
        void Start()
        {
             OnDayNightSwitch?.Invoke(isNight);
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            // if(Input.GetKeyDown(KeyCode.Z))
            // {
            //     Switch();
            // }
#endif
        }

        public void Switch()
        {

            isNight = !isNight;

            OnDayNightSwitch?.Invoke(isNight);
        }



    }
    
}
