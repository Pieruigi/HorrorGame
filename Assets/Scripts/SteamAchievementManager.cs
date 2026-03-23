#if !UNITY_WEBGL
using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace TMM
{
    //public enum SteamAchievementId { STAGE_1_COMPLETED, STAGE_2_COMPLETED, STAGE_3_COMPLETED, STAGE_4_COMPLETED, STAGE_5_COMPLETED }

    public class SteamAchievementManager : SingletonPersistent<SteamAchievementManager>
    {

        // Start is called before the first frame update
        void Start()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("Steam not initialized");
            }
            else
            {
                Debug.Log("Steam account:" + SteamFriends.GetPersonaName());
            }

            DebugAllAchievements();

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            //if (Input.GetKeyDown(KeyCode.Y))
            //{
            //    if (!IsAchievementUnlocked("STAGE_1_COMPLETED"))
            //        UnlockAchievement("STAGE_1_COMPLETED");

            //}

            if (Input.GetKeyDown(KeyCode.U))
            {
                HardResetAchievements();
            }

          
#endif

        }



        public void DebugAllAchievements()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam non inizializzato");
                return;
            }

            uint numAchievements = SteamUserStats.GetNumAchievements();
            //Debug.Log($"Numero achievement trovati: {numAchievements}");

            for (uint i = 0; i < numAchievements; i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                bool achieved = SteamUserStats.GetAchievement(achievementId, out achieved);

                //Debug.Log($"Achievement [{i}]: {achievementId} - Sbloccato: {achieved}");
            }
        }

       


        public void UnlockAchievement(string achievementId)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("Steam non inizializzato - Achievement non sbloccato: " + achievementId);
                return;
            }
            bool success = SteamUserStats.SetAchievement(achievementId);

            if (success)
            {
                SteamUserStats.StoreStats();
                //Debug.Log($"Achievement sbloccato: {achievementId}");
            }
            else
            {
                Debug.LogError($"Errore nello sbloccare achievement: {achievementId}");
            }

            DebugAllAchievements();
        }

        // Verifica se un achievement è già sbloccato
        bool IsAchievementUnlocked(string achievementId)
        {
            if (!SteamManager.Initialized) return false;

            if (SteamUserStats.GetAchievement(achievementId, out bool achieved))
            {
                return achieved;
            }

            Debug.LogError($"Achievement non trovato: {achievementId}");
            return false;
        }

        // Reset achievement (per testing)
        void ResetAchievement(string achievementId)
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.ClearAchievement(achievementId);
            SteamUserStats.StoreStats();
            //Debug.Log($"Achievement resettato: {achievementId}");
        }
        
        void HardResetAchievements()
        {
            if (!SteamManager.Initialized) return;
            
            // Prima resetta normalmente
            for (uint i = 0; i < SteamUserStats.GetNumAchievements(); i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                SteamUserStats.ClearAchievement(achievementId);
            }
            SteamUserStats.StoreStats();
            
            // Poi forza una ricaricata
            SteamUserStats.ResetAllStats(true); // <-- il 'true' è importante!
            SteamAPI.RunCallbacks();
            
            Debug.Log("Hard reset completato");
        }
        
    }
}
#endif