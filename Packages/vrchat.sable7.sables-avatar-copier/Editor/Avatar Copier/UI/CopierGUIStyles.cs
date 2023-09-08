#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SablesTools.AvatarCopier.EditorUI
{
    public static class CopierGUIStyles
    {
        private static GUIStyle BoxStyle = null;
        private static GUIStyle BoxMarginlessStyle = null;
        private static GUIStyle HighlightableBoxMarginlessStyle = null;
        private static GUIStyle LabelRichTextSytle = null;
        private static GUIStyle LabelRightAlignedRichTextSytle = null;

        private static GUIStyle OperationToggleAllButtonStyle = null;

        private static GUIStyle OnlyCarrotFoldoutStyle = null;

        private static GUIStyle MarginedHelpBoxStyle = null;
        private static GUIStyle MainMenuHelpBoxStyle = null;
        private static GUIStyle ObjectComponentCompOperationHelpBoxStyle = null;
        private static GUIStyle SubMenuHelpBoxStyle = null;
        private static GUIStyle UnhighlightableLabelStyle = null;

        private static GUIStyle RegisteredPropertiesVerticalStyle = null;

        private static GUIStyle VirtualObjectFieldButtonStyle = null;

        //private static GUIStyle SmallWarningIconStyle = null;


        private static GUIStyle MainCategoryFoldoutStyle = null;
        private static GUIStyle EmptyCategoryStyle = null;

        public static string LightTextHexColor = "#787878";
        public static string UnselectedLightBlueAttachableTextHexColor = "#55707f";
        public static string LightBlueAttachableTextHexColor = "#88a0bf";
        public static string DimSelectionTextHexColor = "#808080ff";
        public static string SelectionOverlayTextHexColor = "#d9d9d9ff";

        //public static Color AttachableColor = new Color(0.2f, 0.2f, 1.0f);
        //public static Color ReplacedTagColor = new Color(0.65f, 0.65f, 0.25f);
        //public static Color PreExistsTagColor = new Color(0.9f, 0.6f, 0.2f);
        //public static Color UnusedTagColor = new Color(0.08f, 0.08f, 0.08f);
        //public static Color MovedTagColor = new Color(0.3f, 0.6f, 0.95f);
        //public static Color NewComponentTagColor = new Color(0.2f, 1.0f, 0.2f);

        public static string AttachableTextHexColor = "#4040ff";
        public static string OverridingTagHexTextColor = "#ffb010";
        public static string ReplacedTagHexTextColor = "#f0f030";
        public static string PreExistsTagHexTextColor = "#a7a7a7";
        public static string UnusedTagHexTextColor = "#151515";
        public static string MovedTagHexTextColor = "#60b0d0";
        public static string NewComponentTagHexTextColor = "#40ff40";

        public static Color DisabledHelpBoxColor = new Color(0.10f, 0.10f, 0.10f, 0.55f);
        public static Color DisabledLabelColor = new Color(0.633333f, 0.633333f, 0.633333f, 1.0f);

        private static Texture2D BoxBackground = null;
        private static Texture2D HighlightedBoxBackground = null;
        private static Texture2D ClearBoxBackground = null;

        public static Color DefaultBackgroundColor = new Color();

        public static Color DisabledGUIColor = new Color(1, 1, 1, 0.5f);

        //private static GUIStyle ToggleDimEnabledStyle = null;
        //private static GUIStyle TextInputDimEnabledStyle = null;


        private static GUIStyle SelectedLabelStyle = null;

        public static GUIStyle GetOnlyCarrotFoldoutStyle()
        {
            if (OnlyCarrotFoldoutStyle == null)
            {
                OnlyCarrotFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                OnlyCarrotFoldoutStyle.fixedWidth = 20.0f;
                OnlyCarrotFoldoutStyle.padding = new RectOffset(10, 0, 0, 0);
            }

            return OnlyCarrotFoldoutStyle;
        }

        public static GUIStyle GetMainCategoryFoldoutStyle()
        {
            if (MainCategoryFoldoutStyle == null)
            {
                MainCategoryFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                MainCategoryFoldoutStyle.normal.textColor = new Color(0.82f, 0.82f, 0.82f);
                MainCategoryFoldoutStyle.fontSize = EditorStyles.toggle.fontSize;
                MainCategoryFoldoutStyle.font = EditorStyles.toggle.font;
                MainCategoryFoldoutStyle.fixedHeight = EditorStyles.toggle.fixedHeight;
                //MainCategoryFoldoutStyle.margin = EditorStyles.toggle.margin;
                //MainCategoryFoldoutStyle.padding = EditorStyles.toggle.padding;
                MainCategoryFoldoutStyle.stretchHeight = EditorStyles.toggle.stretchHeight;
                MainCategoryFoldoutStyle.imagePosition = EditorStyles.toggle.imagePosition;
                MainCategoryFoldoutStyle.border = EditorStyles.toggle.border;
                MainCategoryFoldoutStyle.alignment = EditorStyles.toggle.alignment;
                MainCategoryFoldoutStyle.fontStyle = EditorStyles.toggle.fontStyle;
                MainCategoryFoldoutStyle.richText = true;
            }

            return MainCategoryFoldoutStyle;
        }

        public static GUIStyle GetOperationToggleAllButtonStyle()
        {
            if (OperationToggleAllButtonStyle == null)
            {
                OperationToggleAllButtonStyle = new GUIStyle(EditorStyles.miniButton);
                RectOffset Margin = OperationToggleAllButtonStyle.margin;
                Margin.left = 20;
                OperationToggleAllButtonStyle.margin = Margin;
            }

            return OperationToggleAllButtonStyle;
        }

        public static GUIStyle GetEmptyCategoryStyle()
        {
            if (EmptyCategoryStyle == null)
            {
                EmptyCategoryStyle = new GUIStyle(EditorStyles.label);
                EmptyCategoryStyle.normal.textColor = new Color(0.82f, 0.82f, 0.82f);
                EmptyCategoryStyle.richText = true;

                EmptyCategoryStyle.padding.left = 13;
            }

            return EmptyCategoryStyle;
        }

        public static GUIStyle GetLabelRichText()
        {
            if (LabelRichTextSytle == null)
            {
                LabelRichTextSytle = new GUIStyle(EditorStyles.label);
                LabelRichTextSytle.richText = true;
            }

            return LabelRichTextSytle;
        }

        public static GUIStyle GetLabelRightAlignedRichText()
        {
            if (LabelRightAlignedRichTextSytle == null)
            {
                LabelRightAlignedRichTextSytle = new GUIStyle(EditorStyles.label);
                LabelRightAlignedRichTextSytle.richText = true;
                LabelRightAlignedRichTextSytle.alignment = TextAnchor.MiddleRight;
            }

            return LabelRightAlignedRichTextSytle;
        }

        public static GUIStyle GetVirtualTextFieldButtonStyle()
        {
            if (VirtualObjectFieldButtonStyle == null)
            {
                VirtualObjectFieldButtonStyle = new GUIStyle(EditorStyles.textField);
                VirtualObjectFieldButtonStyle.margin.left = 40;
                VirtualObjectFieldButtonStyle.fixedWidth = 200.0f;
                VirtualObjectFieldButtonStyle.richText = true;
                //VirtualObjectFieldButtonStyle.stretchWidth = false;
            }

            return VirtualObjectFieldButtonStyle;
        }

        public static GUIStyle GetBoxStyle()
        {
            if (BoxStyle == null)
            {
                BoxStyle = new GUIStyle(GUI.skin.box);
                BoxStyle.normal.background = GetBoxBackground();
                BoxStyle.richText = true;
            }

            return BoxStyle;
        }

        public static GUIStyle GetBoxMarginlessStyle()
        {
            if (BoxMarginlessStyle == null)
            {
                BoxMarginlessStyle = new GUIStyle(GUI.skin.box);
                BoxMarginlessStyle.normal.background = GetBoxBackground();
                BoxMarginlessStyle.margin = new RectOffset(0, 0, 0, 0);
                BoxMarginlessStyle.padding = new RectOffset(0, 0, 0, 0);
                BoxMarginlessStyle.border = new RectOffset(0, 0, 0, 0);
                BoxMarginlessStyle.stretchHeight = false;
            }

            return BoxMarginlessStyle;
        }

        public static GUIStyle GetSelectedLabelStyle()
        {
            if (SelectedLabelStyle == null)
            {
                SelectedLabelStyle = new GUIStyle(EditorStyles.label);
                SelectedLabelStyle.stretchWidth = true;
                SelectedLabelStyle.richText = true;
                SelectedLabelStyle.alignment = TextAnchor.MiddleCenter;
            }

            return SelectedLabelStyle;
        }

        public static GUIStyle GetHighlightableBoxMarginlessStyle()
        {
            if (HighlightableBoxMarginlessStyle == null)
            {
                HighlightableBoxMarginlessStyle = new GUIStyle(GUI.skin.box);
                HighlightableBoxMarginlessStyle.normal.background = GetClearBoxBackground();
                HighlightableBoxMarginlessStyle.margin = new RectOffset(0, 0, 0, 0);
                HighlightableBoxMarginlessStyle.padding = new RectOffset(0, 0, 0, 0);
                HighlightableBoxMarginlessStyle.border = new RectOffset(0, 0, 0, 0);
                HighlightableBoxMarginlessStyle.stretchHeight = false;
                //HighlightableBoxMarginlessStyle.hover.background = GetHighlightedBoxBackground();

                //HighlightableBoxMarginlessStyle.onHover.background = GetHighlightedBoxBackground();
            }

            return HighlightableBoxMarginlessStyle;
        }

        public static GUIStyle GetMarginedHelpBoxStyle()
        {
            if (MarginedHelpBoxStyle == null)
            {
                MarginedHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                RectOffset Margin = MarginedHelpBoxStyle.margin;
                Margin.left = 20;
                Margin.right = 10;
                MarginedHelpBoxStyle.margin = Margin;

                RectOffset Padding = MarginedHelpBoxStyle.padding;
                Padding.left = 5;
                MarginedHelpBoxStyle.padding = Padding;
            }

            return MarginedHelpBoxStyle;
        }

        public static GUIStyle GetObjectCompOperationHelpBoxStyle()
        {
            if (ObjectComponentCompOperationHelpBoxStyle == null)
            {
                ObjectComponentCompOperationHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                RectOffset Margin = ObjectComponentCompOperationHelpBoxStyle.margin;
                Margin.left = 35;
                Margin.right = 10;
                ObjectComponentCompOperationHelpBoxStyle.margin = Margin;

                RectOffset Padding = ObjectComponentCompOperationHelpBoxStyle.padding;
                Padding.left = 5;
                ObjectComponentCompOperationHelpBoxStyle.padding = Padding;
            }

            return ObjectComponentCompOperationHelpBoxStyle;
        }

        public static GUIStyle GetRegisteredPropertiesVerticalStyle()
        {
            if (RegisteredPropertiesVerticalStyle == null)
            {
                RegisteredPropertiesVerticalStyle = new GUIStyle();
                RectOffset Margin = RegisteredPropertiesVerticalStyle.margin;
                Margin.left = 20;
                Margin.right = 10;
                RegisteredPropertiesVerticalStyle.margin = Margin;

                RectOffset Padding = RegisteredPropertiesVerticalStyle.padding;
                Padding.left = 5;
                RegisteredPropertiesVerticalStyle.padding = Padding;
            }

            return RegisteredPropertiesVerticalStyle;
        }

        public static GUIStyle GetMainMenuHelpBoxStyle()
        {
            if (MainMenuHelpBoxStyle == null)
            {
                MainMenuHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                RectOffset Margin = MainMenuHelpBoxStyle.margin;
                MainMenuHelpBoxStyle.margin = Margin;

                RectOffset Padding = MainMenuHelpBoxStyle.padding;
                Padding.left = 5;
                Padding.bottom = 5;
                MainMenuHelpBoxStyle.padding = Padding;
            }

            return MainMenuHelpBoxStyle;
        }

        public static GUIStyle GetSubMainHelpBoxMenuStyle()
        {
            if (SubMenuHelpBoxStyle == null)
            {
                SubMenuHelpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                RectOffset Margin = SubMenuHelpBoxStyle.margin;
                Margin.left = 20;
                Margin.right = 10;
                SubMenuHelpBoxStyle.margin = Margin;

                RectOffset Padding = SubMenuHelpBoxStyle.padding;
                Padding.left = 5;
                Padding.bottom = 5;
                SubMenuHelpBoxStyle.padding = Padding;
            }

            return SubMenuHelpBoxStyle;
        }

        public static GUIStyle GetUnhighlightableLeftToggleStyle()
        {
            if (UnhighlightableLabelStyle == null)
            {
                UnhighlightableLabelStyle = new GUIStyle(EditorStyles.label);
                UnhighlightableLabelStyle.richText = true;
                UnhighlightableLabelStyle.focused.textColor = UnhighlightableLabelStyle.normal.textColor;
                UnhighlightableLabelStyle.active.textColor = UnhighlightableLabelStyle.normal.textColor;
            }

            return UnhighlightableLabelStyle;
        }

        //public static GUIStyle GetSmallWarningIconStyle()
        //{
        //    if (SmallWarningIconStyle == null)
        //    {
        //        SmallWarningIconStyle = new GUIStyle();
        //        SmallWarningIconStyle.normal.background = GetWarningIconTexture();
        //        SmallWarningIconStyle.padding = new RectOffset(0, 0, 0, 0);
        //        SmallWarningIconStyle.margin = new RectOffset(0, 0, 0, 0);
        //    }

        //    return SmallWarningIconStyle;
        //}

        public static GUIStyle GetBoarderlessClickableStyle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);

            style.richText = true;
            style.normal.textColor = new Color(0.82f, 0.82f, 0.82f);

            //style.normal.textColor = Color.white;

            //style.focused.textColor = Color.cyan;

            //style.hover.textColor = Color.cyan;

            //style.onHover.textColor = Color.cyan; 

            return style;
        }

        /*public static GUIStyle GetToggleDimEnabledStyle()
        {
            if (ToggleDimEnabledStyle == null)
            {
                ToggleDimEnabledStyle = new GUIStyle();
                ToggleDimEnabledStyle.normal = ToggleDimEnabledStyle. .background = GetWarningIconTexture();
                ToggleDimEnabledStyle.padding = new RectOffset(0, 0, 0, 0);
                ToggleDimEnabledStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            return ToggleDimEnabledStyle;
        }

        private static GUIStyle GetTextInputDimEnabledStyle()
        {

        }*/

        // Textures


        public static Texture2D GetBoxBackground()
        {
            if (BoxBackground == null)
            {
                BoxBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                BoxBackground.SetPixel(0, 0, new Color(0, 0, 0, 0.15f));
                BoxBackground.Apply();
            }

            return BoxBackground;
        }

        public static Texture2D GetHighlightedBoxBackground()
        {
            if (HighlightedBoxBackground == null)
            {
                HighlightedBoxBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                HighlightedBoxBackground.SetPixel(0, 0, new Color(255, 255, 255, 0.15f));
                HighlightedBoxBackground.Apply();
            }

            return HighlightedBoxBackground;
        }

        public static Texture2D GetClearBoxBackground()
        {
            if (ClearBoxBackground == null)
            {
                ClearBoxBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
                ClearBoxBackground.SetPixel(0, 0, new Color(255, 255, 255, 0.0f));
                ClearBoxBackground.Apply();
            }

            return ClearBoxBackground;
        }
    }
}
#endif