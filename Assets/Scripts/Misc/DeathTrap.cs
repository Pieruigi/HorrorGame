using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class DeathTrap : MonoBehaviour
	{
        [SerializeField]
        FloorTrigger floorTrigger;

        [SerializeField]
        float duration;

        float elapsed = 0;

        bool triggered = false;


        // Start is called before the first frame update
        void Start()
        {
            if (TriggerTileManager.Instance.TriggerTilesDisabled)
                floorTrigger.SwitchOff();
        }

        // Update is called once per frame
        void Update()
        {
            if (!triggered) return;

            elapsed += Time.deltaTime;
            if (elapsed >= duration)
            {
                triggered = false;
                floorTrigger.ResetTrigger();
            }
        }

        void OnEnable()
        {
            floorTrigger.OnTriggered += HandleOnTriggered;
            TriggerTileManager.OnChanged += HandleOnTriggerTileManagerChanged;
        }

        void OnDisable()
        {
            floorTrigger.OnTriggered -= HandleOnTriggered;
            TriggerTileManager.OnChanged -= HandleOnTriggerTileManagerChanged;
        }

        private void HandleOnTriggerTileManagerChanged()
        {
            if (TriggerTileManager.Instance.TriggerTilesDisabled)
                floorTrigger.SwitchOff();
            else if (!AlarmManager.Instance.IsActive())
                floorTrigger.ResetTrigger();
        }

        private void HandleOnTriggered()
        {
            elapsed = 0;
            triggered = true;
            DeathTrapDebuff.Instance.Apply();




        }
    }
}
