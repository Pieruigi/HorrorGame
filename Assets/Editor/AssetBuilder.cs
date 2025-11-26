using System.Collections;
using System.Collections.Generic;
using TMM.Scriptables;
using Unity.VisualScripting.TextureAssets;
using UnityEditor;
using UnityEngine;


namespace TMM.Editor
{
    public class AssetBuilder : MonoBehaviour
    {
        public const string ResourceFolder = "Assets/Resources";

        [MenuItem("Assets/Create/DD2/WallBlock")]
        public static void CreateWallBlockAsset()
        {
            WallBlockAsset asset = ScriptableObject.CreateInstance<WallBlockAsset>();

            string name = "WallBlock.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, WallBlockAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/DD2/Floor")]
        public static void CreateFloorAsset()
        {
            FloorAsset asset = ScriptableObject.CreateInstance<FloorAsset>();

            string name = "Floor.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, FloorAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

         [MenuItem("Assets/Create/DD2/MiniGame")]
        public static void CreateMiniGameAsset()
        {
            MiniGameAsset asset = ScriptableObject.CreateInstance<MiniGameAsset>();

            string name = "MiniGame.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, MiniGameAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }


    }

   

}



