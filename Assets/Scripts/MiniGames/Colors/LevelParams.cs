using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{

    public enum LevelSizeParam { Small, Medium, Big, Crazy, Random }

    public enum NumOfColorsParam { Two, Three, Four, Random }

    public enum LoadFactorParam { Low, High, Full, Random }

    public enum SymmetryParam { None, Horizontal, Vertical, Both, Random }

    public class LevelParams
    {
        static LevelParams instance;
        public static LevelParams Instance
        {
            get { if (instance == null) Create(); return instance; }
        }


        public LevelSizeParam LevelSizeParam { get; private set; } = LevelSizeParam.Medium;

        public NumOfColorsParam NumOfColorsParam { get; private set; } = NumOfColorsParam.Four;

        public LoadFactorParam LoadFactorParam { get; private set; } = LoadFactorParam.Low;

        public SymmetryParam SymmetryParam { get; private set; } = SymmetryParam.Both;

        string sizeKey = "levelSize";
        string numOfColorsKey = "numOfColors";
        string loadFactorKey = "loadFactor";
        string symmetryKey = "symmetry";

        private LevelParams()
        {
            // Load from player prefs
            Debug.Log("sizeParam:"+ PlayerPrefs.GetInt(sizeKey));
            if (PlayerPrefs.HasKey(sizeKey))
                LevelSizeParam = (LevelSizeParam)PlayerPrefs.GetInt(sizeKey);
            if (PlayerPrefs.HasKey(numOfColorsKey))
                NumOfColorsParam = (NumOfColorsParam)PlayerPrefs.GetInt(numOfColorsKey);
            if (PlayerPrefs.HasKey(loadFactorKey))
                LoadFactorParam = (LoadFactorParam)PlayerPrefs.GetInt(loadFactorKey);
            if (PlayerPrefs.HasKey(symmetryKey))
                SymmetryParam = (SymmetryParam)PlayerPrefs.GetInt(symmetryKey);
        }

        public void SetLevelSizeParam(LevelSizeParam param)
        {
            LevelSizeParam = param;
            PlayerPrefs.SetInt(sizeKey, (int)param);
        }

        public void SetNumOfColorsParam(NumOfColorsParam param)
        {
            NumOfColorsParam = param;
            PlayerPrefs.SetInt(numOfColorsKey, (int)param);
        }

        public void SetLoadFactorParam(LoadFactorParam param)
        {
            LoadFactorParam = param;
            PlayerPrefs.SetInt(loadFactorKey, (int)param);
        }

        public void SetSymmetryParam(SymmetryParam param)
        {
            SymmetryParam = param;
            PlayerPrefs.SetInt(symmetryKey, (int)param);
        }

        static void Create()
        {
            instance = new LevelParams();
            
        }
    }

}
