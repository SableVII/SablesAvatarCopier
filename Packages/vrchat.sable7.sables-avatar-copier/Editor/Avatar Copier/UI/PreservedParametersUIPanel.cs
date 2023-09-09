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
            //List<PreservedPropertyData> combinedList = new List<PreservedPropertyData>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties());
            //List<PreservedPropertyData> PreservedProps = CopierSettingsHandler.GetInstance().GetObjectDataValue("PreservedProperties") as List<PreservedPropertyData>;
            //CombinedList.AddRange(PreservedProps.ToArray());

            int preservedPropertiesCount = PreservedPropertyHandler.GetInstance().GetPreservedPropertyCount();
            int preservedPropertiesEnabledCount = PreservedPropertyHandler.GetInstance().GetEnabledPereservedPropertyCount();

            Color originalBackgroundColor = GUI.backgroundColor;
            Color OriginalColor = GUI.color;
            if (PreservedPropertyHandler.GetInstance().GetEnabledPereservedPropertyCount() == 0)
            {
                GUI.backgroundColor = CopierGUIStyles.DisabledHelpBoxColor;
                GUI.color = CopierGUIStyles.DisabledLabelColor;
            }

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            GUI.backgroundColor = originalBackgroundColor; // To not get too dark on Background colors

            GUILayout.BeginHorizontal();

            /// Enabled Attachment Operations in Total.
            //CopierSettingsHandler.GetInstance().TrySetBoolDataField("bPreserveProperties", EditorGUILayout.Toggle(new GUIContent("", "Tooltip."), CopierSettingsHandler.GetInstance().GetBoolDataValue("bPreserveProperties"), EditorStyles.toggle, GUILayout.MaxWidth(15)));

            // Preserved Property Toggle
            bool bPrevValue = preservedPropertiesEnabledCount > 0;
            if (EditorGUILayout.Toggle("", bPrevValue, GUILayout.MaxWidth(15)) != bPrevValue)
            {
                for (int i = 0; i < preservedPropertiesCount; i++)
                {
                    PreservedPropertyHandler.GetInstance().GetPreservedPropertyData(i).bEnabled = preservedPropertiesEnabledCount == 0;
                }
            }

            /// Attachable Object Text
            string countText = AvatarCopierUtils.GetXOutOfTotalText(preservedPropertiesEnabledCount, preservedPropertiesCount);

            string labelText = "Preserved Properties/Fields  <color=" + CopierGUIStyles.LightTextHexColor + ">(" + countText + ")</color>";

            if (preservedPropertiesCount == 0)
            {
                labelText = "<color=" + CopierGUIStyles.LightTextHexColor + ">" + labelText + "</color>";
            }

            //bShowPreservedParameters = MergerGUIStyles.ClickableLabelFoldout(bShowPreservedParameters, new GUIContent(LabelText, TooltipText), MergerGUIStyles.GetMainCategoryFoldoutStyle());
            bShowPreservedParameters = EditorGUILayout.Foldout(bShowPreservedParameters, new GUIContent(labelText), CopierGUIStyles.GetMainCategoryFoldoutStyle());


            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            if (bShowPreservedParameters)
            {
                // Message
                //EditorGUILayout.HelpBox("This is an in-Alpha feature and not totally finished. See Help Tab for more information.", MessageType.Info);

                // Add Preserved Parameter Button
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("");
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Add Preserved Property/Field", "Feature Disabled ATM")))
                {
                    //MergerSettingsHandler.GetInstance().PreservedProperties.Add(new PreservedPropertyData());
                    //PreservedProps.Add(new PreservedPropertyData());
                    PreservedPropertyHandler.GetInstance().AddNewPreservedProperty();
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("");
                GUILayout.EndHorizontal();



                // Show Preserved Paramemters
                // Go in reverse order as it looks better
                for (int preservedPropIndex = preservedPropertiesCount-1; preservedPropIndex >= 0; preservedPropIndex--)
                {
                    PreservedPropertyData preservedPropData =  PreservedPropertyHandler.GetInstance().GetPreservedPropertyData(preservedPropIndex);

                    Color oBGColor = GUI.backgroundColor;
                    Color oColor = GUI.color;
                    if (!preservedPropData.bEnabled)
                    {
                        GUI.backgroundColor = CopierGUIStyles.DisabledHelpBoxColor;
                        GUI.color = CopierGUIStyles.DisabledLabelColor;
                    }

                    GUILayout.BeginVertical("", CopierGUIStyles.GetObjectCompOperationHelpBoxStyle());
                    GUI.backgroundColor = oBGColor;


                    EditorGUILayout.BeginHorizontal();

                    bool originalEnabled = preservedPropData.bEnabled;
                    preservedPropData.bEnabled = EditorGUILayout.ToggleLeft("", preservedPropData.bEnabled, GUILayout.MaxWidth(15));

                    /// Type Popup List
                    //if (preservedPropData.GetIsDefault() && preservedPropData.bEnabled != originalEnabled)
                    //{
                    //    int IndexOfDefaultProp = 0;

                    //    for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
                    //    {
                    //        if (preservedPropData == PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties()[i])
                    //        {
                    //            IndexOfDefaultProp = i;
                    //        }
                    //    }

                    //    PreservedPropertyHandler.GetInstance().SetDefaultPropertyEnabled(IndexOfDefaultProp, preservedPropData.bEnabled);
                    //}

                    // If this is a Renderer preserved prop and a Shared Materials is turned off, don't allow Materials to be copied
                    if (preservedPropData.bEnabled != originalEnabled)
                    {
                        if (typeof(Renderer).IsAssignableFrom(preservedPropData.GetPreservedComponentType()))
                        {
                            if (preservedPropData.GetPropertyName() == "sharedMaterial" || preservedPropData.GetPropertyName() == "sharedMaterials")
                            {
                                if (preservedPropData.bEnabled == false)
                                {
                                    CopierSettingsHandler.GetInstance().TrySetBoolDataField("bCopyMaterials", false);
                                }
                            }
                        }
                    }

                    // Don't allow edits to defaults
                    if (preservedPropData.GetIsDefault())
                    {
                        GUI.enabled = false;
                    }

                    int originalTypeIndex = preservedPropData.GetSelectedTypeIndex();
                    int inputTypeIndex = EditorGUILayout.Popup(preservedPropData.GetSelectedTypeIndex(), AllowedCopyTypeFriendlyNames);
                    if (originalTypeIndex != inputTypeIndex)
                    {
                        preservedPropData.SetSelectedTypeIndex(inputTypeIndex);
                        //PreservedParamater. = TypesListed[OriginalTypeIndex];
                        //PreservedParamater.SelectedPropertyIndex = 0;
                    }

                    GUILayout.FlexibleSpace();

                    /// Property Popup List
                    if (preservedPropData.GetPreservedComponentType() != null)
                    {
                        int originalPropertyTypeIndex = preservedPropData.GetSelectedPropertyIndex();
                        int inputPropertyTypeIndex = EditorGUILayout.Popup(preservedPropData.GetSelectedPropertyIndex(), PreservedPropertyHandler.GetInstance().GetPropertyArrayOfType(preservedPropData.GetPreservedComponentType()));
                        if (originalPropertyTypeIndex != inputPropertyTypeIndex)
                        {
                            preservedPropData.SetSelectedPropertyIndex(inputPropertyTypeIndex);
                        }
                    }

                    EditorGUILayout.EndHorizontal();



                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    // Default label (if Default)
                    if (preservedPropData.GetIsDefault())
                    {
                        GUILayout.Label(new GUIContent("<color=" + CopierGUIStyles.LightTextHexColor + ">Default</color>", "Tooltip"), CopierGUIStyles.GetLabelRichText());
                        GUILayout.FlexibleSpace();
                    }

                    // Remove Button
                    //if (GUILayout.Button(new GUIContent("Remove", "Tooltip"), GUILayout.Width(92)))
                    //{
                    //    for (int i = 0; i < PreservedProps.Count; i++)
                    //    {
                    //        PreservedProps.Remove(preservedPropData);
                    //    }
                    //}
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