using StarterAssets;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMM.AI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace TMM
{
    public class SteamStatsManager : SingletonPersistent<SteamStatsManager>
    {
        float chaseTime = 0;
        bool chasing = false;
        bool pigChasing = false;
        float pigChaseTime = 0;

        //FirstPersonController player;
        
        protected override void Awake()
        {
            base.Awake();

            InitializeSteamStats();

#if UNITY_EDITOR
            //DebugAllStats();
#endif

        }

        private void Start()
        {
            //player = FindFirstObjectByType<FirstPersonController>();
        }

        void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.U))
            {
                ResetAllStats();
            }
#endif

            if (chasing)
            {
                chaseTime += Time.deltaTime;
            }

            if (pigChasing)
            {
                pigChaseTime += Time.deltaTime;
            }
        }

        void OnEnable()
        {
            FloorTrapAlarm.OnTriggered += HandleOnFloorTrapAlarmTriggered;
            SpiderwebTrap.OnTriggered += HandleOnSpiderwebTrapTriggered;
            PiggyTrap.OnTriggered += HandleOnPiggyTrapTriggered;
            OutOfBreathTrap.OnTriggered += HandleOnOutOfBreathTrapTriggered;
            DeathTrap.OnTriggered += HandleOnDeathTrapTriggered;
            PlayerDeath.OnPlayerDead += HandleOnPlayerDead;
            VendingMachine.OnPurchased += HandleOnPurchased;
            TimedBuffDebuff.OnExpired += HandleOnBuffDebuffExpired;
            PlayerChased.OnChaseStarted += HandleOnChaseStarted;
            PlayerChased.OnChaseStopped += HandleOnChaseStopped;
            Wallet.OnBalanceUpdated += HandleOnWalletUpdated;
            Pig.OnStateChanged += HandleOnStateChanged;
        }

        void OnDisable()
        {
            FloorTrapAlarm.OnTriggered -= HandleOnFloorTrapAlarmTriggered;
            SpiderwebTrap.OnTriggered -= HandleOnSpiderwebTrapTriggered;
            PiggyTrap.OnTriggered -= HandleOnPiggyTrapTriggered;
            OutOfBreathTrap.OnTriggered -= HandleOnOutOfBreathTrapTriggered;
            DeathTrap.OnTriggered -= HandleOnDeathTrapTriggered;
            PlayerDeath.OnPlayerDead -= HandleOnPlayerDead;
            VendingMachine.OnPurchased -= HandleOnPurchased;
            TimedBuffDebuff.OnExpired -= HandleOnBuffDebuffExpired;
            PlayerChased.OnChaseStarted -= HandleOnChaseStarted;
            PlayerChased.OnChaseStopped -= HandleOnChaseStopped;
            Wallet.OnBalanceUpdated -= HandleOnWalletUpdated;
            Pig.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(PigState oldState, PigState newState)
        {
            switch (newState)
            {
                case PigState.Chasing:
                    pigChaseTime = 0;
                    pigChasing = true;
                    break;
                case PigState.Idle:
                    if (pigChasing)
                    {
                        var player = FindFirstObjectByType<FirstPersonController>();
                        pigChasing = false;
                        if (!player.IsDead)
                        {
                            IncrementStat("STAT_PIG_TIME", (int)pigChaseTime);
                        }
                        pigChaseTime = 0;
                    }
                    break;
            }
        }

        private void HandleOnWalletUpdated(int amount)
        {
            if(amount > 0)
            {
                IncrementStat("STAT_COIN", amount);
            }

        }

        private void HandleOnChaseStarted()
        {
            chasing = true;
            chaseTime = 0;
        }

        private void HandleOnChaseStopped()
        {
            chasing = false;

            var player = FindFirstObjectByType<FirstPersonController>();

            if (!player.IsDead)
            {
                IncrementStat("STAT_CHASE_TIME", (int)(chaseTime));
            }

            chaseTime = 0;
        }

        private void HandleOnBuffDebuffExpired(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() == typeof(PlayerDeafDebuff))
            {
                var player = FindFirstObjectByType<FirstPersonController>();
                if(!player.IsDead)
                {
                    IncrementStat("STAT_DEAF_TIME", (int)arg0.Duration);
                }
            }
        }

        private void HandleOnPurchased(VendingMachineType type)
        {
            switch (type)
            {
                case VendingMachineType.NoTriggerTiles:
                    IncrementStat("STAT_NO_ALARM", 1);
                    break;
                case VendingMachineType.Map:
                    IncrementStat("STAT_MAP", 1);
                    break;
                case VendingMachineType.CuteClown:
                    IncrementStat("STAT_CUTE", 1);
                    break;
            }
        }

        private void HandleOnPlayerDead()
        {
            IncrementStat("STAT_DEAD", 1);
        }

        private void HandleOnDeathTrapTriggered()
        {
            IncrementStat("STAT_DRUNK", 1);
        }

        private void HandleOnOutOfBreathTrapTriggered()
        {
            IncrementStat("STAT_ADDICT", 1);
        }

        private void HandleOnPiggyTrapTriggered()
        {
            IncrementStat("STAT_BANK", 1);
        }

        private void HandleOnSpiderwebTrapTriggered()
        {
            IncrementStat("STAT_WEB", 1);
        }

        private void HandleOnFloorTrapAlarmTriggered()
        {
             IncrementStat("STAT_ALARM", 1);
        }

        //private void HandleOnMonsterHit(MonsterController monsterController)
        //{
        //    string name = "DESTROYED_BOTS";
        //    GetStatInt(name, out int dbg);
        //    Debug.Log($"TEST - Beforre dest bots:{dbg}");
        //    IncrementStat(name, 1);
        //    GetStatInt(name, out dbg);
        //    Debug.Log($"TEST - After dest bots:{dbg}");

        //    // Get bot destroyed stat
        //    // if (GetStatInt(name, out int count))
        //    // {
        //    //     string format = "DESTROY_BOTS_{0}";
        //    //     Debug.Log($"TEST - Destroyed {count} bot(s).");
        //    //     if (count >= 100)
        //    //         SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 1));
        //    //     if (count >= 250)
        //    //         SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 2));
        //    //     if (count >= 500)
        //    //         SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 3));

        //    // }
        //}

        //private void HandleOnCustomDroneHit(CustomDroneController drone)
        //{
        //    switch (drone.Type)
        //    {
        //        case CustomDroneType.Diamond:
        //            //HandleOnDiamondPicked();
        //            IncrementStat("STOLEN_DIAMONDS", 1);
        //            break;
        //        case CustomDroneType.TimeUp:
        //            IncrementStat("CLOCK_COUNT", 1);
        //            //HandleOnTimeUpPicked();
        //            break;
        //        case CustomDroneType.Medical:
        //            IncrementStat("MEDKIT_COUNT", 1);
        //            break;
        //        case CustomDroneType.Battery:
        //            IncrementStat("BATTERY_COUNT", 1);
        //            break;
        //        case CustomDroneType.Pill:
        //            IncrementStat("PILL_COUNT", 1);
        //            break;
        //        case CustomDroneType.Shield:
        //            IncrementStat("SHIELD_COUNT", 1);
        //            break;


        //    }
        //}


        private void InitializeSteamStats()
        {
            if (!SteamManager.Initialized) return;

            //SteamUserStats.RequestCurrentStats();
            Debug.Log("Steam Stats Manager inizializzato");
        }

//        public int GetProgressStat()
//        {
//            if (!SteamManager.Initialized) return 0;

//            GetStatInt("PROGRESS", out int progress);

//            return progress;
//        }

//        public void UpdateProgressStat()
//        {
//            if (!SteamManager.Initialized) return;

//            SetStat("PROGRESS", GetProgressStat() + 1);

//#if UNITY_EDITOR
//            DebugAllStats();
//#endif


//        }

//        public int GetStageStat(int gameMode)
//        {
//            if (!SteamManager.Initialized) return 0;

//            GetStatInt($"STAGE_{gameMode + 1}", out int stage);

//            return stage;
//        }

//        public int GetStageMaxStat(int gameMode)
//        {
//            if (!SteamManager.Initialized) return 0;

//            GetStatInt($"STAGE_{gameMode + 1}_MAX", out int stage);

//            return stage;
//        }

//        public void UpdateStageStat(int gameMode)
//        {
//            if (!SteamManager.Initialized) return;



//            SetStat($"STAGE_{gameMode + 1}", GetStageStat((int)gameMode) + 1);

//#if UNITY_EDITOR
//            DebugAllStats();
//#endif


//        }

//        public void UpdateStageMaxStat(int gameMode)
//        {
//            if (!SteamManager.Initialized) return;

//            SetStat($"STAGE_{gameMode + 1}_MAX", GetStageMaxStat((int)gameMode) + 1);

//#if UNITY_EDITOR
//            DebugAllStats();
//#endif

//        }

        // ==================== SCRITTURA STATISTICHE ====================

        void SetStat(string statName, int value)
        {
            if (!SteamManager.Initialized) return;

            bool success = SteamUserStats.SetStat(statName, value);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Statistica {statName} aggiornata: {value}");
            }
            else
            {
                Debug.LogError($"Errore nell'aggiornare statistica: {statName}");
            }
        }

        void SetStat(string statName, float value)
        {
            if (!SteamManager.Initialized) return;

            bool success = SteamUserStats.SetStat(statName, value);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Statistica {statName} aggiornata: {value}");
            }
            else
            {
                Debug.LogError($"Errore nell'aggiornare statistica: {statName}");
            }
        }

        void IncrementStat(string statName, int increment = 1)
        {
            if (!SteamManager.Initialized) return;

            if (GetStatInt(statName, out int currentValue))
            {
                SetStat(statName, currentValue + increment);
            }
        }

        void IncrementStat(string statName, float increment = 1)
        {
            if (!SteamManager.Initialized) return;

            if (GetStatFloat(statName, out float currentValue))
            {
                SetStat(statName, currentValue + increment);
            }
        }

        // ==================== LETTURA STATISTICHE ====================

        bool GetStatInt(string statName, out int value)
        {
            value = 0;

            if (!SteamManager.Initialized) return false;

            
            bool success = SteamUserStats.GetStat(statName, out value);
            if (!success)
            {
                Debug.LogWarning($"Statistica {statName} non trovata");
            }

            return success;
        }

        bool GetStatFloat(string statName, out float value)
        {
            value = 0f;

            if (!SteamManager.Initialized) return false;

            bool success = SteamUserStats.GetStat(statName, out value);
            if (!success)
            {
                Debug.LogWarning($"Statistica {statName} non trovata");
            }

            return success;
        }

        // ==================== UTILITY ====================

        void ResetAllStats(bool includeAchievements = false)
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.ResetAllStats(includeAchievements);
            SteamUserStats.StoreStats();
            Debug.Log("Statistiche resetate" + (includeAchievements ? " (inclusi achievement)" : ""));
        }

        void ForceStoreStats()
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.StoreStats();
            Debug.Log("Statistiche forzatamente salvate");
        }

        void RequestStatsRefresh()
        {
            if (!SteamManager.Initialized) return;

            //SteamUserStats.RequestCurrentStats();
            Debug.Log("Statistiche ricaricate da Steam");
        }

        public bool GetGameLevel(out int level)
        {
            return GetStatInt("STAT_LEVEL", out level);
        }

        public void SetGameLevel(int level)
        {
            SetStat("STAT_LEVEL", level);
        }

        public void DebugAllStats()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam non inizializzato");
                return;
            }

            Debug.Log("=== STEAM STATS DEBUG ===");

            // Lista di tutte le stats che hai creato su Steamworks
            string[] statsToCheck = {
                "DESTROYED_BOTS",
                "STOLEN_DIAMONDS",
                "PROGRESS",
                "STAGE_1",
                "STAGE_2",
                "STAGE_3",
                "STAGE_4",
                "STAGE_5",
                "STAGE_6",

            };

            foreach (string statName in statsToCheck)
            {
                if (SteamUserStats.GetStat(statName, out int intValue))
                {
                    Debug.Log($"{statName}: {intValue}");
                }
                else if (SteamUserStats.GetStat(statName, out float floatValue))
                {
                    Debug.Log($"{statName}: {floatValue:F2}");
                }
                else
                {
                    Debug.LogWarning($"{statName}: Non trovata");
                }
            }
        }
    }
}
