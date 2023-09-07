#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;
using SablesTools.AvatarCopier.Errors;
using SablesTools.AvatarCopier.Warnings;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class CopyDetailsUIPanel : BaseUIPanel
    {
        private static CopyDetailsUIPanel _Instance = null;

        public static CopyDetailsUIPanel GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new CopyDetailsUIPanel();
            }

            return _Instance;
        }

        private CopyDetailsUIPanel()
        {

        }

        public Vector2 ScrollPosition = new Vector2();

        public bool bShowAttachmentOps = false;
        public bool bShowScaleOps = false;
        public bool bShowMaterialOps = false;
        public bool bShowEnabledDisabledOps = false;
        public bool bShowComponentOps = false;
        public bool bShowMiscOps = false;
        public bool bShowUnusedCompOps = false;
        public bool bShowPreExistingCompOps = false;
        public bool bShowOverridingCompOps = true;

        protected System.Type[] _AllowedCopyTypesArray = null;

        protected RegisteredReferenceElement _scrollingToRegisteredRef = null;
        protected AttachmentOperation _scrollingToAttachmentOp = null;
        protected ComponentOperation _scrollingToCompOp = null;
        protected bool _scrollingToUnused = false;
        protected bool _awaitingRepaint = false;
        protected bool _awaitingRepaint2 = false; // Good gosh, Idk why I need to do it twice on setting a ComponentOperation's VirtualParent, but it works x.x

        protected List<AvatarCopierWarning> Warnings = new List<AvatarCopierWarning>();
        protected List<AvatarCopierError> Errors = new List<AvatarCopierError>();

        public void DrawMergerDetailsPanel()
        {
            GUILayout.BeginVertical("Copy Information", "window");

            //ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle(), GUILayout.MaxHeight(290));
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle());

            GUILayout.Space(5);

            DrawAttachableOperations();

            DrawCompOperations();

            DrawMaterialOperations();

            DrawScaleOperations();

            DrawEnabledDisabledOperations();

            DrawMiscOperations();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            // End Registered Ref Selection Mode if in it
            if (_scrollingToAttachmentOp != null || _scrollingToRegisteredRef != null || _scrollingToCompOp != null || _scrollingToUnused)
            {
                if (_awaitingRepaint)
                {
                    EditorUI.AvatarCopierWindow.GetInstance().Repaint();
                    _awaitingRepaint = false;
                }
                else if (_awaitingRepaint2)
                {
                    EditorUI.AvatarCopierWindow.GetInstance().Repaint();
                    _awaitingRepaint2 = false;
                }
                else
                {
                    _scrollingToAttachmentOp = null;
                    _scrollingToRegisteredRef = null;
                    _scrollingToCompOp = null;
                    _scrollingToUnused = false;
                }
            }
        }

        protected System.Type[] GetAllowedSystemTypeArray()
        {
            if (_AllowedCopyTypesArray == null)
            {
                _AllowedCopyTypesArray = AvatarCopierUtils.GetAllowedTypesAsArray();
            }

            return _AllowedCopyTypesArray;
        }

        public void ScrollToRegisteredRefRef(RegisteredReferenceElement inRegRefElement)
        {
            _scrollingToRegisteredRef = inRegRefElement;
            CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            _awaitingRepaint = true;
            //_bIsSelecting = true;
        }

        public void ScrollToAttachmentOp(AttachmentOperation inAttachmentOp)
        {
            _scrollingToAttachmentOp = inAttachmentOp;
            CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            _awaitingRepaint = true;
            //_bIsSelecting = true;
        }

        public void ScrollToCompOperation(ComponentOperation inCompOp)
        {
            _scrollingToCompOp = inCompOp;
            CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            _awaitingRepaint = true;
            _awaitingRepaint2 = true;
        }

        public void ScrollToUnused()
        {
            _scrollingToUnused = true;
            CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            _awaitingRepaint = true;
            //_bIsSelecting = true;
        }

        /// Draw Attachable Oprations
        public void DrawAttachableOperations()
        {
            int attachableEnabledCount = AttachmentOperationHandler.GetInstance().GetEnabledAttachementOperationsCount();
            int attachableTotalCount = AttachmentOperationHandler.GetInstance().AttachmentOperations.Count;

            PushColorAndBackgroundColor(attachableEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            Rect attachablesRect = EditorGUILayout.BeginVertical(CopierGUIStyles.GetMainMenuHelpBoxStyle());
            //GUI.backgroundColor = OriginalBackgroundColor; // To not get too dark on Background colors
            PopBackgroundColor();

            string attachableLabel = "Attachable Objects";
            if (attachableTotalCount > 0)
            {
                attachableLabel += " <color=" + CopierGUIStyles.LightTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(attachableEnabledCount, attachableTotalCount) + ")</color>";

                bool bPrevState = attachableEnabledCount != 0;
                DrawUtils.DrawToggleFoldoutWithSymbols(ref bPrevState, ref bShowAttachmentOps, EditorGUILayout.GetControlRect(), attachableLabel, AttachmentOperationHandler.GetInstance().AttachablesHaveWarnings() ? "Warning" : "", "Attachable" );

                if (bPrevState != (attachableEnabledCount != 0))
                {
                    foreach (AttachmentOperation AttachmentOp in AttachmentOperationHandler.GetInstance().AttachmentOperations)
                    {
                        AttachmentOp.bUserSetEnabled = bPrevState;
                    }

                    ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                }
            }
            else
            {
                EditorGUILayout.LabelField(attachableLabel);
            }

            // If Selecting an Attachment
            if (_scrollingToAttachmentOp != null)
            {
                bShowAttachmentOps = true;
            }

            // Each of the Attachment Operations
            if (bShowAttachmentOps)
            {
                ///
                foreach (AttachmentOperation AttachmentOp in AttachmentOperationHandler.GetInstance().AttachmentOperations)
                {
                    PushColorAndBackgroundColor(!AttachmentOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    Rect AttachmentUIRect = EditorGUILayout.BeginVertical(CopierGUIStyles.GetMarginedHelpBoxStyle());
                    //GUI.backgroundColor = oBGColor;
                    PopBackgroundColor();

                    // Set Scroll Position here if Selecting this Attachable.
                    if (_scrollingToAttachmentOp != null)
                    {
                        ScrollPosition = new Vector2(0, AttachmentUIRect.y + DrawUtils.ScrollToPositionOffset);
                    }


                    EditorGUILayout.BeginHorizontal(new GUIStyle());
                    bool PreviousState = AttachmentOp.bUserSetEnabled;
                    AttachmentOp.bUserSetEnabled = EditorGUILayout.ToggleLeft(" " + AttachmentOp.SourceAttachableObject.name, AttachmentOp.bUserSetEnabled, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                    if (PreviousState != AttachmentOp.bUserSetEnabled)
                    {
                        ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                    }


                    GUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();

                    float prevLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 200;

                    GUILayout.Space(25.0f);

                    EditorGUILayout.LabelField("Attachment Point");

                    EditorGUIUtility.labelWidth = prevLabelWidth;

                    Rect ControlRect = EditorGUILayout.GetControlRect();

                    string attachmentPointName = "<none>";
                    if (AttachmentOp.AttachmentPoint != null)
                    {
                        attachmentPointName = AttachmentOp.AttachmentPoint.Name;
                    }
                    else
                    {
                        attachmentPointName = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Missing : " + AttachmentOp.SourceAttachableObject.transform.parent.name + "></color>";
                    }

                    bool bIsSelected = false;
                    if (MergeTreeUIPanel.GetInstance().GetSelectionMode() == SelectionModeType.ATTACHABLE_ATTACHMENT_POINT && MergeTreeUIPanel.GetInstance().GetSelectedAttachmentOperation() == AttachmentOp)
                    {
                        bIsSelected = true;
                    }

                    string buttonLabelText = attachmentPointName;
                    if (bIsSelected)
                    {
                        buttonLabelText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + ">" + buttonLabelText + "</color>";
                    }

                    // Attachment Name
                    if (GUI.Button(ControlRect, new GUIContent(buttonLabelText), CopierGUIStyles.GetVirtualTextFieldButtonStyle()))
                    {
                        if (AttachmentOp.AttachmentPoint != null)
                        {
                            MergeTreeHandler.GetInstance().SetSelectedVirtualObj(AttachmentOp.AttachmentPoint, true, true);
                            MergeTreeUIPanel.GetInstance().ScrollToSelected();
                        }
                        else
                        {
                            GameObject[] SelectedGameObjects = { AttachmentOp.SourceAttachableObject.transform.parent.gameObject };
                            Selection.objects = SelectedGameObjects;
                        }
                    }

                    if (AttachmentOp.AttachmentPoint == null)
                    {
                        //GUI.Label(GUILayoutUtility.GetLastRect(), "<color=" + MergerGUIStyles.SelectionOverlayTextHexColor + "><I>--Missing--</I></color>", MergerGUIStyles.GetSelectedLabelStyle());
                        GUI.DrawTexture(new Rect(GUILayoutUtility.GetLastRect().x - DrawUtils.WarningSymbolXOffset - 7.0f, GUILayoutUtility.GetLastRect().y, DrawUtils.SmallIconSymbolSize, DrawUtils.SmallIconSymbolSize), AvatarCopierUtils.GetIconTexture("Warning"), ScaleMode.ScaleToFit);
                    }


                    string changeButtonLabel = "Change";
                    if (bIsSelected)
                    {
                        GUI.Label(ControlRect, "<color=" + CopierGUIStyles.SelectionOverlayTextHexColor + "><I>--Selecting--</I></color>", CopierGUIStyles.GetSelectedLabelStyle());
                        changeButtonLabel = "Cancel";
                    }

                    // Source Object Ref


                    if (GUILayout.Button(new GUIContent(changeButtonLabel, "Selects the Game Object in the Source Avatar this Attachable Object is going to copy over."), GUILayout.Width(65)))
                    {
                        if (bIsSelected)
                        {
                            MergeTreeUIPanel.GetInstance().ClearSelectedParameters();
                        }
                        else
                        {
                            MergeTreeUIPanel.GetInstance().SetAttachableAttachmentPointSelectionMode(AttachmentOp);
                        }
                    }

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.EndHorizontal();

                    PopColor();

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            PopColor();
        }


        /// Draw Component Operations
        public void DrawCompOperations()
        {
            // Including Unused (?) - should we?
            int componentCompOpTotalCount = ComponentOperationHandler.GetInstance().GetCompOpTotal();
            int componentCompOpEnabledCount = ComponentOperationHandler.GetInstance().GetEnabledCompOpTotal();

            PushColorAndBackgroundColor(ComponentOperationHandler.GetInstance().OverridingCompOperations.Count == 0 && ComponentOperationHandler.GetInstance().PreExistingCompOperations.Count == 0 && ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);


            Rect r = EditorGUILayout.BeginVertical(CopierGUIStyles.GetMainMenuHelpBoxStyle());
            PopBackgroundColor();


            // Check for Warning in Pre-Existing
            bool preExistingHasWarning = false;
            foreach (PreExistingComponentOperation preExistingCompOp in ComponentOperationHandler.GetInstance().PreExistingCompOperations)
            {
                if (preExistingCompOp.IsFullyEnabled() && preExistingCompOp.HasUnSuppressedWarning())
                {
                    preExistingHasWarning = true;
                    break;
                }
            }

            // Check for Warning in Overriding
            bool overridingHasWarning = false;
            foreach (OverridingComponentOperation overridingCompOp in ComponentOperationHandler.GetInstance().OverridingCompOperations)
            {
                if (overridingCompOp.IsFullyEnabled() && overridingCompOp.HasUnSuppressedWarning())
                {
                    overridingHasWarning = true;
                    break;
                }
            }

            int overridingCompOpTotalCount = ComponentOperationHandler.GetInstance().OverridingCompOperations.Count;
            int overridingCompOpEnabledCount = ComponentOperationHandler.GetInstance().GetEnabledOverridingCount();
            int preExistingCompOpTotalCount = ComponentOperationHandler.GetInstance().PreExistingCompOperations.Count;
            int preExistingCompOpEnabledCount = ComponentOperationHandler.GetInstance().GetEnabledPreExistingCount();


            string compOpLabel = "Component Operations";
            if (ComponentOperationHandler.GetInstance().OverridingCompOperations.Count != 0 || ComponentOperationHandler.GetInstance().PreExistingCompOperations.Count != 0 || ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count != 0)
            {
                compOpLabel += " <color=" + CopierGUIStyles.LightTextHexColor + ">";
                compOpLabel += "(Overriding: " + AvatarCopierUtils.GetXOutOfTotalText(overridingCompOpEnabledCount, overridingCompOpTotalCount) + ")";
                compOpLabel += " (Pre-Existing: " + AvatarCopierUtils.GetXOutOfTotalText(preExistingCompOpEnabledCount, preExistingCompOpTotalCount) + ")";
                compOpLabel += "</color>";
            }

            DrawUtils.DrawTextWithSymbols(EditorGUILayout.GetControlRect(), compOpLabel, (preExistingHasWarning || overridingHasWarning) ? "Warning" : "");

            // Set Scroll Position here if Selecting this Attachable.
            if (_scrollingToRegisteredRef != null || _scrollingToCompOp != null || _scrollingToUnused)
            {
                bShowComponentOps = true;
            }

            // Options for Comp Operations
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            /// Hide operations if unused
            if (CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType)
            {
                CopierSettingsHandler.GetInstance().bHideUnusedCompOpTypes = EditorGUILayout.ToggleLeft(new GUIContent("Hide Unused Types", "Tooltip"), CopierSettingsHandler.GetInstance().bHideUnusedCompOpTypes);
            }

            /// Organize by Type/GameObject toggle
            bool bPrevByType = CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType;
            CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType = EditorGUILayout.ToggleLeft(new GUIContent("Organize by Type", "Tooltip"), CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType);
            if (bPrevByType != CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType)
            {
                ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;


            // Draw Unused Comp Operations
            if (ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count != 0)
            {
                DrawUnusedCompOperations();
            }
            else
            {
                bShowUnusedCompOps = false;
            }


            /// Draw Pre-Existing
            Rect preExistingCategoryRect = EditorGUILayout.BeginVertical(CopierGUIStyles.GetMarginedHelpBoxStyle());

            int preExistingNonReplacedTotalCount = ComponentOperationHandler.GetInstance().GetNonReplacedTotalCount();
            int preExistingReplacedTotalCount = ComponentOperationHandler.GetInstance().GetReplacedTotalCount();
            if (preExistingNonReplacedTotalCount > 0 || preExistingReplacedTotalCount > 0)
            {
                string preExistingLabelText = "Pre-Existing" + "<color=" + CopierGUIStyles.LightTextHexColor + ">";

                if (preExistingNonReplacedTotalCount > 0)
                {
                    preExistingLabelText += " (Pre-Exists: " + AvatarCopierUtils.GetXOutOfTotalText(ComponentOperationHandler.GetInstance().GetNonReplacedEnabledCount(), preExistingNonReplacedTotalCount) + ")";
                }

            
                if (preExistingReplacedTotalCount > 0)
                {
                    preExistingLabelText += " (Replaced: " + AvatarCopierUtils.GetXOutOfTotalText(ComponentOperationHandler.GetInstance().GetReplacedEnabledCount(), preExistingReplacedTotalCount) + ")";
                }

                //if (preExistingHasWarning)
                //{
                //    preExistingLabelText = "      " + preExistingLabelText;
                //}

                preExistingLabelText += "</color>";

                bShowPreExistingCompOps = DrawUtils.DrawFoldoutWithSymbols(bShowPreExistingCompOps, EditorGUILayout.GetControlRect(), preExistingLabelText, preExistingHasWarning ? "Warning" : "");
            }
            else
            {
                EditorGUILayout.LabelField("Pre-Existing");
            }

            // Open if scrolling to Pre-Existing Component Operation
            if (_scrollingToCompOp as PreExistingComponentOperation != null || (_scrollingToRegisteredRef != null && _scrollingToRegisteredRef.RegisteredDataRef.CompOp as PreExistingComponentOperation != null))
            {
                bShowPreExistingCompOps = true;
            }

            if (bShowPreExistingCompOps)
            {
                // Draw by Type
                if (CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType)
                {
                    DrawPreExistingCompOpsByType();
                }

                // Draw by Object
                if (CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType == false)
                {
                    for (int i = 0; i < MergeTreeHandler.GetInstance().GetOrderedVirtualObjectsCount(); i++)
                    {
                        DrawPreExisitingCompOpsByObject(MergeTreeHandler.GetInstance().GetOrderedVirtualObject(i));
                    }
                }
            }

            EditorGUILayout.EndVertical();


            /// Draw Overridables
            Rect overridingCategoryRect = EditorGUILayout.BeginVertical(CopierGUIStyles.GetMarginedHelpBoxStyle());

            int overridingReplacingTotalCount = ComponentOperationHandler.GetInstance().GetReplacingTotalCount();
            int overridingNewTotalCount = ComponentOperationHandler.GetInstance().GetNewTotalCount();

            if (overridingReplacingTotalCount > 0 || overridingNewTotalCount > 0)
            {
                string overridingLabelText = "Overriding<color=" + CopierGUIStyles.LightTextHexColor + ">";

                if (overridingNewTotalCount > 0)
                {
                    overridingLabelText += " (New: " + AvatarCopierUtils.GetXOutOfTotalText(ComponentOperationHandler.GetInstance().GetNewEnabledCount(), overridingNewTotalCount) + ")";
                }

                if (overridingReplacingTotalCount > 0)
                {
                    overridingLabelText += " (Replacing: " + AvatarCopierUtils.GetXOutOfTotalText(ComponentOperationHandler.GetInstance().GetReplacedEnabledCount(), overridingReplacingTotalCount) + ")";
                }

                //if (overridingHasWarning)
                //{
                //    overridingLabelText = "      " + overridingLabelText;
                //}

                overridingLabelText += "</color>";

                //if (overridingHasWarning)
                //{
                //    GUI.DrawTexture(new Rect(overridingCategoryRect.x + MergerDrawGlobals.WarningSymbolXOffset + 23.0f, overridingCategoryRect.y + 3.0f, MergerDrawGlobals.SmallIconSymbolSize, MergerDrawGlobals.SmallIconSymbolSize), MergerGlobals.GetIconTexture("Warning"), ScaleMode.ScaleToFit);
                //}
                bool bToggle = ComponentOperationHandler.GetInstance().GetEnabledOverridingCount() != 0;
                DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShowOverridingCompOps, EditorGUILayout.GetControlRect(), overridingLabelText, overridingHasWarning ? "Warning" : "");

                if (bToggle != (ComponentOperationHandler.GetInstance().GetEnabledOverridingCount() != 0))
                {
                    foreach (OverridingComponentOperation overridingCompOp in ComponentOperationHandler.GetInstance().OverridingCompOperations)
                    {
                        overridingCompOp.bUserSetEnabled = bToggle;
                    }

                    ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Overriding");
            }


            // Open if scrolling to Overriding Component Operation
            if (_scrollingToCompOp as OverridingComponentOperation != null || (_scrollingToRegisteredRef != null && _scrollingToRegisteredRef.RegisteredDataRef.CompOp as OverridingComponentOperation != null))
            {
                bShowOverridingCompOps = true;
            }

            // Open if one of the Component Operation contains the scrolling to Reference

            if (bShowOverridingCompOps)
            {
                // Draw by Type
                if (CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType)
                {
                    DrawOverridingCompOpsByType();
                }

                // Draw by Object
                if (CopierSettingsHandler.GetInstance().bOrganizeCompOpsByType == false)
                {
                    for (int i = 0; i < MergeTreeHandler.GetInstance().GetOrderedVirtualObjectsCount(); i++)
                    {
                        DrawOverridingCompOpsByObject(MergeTreeHandler.GetInstance().GetOrderedVirtualObject(i));
                    }
                }                    
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            PopColor();
        }

        public void DrawPreExisitingCompOpsByObject(VirtualGameObject currentVirtualObj)
        {
            int preExistingTotalCount = currentVirtualObj.GetPreExistingCount();
            int preExistingEnabledTotalCount = currentVirtualObj.GetEnabledPreExistingCount();

            if (preExistingTotalCount > 0)
            {
                // Grabbing original background color
                bool bEnabledCondition = preExistingTotalCount != 0 && preExistingEnabledTotalCount > 0;

                PushColorAndBackgroundColor(!bEnabledCondition, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                EditorGUILayout.BeginVertical(CopierGUIStyles.GetMarginedHelpBoxStyle());
                PopBackgroundColor();

                Rect objectRect = EditorGUILayout.BeginHorizontal();

                // Select Button - Button here to be over the foldout toggle
                if (GUI.Button(new Rect(objectRect.x + 300.0f, objectRect.y, 65.0f, objectRect.height), "Select"))
                {
                    MergeTreeHandler.GetInstance().SetSelectedVirtualObj(currentVirtualObj, false, true);
                }

                /// GameObject Foldout
                string preExistingCompOpCountText = AvatarCopierUtils.GetXOutOfTotalText(preExistingEnabledTotalCount, preExistingTotalCount);
                string compLabelText = currentVirtualObj.Name + " <color=" + CopierGUIStyles.LightTextHexColor + ">(" + preExistingCompOpCountText + ")</color>";

                if (preExistingTotalCount == 0 || preExistingEnabledTotalCount == 0)
                {
                    compLabelText = "<color=" + CopierGUIStyles.LightTextHexColor + ">" + compLabelText + "</color>";
                }

                currentVirtualObj.bShowPreExistingCompOps = DrawUtils.DrawFoldoutWithSymbols(currentVirtualObj.bShowPreExistingCompOps, EditorGUILayout.GetControlRect(), compLabelText, currentVirtualObj.PreExistingHasWarning() ? "Warning" : "");

                // Open up if selecting a registered ref is in one of the CompOps
                if (_scrollingToRegisteredRef != null && currentVirtualObj.bShowPreExistingCompOps == false)
                {
                    // Try Open if _ScrollingRef's CompOp is PreExisting
                    for (int i = 0; i < preExistingTotalCount; i++)
                    {
                        if (_scrollingToRegisteredRef.RegisteredDataRef.CompOp == currentVirtualObj.GetPreExisting(i))
                        {
                            currentVirtualObj.bShowPreExistingCompOps = true;
                            break;
                        }
                    }
                }

                // Open if Scrolling to CompOp
                if (_scrollingToCompOp != null && currentVirtualObj.bShowPreExistingCompOps == false)
                {
                    // Try Open if _ScrollingCompOp's CompOp is PreExisting
                    for (int i = 0; i < preExistingTotalCount; i++)
                    {
                        if (_scrollingToCompOp == currentVirtualObj.GetPreExisting(i))
                        {
                            currentVirtualObj.bShowPreExistingCompOps = true;
                            break;
                        }
                    }
                }


                EditorGUILayout.EndHorizontal();

                // Show and Draw Component Operations
                if (currentVirtualObj.bShowPreExistingCompOps)
                {
                    // Draw Pre-Existing Component Operations
                    for (int i = 0; i < preExistingTotalCount; i++)
                    {
                        DrawCompOperation(currentVirtualObj.GetPreExisting(i));
                    }
                }

                PopColor();

                EditorGUILayout.EndVertical();
            }
        }

        public void DrawOverridingCompOpsByObject(VirtualGameObject currentVirtualObj)
        {
            int overridableEnabledCount = currentVirtualObj.GetEnabledOverridingCount();
            int overridableTotalCount = currentVirtualObj.GetOverridingCount();

            if (overridableTotalCount > 0)
            {
                // Grabbing original background color
                bool bEnabledCondition = overridableTotalCount != 0 && overridableEnabledCount > 0;

                PushColorAndBackgroundColor(!bEnabledCondition, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                PopBackgroundColor();

                Rect objectRect = EditorGUILayout.BeginHorizontal();

                string compLabelText = currentVirtualObj.Name + " <color=" + CopierGUIStyles.LightTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(overridableEnabledCount, overridableTotalCount) + ")</color>";

                bool bShow = currentVirtualObj.bShowOverridingCompOps;
                bool bToggle = overridableEnabledCount != 0;
                DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShow, EditorGUILayout.GetControlRect(), compLabelText, currentVirtualObj.OverridingHasWarning() ? "Warning" : "");

                currentVirtualObj.bShowOverridingCompOps = bShow;

                // Toggle Enabled Status of all Comps
                if (bToggle != (overridableEnabledCount != 0))
                {
                    for (int i = 0; i < overridableTotalCount; i++)
                    {
                        currentVirtualObj.GetOverriding(i).bUserSetEnabled = bToggle;
                    }

                    ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                }

                // Open up if selecting a registered ref is in one of the CompOps
                if (_scrollingToRegisteredRef != null && currentVirtualObj.bShowOverridingCompOps == false)
                {
                    // Try Open if _ScrollingRef's Component Operation is Overriding
                    if (currentVirtualObj.bShowOverridingCompOps == false)
                    {
                        for (int i = 0; i < overridableTotalCount; i++)
                        {
                            if (_scrollingToRegisteredRef.RegisteredDataRef.CompOp == currentVirtualObj.GetOverriding(i))
                            {
                                currentVirtualObj.bShowOverridingCompOps = true;
                                break;
                            }
                        }
                    }
                }

                // Open if Scrolling to Comp Op
                if (_scrollingToCompOp != null && currentVirtualObj.bShowOverridingCompOps == false)
                {
                    // Try Open if _ScrollingCompOp's Component Operation is Overriding
                    if (currentVirtualObj.bShowOverridingCompOps == false)
                    {
                        for (int i = 0; i < overridableTotalCount; i++)
                        {
                            if (_scrollingToCompOp == currentVirtualObj.GetOverriding(i))
                            {
                                currentVirtualObj.bShowOverridingCompOps = true;
                                break;
                            }
                        }
                    }
                }

                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                // Show and Draw Component Operations
                if (currentVirtualObj.bShowOverridingCompOps)
                {
                    for (int i = 0; i < overridableTotalCount; i++)
                    {
                        DrawCompOperation(currentVirtualObj.GetOverriding(i));
                    }
                }

                PopColor();

                EditorGUILayout.EndVertical();
            }
        }

        public void DrawPreExistingCompOpsByType()
        {
            // Drawing each by their GameObjects
            foreach (System.Type copyType in GetAllowedSystemTypeArray())
            {
                // Checking to make sure Type exists or not
                if (ComponentOperationHandler.GetInstance().TypeCompOpLists.ContainsKey(copyType) == false || ComponentOperationHandler.GetInstance().TypeCompOpLists[copyType].GetPreExistingCount() == 0)
                {
                    if (CopierSettingsHandler.GetInstance().bHideUnusedCompOpTypes)
                    {
                        continue;
                    }

                    // Grabbing original background color
                    PushColorAndBackgroundColor(true, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();

                    GUILayout.BeginHorizontal();

                    //EditorGUILayout.LabelField("  " + MergerGlobals.TypeToFriendlyName(copyType));
                    DrawUtils.DrawTextWithSymbols(EditorGUILayout.GetControlRect(), AvatarCopierUtils.TypeToFriendlyName(copyType), AvatarCopierUtils.TypeToFriendlyName(copyType));

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    PopColor();
                    continue;
                }

                TypeComponentOperationList typeCompOpList = ComponentOperationHandler.GetInstance().TypeCompOpLists[copyType];

                int preExistingTotalCount = typeCompOpList.GetPreExistingCount();
                int enabledPreExistingCount = typeCompOpList.GetEnabledPreExistingCount();

                // Grabbing original background color
                PushColorAndBackgroundColor(enabledPreExistingCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                PopBackgroundColor();

                EditorGUILayout.BeginHorizontal();

                /// GameObject Foldout
                string copyComponentOperationOfTypeCountText = AvatarCopierUtils.GetXOutOfTotalText(enabledPreExistingCount, preExistingTotalCount);
                string compLabelText = AvatarCopierUtils.TypeToFriendlyName(copyType) + "  <color=" + CopierGUIStyles.LightTextHexColor + ">(" + copyComponentOperationOfTypeCountText + ")</color>";

                bool bHasWarning = typeCompOpList.PreExistingHasWarning();
                typeCompOpList.bPreExistingOpenInUI = DrawUtils.DrawFoldoutWithSymbols(typeCompOpList.bPreExistingOpenInUI, EditorGUILayout.GetControlRect(), compLabelText, bHasWarning ? "Warning" : "", AvatarCopierUtils.TypeToFriendlyName(copyType));

                EditorGUILayout.EndHorizontal();

                // Open up if selecting a registered ref's Comp Op is of the same type
                if (_scrollingToRegisteredRef != null)
                {
                    if (_scrollingToRegisteredRef.RegisteredDataRef.CompOp.ComponentType == typeCompOpList.ComponentType)
                    {
                        typeCompOpList.bPreExistingOpenInUI = true;
                    }
                }

                // Open if Scrolling to Comp Op
                if (_scrollingToCompOp != null)
                {
                    if (_scrollingToCompOp.ComponentType == typeCompOpList.ComponentType)
                    {
                        typeCompOpList.bPreExistingOpenInUI = true;
                    }
                }

                if (typeCompOpList.bPreExistingOpenInUI)
                {
                    for (int i = 0; i < typeCompOpList.GetPreExistingCount(); i++)
                    {
                        DrawCompOperation(typeCompOpList.GetPreExisting(i));
                    }
                }

                PopColor();

                EditorGUILayout.EndVertical();
            }
        }

        public void DrawOverridingCompOpsByType()
        {
            // Drawing each by their GameObjects
            foreach (System.Type copyType in GetAllowedSystemTypeArray())
            {
                // Checking to make sure Type exists or not
                if (ComponentOperationHandler.GetInstance().TypeCompOpLists.ContainsKey(copyType) == false || ComponentOperationHandler.GetInstance().TypeCompOpLists[copyType].GetOverridingCount() == 0)
                {
                    if (CopierSettingsHandler.GetInstance().bHideUnusedCompOpTypes)
                    {
                        continue;
                    }

                    // Grabbing original background color
                    PushColorAndBackgroundColor(true, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();

                    GUILayout.BeginHorizontal();

                    //EditorGUILayout.LabelField("  " + MergerGlobals.TypeToFriendlyName(copyType));

                    DrawUtils.DrawTextWithSymbols(EditorGUILayout.GetControlRect(), AvatarCopierUtils.TypeToFriendlyName(copyType), AvatarCopierUtils.TypeToFriendlyName(copyType));

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    PopColor();
                    continue;
                }

                TypeComponentOperationList typeCompOpList = ComponentOperationHandler.GetInstance().TypeCompOpLists[copyType];

                int overridableEnabledCount = typeCompOpList.GetEnabledOverridingCount();
                int overridableTotalCount = typeCompOpList.GetOverridingCount();

                //// Grabbing original background color
                PushColorAndBackgroundColor(overridableEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                PopBackgroundColor();

                Rect r = EditorGUILayout.BeginHorizontal();

                string compLabelText = AvatarCopierUtils.TypeToFriendlyName(copyType) + "  <color=" + CopierGUIStyles.LightTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(overridableEnabledCount, overridableTotalCount) + ")</color>";

                bool bShow = typeCompOpList.bOverridingOpenInUI;
                bool bToggle = overridableEnabledCount != 0;
                DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShow, EditorGUILayout.GetControlRect(), compLabelText, typeCompOpList.OverridingHasWarning() ? "Warning" : "", AvatarCopierUtils.TypeToFriendlyName(copyType));

                typeCompOpList.bOverridingOpenInUI = bShow;

                if (bToggle != (overridableEnabledCount != 0))
                {
                    for (int i = 0; i < overridableTotalCount; i++)
                    {
                        typeCompOpList.GetOverriding(i).bUserSetEnabled = bToggle;
                    }

                    ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                }

                EditorGUILayout.EndHorizontal();

                // Open up if selecting a registered ref's Comp Op is of the same type
                if (_scrollingToRegisteredRef != null)
                {
                    if (_scrollingToRegisteredRef.RegisteredDataRef.CompOp.ComponentType == typeCompOpList.ComponentType)
                    {
                        typeCompOpList.bOverridingOpenInUI = true;
                    }
                }

                // Open if Scrolling to Comp Op
                if (_scrollingToCompOp != null)
                {
                    if (_scrollingToCompOp.ComponentType == typeCompOpList.ComponentType)
                    {
                        typeCompOpList.bOverridingOpenInUI = true;
                    }
                }

                if (typeCompOpList.bOverridingOpenInUI)
                {
                    for (int i = 0; i < typeCompOpList.GetOverridingCount(); i++)
                    {
                        DrawCompOperation(typeCompOpList.GetOverriding(i));
                    }
                }

                PopColor();

                EditorGUILayout.EndVertical();
            }
        }

        public void DrawUnusedCompOperations()
        {
            if (ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count <= 0)
            {
                return;
            }
            int TotalCount = ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count;

            EditorGUILayout.BeginVertical(CopierGUIStyles.GetMarginedHelpBoxStyle());
            PopBackgroundColor();

            Rect TitleRect = EditorGUILayout.BeginHorizontal();

            /// GameObject Foldout
            string unusedLabelText = "Unused Component Operations  <color=" + CopierGUIStyles.LightTextHexColor + ">(" + ComponentOperationHandler.GetInstance().UnusedComponentOperations.Count + ")</color>";
            //string CompTooltipText = "Tooltip add me plz";

            bShowUnusedCompOps = DrawUtils.DrawFoldoutWithSymbols(bShowUnusedCompOps, EditorGUILayout.GetControlRect(), unusedLabelText, "Warning");

            EditorGUILayout.EndHorizontal();

            if (_scrollingToUnused)
            {
                ScrollPosition = new Vector2(0, TitleRect.y + DrawUtils.ScrollToPositionOffset);
                bShowUnusedCompOps = true;
            }

            if (bShowUnusedCompOps)
            {
                // Drawing each Comp Operation that is Unused
                foreach (ComponentOperation compOp in ComponentOperationHandler.GetInstance().UnusedComponentOperations)
                {
                    DrawCompOperation(compOp);
                }
            }

            //PopColor();

            EditorGUILayout.EndVertical();
        }

        /// Draw a single Component Comp Operation
        public void DrawCompOperation(ComponentOperation compOp)
        {
            PreExistingComponentOperation preExistingCompOp = compOp as PreExistingComponentOperation;
            OverridingComponentOperation overridingCompOp = compOp as OverridingComponentOperation;

            bool bHasWarning = compOp.HasUnSuppressedWarning();

            bool bAttachableDisabled = false;
            if (compOp.VirtualGameObjectRef != null)
            {
                if (compOp.VirtualGameObjectRef.bIsAttachable && compOp.VirtualGameObjectRef.AttachmentOp.IsFullyEnabled() == false)
                {
                    bAttachableDisabled = true;
                }
            }

            // Grabbing original background color
            PushColorAndBackgroundColor(!compOp.bUserSetEnabled || bAttachableDisabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            EditorGUILayout.BeginVertical(CopierGUIStyles.GetObjectCompOperationHelpBoxStyle());
            PopBackgroundColor();

            EditorGUILayout.BeginHorizontal(new GUIStyle());
            //Rect r = EditorGUILayout.BeginHorizontal(new GUIStyle());

            //// Draw Component Operation Label
            string compOpLabel = " " + AvatarCopierUtils.TypeToFriendlyName(compOp.OriginComponent.GetType());

            // Draw Toggle and Name Line
            if (overridingCompOp != null)
            {
                compOp.bUserSetEnabled = DrawUtils.DrawToggleWithSymbols(compOp.bUserSetEnabled, EditorGUILayout.GetControlRect(), compOpLabel, AvatarCopierUtils.TypeToFriendlyName(compOp.ComponentType));
            }
            else
            {
                DrawUtils.DrawTextWithSymbols(EditorGUILayout.GetControlRect(), compOpLabel, preExistingCompOp.bIsAttachable ? "Attachable" : "", AvatarCopierUtils.TypeToFriendlyName(compOp.ComponentType));
            }

            if (overridingCompOp != null)
            {
                if (compOp.OriginalVirtualGameObjectRef != compOp.VirtualGameObjectRef)
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.MovedTagHexTextColor + ">Moved</color>", CopierGUIStyles.GetBoxStyle());
                }

                if (overridingCompOp.IsUnused)
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.UnusedTagHexTextColor + ">Unused</color>", CopierGUIStyles.GetBoxStyle());
                }
                else if (overridingCompOp.IsReplacing)
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.OverridingTagHexTextColor + ">Overriding</color>", CopierGUIStyles.GetBoxStyle());
                }
                else
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.NewComponentTagHexTextColor + ">New</color>", CopierGUIStyles.GetBoxStyle());
                }
            }
            else
            {
                if (preExistingCompOp.IsBeingOverriden)
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.ReplacedTagHexTextColor + ">Replaced</color>", CopierGUIStyles.GetBoxStyle());
                }
                else
                {
                    EditorGUILayout.LabelField("<color=" + CopierGUIStyles.PreExistsTagHexTextColor + ">Pre-Exists</color>", CopierGUIStyles.GetBoxStyle());
                }
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal(); // End Object Name Horizontal from the above if statements


            // Draw Virtual Object Parent Field
            Rect compOpRect = EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.BeginHorizontal();

            if (_scrollingToCompOp == compOp)
            {
                ScrollPosition = new Vector2(0, compOpRect.y + DrawUtils.ScrollToPositionOffset);
            }

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;

            GUILayout.Space(25.0f);

            EditorGUILayout.LabelField("Parent Object");
            EditorGUIUtility.labelWidth = prevLabelWidth;

            Rect controlRect = EditorGUILayout.GetControlRect();

            string virtualObjectName = "";
            if (compOp.VirtualGameObjectRef != null)
            {
                virtualObjectName = compOp.VirtualGameObjectRef.Name;
            }
            else // If unused
            {
                virtualObjectName = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Missing : " + compOp.OriginComponent.transform.name + "></color>";
            }

            // Is Selected
            bool bIsSelected = false;
            if (MergeTreeUIPanel.GetInstance().GetSelectionMode() == SelectionModeType.COPY_OPERATION_PARENT && MergeTreeUIPanel.GetInstance().GetSelectedCompOp() == compOp)
            {
                bIsSelected = true;
            }

            string buttonLabelText = virtualObjectName;
            if (bIsSelected)
            {
                buttonLabelText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + ">" + buttonLabelText + "</color>";
            }

            if (GUI.Button(controlRect, buttonLabelText, CopierGUIStyles.GetVirtualTextFieldButtonStyle()))
            {
                if (compOp.VirtualGameObjectRef != null)
                {
                    MergeTreeHandler.GetInstance().SetSelectedVirtualObj(compOp.VirtualGameObjectRef, false, true);
                    MergeTreeUIPanel.GetInstance().ScrollToSelected();
                }
                else //if unused select the expected location in source
                {
                    GameObject[] SelectedGameObjects = { compOp.OriginComponent.gameObject };
                    Selection.objects = SelectedGameObjects;
                }
            }

            if (compOp as PreExistingComponentOperation != null)
            {
                GUI.enabled = false;
            }

            // Parent Ref Button
            if (bIsSelected)
            {
                GUI.Label(GUILayoutUtility.GetLastRect(), "<color=" + CopierGUIStyles.SelectionOverlayTextHexColor + "><I>--Selecting--</I></color>", CopierGUIStyles.GetSelectedLabelStyle());

                if (GUILayout.Button(new GUIContent("Cancel"), GUILayout.Width(65)))
                {
                    MergeTreeUIPanel.GetInstance().ClearSelectedParameters();
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Change"), GUILayout.Width(65)))
                {
                    MergeTreeUIPanel.GetInstance().SetCompOpParentSelectionMode(overridingCompOp);
                }
            }

            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();


            // Draw used References
            DrawUsedReferences(compOp);


            //GUI.color = oColor;
            PopColor();
            EditorGUILayout.EndVertical();
        }

        public void DrawUsedReferences(ComponentOperation compOp)
        {
            /// Registered Property/Fields
            if (compOp.RegisteredRefCollection.RegisteredReferences.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();


                /// Used References Foldout
                EditorGUILayout.BeginVertical(CopierGUIStyles.GetRegisteredPropertiesVerticalStyle());

                // Draw Used References Foldout
                Rect unusedRect = EditorGUILayout.BeginHorizontal();
                bool bHasWarning = compOp.RegisteredRefCollection.HasWarning();

                string foldoutLabelText = "Used References <color=" + CopierGUIStyles.LightTextHexColor + ">(" + compOp.RegisteredRefCollection.RegisteredReferences.Count + ")</color>";

                compOp.RegisteredRefCollection.bOpenInUI = DrawUtils.DrawFoldoutWithSymbols(compOp.RegisteredRefCollection.bOpenInUI, EditorGUILayout.GetControlRect(), foldoutLabelText, bHasWarning ? "Warning" : "");

                EditorGUILayout.EndHorizontal();

                // Open up if selecting a registered ref's Comp Op is the same
                if (_scrollingToRegisteredRef != null)
                {
                    if (_scrollingToRegisteredRef.RegisteredDataRef.CompOp == compOp)
                    {
                        compOp.RegisteredRefCollection.bOpenInUI = true;
                    }
                }

                if (compOp.RegisteredRefCollection.bOpenInUI)
                {
                    foreach (RegisteredReference RefPropFieldData in compOp.RegisteredRefCollection.RegisteredReferences)
                    {
                        string RefPropFieldName = RefPropFieldData.PropFieldName;

                        for (int RefElementIndex = 0; RefElementIndex < RefPropFieldData.GetReferenceCount(); RefElementIndex++)
                        {
                            //asdf += 1;
                            Rect RegisteredRefRec = EditorGUILayout.BeginHorizontal();
                            RegisteredReferenceElement RefDataElement = RefPropFieldData.GetReferenceElement(RefElementIndex);

                            // If selecting a reference, set ScrollPosition to that of the Reg-Element
                            if (_scrollingToRegisteredRef == RefDataElement)
                            {
                                ScrollPosition = new Vector2(0, RegisteredRefRec.y + DrawUtils.ScrollToPositionOffset);
                            }


                            string CurrentLabelText = RefPropFieldName;

                            string ListExtention = "";
                            if (RefPropFieldData.GetIsList())
                            {
                                CurrentLabelText += " [" + RefElementIndex + "]";
                            }

                            if (CurrentLabelText.Length > 35 - ListExtention.Length)
                            {
                                CurrentLabelText = CurrentLabelText.Remove(31 - ListExtention.Length);
                                CurrentLabelText += "...";
                            }

                            CurrentLabelText += ListExtention + ":";
                            //EditorGUILayout.LabelField(CurrentLabelText);



                            string RefName = "<none>";

                            if (RefDataElement.RefWarning != null)
                            {                                
                                RefName = RefDataElement.ExpectedGameObject.name;
                            }
                            else
                            {
                                if (RefDataElement.bIsNotVirtualGameObjectType)
                                {
                                    if (RefDataElement.GameObjectReference != null)
                                    {
                                        RefName = RefDataElement.GameObjectReference.name;
                                    }
                                }
                                else
                                {
                                    if (RefDataElement.VirtualReference != null)
                                    {
                                        RefName = RefDataElement.VirtualReference.Name;
                                    }
                                }
                            }


                            if (RefName.Length > 25)
                            {
                                RefName = RefName.Remove(21);
                                RefName += "...";
                            }



                            //EditorGUILayout.BeginHorizontal();

                            GUILayout.Space(25.0f);

                            //ControlRect = new Rect(ControlRect.x + 30.0f, ControlRect.y, 210.0f, ControlRect.height);
                            float prevLabelWidth = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = 150.0f;
                            //EditorGUILayout.LabelField(new GUIContent(CurrentLabelText, RefPropFieldName + ListExtention));
                            DrawUtils.DrawTextWithSymbols(EditorGUILayout.GetControlRect(), CurrentLabelText, AvatarCopierUtils.TypeToFriendlyName(RefDataElement.ReferenceType));
                            EditorGUIUtility.labelWidth = prevLabelWidth;

                            //ControlRect = new Rect(ControlRect.x + 230.0f, ControlRect.y, 200.0f, ControlRect.height);
                            //Rect ControlRect = EditorGUILayout.GetControlRect();

                            // Dim Button Text if selecting this RegRef or if this is a warning
                            bool bIsSelected = false;
                            if (MergeTreeUIPanel.GetInstance().GetSelectionMode() == SelectionModeType.REGISTERED_REFERENCE && MergeTreeUIPanel.GetInstance().GetSelectedRegisteredRef() == RefDataElement)
                            {
                                bIsSelected = true;
                            }

                            //bool bAttachmentDisabled = false;
                            //if (RefDataElement.RefWarning != null)
                            //{
                            //    bAttachmentDisabled = true;
                            //}

                            // Button Text
                            string buttonText = RefName;
                            if (bIsSelected && RefDataElement.RefWarning == null || RefName == "<none>") // Dim Button Text if selecting and there is no Warning
                            {
                                buttonText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + ">" + buttonText + "</color>";
                            }
                            else if (RefDataElement.RefWarning as AvatarCopierReferenceAttachableDisabledWarning != null)
                            {
                                    buttonText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Disabled : " + buttonText + "></color>";
                            }
                            else if (RefDataElement.RefWarning as AvatarCopierReferenceAttachableUnattachedWarning != null)
                            {
                                    buttonText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Unattached : " + buttonText + "></color>";
                            }
                            else if (RefDataElement.RefWarning as AvatarCopierMissingReferenceWarning != null)
                            {
                                buttonText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Missing : " + buttonText + "></color>";
                                //GUI.Label(GUILayoutUtility.GetLastRect(), "<color=" + MergerGUIStyles.SelectionOverlayTextHexColor + "><I>--Missing--</I></color>", MergerGUIStyles.GetSelectedLabelStyle());
                            }
                            else if (RefDataElement.RefWarning as AvatarCopierExpectedComponentMissingWarning != null)
                            {
                                buttonText = "<color=" + CopierGUIStyles.DimSelectionTextHexColor + "><Missing Comp : " + buttonText + "></color>";
                                //GUI.Label(GUILayoutUtility.GetLastRect(), "<color=" + MergerGUIStyles.SelectionOverlayTextHexColor + "><I>--Missing Component--</I></color>", MergerGUIStyles.GetSelectedLabelStyle());
                            }

                            // Draw button thingy
                            if (GUILayout.Button(new GUIContent(buttonText, RefName), CopierGUIStyles.GetVirtualTextFieldButtonStyle()))
                            {
                                if (RefDataElement.bFailedToMatch)
                                {
                                    GameObject[] SelectedGameObjects = { RefDataElement.ExpectedGameObject };
                                    Selection.objects = SelectedGameObjects;
                                }
                                else
                                {
                                    if (RefDataElement.bIsNotVirtualGameObjectType)
                                    {
                                        if (RefDataElement.GameObjectReference != null)
                                        {
                                            GameObject[] SelectedGameObjects = { RefDataElement.ExpectedGameObject };
                                            Selection.objects = SelectedGameObjects;
                                        }
                                    }
                                    else
                                    {
                                        // Don't select in the Merge Tree if there is no VirtualRef or if that VirtualReference is an Attachable that is Disabled
                                        if (RefDataElement.VirtualReference != null && !(RefDataElement.VirtualReference.bIsAttachable && RefDataElement.VirtualReference.AttachmentOp.IsFullyEnabled() == false))
                                        {
                                            MergeTreeHandler.GetInstance().SetSelectedVirtualObj(RefDataElement.VirtualReference, false, true);
                                            MergeTreeUIPanel.GetInstance().ScrollToSelected();
                                        }

                                        if (RefDataElement.RefWarning as AvatarCopierReferenceAttachableDisabledWarning != null)
                                        {
                                            ScrollToAttachmentOp(RefDataElement.VirtualReference.AttachmentOp);
                                        }
                                        else if (RefDataElement.RefWarning as AvatarCopierMissingReferenceWarning != null)
                                        {
                                            GameObject[] SelectedGameObjects = { RefDataElement.ExpectedGameObject };
                                            Selection.objects = SelectedGameObjects;
                                        }
                                        else if (RefDataElement.RefWarning as AvatarCopierExpectedComponentMissingWarning != null)
                                        {
                                            MergeTreeHandler.GetInstance().SetSelectedVirtualObj(RefDataElement.VirtualReference);
                                        }
                                    }
                                }
                            }

                            if (RefDataElement.RefWarning != null)
                            {
                                GUI.DrawTexture(new Rect(GUILayoutUtility.GetLastRect().x - DrawUtils.WarningSymbolXOffset - 7.0f, GUILayoutUtility.GetLastRect().y, DrawUtils.SmallIconSymbolSize, DrawUtils.SmallIconSymbolSize), AvatarCopierUtils.GetIconTexture("Warning"), ScaleMode.ScaleToFit);
                            }

                            // Draw --Selecting-- Text if selecting for this object, if not selecting and this Virtual is an Attachable that is disabled, show --Attachable Disabled--
                            if (bIsSelected)
                            {
                                GUI.Label(GUILayoutUtility.GetLastRect(), "<color=" + CopierGUIStyles.SelectionOverlayTextHexColor + "><I>--Selecting--</I></color>", CopierGUIStyles.GetSelectedLabelStyle());
                            }

                            //ControlRect = new Rect(ControlRect.x + 210.0f, ControlRect.y, 100.0f, ControlRect.height);

                            // Source Object Ref
                            if (bIsSelected)
                            {
                                if (GUILayout.Button(new GUIContent("Cancel"), GUILayout.Width(65)))
                                {
                                    MergeTreeUIPanel.GetInstance().ClearSelectedParameters();
                                }
                            }
                            else
                            {
                                if (GUILayout.Button(new GUIContent("Change"), GUILayout.Width(65)))
                                {
                                    MergeTreeUIPanel.GetInstance().SetRegisteredReferenceSelectionMode(RefDataElement);
                                }
                            }


                            GUILayout.FlexibleSpace();

                            //EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    //EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
        }

        /// Draw Material Operations
        public void DrawMaterialOperations()
        {
            int materialOperationEnabledCount = MaterialOperationHandler.GetInstance().GetEnabledMaterialOperationCount();
            int materialOperationTotalCount = MaterialOperationHandler.GetInstance().GetMaterialOperationCount();

            PushColorAndBackgroundColor(materialOperationEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            {
                PopBackgroundColor();

                string materialLabelText = "Material Operations";

                if (materialOperationTotalCount == 0)
                {
                    EditorGUILayout.LabelField(materialLabelText);
                    bShowMaterialOps = false;
                }
                else
                {
                    if (materialOperationTotalCount != 0)
                    {
                        materialLabelText += " <color=" + CopierGUIStyles.DimSelectionTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(materialOperationEnabledCount, materialOperationTotalCount) + ")</color>";
                    }

                    bool bToggle = materialOperationEnabledCount != 0;

                    DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShowMaterialOps, EditorGUILayout.GetControlRect(), materialLabelText);

                    if (bToggle != (materialOperationEnabledCount != 0))
                    {
                        for (int i = 0; i < materialOperationTotalCount; i++)
                        {
                            MaterialOperationHandler.GetInstance().GetMaterialOperation(i).bUserSetEnabled = bToggle;
                        }
                    }
                }

                // Each of the Material Operations
                if (bShowMaterialOps)
                {
                    // Options for Material Operations
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();

                    /// Default value for Smartish Material Copy
                    bool bPrevValue = CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseSmartMaterialCopy");
                    CopierSettingsHandler.GetInstance().TrySetDataField("bDefaultUseSmartMaterialCopy", EditorGUILayout.ToggleLeft(new GUIContent("Toggle Smart-ish Copy", "Smart-ish attempts to match materials from the Overriding Renderer by the material's name. This may or may not give better results than sorting by Index."), bPrevValue));
                    bool bNewValue = CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseSmartMaterialCopy");
                    if (bPrevValue != bNewValue)
                    {
                        for (int i = 0; i < MaterialOperationHandler.GetInstance().GetMaterialOperationCount(); i++)
                        {
                            MaterialOperationHandler.GetInstance().GetMaterialOperation(i).bSmartCopy = bNewValue;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;

                    /// Draw each MaterialOperation
                    for (int i = 0; i < materialOperationTotalCount; i++)
                    {
                        MaterialOperation materialOp = MaterialOperationHandler.GetInstance().GetMaterialOperation(i);

                        // Grabbing original background color
                        PushColorAndBackgroundColor(!materialOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);


                        //
                        GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                        {
                            //GUI.backgroundColor = oBGColor;
                            PopBackgroundColor();

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal(new GUIContent("", "Toggle this Material Operation from being performed."), new GUIStyle());
                                {
                                    bool PreviousState = materialOp.bUserSetEnabled;
                                    bool NextValue = EditorGUILayout.ToggleLeft(" " + materialOp.OverridingCompOp.VirtualGameObjectRef.Name, materialOp.IsFullyEnabled(), CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                                    if (PreviousState != NextValue)
                                    {
                                        materialOp.bUserSetEnabled = NextValue;
                                    }
                                }
                                GUILayout.EndHorizontal();

                                if (GUILayout.Button("Select"))
                                {
                                    MergeTreeHandler.GetInstance().SetSelectedVirtualObj(materialOp.OverridingCompOp.VirtualGameObjectRef, false, true);
                                }

                                if (GUILayout.Button("Renderer"))
                                {
                                    ScrollToCompOperation(materialOp.OverridingCompOp);
                                    //MergeTreeHandler.GetInstance().SetSelectedVirtualObj(materialOp.OverridingCompOp.VirtualGameObjectRef, false, true);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            // Material Type List
                            EditorGUI.indentLevel += 1;

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal(GUILayout.Width(400));
                                {
                                    EditorGUILayout.LabelField(new GUIContent("Smart-ish Copy", "Toggle useage of Smart-ish Material Copy method to copy Materials from the Overriding Renderer"), CopierGUIStyles.GetLabelRightAlignedRichText(), GUILayout.MaxWidth(200));

                                    //materialOp.SetScale(EditorGUILayout.Vector3Field("", materialOp.GetScale()));
                                    materialOp.bSmartCopy = EditorGUILayout.ToggleLeft("", materialOp.bSmartCopy);
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel -= 1;

                        PopColor();
                        //GUI.color = oColor;

                    }
                }
            }
            GUILayout.EndVertical();

            //GUI.color = OriginalColor;
            PopColor();
        }


        /// Draw Scale Operations
        public void DrawScaleOperations()
        {
            int scaleOperationEnabledCount = ScaleOperationHandler.GetInstance().GetEnabledScaledOperationCount();
            int scaleOperationTotalCount = ScaleOperationHandler.GetInstance().GetScaleOperationCount();

            PushColorAndBackgroundColor(scaleOperationEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            {
                PopBackgroundColor();

                string scaleLabelText = "Scale Operations";

                if (scaleOperationTotalCount == 0)
                {
                    EditorGUILayout.LabelField(scaleLabelText);
                    bShowScaleOps = false;
                }
                else
                {
                    if (scaleOperationTotalCount != 0)
                    {
                        scaleLabelText += " <color=" + CopierGUIStyles.DimSelectionTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(scaleOperationEnabledCount, scaleOperationTotalCount) + ")</color>";
                    }

                    bool bToggle = scaleOperationEnabledCount != 0;
                    //bool bNextState = DrawToggleFoldout("Scale Operations", "", "", ScaleOperationEnabledCount, ScaleOperationTotalCount, bPrevState, ref bShowScaleOperations);

                    DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShowScaleOps, EditorGUILayout.GetControlRect(), scaleLabelText);

                    if (bToggle != (scaleOperationEnabledCount != 0))
                    {
                        for (int i = 0; i < scaleOperationTotalCount; i++)
                        {
                            ScaleOperationHandler.GetInstance().GetScaleOperation(i).bUserSetEnabled = bToggle;
                        }
                    }
                }

                // Each of the Scale Operations
                if (bShowScaleOps)
                {
                    //// Options for Scale Operations
                    //EditorGUI.indentLevel++;
                    //EditorGUILayout.BeginHorizontal();

                    //GUILayout.FlexibleSpace();

                    ///// Default value for Smartish Material Copy
                    //float prevValue = CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon");
                    //CopierSettingsHandler.GetInstance().TrySetDataField("ScaleEpsilon", EditorGUILayout.DelayedFloatField(new GUIContent("Scale Epsilon", "The amount of difference in each element of a GameObject's scale that would create a Scale Operation."), prevValue));
                    //float newValue = CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon");
                    //if (prevValue != newValue)
                    //{
                    //    // Re-find Scale Operations
                    //    ScaleOperationHandler.GetInstance().Reset();
                    //    ScaleOperationHandler.GetInstance().CreateScaleOperations();
                    //}

                    //EditorGUILayout.Space(20);

                    //EditorGUILayout.EndHorizontal();
                    //EditorGUI.indentLevel--;

                    /// Draw each ScaleOperation
                    for (int i = 0; i < scaleOperationTotalCount; i++)
                    {
                        ScaleOperation scaleOp = ScaleOperationHandler.GetInstance().GetScaleOperation(i);

                        // Grabbing original background color
                        PushColorAndBackgroundColor(!scaleOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);


                        //
                        GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                        {
                            //GUI.backgroundColor = oBGColor;
                            PopBackgroundColor();

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal(new GUIContent("", "Toggle this Scale Operation from being performed."), new GUIStyle());
                                {
                                    bool PreviousState = scaleOp.bUserSetEnabled;
                                    bool NextValue = EditorGUILayout.ToggleLeft(" " + scaleOp.VirtualGameObj.Name, scaleOp.bUserSetEnabled, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                                    if (PreviousState != NextValue)
                                    {
                                        scaleOp.bUserSetEnabled = NextValue;
                                    }
                                }
                                GUILayout.EndHorizontal();

                                if (GUILayout.Button("Select"))
                                {
                                    MergeTreeHandler.GetInstance().SetSelectedVirtualObj(scaleOp.VirtualGameObj, false, true);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            // Scale Vector display
                            EditorGUI.indentLevel += 3;

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.BeginHorizontal(GUILayout.Width(400));
                                {
                                    EditorGUILayout.LabelField(new GUIContent("Scale", "Tooltip"), CopierGUIStyles.GetLabelRightAlignedRichText(), GUILayout.MaxWidth(100));

                                    scaleOp.SetScale(EditorGUILayout.Vector3Field("", scaleOp.GetScale()));
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                        }
                        EditorGUILayout.EndVertical();

                        EditorGUI.indentLevel -= 3;

                        PopColor();
                        //GUI.color = oColor;

                    }
                }
            }
            GUILayout.EndVertical();

            //GUI.color = OriginalColor;
            PopColor();
        }


        /// Draw Enabled/Disabled Operations
        public void DrawEnabledDisabledOperations()
        {
            int EnabledDisabledOperationEnabledCount = EnabledDisabledOperationHandler.GetInstance().GetEnabledEnabledDisabledOperationCount();
            int EnabledDisabledOperationTotalCount = EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperationCount();

            PushColorAndBackgroundColor(EnabledDisabledOperationEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            //GUI.backgroundColor = OriginalBackgroundColor; // To not get too dark on Background colors
            PopBackgroundColor();

            string enabledDisabledLabelText = "Enabled/Disabled Operations";

            // Toggle
            if (EnabledDisabledOperationTotalCount == 0)
            {
                EditorGUILayout.LabelField(enabledDisabledLabelText);
                bShowEnabledDisabledOps = false;
            }
            else
            {
                bool bToggle = EnabledDisabledOperationEnabledCount != 0;
                //bool bNextState = DrawToggleFoldout("Enabled/Disabled Operations", "", "", EnabledDisabledOperationEnabledCount, EnabledDisabledOperationTotalCount, bPrevState, ref bShowEnabledDisabledOperations);

                DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShowEnabledDisabledOps, EditorGUILayout.GetControlRect(), enabledDisabledLabelText);

                if (bToggle != (EnabledDisabledOperationEnabledCount != 0))
                {
                    for (int i = 0; i < EnabledDisabledOperationTotalCount; i++)
                    {
                        EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperation(i).bUserSetEnabled = bToggle;
                    }
                }
            }


            // Each of the Enabled/Disabled Operations
            if (bShowEnabledDisabledOps)
            {
                ///// Toggle All Button
                //EditorGUI.indentLevel++;
                //EditorGUILayout.BeginHorizontal();

                //if (GUILayout.Button(new GUIContent(EnabledDisabledOperationEnabledCount > 0 ? "Toggle All Off" : "Toggle All On", "Tooltip needed plz :3"), MergerGUIStyles.GetOperationToggleAllButtonStyle(), GUILayout.MaxWidth(104)))
                //{
                //    for (int i = 0; i < EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperationCount(); i++)
                //    {
                //        EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperation(i).bUserSetEnabled = EnabledDisabledOperationEnabledCount == 0;
                //    }
                //}

                //GUILayout.FlexibleSpace();
                //EditorGUILayout.EndHorizontal();
                //EditorGUI.indentLevel--;

                /// Draw Each Enabled/Disabled Operation
                for (int i = 0; i < EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperationCount(); i++)
                {
                    EnabledDisabledOperation EnabledDisabledOp = EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperation(i);

                    // Grabbing original background color
                    PushColorAndBackgroundColor(!EnabledDisabledOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);


                    GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();
                    //GUI.backgroundColor = oBGColor;

                    GUILayout.BeginHorizontal(new GUIContent("", "Toggle this Enabled/Disabled Operation from being performed."), new GUIStyle());
                    bool PreviousState = EnabledDisabledOp.bUserSetEnabled;
                    bool NextValue = EditorGUILayout.ToggleLeft(" " + EnabledDisabledOp.GetVirtualObject().Name, EnabledDisabledOp.bUserSetEnabled, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                    if (PreviousState != NextValue)
                    {
                        EnabledDisabledOp.bUserSetEnabled = NextValue;
                    }

                    /// Set Value Line
                    EditorGUILayout.LabelField(new GUIContent(EnabledDisabledOp.EnabledStatus ? "Enabled  " : "Disabled  ", "Tooltip"), CopierGUIStyles.GetLabelRightAlignedRichText(), GUILayout.MaxWidth(100));
                    EnabledDisabledOp.EnabledStatus = EditorGUILayout.Toggle("", EnabledDisabledOp.EnabledStatus);
                    GUILayout.EndHorizontal();

                    PopColor();

                    GUILayout.EndVertical();

                }
            }
            GUILayout.EndVertical();

            PopColor();
            //GUI.color = OriginalColor;
        }


        /// Draw Misc Operations
        public void DrawMiscOperations()
        {
            int MiscOperationEnabledCount = MiscOperationHandler.GetInstance().GetTotalMiscEnabledOperationCount();
            int MiscOperationTotalCount = MiscOperationHandler.GetInstance().GetTotalMiscOperationCount();

            PushColorAndBackgroundColor(MiscOperationEnabledCount == 0, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

            GUILayout.BeginVertical("", CopierGUIStyles.GetMainMenuHelpBoxStyle());
            PopBackgroundColor();

            string miscLabelText = "Misc Operations";

            if (MiscOperationTotalCount == 0)
            {
                EditorGUILayout.LabelField(miscLabelText);
                bShowMiscOps = false;
            }
            else
            {
                if (MiscOperationTotalCount != 0)
                {
                    miscLabelText += " <color=" + CopierGUIStyles.DimSelectionTextHexColor + ">(" + AvatarCopierUtils.GetXOutOfTotalText(MiscOperationEnabledCount, MiscOperationTotalCount) + ")</color>";
                }

                bool bToggle = MiscOperationEnabledCount != 0;
                //bool bNextState = DrawToggleFoldout("Misc Operations", "", "", MiscOperationEnabledCount, MiscOperationTotalCount, bPrevState, ref bShowMiscOperations);
                DrawUtils.DrawToggleFoldoutWithSymbols(ref bToggle, ref bShowMiscOps, EditorGUILayout.GetControlRect(), miscLabelText);

                if (bToggle != (MiscOperationEnabledCount != 0))
                {
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled = bToggle;
                    MiscOperationHandler.GetInstance().AvatarIDOp.bUserSetEnabled = bToggle;
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled = bToggle;
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled = bToggle;
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled = bToggle;
                }
            }


            // Each of the Misc Operations
            if (bShowMiscOps)
            {
                ///// Toggle All Button
                //EditorGUI.indentLevel++;
                //EditorGUILayout.BeginHorizontal();

                //if (GUILayout.Button(new GUIContent(MiscOperationEnabledCount > 0 ? "Toggle All Off" : "Toggle All On", "Tooltip needed plz :3"), MergerGUIStyles.GetOperationToggleAllButtonStyle(), GUILayout.MaxWidth(104)))
                //{
                //    MiscOperationHandler.GetInstance().SetAllEnabledStatus(MiscOperationEnabledCount == 0);
                //}

                //GUILayout.FlexibleSpace();
                //EditorGUILayout.EndHorizontal();
                //EditorGUI.indentLevel--;


                /// Reposition Avatar Operation
                if (MiscOperationHandler.GetInstance().RepositionOp != null)
                {
                    PushColorAndBackgroundColor(!MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                    //GUI.backgroundColor = oBGColor;
                    PopBackgroundColor();


                    GUILayout.BeginHorizontal(new GUIContent("", "Toggle this  Operation from being performed."), new GUIStyle());
                    bool PreviousState = MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled || MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled || MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled;
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled = EditorGUILayout.ToggleLeft(" Reposition Avatar", PreviousState, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                    if (PreviousState != MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled)
                    {
                        //MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled = NextValue;
                        MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled = MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled;
                        MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled = MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled;
                        MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled = MiscOperationHandler.GetInstance().RepositionOp.bUserSetEnabled;
                    }
                    GUILayout.EndHorizontal();


                    /// Position
                    PushColorAndBackgroundColor(!MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginHorizontal(CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled = EditorGUILayout.Toggle(new GUIContent("", "Toggle Reposition Position."), MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled, EditorStyles.toggle, GUILayout.MaxWidth(15));
                    EditorGUILayout.LabelField(new GUIContent("Position", "Tooltip"), CopierGUIStyles.GetLabelRichText(), GUILayout.MaxWidth(100));

                    MiscOperationHandler.GetInstance().RepositionOp.RepositionLocation = EditorGUILayout.Vector3Field("", MiscOperationHandler.GetInstance().RepositionOp.RepositionLocation);
                    GUILayout.EndHorizontal();
                    PopColor();


                    /// Rotation
                    PushColorAndBackgroundColor(!MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginHorizontal(CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled = EditorGUILayout.Toggle(new GUIContent("", "Toggle Reposition Rotation."), MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled, EditorStyles.toggle, GUILayout.MaxWidth(15));
                    EditorGUILayout.LabelField(new GUIContent("Rotation", "Tooltip"), CopierGUIStyles.GetLabelRichText(), GUILayout.MaxWidth(100));

                    MiscOperationHandler.GetInstance().RepositionOp.RepositionRotation = EditorGUILayout.Vector3Field("", MiscOperationHandler.GetInstance().RepositionOp.RepositionRotation);
                    GUILayout.EndHorizontal();
                    PopColor();


                    /// Scale
                    PushColorAndBackgroundColor(!MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginHorizontal(CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();
                    //GUI.backgroundColor = oBGColorA;
                    MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled = EditorGUILayout.Toggle(new GUIContent("", "Toggle Reposition Scale."), MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled, EditorStyles.toggle, GUILayout.MaxWidth(15));
                    EditorGUILayout.LabelField(new GUIContent("Scale", "Tooltip"), CopierGUIStyles.GetLabelRichText(), GUILayout.MaxWidth(100));

                    MiscOperationHandler.GetInstance().RepositionOp.RepositionScale = EditorGUILayout.Vector3Field("", MiscOperationHandler.GetInstance().RepositionOp.RepositionScale);
                    GUILayout.EndHorizontal();

                    PopColor();


                    EditorGUILayout.EndVertical();

                    PopColor();
                }


                /// Avatar Blueprint ID
                if (MiscOperationHandler.GetInstance().AvatarIDOp != null)
                {
                    PushColorAndBackgroundColor(!MiscOperationHandler.GetInstance().AvatarIDOp.bUserSetEnabled, CopierGUIStyles.DisabledLabelColor, CopierGUIStyles.DisabledHelpBoxColor);

                    GUILayout.BeginVertical("", CopierGUIStyles.GetMarginedHelpBoxStyle());
                    PopBackgroundColor();

                    GUILayout.BeginHorizontal(new GUIContent("", "Toggle this Enabled/Disabled Operation from being performed."), new GUIStyle());

                    bool PreviousState = MiscOperationHandler.GetInstance().AvatarIDOp.bUserSetEnabled;
                    bool NextValue = EditorGUILayout.ToggleLeft(new GUIContent(" Avatar Blueprint ID", "Tooltip"), MiscOperationHandler.GetInstance().AvatarIDOp.bUserSetEnabled, CopierGUIStyles.GetUnhighlightableLeftToggleStyle());

                    if (PreviousState != NextValue)
                    {
                        MiscOperationHandler.GetInstance().AvatarIDOp.bUserSetEnabled = NextValue;
                    }


                    /// Set Value Line
                    MiscOperationHandler.GetInstance().AvatarIDOp.AvatarID = EditorGUILayout.DelayedTextField(MiscOperationHandler.GetInstance().AvatarIDOp.AvatarID);
                    GUILayout.EndHorizontal();

                    PopColor();
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
            PopColor();
        }
    }
}

#endif