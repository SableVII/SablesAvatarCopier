#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class HelpUIPanel
    {
        private static HelpUIPanel _Instance = null;

        public static HelpUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new HelpUIPanel();
            }

            return _Instance;
        }

        private HelpUIPanel()
        {

        }

        public Vector2 ScrollPosition = new Vector2();

        public void DrawHelpPanel()
        {
            GUILayout.BeginVertical("Help", "window");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawHelp();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void DrawHelp()
        {
            EditorGUILayout.LabelField("Meow the meow meow!!");
        }
    }
}

#endif