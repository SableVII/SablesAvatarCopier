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
    public class PreservedParametersUIPanel
    {
        private static PreservedParametersUIPanel _Instance = null;

        public static PreservedParametersUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new PreservedParametersUIPanel();
            }

            return _Instance;
        }

        private PreservedParametersUIPanel()
        {
            AllowedCopyTypeFriendlyNames = PreservedPropertyHandler.GetInstance().GetAllowedCopyTypeFriendlyNameArray();
        }
        
        public Vector2 ScrollPosition = new Vector2();

        bool bShowPreservedParameters = false;

        string[] AllowedCopyTypeFriendlyNames; 

        public void DrawPreservedPropertiessUIPanel()
        {
            GUILayout.BeginVertical("Advanced", "window");

            //ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle(), GUILayout.MaxHeight(290));
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawPreservedProperties();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

        }

        public void DrawPreservedProperties()
        {
            List<PreservedPropertyData> CombinedList = new List<PreservedPropertyData>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties());
            List<PreservedPropertyData> PreservedProps = CopierSettingsHandler.GetInstance().GetObjectDataValue("PreservedProperties") as List<PreservedPropertyData>;
            CombinedList.AddRange(PreservedProps.ToArray());

            int PreservedPropertiesCount = CombinedList.Count;
            int PreservedPropertiesEnabledCount = 0;
            foreach (PreservedPropertyData PreservedPropData in CombinedList)
            {
                if (PreservedPropData.bEnabled)
                {
                    PreservedPropertiesEnabledCount++;
                }
            }//

            Color OriginalBackgroundColor = GUI.backgroundColor;
            Color OriginalColor = GUI.color;
            if (!CopierSettingsHandler.GetInstance().GetBoolDataValue("bPreserveProperties"))
            {
                GUI.backgroundColor = CopierGUIStyles.DisabledHelpBoxColor;
                GUI.color = CopierGUIStyles.DisabledLabelColor;
            }

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            GUI.backgroundColor = OriginalBackgroundColor; // To not get too dark on Background colors

            GUILayout.BeginHorizontal();

            /// Enabled Attachment Operations in Total.
            CopierSettingsHandler.GetInstance().TrySetDataField("bPreserveProperties", EditorGUILayout.Toggle(new GUIContent("", "Tooltip."), CopierSettingsHandler.GetInstance().GetBoolDataValue("bPreserveProperties"), EditorStyles.toggle, GUILayout.MaxWidth(15)));

            /// Attachable Object Text
            string CountText = AvatarCopierUtils.GetXOutOfTotalText((CopierSettingsHandler.GetInstance().GetBoolDataValue("bPreserveProperties") ? PreservedPropertiesCount : 0), PreservedPropertiesCount);

            string LabelText = "Preserved Properties/Fields  <color=" + CopierGUIStyles.LightTextHexColor + ">(" + CountText + ")</color>";

            if (PreservedPropertiesCount == 0)
            {
                LabelText = "<color=" + CopierGUIStyles.LightTextHexColor + ">" + LabelText + "</color>";
            }

            //bShowPreservedParameters = MergerGUIStyles.ClickableLabelFoldout(bShowPreservedParameters, new GUIContent(LabelText, TooltipText), MergerGUIStyles.GetMainCategoryFoldoutStyle());
            bShowPreservedParameters = EditorGUILayout.Foldout(bShowPreservedParameters, new GUIContent(LabelText), CopierGUIStyles.GetMainCategoryFoldoutStyle());


            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            if (bShowPreservedParameters)
            {
                // Message
                EditorGUILayout.HelpBox("This is an in-Alpha feature and not totally finished. See Help Tab for more information.", MessageType.Info);

                // Add Preserved Parameter Button
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Add Preserved Property/Field", "Tooltip")))
                {
                    //MergerSettingsHandler.GetInstance().PreservedProperties.Add(new PreservedPropertyData());
                    PreservedProps.Add(new PreservedPropertyData());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("");
                GUILayout.EndHorizontal();



                // Show Preserved Paramemters
                // Go in reverse order as it looks better
                for (int PreservedPropIndex = CombinedList.Count-1; PreservedPropIndex >= 0; PreservedPropIndex--)
                {
                    PreservedPropertyData PreservedPropData = CombinedList[PreservedPropIndex];

                    Color oBGColor = GUI.backgroundColor;
                    Color oColor = GUI.color;
                    if (!PreservedPropData.bEnabled)
                    {
                        GUI.backgroundColor = CopierGUIStyles.DisabledHelpBoxColor;
                        GUI.color = CopierGUIStyles.DisabledLabelColor;
                    }

                    GUILayout.BeginVertical("", CopierGUIStyles.GetObjectCompOperationHelpBoxStyle());
                    GUI.backgroundColor = oBGColor;


                    EditorGUILayout.BeginHorizontal();

                    bool OriginalEnabled = PreservedPropData.bEnabled;
                    PreservedPropData.bEnabled = EditorGUILayout.ToggleLeft("", PreservedPropData.bEnabled, GUILayout.MaxWidth(15));

                    /// Type Popup List
                    if (PreservedPropData.GetIsDefault() && PreservedPropData.bEnabled != OriginalEnabled)
                    {
                        int IndexOfDefaultProp = 0;

                        for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
                        {
                            if (PreservedPropData == PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties()[i])
                            {
                                IndexOfDefaultProp = i;
                            }
                        }

                        PreservedPropertyHandler.GetInstance().SetDefaultPropertyEnabled(IndexOfDefaultProp, PreservedPropData.bEnabled);
                    }

                    // If this is a Renderer preserved prop and a Shared Materials is turned off, don't allow Materials to be copied
                    if (PreservedPropData.bEnabled != OriginalEnabled)
                    {
                        if (typeof(Renderer).IsAssignableFrom(PreservedPropData.GetPreservedComponentType()))
                        {
                            if (PreservedPropData.GetPropertyName() == "sharedMaterial" || PreservedPropData.GetPropertyName() == "sharedMaterials")
                            {
                                if (PreservedPropData.bEnabled == false)
                                {
                                    CopierSettingsHandler.GetInstance().TrySetDataField("bCopyMaterials", false);
                                }
                            }
                        }
                    }

                    // Don't allow edits to defaults
                    if (PreservedPropData.GetIsDefault())
                    {
                        GUI.enabled = false;
                    }

                    int OriginalTypeIndex = PreservedPropData.GetSelectedTypeIndex();
                    int InputTypeIndex = EditorGUILayout.Popup(PreservedPropData.GetSelectedTypeIndex(), AllowedCopyTypeFriendlyNames);
                    if (OriginalTypeIndex != InputTypeIndex)
                    {
                        PreservedPropData.SetSelectedTypeIndex(InputTypeIndex);
                        //PreservedParamater. = TypesListed[OriginalTypeIndex];
                        //PreservedParamater.SelectedPropertyIndex = 0;
                    }

                    GUILayout.FlexibleSpace();

                    /// Property Popup List
                    if (PreservedPropData.GetPreservedComponentType() != null)
                    {
                        int OriginalPropertyTypeIndex = PreservedPropData.GetSelectedPropertyIndex();
                        int InputPropertyTypeIndex = EditorGUILayout.Popup(PreservedPropData.GetSelectedPropertyIndex(), PreservedPropertyHandler.GetInstance().GetPropertyArrayOfType(PreservedPropData.GetPreservedComponentType()));
                        if (OriginalPropertyTypeIndex != InputPropertyTypeIndex)
                        {
                            PreservedPropData.SetSelectedPropertyIndex(InputPropertyTypeIndex);
                        }
                    }

                    EditorGUILayout.EndHorizontal();



                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    // Default label (if Default)
                    if (PreservedPropData.GetIsDefault())
                    {
                        GUILayout.Label(new GUIContent("<color=" + CopierGUIStyles.LightTextHexColor + ">Default</color>", "Tooltip"), CopierGUIStyles.GetLabelRichText());
                        GUILayout.FlexibleSpace();
                    }

                    // Remove Button
                    if (GUILayout.Button(new GUIContent("Remove", "Tooltip"), GUILayout.Width(92)))
                    {
                        for (int i = 0; i < PreservedProps.Count; i++)
                        {
                            PreservedProps.Remove(PreservedPropData);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;


                    GUI.color = oColor;

                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
        }
    }
}

#endif