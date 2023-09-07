#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public enum SelectionModeType
    {
        NONE,
        REGISTERED_REFERENCE,
        ATTACHABLE_ATTACHMENT_POINT,
        COPY_OPERATION_PARENT
    }

    public class MergeTreeUIPanel
    {
        private static MergeTreeUIPanel _Instance = null;

        public static MergeTreeUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new MergeTreeUIPanel();
            }

            return _Instance;
        }

        private MergeTreeUIPanel()
        {

        }

        public Vector2 ScrollPosition = new Vector2();
        protected bool _awaitingRepaint = false;
        protected bool _bScrollToSelection = false;

        //protected bool bInSelectionMode = false;
        protected SelectionModeType SelectionMode = SelectionModeType.NONE;

        protected RegisteredReferenceElement SelectedRegisteredRef = null;

        protected AttachmentOperation SelectedAttachmentOperation = null;

        protected OverridingComponentOperation SelectedOverridingCompOpForParent = null;

        protected System.Type ExpectedSelectionComponentType = null;

        public SelectionModeType GetSelectionMode()
        {
            return SelectionMode;
        }

        public RegisteredReferenceElement GetSelectedRegisteredRef()
        {
            return SelectedRegisteredRef;
        }

        public AttachmentOperation GetSelectedAttachmentOperation()
        {
            return SelectedAttachmentOperation;
        }

        public ComponentOperation GetSelectedCompOp()
        {
            return SelectedOverridingCompOpForParent;
        }

        public void ScrollToSelected()
        {
            _bScrollToSelection = true;
            _awaitingRepaint = true;
        }

        public void SetRegisteredReferenceSelectionMode(RegisteredReferenceElement inRegisteredRef)
        {
            ClearSelectedParameters();

            if (inRegisteredRef == null)
            {
                return;
            }

            SelectionMode = SelectionModeType.REGISTERED_REFERENCE;

            SelectedRegisteredRef = inRegisteredRef;
            ExpectedSelectionComponentType = SelectedRegisteredRef.ReferenceType;

            if (IsUnselectableByRegisteredRef(MergeTreeHandler.GetInstance().SelectedVirtualGameObject))
            {
                MergeTreeHandler.GetInstance().SelectedVirtualGameObject = null;
            }
        }

        public void SetAttachableAttachmentPointSelectionMode(AttachmentOperation inAttachmentOperation)
        {
            ClearSelectedParameters();

            if (inAttachmentOperation == null)
            {
                return;
            }

            SelectionMode = SelectionModeType.ATTACHABLE_ATTACHMENT_POINT;

            SelectedAttachmentOperation = inAttachmentOperation;
            ExpectedSelectionComponentType = typeof(Transform);
        }

        public void SetCompOpParentSelectionMode(OverridingComponentOperation inOverridingCompOp)
        {
            ClearSelectedParameters();

            if (inOverridingCompOp == null)
            {
                return;
            }

            SelectionMode = SelectionModeType.COPY_OPERATION_PARENT;

            SelectedOverridingCompOpForParent = inOverridingCompOp;
            ExpectedSelectionComponentType = typeof(Transform);

            if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject != null && IsUnselectableByCompOpParentSetting(MergeTreeHandler.GetInstance().SelectedVirtualGameObject))
            {
                MergeTreeHandler.GetInstance().SelectedVirtualGameObject = null;
            }
        }

        public void ClearSelectedParameters()
        {
            SelectionMode = SelectionModeType.NONE;

            SelectedRegisteredRef = null;
            SelectedAttachmentOperation = null;
            SelectedOverridingCompOpForParent = null;
            ExpectedSelectionComponentType = null;
        }

        public bool IsUnselectableByRegisteredRef(VirtualGameObject VirtualObj)
        {
            // Check to see if this Virtual Object can be selected in selection mode. True if VirtualObject has a Comp Operation that has the same type thats required for selection
            bool bUnselectable = false;
            if (VirtualObj != null && ExpectedSelectionComponentType != typeof(GameObject) && ExpectedSelectionComponentType != typeof(Transform))
            {
                bUnselectable = true;

                // Check Pre-Existing Component Operations
                for (int i = 0; i < VirtualObj.GetPreExistingCount(); i++)
                {
                    if (VirtualObj.GetPreExisting(i).ComponentType == ExpectedSelectionComponentType)
                    {
                        bUnselectable = false;
                        break;
                    }
                }

                // Check Overriding Component Operations
                if (bUnselectable == true)
                {
                    for (int i = 0; i < VirtualObj.GetOverridingCount(); i++)
                    {
                        if (VirtualObj.GetOverriding(i).ComponentType == ExpectedSelectionComponentType)
                        {
                            bUnselectable = false;
                            break;
                        }
                    }
                }
            }

            return bUnselectable;
        }

        public bool IsUnselectableByCompOpParentSetting(VirtualGameObject virtualObj)
        {
            return !virtualObj.CanAcceptOverridingCompOp(SelectedOverridingCompOpForParent);
        }

        public bool IsUnselectableAttachableAttachmentPoint(VirtualGameObject virtualObj)
        {
            if (virtualObj == SelectedAttachmentOperation.VirtualObject || virtualObj.bIsAttachable)
            {
                return true;
            }

            return false;
        }

        public void DrawMergeTreePanel()
        {
            GUILayout.BeginVertical("Merge Tree", "window");

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            if (MergeTreeHandler.GetInstance().VirtualTreeRoot != null)
            {
                DrawTree_R(MergeTreeHandler.GetInstance().VirtualTreeRoot);
            }


            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            if (SelectionMode != SelectionModeType.NONE)
            {
                // Reference Selection Block
                GUILayout.BeginVertical("Selecting Ref", "window", GUILayout.MaxHeight(80), GUILayout.MinHeight(80));

                switch (SelectionMode)
                {
                    case SelectionModeType.REGISTERED_REFERENCE:
                        DrawRegisteredReferenceSelectionBox();
                        break;
                    case SelectionModeType.ATTACHABLE_ATTACHMENT_POINT:
                        DrawAttachableAttachmentPointSelectionBox();
                        break;
                    case SelectionModeType.COPY_OPERATION_PARENT:
                        DrawCompOperationParentSelectionBox();
                        break;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }


            // Currently Selected information block
            if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject != null)
            {
                GUILayout.BeginVertical(MergeTreeHandler.GetInstance().SelectedVirtualGameObject.Name, "window", GUILayout.MaxHeight(200), GUILayout.MinHeight(200));

                // Name Label
                //EditorGUILayout.LabelField(MergeTreeHandler.GetInstance().SelectedVirtualGameObject.Name);

                // Tags
                // Virtual Tag
                Color OBackgroundColor = GUI.backgroundColor;
                if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.bIsRoot)
                {
                    GUI.backgroundColor = new Color(0, 0, 200);
                    GUILayout.Label(new GUIContent(" Root ", "Tooltip"), GUI.skin.GetStyle("HelpBox"));
                    GUILayout.FlexibleSpace();
                }
                if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.bIsAttachable)
                {
                    GUI.backgroundColor = new Color(200,0,160);
                    EditorGUILayout.LabelField(new GUIContent(" Attachable ", "Tooltip"), GUI.skin.GetStyle("HelpBox"));
                    GUILayout.FlexibleSpace();
                }
                /*if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.MatchedData == null)
                {
                    GUI.backgroundColor = new Color(0, 225, 20);
                    EditorGUILayout.LabelField(new GUIContent(" New ", "Tooltip"), GUI.skin.GetStyle("HelpBox"));
                    GUILayout.FlexibleSpace();
                }*/


                GUI.backgroundColor = OBackgroundColor;

                // Buttons Horizontal
                EditorGUILayout.BeginHorizontal();

                // Select Destination Game Object
                if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedDestination == null)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button(new GUIContent("Destination", "Tooltip"), EditorStyles.miniButton) && MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedDestination != null)
                {
                    GameObject[] SelectedGameObjects = { MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedDestination };
                    Selection.objects = SelectedGameObjects;
                }
                GUI.enabled = true;

                // Select Source Game Object
                if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedSource == null)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button(new GUIContent("Source", "Tooltip"), EditorStyles.miniButton) && MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedSource != null)
                {
                    GameObject[] SelectedGameObjects = { MergeTreeHandler.GetInstance().SelectedVirtualGameObject.LinkedSource };
                    Selection.objects = SelectedGameObjects;
                }
                GUI.enabled = true;

                // Ending Button Horizontal
                EditorGUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
            else if (SelectionMode != SelectionModeType.NONE) // Show a "Select Something" when something needs selecting.
            {
                GUILayout.BeginVertical("<none>", "window", GUILayout.MaxHeight(200), GUILayout.MinHeight(200));

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            // End Registered Ref Selection Mode if in it
            if (_bScrollToSelection)
            {
                if (_awaitingRepaint)
                {
                    AvatarCopierWindow.GetInstance().Repaint();
                    _awaitingRepaint = false;
                }
                else
                {
                    _bScrollToSelection = false;
                }
            }
        }

        protected void DrawRegisteredReferenceSelectionBox()
        {
            if (SelectionMode != SelectionModeType.REGISTERED_REFERENCE)
            {
                return;
            }

            EditorGUILayout.LabelField(SelectedRegisteredRef.RegisteredDataRef.PropFieldName);

            EditorGUILayout.LabelField(AvatarCopierUtils.TypeToFriendlyName(SelectedRegisteredRef.ReferenceType));


            // Select Button
            // Check to see if the selected VirtualObject contains the components of the right type
            bool bContainsCorrectComponentType = false;

            if (ExpectedSelectionComponentType == typeof(Transform) || MergeTreeHandler.GetInstance().SelectedVirtualGameObject == null)
            {
                bContainsCorrectComponentType = true;
            }
            else
            {
                if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject != null)
                {
                    GUI.enabled = false;

                    for (int i = 0; i < MergeTreeHandler.GetInstance().SelectedVirtualGameObject.GetPreExistingCount(); i++)
                    {
                        if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.GetPreExisting(i).ComponentType == ExpectedSelectionComponentType)
                        {
                            bContainsCorrectComponentType = true;
                        }
                    }

                    if (bContainsCorrectComponentType == false)
                    {
                        for (int i = 0; i < MergeTreeHandler.GetInstance().SelectedVirtualGameObject.GetOverridingCount(); i++)
                        {
                            if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject.GetOverriding(i).ComponentType == ExpectedSelectionComponentType)
                            {
                                bContainsCorrectComponentType = true;
                            }
                        }
                    }
                }
            }


            if (bContainsCorrectComponentType)
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button(new GUIContent("Select"), EditorStyles.miniButton) && bContainsCorrectComponentType)
            {
                SelectedRegisteredRef.VirtualReference = MergeTreeHandler.GetInstance().SelectedVirtualGameObject;

                ComponentOperationHandler.GetInstance().RegisterForRefRefresh();

                ClearSelectedParameters();
            }
            GUI.enabled = true;
        }

        protected void DrawAttachableAttachmentPointSelectionBox()
        {
            if (SelectionMode != SelectionModeType.ATTACHABLE_ATTACHMENT_POINT)
            {
                return;
            }

            EditorGUILayout.LabelField(SelectedAttachmentOperation.SourceAttachableObject.name);

            EditorGUILayout.LabelField(AvatarCopierUtils.TypeToFriendlyName(typeof(Transform)));

            if (GUILayout.Button(new GUIContent("Select"), EditorStyles.miniButton))
            {
                SelectedAttachmentOperation.SetAttachmentPoint(MergeTreeHandler.GetInstance().SelectedVirtualGameObject);

                MergeTreeHandler.GetInstance().SelectedVirtualGameObject.bIsOpenInMergeTree = true;

                MergeTreeHandler.GetInstance().UpdateAttachables();

                ComponentOperationHandler.GetInstance().RegisterForRefRefresh();

                ClearSelectedParameters();

                MergeTreeHandler.GetInstance().ResetOrderedVirtualObjects();
            }
            GUI.enabled = true;
        }

        protected void DrawCompOperationParentSelectionBox()
        {
            if (SelectionMode != SelectionModeType.COPY_OPERATION_PARENT)
            {
                return;
            }

            EditorGUILayout.LabelField("Component Operation");

            EditorGUILayout.LabelField(AvatarCopierUtils.TypeToFriendlyName(typeof(Transform)));

            if (GUILayout.Button(new GUIContent("Select"), EditorStyles.miniButton))
            {
                //SelectedAttachmentOperation.Get = MergeTreeHandler.GetInstance().SelectedVirtualGameObject;
                //Debug.Log("RegisteredRef Set! ^^");

                ComponentOperationHandler.GetInstance().ChangeCompOpVirtualObjectParent(SelectedOverridingCompOpForParent, MergeTreeHandler.GetInstance().SelectedVirtualGameObject, false);
                ComponentOperationHandler.GetInstance().RefreshCompOps();

                ComponentOperationHandler.GetInstance().RegisterForRefRefresh();

                ClearSelectedParameters();
            }
            GUI.enabled = true;
        }

        protected void DrawTree_R(VirtualGameObject currentVirtualGameObject, int depth = 0)
        {
            if (currentVirtualGameObject == null)
            {
                return;
            }

            if (currentVirtualGameObject.bIsAttachable && (!currentVirtualGameObject.AttachmentOp.IsFullyEnabled()))
            {
                return;
            }

            // Check to see if this Virtual Object can be selected in selection mode. True if VirtualObject has a Comp Operation that has the same type thats required for selection
            bool bUnselectable = false;
            if (SelectionMode == SelectionModeType.REGISTERED_REFERENCE)
            {
                bUnselectable = IsUnselectableByRegisteredRef(currentVirtualGameObject);
            }

            if (SelectionMode == SelectionModeType.COPY_OPERATION_PARENT)
            {
                bUnselectable = IsUnselectableByCompOpParentSetting(currentVirtualGameObject);
            }

            if (SelectionMode == SelectionModeType.ATTACHABLE_ATTACHMENT_POINT)
            {
                bUnselectable = IsUnselectableAttachableAttachmentPoint(currentVirtualGameObject);
            }

            // Begin Horizontal - Color background if selected
            Rect horizontalRect = new Rect();
            if (MergeTreeHandler.GetInstance().SelectedVirtualGameObject == currentVirtualGameObject)
            {
                horizontalRect = EditorGUILayout.BeginHorizontal(CopierGUIStyles.GetBoxMarginlessStyle());
                if (_bScrollToSelection)
                {
                    ScrollPosition = new Vector2(horizontalRect.x, horizontalRect.y + DrawUtils.ScrollToPositionOffset);
                }
            }
            else
            {
                horizontalRect = EditorGUILayout.BeginHorizontal(CopierGUIStyles.GetHighlightableBoxMarginlessStyle());
            }


            // Get Virtual Object Name string with Rich Text identifieres
            string labelText = "";
            if (bUnselectable)
            {
                if (currentVirtualGameObject.bIsAttachable)
                {
                    labelText = "<color=" + CopierGUIStyles.UnselectedLightBlueAttachableTextHexColor + ">";
                }
                else
                {
                    labelText = "<color=" + CopierGUIStyles.LightTextHexColor + ">";
                }
            }
            else if (currentVirtualGameObject.bIsAttachable)
            {                
                labelText = "<color=" + CopierGUIStyles.LightBlueAttachableTextHexColor + ">";
            }
            labelText += currentVirtualGameObject.Name;
            if (bUnselectable || currentVirtualGameObject.bIsAttachable)
            {
                labelText += "</color>";
            }


            // Draw Line
            bool bLabelClicked = false;

            Rect indentedRect = EditorGUILayout.GetControlRect();

            // Indent based on dpeth
            indentedRect.x += depth * 15.0f;
            
            // Make room for foldout
            //if (CurrentVirtualGameObject.GetChildCount() > 0)
            //{
                indentedRect.x += 16;
            //}

            if (currentVirtualGameObject.bIsTopLevelAttachable)
            {
                bLabelClicked = DrawUtils.DrawButtonWithSymbols(indentedRect, labelText, CopierGUIStyles.GetLabelRichText(), "Attachable");
            }
            else
            {
                bLabelClicked = DrawUtils.DrawButtonWithSymbols(indentedRect, labelText, CopierGUIStyles.GetLabelRichText(), "Game Object");
            }

            // Foldout
            if (currentVirtualGameObject.GetChildCount() > 0)
            {
                Rect carrotRect = horizontalRect;
                carrotRect.x += depth * 15.0f;
                currentVirtualGameObject.bIsOpenInMergeTree = EditorGUI.Foldout(carrotRect, currentVirtualGameObject.bIsOpenInMergeTree, new GUIContent(""), CopierGUIStyles.GetOnlyCarrotFoldoutStyle());
            }

            EditorGUILayout.EndHorizontal();

            // On Name Clicked
            if (bLabelClicked && !bUnselectable)
            {
                MergeTreeHandler.GetInstance().SetSelectedVirtualObj(currentVirtualGameObject, false, true);
            }

            // Draw Childern
            if (currentVirtualGameObject.bIsOpenInMergeTree)
            {
                //EditorGUI.indentLevel++;
                depth++;
                for (int i = 0; i < currentVirtualGameObject.GetChildCount(); i++)
                {
                    DrawTree_R(currentVirtualGameObject.GetChild(i), depth);
                }
                depth--;
                //EditorGUI.indentLevel--;
            }
        }
    }
}

#endif