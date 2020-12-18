using UnityEngine;
using UnityEditor;

namespace EZEffects
{
    public class CreateTracerEffect
    {
        [MenuItem("Assets/Create/New Tracer Effect")]

        public static void Create()
        {
            EffectTracer asset = ScriptableObject.CreateInstance<EffectTracer>();

            AssetDatabase.CreateAsset(asset, "Assets/NewTracerEffect.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}