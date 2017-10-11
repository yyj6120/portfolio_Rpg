#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace Utils.Editor
{
    public static class ScriptableObjectUtility
    {
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
        }
    }
}
#endif