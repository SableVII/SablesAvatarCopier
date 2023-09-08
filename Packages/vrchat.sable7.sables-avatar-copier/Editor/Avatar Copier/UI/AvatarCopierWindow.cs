#if (UNITY_EDITOR) 
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Operations;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.EditorUI
{
    public class ComparePair
    {
        //public GameObject GameObj = null;
        public string Name = "";
        public Component[] ComponentList;
    }

    public partial class AvatarCopierWindow : EditorWindow
    {
        private static AvatarCopierWindow _Instance = null;

        public static AvatarCopierWindow GetInstance()
        {
            return _Instance;
        }

        //Vector2 ScrollPosition = new Vector2();    

        //bool bInputHierarchyUpdated = false;
        bool bIsResizing = false;
        protected readonly float _minMergeTreePanelWidth = 250.0f;
        protected readonly float _minMergeDetailsPanelWidth = 500.0f;
        protected readonly float _minWindowHeight = 500.0f;
        float SliderX = 300.0f;

        protected GameObject _previousDestinationAvatar = null;
        protected GameObject _previousSourceAvatar = null;

        public AvatarCopierWindow()
        {
            minSize = new Vector2(_minMergeTreePanelWidth + _minMergeDetailsPanelWidth, _minWindowHeight);
            position = new Rect(position.x, position.y, minSize.x + 100.0f, minSize.y + 300.0f);

            _Instance = this;
        }

        protected void OnEnabled()
        {

        }

        /// Icons:
        //https://github.com/halak/unity-editor-icons
        //d_console.warnicon@2x


        [MenuItem("Window/Sable's Tools/Avatar Copier")]

        static void Initialize()
        {
            AvatarCopierWindow window = (AvatarCopierWindow)EditorWindow.GetWindow(typeof(AvatarCopierWindow), true, "Avatar Copier");
        }

        private void OnFocus()
        {
            if (CopierSettingsHandler.GetInstance().SelectedTabIndex != 0)
            {
                Refresh();
            }
            else
            {
                if (CopierSettingsHandler.GetInstance().Destination as GameObject == null || CopierSettingsHandler.GetInstance().Source as GameObject == null)
                {
                    Refresh();
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
                }
            }
        }

        public void Refresh()
        {
            if (CopierSettingsHandler.GetInstance().Destination as GameObject == null)
            {
                CopierSettingsHandler.GetInstance().Destination = null;
            }

            if (CopierSettingsHandler.GetInstance().Source as GameObject == null)
            {
                CopierSettingsHandler.GetInstance().Source = null;
            }

            WarningHandler.GetInstance().Reset();
            InputErrorHandler.GetInstance().CheckInputsForErrors();

            if (CopierSettingsHandler.GetInstance().Destination != null && CopierSettingsHandler.GetInstance().Source != null
                && CopierSettingsHandler.GetInstance().Destination != CopierSettingsHandler.GetInstance().Source && !WarningHandler.GetInstance().HasHaultingErrors())
            {
                AvatarMatchHandler.GetInstance().Reset();
                AttachmentOperationHandler.GetInstance().SavedReset();
                MergeTreeHandler.GetInstance().SavedReset();
                ScaleOperationHandler.GetInstance().SavedReset();
                MiscOperationHandler.GetInstance().SavedReset();
                ComponentOperationHandler.GetInstance().SavedReset();
                EnabledDisabledOperationHandler.GetInstance().SavedReset();
                MaterialOperationHandler.GetInstance().SavedReset();
                //MergerWarningHandler.GetInstance().Reset();

                //MergerSettingsHandler.GetInstance().bCantSearchForAttachables = false;

                //AvatarMatchHandler.GetInstance().MatchAvatar();

                // Create the Virtual Tree from the default Destination Objects

                GetHandlersSetFromInputs();


                AttachmentOperationHandler.GetInstance().ApplySavedData();
                ComponentOperationHandler.GetInstance().ApplySavedData();
                EnabledDisabledOperationHandler.GetInstance().ApplySavedData();
                ScaleOperationHandler.GetInstance().ApplySavedData();
                MiscOperationHandler.GetInstance().ApplySavedData();

                MergeTreeHandler.GetInstance().UpdateAttachables();
                MergeTreeHandler.GetInstance().ApplySavedData();
                MaterialOperationHandler.GetInstance().ApplySavedData();

                CopierSettingsHandler.GetInstance().bScannedAvatars = true;

                MergeTreeHandler.GetInstance().ResetOrderedVirtualObjects();
                ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
            }
            else
            {
                FullResetHandlers(false);
            }
        }

        virtual public void Awake()
        {
            CopierSettingsHandler.GetInstance().LoadData();
            CopierSettingsHandler.GetInstance().EnsurePreservedPropertyData();
        }

        virtual public void OnDestroy()
        {
            CopierSettingsHandler.GetInstance().RegisterSave();
        }

        // Start is called before the first frame update
        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.BeginVertical(GUILayout.MinWidth(300.0f));
            GUILayout.BeginArea(new Rect(0, 0, SliderX, position.height));
            // For re-sizing information
            //https://gram.gs/gramlog/creating-editor-windows-in-unity/
            MergeTreeUIPanel.GetInstance().DrawMergeTreePanel();
            //EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            // WindowResizer
            Rect ResizerRect = new Rect(SliderX - 5, 0, 10, position.height);
            GUILayout.BeginArea(ResizerRect);
            GUILayout.EndArea();
            EditorGUIUtility.AddCursorRect(ResizerRect, MouseCursor.ResizeHorizontal);

            // Mouse Event Handling
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && ResizerRect.Contains(Event.current.mousePosition))
                    {
                        bIsResizing = true;
                    }
                    break;

                case EventType.MouseUp:
                    bIsResizing = false;
                    break;
            }

            bool bRepaintNeeded = false;
            if (bIsResizing)
            {
                SliderX = Event.current.mousePosition.x;

                bRepaintNeeded = true;
            }

            // Resizer clamping
            if (SliderX < _minMergeTreePanelWidth)
            {
                SliderX = _minMergeTreePanelWidth;
                bRepaintNeeded = true;
            }

            if (SliderX > position.width - _minMergeDetailsPanelWidth)
            {
                SliderX = position.width - _minMergeDetailsPanelWidth;
                bRepaintNeeded = true;
            }


            if (bRepaintNeeded)
            {
                Repaint();
            }

            //EditorGUILayout.BeginVertical(GUILayout.MinWidth(500.0f));
            GUILayout.BeginArea(new Rect(SliderX, 0, position.width-SliderX, position.height));
            {
                ComponentOperationHandler.GetInstance().TryRefRefresh();

                GUILayout.Space(10);

                // Destination Avatar
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(new GUIContent("", "The Avatar you wish to copy To. (Requires VRC Avatar Descriptor Component or Animator Component with an Avatar set.)"), new GUIStyle());
                string destinationLabel = "Destination";
                if (CopierSettingsHandler.GetInstance().Destination != null)
                {
                    destinationLabel += "<color=#";
                    if (CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar)
                    {
                        destinationLabel += "55cc55>                           Avatar";
                    }
                    else
                    {
                        destinationLabel += "55cccc>                  Non-Avatar";
                    }
                    destinationLabel += "</color>";
                }
                EditorGUILayout.LabelField(destinationLabel, CopierGUIStyles.GetLabelRichText());
                GameObject NewDestinationObject = EditorGUILayout.ObjectField("", CopierSettingsHandler.GetInstance().Destination, typeof(GameObject), true) as GameObject;
                GUILayout.Space(7);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                // Source Avatar
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(new GUIContent("", "The Avatar you wish to copy From. (Requires VRC Avatar Descriptor Component or Animator Component with an Avatar set.)"), new GUIStyle());
                string sourceLabel = "Source";
                if (CopierSettingsHandler.GetInstance().Source != null)
                {
                    sourceLabel += "<color=#";
                    if (CopierSettingsHandler.GetInstance().bSourceIsAnAvatar)
                    {
                        sourceLabel += "55cc55>                                   Avatar";
                    }
                    else
                    {
                        sourceLabel += "55cccc>                          Non-Avatar";
                    }
                    sourceLabel += "</color>";
                }
                EditorGUILayout.LabelField(sourceLabel, CopierGUIStyles.GetLabelRichText());
                GameObject NewSourceGameObject = EditorGUILayout.ObjectField("", CopierSettingsHandler.GetInstance().Source, typeof(GameObject), true) as GameObject;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();


                //// Direct Options
                //EditorGUILayout.BeginHorizontal();
                //{
                //    GUILayout.FlexibleSpace();

                //    // Match Inputs together
                //    bool bPrevMatch = MergerSettingsHandler.GetInstance().bMatchInputsTogether;
                //    MergerSettingsHandler.GetInstance().bMatchInputsTogether = EditorGUILayout.Toggle(new GUIContent("Match Inputs Together", "Make the Destination Input and the Source Input match together. This is useful if different Avatar's have different root-names"), MergerSettingsHandler.GetInstance().bMatchInputsTogether);
                //    if (bPrevMatch != MergerSettingsHandler.GetInstance().bMatchInputsTogether)
                //    {
                //        Refresh();
                //    }

                //    GUILayout.FlexibleSpace();
                //}
                //EditorGUILayout.EndHorizontal();


                bool bInputsAreNew = false;
                // Check and Clean Destination Input
                if (NewDestinationObject != _previousDestinationAvatar)
                {
                    // Sanatize out potential bad Inputs for Destination
                    if (NewDestinationObject as GameObject == null || NewDestinationObject.scene != EditorSceneManager.GetActiveScene())
                    {
                        NewDestinationObject = null;
                        CopierSettingsHandler.GetInstance().CloneName = "";
                    }

                    // Getting Destination Avatar
                    //if (NewDestinationObject != null)
                    //{
                    //    NewDestinationObject = UpScanForAvatar_R(NewDestinationObject);
                    //}

                    // Set needing to scan the avatars as Destination Avatars have changed
                    if (NewDestinationObject != CopierSettingsHandler.GetInstance().Destination)
                    {
                        CopierSettingsHandler.GetInstance().Destination = NewDestinationObject;
                        CopierSettingsHandler.GetInstance().bScannedAvatars = false;

                        if (CopierSettingsHandler.GetInstance().Destination != null  && (CopierSettingsHandler.GetInstance().CloneName == ""
                            || (_previousDestinationAvatar != null && CopierSettingsHandler.GetInstance().CloneName == _previousDestinationAvatar.name + " (Merged)")))
                        {
                            CopierSettingsHandler.GetInstance().CloneName = CopierSettingsHandler.GetInstance().Destination.name + " (Merged)";
                        }
                    }

                    bInputsAreNew = true;
                    CopierSettingsHandler.GetInstance().Destination = NewDestinationObject;
                    _previousDestinationAvatar = NewDestinationObject;
                }

                // Check and Clean Source Input
                if (NewSourceGameObject != _previousSourceAvatar)
                {
                    // Sanatize out potential bad Inputs for Destination
                    if (NewSourceGameObject as GameObject == null || NewSourceGameObject.scene != EditorSceneManager.GetActiveScene())
                    {
                        NewSourceGameObject = null;
                    }

                    // Checking Destination Avatar
                    //if (NewSourceGameObject != null)
                    //{
                    //    NewSourceGameObject = UpScanForAvatar_R(NewSourceGameObject);
                    //}

                    // Set needing to scan the avatars
                    if (NewSourceGameObject != CopierSettingsHandler.GetInstance().Source)
                    {
                        CopierSettingsHandler.GetInstance().Source = NewSourceGameObject;
                        CopierSettingsHandler.GetInstance().bScannedAvatars = false;
                    }

                    bInputsAreNew = true;
                    CopierSettingsHandler.GetInstance().Source = NewSourceGameObject;
                    _previousSourceAvatar = NewSourceGameObject;
                }


                // Checking for Errors
                if (bInputsAreNew)
                {
                    InputsUpdated();
                }


                // Refresh Button
                /*GUILayout.Space(7);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Refresh"))
                {
                    FullResetHandlers(true);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();*/


                // Print Properties Button  // For Debuggin use only!
                /*GUILayout.Space(7);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Print Properties"))
                {
                    System.Type CheckType = typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor);
                    ComponentPreserveablePropertyInfo PropInfo = PreservedPropertyHandler.GetInstance().GetComponentPropertyInfo(CheckType);

                    Debug.Log(MergerGlobals.TypeToFriendlyName(PropInfo.ComponentType) + " [" + PreservedPropertyHandler.GetInstance().GetIndexFromSystemType(PropInfo.ComponentType) + "]: ");

                    for (int i = 0; i < PropInfo.PropertyNames.Count; i++)
                    {
                        Debug.Log(" ~ Property[" + i + "]: " + PropInfo.PropertyNames[i] + "  Property Type: " + PropInfo.PropertyTypes[i]);
                    }

                    System.Reflection.FieldInfo[] fields = CheckType.GetFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].IsPublic)
                        {
                            Debug.Log(" ~ Field[" + i + "]: " + fields[i].Name);
                        }
                    }

                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();*/

                /// Help Box
                if (WarningHandler.GetInstance().HasErrors())
                {
                    for (int i = 0; i < WarningHandler.GetInstance().GetErrorCount(); i++)
                    {
                        EditorGUILayout.HelpBox(WarningHandler.GetInstance().GetError(i).ErrorMessage, MessageType.Error);
                    }
                }

                if (WarningHandler.GetInstance().HasUnSurpressedWarnings())
                {
                    string WarningWordText = "Warning";
                    if (WarningHandler.GetInstance().GetWarningsCount() > 1)
                    {
                        WarningWordText += "s have";
                    }
                    else
                    {
                        WarningWordText += " has";
                    }

                    EditorGUILayout.HelpBox(WarningHandler.GetInstance().GetUnSurpressedWarningsCount() + " " + WarningWordText +" been found. View and address the warnings in the Warnings tab.", MessageType.Warning);
                }

                if (WarningHandler.GetInstance().HasErrors() == false && WarningHandler.GetInstance().HasUnSurpressedWarnings() == false)
                {
                    if (CopierSettingsHandler.GetInstance().Destination == null || CopierSettingsHandler.GetInstance().Source == null)
                    {
                        EditorGUILayout.HelpBox("Place the Avatar you want to copy TO in the Destination Avatar field and place the Avatar you want to copy FROM in the Source Avatar field.", MessageType.Info);
                    }
                    else
                    {
                        // All ready to go!
                        EditorGUILayout.HelpBox("Everything is good to go! No warnings and everything should copy without any detectable issues!", MessageType.Info);
                    }
                }

                /// Run and scan avatar
                if (WarningHandler.GetInstance().HasHaultingErrors() == false)
                {
                    if (CopierSettingsHandler.GetInstance().Source != null && CopierSettingsHandler.GetInstance().Destination != null && !CopierSettingsHandler.GetInstance().bScannedAvatars)
                    {
                        FullResetHandlers(false);

                        CopierSettingsHandler.GetInstance().RegisterSave();

                        GetHandlersSetFromInputs();

                        MergeTreeHandler.GetInstance().ResetOrderedVirtualObjects();
                        ComponentOperationHandler.GetInstance().RegisterForRefRefresh();
                    }
                }

                if ((CopierSettingsHandler.GetInstance().Source == null || CopierSettingsHandler.GetInstance().Destination == null) && CopierSettingsHandler.GetInstance().bScannedAvatars)
                {
                    CopierSettingsHandler.GetInstance().bScannedAvatars = false;
                    FullResetHandlers(true);
                }

                GUILayout.Space(10);

                // Copy Avatar Button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (CopierSettingsHandler.GetInstance().Destination == null || CopierSettingsHandler.GetInstance().Source == null || WarningHandler.GetInstance().HasHaultingErrors())
                {
                    GUI.enabled = false;
                }


                bool bExecute = false;
                if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone") && CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar == true)
                {
                    if (GUILayout.Button("Create Avatar"))
                    {
                        bExecute = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Merge Avatar"))
                    {
                        bExecute = true;
                    }
                }

                if (bExecute)
                {
                    AvatarCopierExecutor.GetInstance().Execute();
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 0;
                    CopierSettingsHandler.GetInstance().Destination = null;
                }

                GUI.enabled = true;
                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();


                // Copy Name
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    // Set Label Width
                    float prevLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80.0f;

                    // Create Copy Toggle button
                    if (CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar == false)
                    {
                        GUI.color = CopierGUIStyles.DisabledGUIColor;
                    }
                    CopierSettingsHandler.GetInstance().TrySetBoolDataField("bCreateDestinationClone", EditorGUILayout.Toggle(new GUIContent("Create Copy", "If true, then create a new instance instead of copying into the Destination Avatar."), CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone")));


                    // Clone Name
                    if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone") == false)
                    {
                        GUI.color = CopierGUIStyles.DisabledGUIColor;
                    }

                    //if (MergerSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone"))
                    //{

                        EditorGUILayout.LabelField("            Clone Name"); //  All dem spaces to seperate Clone Name input field from the Toggle field its next to
                        string OldCloneName = CopierSettingsHandler.GetInstance().CloneName;
                        CopierSettingsHandler.GetInstance().CloneName = EditorGUILayout.DelayedTextField(new GUIContent("", "Tooltip Clone Name"), CopierSettingsHandler.GetInstance().CloneName);
                        if (OldCloneName != CopierSettingsHandler.GetInstance().CloneName)
                        {
                            if (MergeTreeHandler.GetInstance().VirtualTreeRoot != null)
                            {
                                MergeTreeHandler.GetInstance().VirtualTreeRoot.Name = CopierSettingsHandler.GetInstance().CloneName;
                            }
                        }
                    //}

                    EditorGUIUtility.labelWidth = prevLabelWidth;
                    GUI.color = Color.white;

                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();



                /// Draw and list Duplicates if any.
                /*if (AvatarMatchHandler.GetInstance().DoesDestinationHaveDupliacates() || AvatarMatchHandler.GetInstance().DoesSourceHaveDuplicates())
                {
                    DuplicatesUI.DrawDuplicatesWarningBox();
                    //GUILayout.FlexibleSpace();
                }*/

                GUILayout.Space(10);


                // Nenu Tabs
                int prevSelectedTabIndex = CopierSettingsHandler.GetInstance().SelectedTabIndex;
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Merge Details", EditorStyles.miniButton))
                {
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
                }
                else if (GUILayout.Button("Warnings (" + WarningHandler.GetInstance().GetUnSurpressedWarningsCount() + ")", EditorStyles.miniButton))
                {
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 2;
                }
                else if (GUILayout.Button("Settings", EditorStyles.miniButton))
                {
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 3;
                }
                else if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bShowAdvancedSettings") && GUILayout.Button("Advanced Settings", EditorStyles.miniButton))
                {
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 4;
                }
                else if (GUILayout.Button("Help", EditorStyles.miniButton))
                {
                    CopierSettingsHandler.GetInstance().SelectedTabIndex = 5;
                }
                EditorGUILayout.EndHorizontal();

                // Tab has changed
                if (CopierSettingsHandler.GetInstance().SelectedTabIndex != prevSelectedTabIndex && CopierSettingsHandler.GetInstance().SelectedTabIndex == 0)
                {
                    Refresh();
                }

                switch (CopierSettingsHandler.GetInstance().SelectedTabIndex)
                {
                    case 0:
                        SuccessUIPanel.GetInstance().DrawSuccessPanel();
                        break;
                    case 1:
                        CopyDetailsUIPanel.GetInstance().DrawMergerDetailsPanel();
                        break;
                    case 2:
                        WarningsUIPanel.GetInstance().DrawWarningsPanel();
                        break;
                    case 3:
                        SettingsUIPanel.GetInstance().DrawSettingsPanel();
                        break;
                    case 4:
                        PreservedParametersUIPanel.GetInstance().DrawPreservedPropertiessUIPanel();
                        break;
                    case 5:
                        HelpUIPanel.GetInstance().DrawHelpPanel();
                        break;
                }

                GUILayout.Label("\t\t~By Sable7 <3");

            }
            //EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            EditorGUILayout.EndHorizontal();

            CopierSettingsHandler.GetInstance().SaveIfRegistered();
        }

        void FullResetHandlers(bool bResetWarnings)
        {
            CopierSettingsHandler.GetInstance().bScannedAvatars = false;

            AvatarMatchHandler.GetInstance().Reset();
            AttachmentOperationHandler.GetInstance().Reset();
            ComponentOperationHandler.GetInstance().Reset();
            MergeTreeHandler.GetInstance().Reset();
            ScaleOperationHandler.GetInstance().Reset();
            EnabledDisabledOperationHandler.GetInstance().Reset();
            MiscOperationHandler.GetInstance().Reset();
            MaterialOperationHandler.GetInstance().Reset();

            if (bResetWarnings)
            {
                WarningHandler.GetInstance().Reset();
            }
        }

        GameObject UpScanForAvatar_R(GameObject InGameObject)
        {
            if (InGameObject == null)
            {
                return null;
            }

            VRC.SDK3.Avatars.Components.VRCAvatarDescriptor fillerComponent = null;
            if (InGameObject.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out fillerComponent))
            {
                return InGameObject;
            }

            Animator fillerAnimator = null;
            if (InGameObject.TryGetComponent<Animator>(out fillerAnimator))
            {
                if (fillerAnimator.avatar != null)
                {
                    return InGameObject;
                }
            }

            if (InGameObject.transform.parent != null && InGameObject.transform.parent.gameObject != null)
            {
                return UpScanForAvatar_R(InGameObject.transform.parent.gameObject);
            }

            return null;
        }

        protected void GetHandlersSetFromInputs()
        {
            AvatarMatchHandler.GetInstance().CreateMappings();

            MergeTreeHandler.GetInstance().CreateVirtualTreeFromDestinationAvatar();

            AvatarMatchHandler.GetInstance().LinkNonAttachablesInSource();

            AttachmentOperationHandler.GetInstance().GenerateAttachmentOperations();

            ComponentOperationHandler.GetInstance().GenerateComponentOperations();

            MaterialOperationHandler.GetInstance().CreateMaterialOperations();

            ScaleOperationHandler.GetInstance().CreateScaleOperations();

            MiscOperationHandler.GetInstance().CreateMiscOperations();

            EnabledDisabledOperationHandler.GetInstance().CreateEnabledDisabledOperations();

            CopierSettingsHandler.GetInstance().bScannedAvatars = true;
        }

        public void InputsUpdated()
        {
            // Checking to see if the input Destination Avatar is an Avatar or not
            CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar = false;
            if (CopierSettingsHandler.GetInstance().Destination != null)
            {
                Animator animator = CopierSettingsHandler.GetInstance().Destination.GetComponent<Animator>() as Animator;
                Avatar avi = null;
                if (animator)
                {
                    avi = animator.avatar;
                }

                if (avi != null)
                {
                    CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar = true;
                }
            }

            // Checking to see if the input Source Avatar is a Avatar or not
            CopierSettingsHandler.GetInstance().bSourceIsAnAvatar = false;
            if (CopierSettingsHandler.GetInstance().Source != null)
            {
                Animator animator = CopierSettingsHandler.GetInstance().Source.GetComponent<Animator>() as Animator;
                Avatar avi = null;
                if (animator)
                {
                    avi = animator.avatar;
                }

                if (avi != null)
                {
                    CopierSettingsHandler.GetInstance().bSourceIsAnAvatar = true;
                }
            }

            WarningHandler.GetInstance().Reset();
            FullResetHandlers(true);
            if (CopierSettingsHandler.GetInstance().SelectedTabIndex == 0)
            {
                CopierSettingsHandler.GetInstance().SelectedTabIndex = 1;
            }
            InputErrorHandler.GetInstance().CheckInputsForErrors();

            // Create Error if inputs are the same
            if (!WarningHandler.GetInstance().HasHaultingErrors() && CopierSettingsHandler.GetInstance().Destination != null && CopierSettingsHandler.GetInstance().Source != null && CopierSettingsHandler.GetInstance().Destination == CopierSettingsHandler.GetInstance().Source)
            {
                WarningHandler.GetInstance().AddError(new Errors.AvatarCopierSameInputsError());
            }

            CopierSettingsHandler.GetInstance().bCantSearchForAttachables = false;
            // Can't search for attachables if Source is not an Avatar
            if (CopierSettingsHandler.GetInstance().bSourceIsAnAvatar == false)
            {
                CopierSettingsHandler.GetInstance().bCantSearchForAttachables = true;
            }

            CopierSettingsHandler.GetInstance().bMatchInputsTogether = false;
            if (CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar && CopierSettingsHandler.GetInstance().bSourceIsAnAvatar)
            {
                CopierSettingsHandler.GetInstance().bMatchInputsTogether = true;
            }
        }
    }
}
#endif