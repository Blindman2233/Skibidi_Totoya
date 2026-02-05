using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace DS.Utilities
{
    public static class DSStyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                // Try load via old "Editor Default Resources" mechanism
                StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);

                // Fallback for newer Unity versions: try load by asset path
                if (styleSheet == null)
                {
                    string fileName = Path.GetFileName(styleSheetName); // e.g. DSGraphViewStyles.uss
                    string[] guids = AssetDatabase.FindAssets($"{Path.GetFileNameWithoutExtension(fileName)} t:StyleSheet");

                    foreach (string guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                        if (assetPath.EndsWith(fileName))
                        {
                            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(assetPath);
                            break;
                        }
                    }
                }

                if (styleSheet == null)
                {
                    UnityEngine.Debug.LogWarning($"[DSStyleUtility] StyleSheet not found: {styleSheetName}");
                    continue;
                }

                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}