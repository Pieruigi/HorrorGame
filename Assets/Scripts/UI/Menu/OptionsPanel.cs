using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
    public class OptionsPanel : MonoBehaviour
    {


        [SerializeField]
        Slider mouseSpeedSlider;

        [SerializeField]
        Slider volumeSlider;

        [SerializeField]
        Toggle verticalMouseToggle;

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            mouseSpeedSlider.onValueChanged.AddListener(HandleOnMouseSpeedChanged);

            // Init
            var v = PlayerPrefs.GetInt(OptionsManager.MouseSpeedOptionParam, OptionsManager.MouseSpeedOptionDefault);
            mouseSpeedSlider.value = v;

            volumeSlider.onValueChanged.AddListener(HandleOnVolumeChanged);
            v = PlayerPrefs.GetInt(OptionsManager.VolumeOptionParam, OptionsManager.VolumeOptionDefault);
            volumeSlider.value = v;

            verticalMouseToggle.onValueChanged.AddListener(HandleOnVerticalMouseChanged);
            v = PlayerPrefs.GetInt(OptionsManager.VerticalMouseOptionParam, OptionsManager.VerticalMouseOptionDefault);
            verticalMouseToggle.isOn = v != 0;
        }

        

        void OnDisable()
        {
            mouseSpeedSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.RemoveAllListeners();
            verticalMouseToggle.onValueChanged.RemoveAllListeners();
        }

        private void HandleOnVerticalMouseChanged(bool value)
        {
            if (!OptionsManager.Instance) return;
            PlayerPrefs.SetInt(OptionsManager.VerticalMouseOptionParam, value ? 1 : 0);
            OptionsManager.Instance.SaveOptions();
        }

        private void HandleOnMouseSpeedChanged(float value)
        {
            if(!OptionsManager.Instance) return;
            PlayerPrefs.SetInt(OptionsManager.MouseSpeedOptionParam, (int)value);
            OptionsManager.Instance.SaveOptions();
        }

        private void HandleOnVolumeChanged(float value)
        {
            if (!OptionsManager.Instance) return;
            PlayerPrefs.SetInt(OptionsManager.VolumeOptionParam, (int)value);
            OptionsManager.Instance.SaveOptions();
        }
    }
}
