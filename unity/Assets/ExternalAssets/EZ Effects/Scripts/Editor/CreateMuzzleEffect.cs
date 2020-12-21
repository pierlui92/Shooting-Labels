using UnityEngine;
using UnityEditor;

namespace EZEffects
{
    public class CreateMuzzleEffect
    {
        [MenuItem("Assets/Create/New Muzzle Effect")]

        public static void Create()
        {
            EffectMuzzleFlash asset = ScriptableObject.CreateInstance<EffectMuzzleFlash>();

            AssetDatabase.CreateAsset(asset, "Assets/NewMuzzleEffect.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}