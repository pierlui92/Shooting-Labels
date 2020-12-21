using UnityEngine;
using UnityEditor;

namespace EZEffects
{
    public class CreateImpactEffect
    {
        [MenuItem("Assets/Create/New Impact Effect")]

        public static void Create()
        {
            EffectImpact asset = ScriptableObject.CreateInstance<EffectImpact>();

            AssetDatabase.CreateAsset(asset, "Assets/NewImpactEffect.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}