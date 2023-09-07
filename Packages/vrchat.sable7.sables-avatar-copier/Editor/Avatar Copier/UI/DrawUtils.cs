#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SablesTools.AvatarCopier.EditorUI
{
    public static class DrawUtils
    {
        public static readonly float ScrollToPositionOffset = -80.0f;

        public static readonly float SmallIconSymbolSize = 19.0f;
        public static readonly float WarningSymbolXOffset = 19.0f;

        //public static void DrawSymbols(Rect rect, ref string text, params string[] iconNames)
        //{
        //    for (int i = 0; i < iconNames.Length; i++)
        //    {
        //        GUI.DrawTexture(new Rect(rect.x, rect.y, SmallIconSymbolSize, SmallIconSymbolSize), MergerGlobals.GetIconTexture(iconNames[i]), ScaleMode.ScaleToFit);

        //        //text = "      " + text;

        //        rect.x += WarningSymbolXOffset;
        //    }

        //    rect.x += 2;
        //}

        public static Rect DrawSymbols(Rect rect, params string[] iconNames)
        {
            bool bDrewSymbol = false;
            for (int i = 0; i < iconNames.Length; i++)
            {
                Texture2D texture = AvatarCopierUtils.GetIconTexture(iconNames[i]);

                if (texture == null)
                {
                    continue;
                }

                if (bDrewSymbol == false)
                {
                    rect.x -= 4;
                    bDrewSymbol = true;
                }


                GUI.DrawTexture(new Rect(rect.x, rect.y, SmallIconSymbolSize, SmallIconSymbolSize), texture, ScaleMode.ScaleToFit);

                rect.x += WarningSymbolXOffset;
            }

            if (bDrewSymbol)
            {
                // A little extra margin for text
                rect.x += 2;
            }

            return rect;
        }

        public static void DrawTextWithSymbols(Rect rect, string text, params string[] iconNames)
        {
            rect.x += 3.0f;

            rect = DrawSymbols(rect, iconNames);

            EditorGUI.LabelField(rect, text, CopierGUIStyles.GetLabelRichText());
        }

        public static bool DrawButtonWithSymbols(Rect rect, string text, params string[] iconNames)
        {
            return DrawButtonWithSymbols(rect, text, EditorStyles.miniButton, iconNames);
        }

        public static bool DrawButtonWithSymbols(Rect rect, string text, GUIStyle buttonStyle, params string[] iconNames)
        {
            Rect inRect = rect;
            rect = DrawSymbols(rect, iconNames);

            EditorGUI.LabelField(rect, text, CopierGUIStyles.GetLabelRichText());
            return GUI.Button(inRect, "", buttonStyle);
        }

        public static bool DrawToggleWithSymbols(bool bToggleValue, Rect rect, string text, params string[] iconNames)
        {
            return DrawToggleWithSymbols(bToggleValue, rect, text, EditorStyles.toggle, iconNames);
        }

        public static bool DrawToggleWithSymbols(bool bToggleValue, Rect rect, string text, GUIStyle toggleStyle = null, params string[] iconNames)
        {
            Rect inRect = rect;
            Rect adjRect = rect;
            adjRect.width = 15.0f;

            bToggleValue = EditorGUI.Toggle(adjRect, "", bToggleValue, toggleStyle);            
            rect.x += 22;

            rect = DrawSymbols(rect, iconNames);

            EditorGUI.LabelField(rect, text, CopierGUIStyles.GetLabelRichText());

            if (GUI.Button(inRect, "", CopierGUIStyles.GetLabelRichText()))
            {
                return !bToggleValue;
            }

            return bToggleValue;
        }

        public static bool DrawFoldoutWithSymbols(bool bFoldoutValue, Rect rect, string text, params string[] iconNames)
        {
            return DrawFoldoutWithSymbols(bFoldoutValue, rect, text, EditorStyles.foldout, iconNames);
        }

        public static bool DrawFoldoutWithSymbols(bool bFoldoutValue, Rect rect, string text, GUIStyle foldoutStyle, params string[] iconNames)
        {
            Rect inRect = rect;
            bFoldoutValue = EditorGUI.Foldout(rect, bFoldoutValue, "", foldoutStyle);
            rect.x += 18;

            rect = DrawSymbols(rect, iconNames);

            EditorGUI.LabelField(rect, text, CopierGUIStyles.GetLabelRichText());

            if (GUI.Button(inRect, "", CopierGUIStyles.GetLabelRichText()))
            {
                return !bFoldoutValue;
            }

            return bFoldoutValue;
        }

        public static void DrawToggleFoldoutWithSymbols(ref bool bToggleValue, ref bool bFoldoutValue, Rect rect, string text, params string[] iconNames)
        {
            DrawToggleFoldoutWithSymbols(ref bToggleValue, ref bFoldoutValue, rect, text, EditorStyles.toggle, EditorStyles.foldout, iconNames);
        }

        public static void DrawToggleFoldoutWithSymbols(ref bool bToggleValue, ref bool bFoldoutValue, Rect rect, string text, GUIStyle toggleStyle, GUIStyle foldoutStyle, params string[] iconNames)
        {
            Rect inRect = rect;
            Rect adjRect = rect;
            adjRect.width = 15.0f;

            bToggleValue = EditorGUI.Toggle(adjRect, bToggleValue, toggleStyle);
            rect.x += 18;

            bFoldoutValue = EditorGUI.Foldout(rect, bFoldoutValue, "", foldoutStyle);
            rect.x += 18;

            rect = DrawSymbols(rect, iconNames);

            EditorGUI.LabelField(rect, text, CopierGUIStyles.GetLabelRichText());

            if (GUI.Button(inRect, "", CopierGUIStyles.GetLabelRichText()))
            {
                bFoldoutValue = !bFoldoutValue;
            }
        }
    }
}
#endif