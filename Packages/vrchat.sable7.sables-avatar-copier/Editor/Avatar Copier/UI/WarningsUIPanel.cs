#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Warnings;
using SablesTools.AvatarCopier.Errors;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class WarningsUIPanel
    {
        private static WarningsUIPanel _Instance = null;

        public static WarningsUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new WarningsUIPanel();
            }

            return _Instance;
        }

        private WarningsUIPanel()
        {

        }

        public Vector2 ScrollPosition = new Vector2();

        public void DrawWarningsPanel()
        {
            GUILayout.BeginVertical("Warnings", "window");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawWarnings();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void DrawWarnings()
        {
            for (int i = 0; i < WarningHandler.GetInstance().GetErrorCount(); i++)
            {
                AvatarCopierError currentError = WarningHandler.GetInstance().GetError(i);

                GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());

                /// Missing Skeleton Bone Name  - Should only be one Error of this type
                if (currentError.GetType() == typeof(AvatarCopierSourceMissingSkeletalReferenceError))
                {
                    AvatarCopierSourceMissingSkeletalReferenceError missingSkeleBoneError = currentError as AvatarCopierSourceMissingSkeletalReferenceError;

                    //GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(" Missing Skeleton Bones in Source Avatar");
                    //GUILayout.EndHorizontal();

                    EditorGUILayout.LabelField(" \tIn order to detect Attachable Objects, the original hierarchy of the Avatar's skeleton must all be present, not moved out of the avatar, moved under an Attachable Object, nor renamed.");

                    GUILayout.BeginVertical();
                    for (int j = 0; j < missingSkeleBoneError.GetMissingSkeletalBoneCount(); j++)
                    {
                        string missingName = missingSkeleBoneError.GetMissingSkeletalBoneName(j);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.indentLevel += 2;

                        GUILayout.Label("\t" + missingName);

                        EditorGUI.indentLevel -= 2;
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }

            for (int i = 0; i < WarningHandler.GetInstance().GetWarningsCount(); i++)
            {
                AvatarCopierWarning currentWarning = WarningHandler.GetInstance().GetWarning(i);

                if (currentWarning.bSurpressed)
                {
                    continue;
                }

                //if (CurrentWarning.bSurpressed)
                //{
                //    continue;
                //}

                GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());


                /// Duplicate Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierDuplicateObjectNamesWarning))
                {
                    AvatarCopierDuplicateObjectNamesWarning DuplicateWarning = currentWarning as AvatarCopierDuplicateObjectNamesWarning;

                    GUILayout.BeginHorizontal();
                    string s = "Destination";
                    if (DuplicateWarning.bIsFromSource)
                    {
                        s = "Source";
                    }
                    EditorGUILayout.LabelField(" Duplicate Names Detected in " + s + " Avatar:   " + DuplicateWarning.DuplicateName );
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();                
                    for (int j = 0; j < DuplicateWarning.GetDuplicateCount(); j++)
                    {
                        GameObject DupObject = DuplicateWarning.GetDuplicate(j);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.indentLevel += 2;
                        string EntryParentAndName = "\t" + DupObject.name;
                        if (DupObject.transform.parent != null)
                        {
                            EntryParentAndName = "\t" + DupObject.transform.parent.gameObject.name + " -> " + DupObject.name;
                        }

                        GUILayout.Label(EntryParentAndName);

                        // Select button
                        if (GUILayout.Button(new GUIContent("Select", "Selects the Game Object in the Destination Avatar"), GUILayout.Width(64)))
                        {
                            GameObject[] SelectedGameObjects = { DupObject };
                            Selection.objects = SelectedGameObjects;
                        }
                        EditorGUI.indentLevel -= 2;
                        EditorGUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }

                /// Missing Reference Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierMissingReferenceWarning))
                {
                    AvatarCopierMissingReferenceWarning refWarning = currentWarning as AvatarCopierMissingReferenceWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Missing Reference in " + refWarning.RegRefElement.RegisteredDataRef.CompOp.VirtualGameObjectRef.Name);

                    EditorGUILayout.LabelField(" Component Type: " + AvatarCopierUtils.TypeToFriendlyName(refWarning.RegRefElement.RegisteredDataRef.CompOp.ComponentType));

                    EditorGUILayout.LabelField(" Field: " + refWarning.RegRefElement.RegisteredDataRef.PropFieldName);

                    if (GUILayout.Button("Select Reference"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToRegisteredRefRef(refWarning.RegRefElement);
                    }

                    EditorGUILayout.EndVertical();
                }

                /// Not Expected Component Type Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierExpectedComponentMissingWarning))
                {
                    AvatarCopierExpectedComponentMissingWarning refWarning = currentWarning as AvatarCopierExpectedComponentMissingWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Expecting Component Type " + refWarning.RegRefElement.ReferenceType + " not found in Object reference ");

                    EditorGUILayout.LabelField(" Reference " + refWarning.RegRefElement.RegisteredDataRef.CompOp.VirtualGameObjectRef.Name + " does not contain the expected Component Type, " + refWarning.RegRefElement.ReferenceType);

                    EditorGUILayout.LabelField(" Field: " + refWarning.RegRefElement.RegisteredDataRef.PropFieldName);

                    if (GUILayout.Button("Select Reference"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToRegisteredRefRef(refWarning.RegRefElement);
                    }

                    EditorGUILayout.EndVertical();
                }

                /// Attachment Unattached Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierAttachableNoAttachmentPointWarning))
                {
                    AvatarCopierAttachableNoAttachmentPointWarning attachWarning = currentWarning as AvatarCopierAttachableNoAttachmentPointWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Attachable unattached in " + attachWarning.AttachmentOp.VirtualObject.Name);

                    EditorGUILayout.LabelField(" The Attachable Object is missing it's Attachment point. The Attachable Object will not be copied over potentially causing other issues in other Operations.");

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select Attachable"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToAttachmentOp(attachWarning.AttachmentOp);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }

                /// Attachment Disabled Reference Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierReferenceAttachableDisabledWarning))
                {
                    AvatarCopierReferenceAttachableDisabledWarning refWarning = currentWarning as AvatarCopierReferenceAttachableDisabledWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Attachable Reference Disabled in " + refWarning.RegRefElement.RegisteredDataRef.CompOp.VirtualGameObjectRef.Name);

                    EditorGUILayout.LabelField(" The Reference used is of an Attachable Object, " + refWarning.RegRefElement.VirtualReference.AttachmentOp.VirtualObject.Name + " that is currently disabled. The Reference will be set to <none> on copy");

                    EditorGUILayout.LabelField(" Field: " + refWarning.RegRefElement.RegisteredDataRef.PropFieldName);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select Attachable"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToAttachmentOp(refWarning.RegRefElement.VirtualReference.AttachmentOp);
                    }
                    if (GUILayout.Button("Select Reference"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToRegisteredRefRef(refWarning.RegRefElement);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }

                /// Attachment Unattached Reference Warnings
                if (currentWarning.GetType() == typeof(AvatarCopierReferenceAttachableUnattachedWarning))
                {
                    AvatarCopierReferenceAttachableUnattachedWarning refWarning = currentWarning as AvatarCopierReferenceAttachableUnattachedWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Attachable unattached in " + refWarning.RegRefElement.RegisteredDataRef.CompOp.VirtualGameObjectRef.Name);

                    EditorGUILayout.LabelField(" The Reference used is of an Attachable Object, " + refWarning.RegRefElement.VirtualReference.AttachmentOp.VirtualObject.Name + " that is currently Unattached. The Reference will be set to <none> on copy");

                    EditorGUILayout.LabelField(" Field: " + refWarning.RegRefElement.RegisteredDataRef.PropFieldName);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select Attachable"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToAttachmentOp(refWarning.RegRefElement.VirtualReference.AttachmentOp);
                    }
                    if (GUILayout.Button("Select Reference"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToRegisteredRefRef(refWarning.RegRefElement);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }

                /// Unused Component Operations
                if (currentWarning.GetType() == typeof(AvatarCopierUnusedWarning))
                {
                    AvatarCopierReferenceAttachableDisabledWarning refWarning = currentWarning as AvatarCopierReferenceAttachableDisabledWarning;

                    Rect TestRect = EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(" Unused Component Operations (" + ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count + ")");

                    EditorGUILayout.LabelField(" There are (" + ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count + ") components that were not able to find an connection on the Destination Avatar");

                    if (GUILayout.Button("Go To Unused"))
                    {
                        CopyDetailsUIPanel.GetInstance().ScrollToUnused();
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }
        }
    }
}

#endif