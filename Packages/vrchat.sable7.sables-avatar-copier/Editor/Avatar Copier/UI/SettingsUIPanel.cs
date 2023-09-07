#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SablesTools.AvatarCopier.Handlers;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class SettingsUIPanel
    {
        private static SettingsUIPanel _Instance = null;

        public static SettingsUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new SettingsUIPanel();
            }

            return _Instance;
        }

        private SettingsUIPanel()
        {

        }

        protected bool _bShowDefaults = false;

        public Vector2 ScrollPosition = new Vector2();

        public void DrawSettingsPanel()
        {
            GUILayout.BeginVertical("Settings", "window");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawSettingButtons();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void DrawSettingButtons()
        {
            CopierSettingsHandler.GetInstance().TrySetDataField("bUnpackPrefab", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bUnpackPrefab"), "Unpack Prefab",
                "Unpacks the Destination Avatar from being a Prefab if not making a copy of the Destination Avatar. !! THIS CANNOT BE UNDONE !!", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

            //float PreviousScaleEpsilon = MergerSavedData.GetInstance().ScaleEpsilon;
            //MergerSavedData.GetInstance().ScaleEpsilon = DrawFloatSettingsToggle(MergerSavedData.GetInstance().ScaleEpsilon, "Scale Epsilon",
            //    "The amount of difference in each element of a GameObject's scale that would create a Scale Operation.", MergerGUIStyles.GetMainMenuHelpBoxStyle());

            //if (PreviousScaleEpsilon != MergerSavedData.GetInstance().ScaleEpsilon)
            //if (MergerSavedData.GetInstance().TrySetProperty("ScaleEpsilon", MergerSavedData.GetInstance().ScaleEpsilon, DrawFloatSettingsToggle(MergerSavedData.GetInstance().ScaleEpsilon, "Scale Epsilon",
            //    "The amount of difference in each element of a GameObject's scale that would create a Scale Operation.", MergerGUIStyles.GetMainMenuHelpBoxStyle())))
            if (CopierSettingsHandler.GetInstance().TrySetDataField("ScaleEpsilon", DrawFloatSettingsToggle(CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"), "Scale Epsilon",
                "The amount of difference in each element of a GameObject's scale that would create a Scale Operation.", CopierGUIStyles.GetMainMenuHelpBoxStyle())))
            {
                ScaleOperationHandler.GetInstance().Reset();
                ScaleOperationHandler.GetInstance().CreateScaleOperations();
            }

            //bool bPrevCopyMaterials = CopierSettingsHandler.GetInstance().GetBoolDataValue("bCopyMaterials");
            //CopierSettingsHandler.GetInstance().TrySetDataField("bCopyMaterials", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bCopyMaterials"), "Copy Materials",
            //    "Allow Renderer Materials to be copied over.", CopierGUIStyles.GetMainMenuHelpBoxStyle(), false));
            //bool bCurrentCopyMaterials = CopierSettingsHandler.GetInstance().GetBoolDataValue("bCopyMaterials");

            //if (bCurrentCopyMaterials)
            //{
            //    CopierSettingsHandler.GetInstance().TrySetDataField("bSmartCopyMaterials", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bSmartCopyMaterials"), "Smart-ish Materials Copy",
            //        "Copies and organzies the Materials based on the Material's name.", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

            //    EditorGUILayout.HelpBox("Smart-ish Material Copy, copies by Name compaires the Destination Material names to that of the Source Materials names. This is not garunteed to give accurate results. However this may be slightly more accurate than copying materials by their index. See more about this feature in the Help Tab.", MessageType.Info);
            //}

            //GUILayout.EndVertical();

            //if (bPrevCopyMaterials != bCurrentCopyMaterials)
            //{
            //    List<PreservedPropertyData> CombinedList = new List<PreservedPropertyData>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties());
            //    List<PreservedPropertyData> PreservedProps = CopierSettingsHandler.GetInstance().GetObjectDataValue("PreservedProperties") as List<PreservedPropertyData>;
            //    CombinedList.AddRange(PreservedProps.ToArray());

            //    foreach (PreservedPropertyData PropData in CombinedList)
            //    {
            //        if (typeof(Renderer).IsAssignableFrom(PropData.GetPreservedComponentType()))
            //        {
            //            if (PropData.GetPropertyName() == "sharedMaterial" || PropData.GetPropertyName() == "sharedMaterials")
            //            {
            //                if (bCurrentCopyMaterials)
            //                {
            //                    PropData.bEnabled = true;
            //                }
            //            }
            //        }
            //    }
            //}

            GUILayout.BeginVertical(new GUIContent(""), CopierGUIStyles.GetMainMenuHelpBoxStyle());

            _bShowDefaults = EditorGUILayout.Foldout(_bShowDefaults, "Defaults");

            EditorGUI.indentLevel++;

            if (_bShowDefaults)
            {
                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseAttachableOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseAttachableOperations"), "Attachable Default Operations",
                    "The default Enabled status of Attachable Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseCompOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseCompOperations"), "Component Default Operations",
                    "The default Enabled status of Component Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseMaterialOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMaterialOperations"), "Material Default Operations",
                    "The default Enabled status of Material Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseScaleOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseScaleOperations"), "Scale Default Operations",
                    "The default Enabled status of Scale Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseEnabledDisabledOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseEnabledDisabledOperations"), "Object Enabled/Disabled Default Operations",
                    "The default Enabled status of Object Enabled/Disabled Default Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));

                CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseMiscOperations", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations"), "Misc Default Operations",
                    "The default Enabled status of Misc Operations", CopierGUIStyles.GetMainMenuHelpBoxStyle()));
            }

            EditorGUI.indentLevel--;

            GUILayout.EndVertical();


            CopierSettingsHandler.GetInstance().TrySetDataField("bShowAdvancedSettings", DrawSettingsToggle(CopierSettingsHandler.GetInstance().GetBoolDataValue("bShowAdvancedSettings"), "Show Advanced Settings",
                "Shows Advanced/Experimental Settings. Only for very specific use cases.", CopierGUIStyles.GetMainMenuHelpBoxStyle()));
        }

        protected bool DrawSettingsToggle(bool bValue, string LabelText, string TooltipText, GUIStyle Style, bool bCloseVertial = true)
        {
            Color oBGColor = GUI.backgroundColor;
            if (!bValue)
            {
                GUI.backgroundColor = CopierGUIStyles.DisabledHelpBoxColor;
            }
            GUILayout.BeginVertical(new GUIContent("", TooltipText), Style);
            GUI.backgroundColor = oBGColor;

            Color oColor = GUI.color;
            if (!bValue)
            {
                GUI.color = CopierGUIStyles.DisabledLabelColor;
            }

            bValue = EditorGUILayout.ToggleLeft(LabelText, bValue, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

            GUI.color = oColor;

            if (bCloseVertial)
            {
                GUILayout.EndVertical();
            }

            return bValue;
        }

        protected float DrawFloatSettingsToggle(float inValue, string LabelText, string TooltipText, GUIStyle Style, bool bCloseVertical = true)
        {
            GUILayout.BeginVertical(new GUIContent("", TooltipText), Style);

            float bValue = EditorGUILayout.DelayedFloatField(LabelText, inValue);

            if (bCloseVertical)
            {
                GUILayout.EndVertical();
            }

            return bValue;
        }
    }
}

#endif