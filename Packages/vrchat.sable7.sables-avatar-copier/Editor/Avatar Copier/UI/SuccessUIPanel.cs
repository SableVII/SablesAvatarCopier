#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using SablesTools.AvatarCopier.Handlers;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class SuccessUIPanel
    {
        private static SuccessUIPanel _Instance = null;

        public static SuccessUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new SuccessUIPanel();
            }

            return _Instance;
        }

        private SuccessUIPanel()
        {

        }

        public Vector2 ScrollPosition = new Vector2();

        public void DrawSuccessPanel()
        {
            GUILayout.BeginVertical("Success", "window");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawSuccess();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void DrawSuccess()
        {
            /// Named
            if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone"))
            {
                EditorGUILayout.LabelField("Sucessfully created " + AvatarCopierExecutor.GetInstance().ClonedName + " from " + AvatarCopierExecutor.GetInstance().CopiedDestinationAvatarName + " and " + AvatarCopierExecutor.GetInstance().CopiedSourceAvatarName);
            }
            else
            {
                EditorGUILayout.LabelField("Sucessfully merged " + AvatarCopierExecutor.GetInstance().CopiedSourceAvatarName + " into " + AvatarCopierExecutor.GetInstance().CopiedDestinationAvatarName);
            }


            /// Warnings
            if (WarningHandler.GetInstance().GetWarningsCount() > 0)  // Fix ME!!
            {
                Rect r = EditorGUILayout.BeginHorizontal();
                GUI.DrawTexture(new Rect(r.x, r.y, DrawUtils.SmallIconSymbolSize, DrawUtils.SmallIconSymbolSize), AvatarCopierUtils.GetIconTexture("Warning"), ScaleMode.ScaleToFit);
                EditorGUILayout.LabelField("      Completed with " + WarningHandler.GetInstance().GetWarningsCount() + " Warnings");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Take care to double check " + (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone") ? "new " : " merged") + "Avatar for any potential issues that could break existing functionality");
            }
            else
            {
                EditorGUILayout.LabelField("Completed with zero detectable issues!");
            }

            /// Newly Added Component Count
            if (AvatarCopierExecutor.GetInstance().NewComponentsTransfered > 0)
            {
                EditorGUILayout.LabelField(AvatarCopierExecutor.GetInstance().NewComponentsTransfered + " New Components have been added!");
            }

            /// Pre-existing Components Replaced
            if (AvatarCopierExecutor.GetInstance().ComponentsReplaced > 0)
            {
                EditorGUILayout.LabelField(AvatarCopierExecutor.GetInstance().ComponentsReplaced + " Pre-existing Components have been repalced!");
            }

            /// Attachables attached
            if (AvatarCopierExecutor.GetInstance().AttachablesAttached > 0)
            {
                EditorGUILayout.LabelField(AvatarCopierExecutor.GetInstance().AttachablesAttached + " Attachables attached!");
            }

            /// Scales Adjusted
            if (AvatarCopierExecutor.GetInstance().AdjustedScales > 0)
            {
                EditorGUILayout.LabelField(AvatarCopierExecutor.GetInstance().AdjustedScales + " Game Object Scales adjusted!");
            }

            /// Enabled Statuses Changed
            if (AvatarCopierExecutor.GetInstance().EnabledStatusesChanged > 0)
            {
                EditorGUILayout.LabelField(AvatarCopierExecutor.GetInstance().EnabledStatusesChanged + " Game Object Enabled Status changed!");
            }

            /// Button to exit
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Return"))
            {
                CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}

#endif