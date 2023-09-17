#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier
{
    public class AvatarCopierExecutor
    {
        private static AvatarCopierExecutor _Instance = null;

        private int _NewComponentsTransfered = 0;
        public int NewComponentsTransfered { get { return _NewComponentsTransfered; } }

        private int _ComponentsReplaced = 0;
        public int ComponentsReplaced { get { return _ComponentsReplaced; } }

        private int _AttachablesAttached = 0;
        public int AttachablesAttached { get { return _AttachablesAttached; } }

        private int _AdjustedScales = 0;
        public int AdjustedScales { get { return _AdjustedScales; } }

        private int _EnabledStatusesChanged = 0;
        public int EnabledStatusesChanged { get { return _EnabledStatusesChanged; } }

        private string _CopiedDestinationAvatarName = "<CopiedDestinationAvatarName>";
        public string CopiedDestinationAvatarName { get { return _CopiedDestinationAvatarName; } }

        private string _CopiedSourceAvatarName = "<CopiedSourceAvatarName>";
        public string CopiedSourceAvatarName { get { return _CopiedSourceAvatarName; } }

        private string _ClonedName = "<ClonedName>";
        public string ClonedName { get { return _ClonedName; } }

        private Dictionary<ComponentOperation, Dictionary<string, float>> SkinnedMeshRendererBlendshapes = new Dictionary<ComponentOperation, Dictionary<string, float>>();

        //private Dictionary<ComponentOperation, Material[]> RendererSavedMaterial = new Dictionary<ComponentOperation, Material[]>();

        private Dictionary<OverridingComponentOperation, EyeLookSetting> EyeLookSettingDict = new Dictionary<OverridingComponentOperation, EyeLookSetting>();

        private class EyeLookSetting
        {
            public string BlinkName = "";
            public string LookingUpName = "";
            public string LookingDownName = "";
        }        

        public static AvatarCopierExecutor GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new AvatarCopierExecutor();
            }

            return _Instance;
        }

        private AvatarCopierExecutor()
        {

        }

        //public Dictionary<VirtualGameObject, GameObject> VirtualToNewRealGameObjects = new Dictionary<VirtualGameObject, GameObject>();

        protected void ResetExecuteData()
        {
            _NewComponentsTransfered = 0;
            _ComponentsReplaced = 0;
            _AttachablesAttached = 0;
            _AdjustedScales = 0;
            _EnabledStatusesChanged = 0;
            _CopiedDestinationAvatarName = CopierSettingsHandler.GetInstance().Destination.name;
            _CopiedSourceAvatarName = CopierSettingsHandler.GetInstance().Source.name;
            _ClonedName = CopierSettingsHandler.GetInstance().CloneName;
        }

        public void Execute()
        {
            ResetExecuteData();

#if UNITY_EDITOR
            if (PrefabUtility.IsPartOfPrefabInstance(CopierSettingsHandler.GetInstance().Destination) && CopierSettingsHandler.GetInstance().GetBoolDataValue("bUnpackPrefab") && !CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone"))
            {
                PrefabUtility.UnpackPrefabInstance(CopierSettingsHandler.GetInstance().Destination, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
#endif
            // Preserve Properties
            if (PreservedPropertyHandler.GetInstance().GetEnabledPereservedPropertyCount() > 0)
            {
                SavePreserveableProperties();
            }

            VerySpecificPreCompOperations();

            MakeVirtualTreeReal();

            ApplyMiscOperationsPreAttachablesAttachment();

            //// Attach Attachables
            AttachAttachables();

            ComponentMergeFuncs.SetRegisteredReferences();

            ApplyPreserveableProperties();

            ApplyMaterialOperations();

            ApplyEnabledDisabledOperations();

            ApplyMiscOperationsPostAttachableAttachment();

            //// Apply Scale Operations
            ApplyScaleOperations();

            VerySpecificPostCompOperations();
        }

        protected void MakeVirtualTreeReal()
        {
            GameObject NewCopiedAvatar = CopierSettingsHandler.GetInstance().Destination;
            if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCreateDestinationClone") && CopierSettingsHandler.GetInstance().bDestinationIsAnAvatar == true)
            {
                NewCopiedAvatar = GameObject.Instantiate(CopierSettingsHandler.GetInstance().Destination);
                NewCopiedAvatar.transform.SetSiblingIndex(CopierSettingsHandler.GetInstance().Destination.transform.GetSiblingIndex());
                NewCopiedAvatar.name = CopierSettingsHandler.GetInstance().CloneName;

                // Remove all Components
                //StripAllComponents_R(NewCopiedAvatar);

                Undo.RegisterCreatedObjectUndo(NewCopiedAvatar, "Avatar Copier Created Merged Clone");
            }
            else
            {
                Undo.RegisterFullObjectHierarchyUndo(NewCopiedAvatar, "Avatar Copier Copied Avatar");
            }



            // Crawl Through and Link the newly created Copied Avatar and the VirtualGameObjects in the Virtual Tree
            CrawlThroughAndLinkVirtual_R(NewCopiedAvatar, MergeTreeHandler.GetInstance().VirtualTreeRoot);
        }

        protected void AttachAttachables()
        {
            /*if (!MergerSettingsHandler.GetInstance().bCopyAttachables)
            {
                return;
            }*/

            for (int i = 0; i < AvatarMatchHandler.GetInstance().TopLevelAttachableGameObjectsCount; i++)
            {
                VirtualGameObject topLevelVirtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(AvatarMatchHandler.GetInstance().GetTopLevelAttachableGameObject(i));

                if (!topLevelVirtualObj.AttachmentOp.IsFullyEnabled())
                {
                    continue;
                }

                GameObject SourceGameObject = topLevelVirtualObj.AttachmentOp.SourceAttachableObject;
                GameObject NewRunTimeAttachableGameObject = GameObject.Instantiate(SourceGameObject, topLevelVirtualObj.Parent.RunTimeObject.transform);
                NewRunTimeAttachableGameObject.transform.localPosition = SourceGameObject.transform.localPosition;
                NewRunTimeAttachableGameObject.transform.localEulerAngles = SourceGameObject.transform.localEulerAngles;
                NewRunTimeAttachableGameObject.transform.localScale = SourceGameObject.transform.localScale;
                NewRunTimeAttachableGameObject.name = topLevelVirtualObj.Name;

                _AttachablesAttached++;

                Undo.RegisterCreatedObjectUndo(NewRunTimeAttachableGameObject, "Avatar Copier Copied Avatar");

                //StripAllComponents_R(NewRunTimeAttachableGameObject);

                // Now Crawl through and Link objects
                CrawlThroughAndLinkVirtual_R(NewRunTimeAttachableGameObject, topLevelVirtualObj, true);
            }
        }

        //protected void StripAllComponents_R(GameObject CurrentGameObject)
        //{
        //    List<Component> Components = new List<Component>(CurrentGameObject.GetComponents(typeof(Component)));

        //    // Delete Components
        //    foreach (Component Comp in Components)
        //    {
        //        if (MergerGlobals.AllowedCopyTypes.Contains(Comp.GetType()) || Comp.GetType() == typeof(VRC.Core.PipelineManager) || Comp.GetType() == typeof(ParticleSystemRenderer))
        //        {
        //            GameObject.DestroyImmediate(Comp);
        //        }
        //    }

        //    // Loop through childern
        //    for (int i = 0; i < CurrentGameObject.transform.childCount; i++)
        //    {
        //        StripAllComponents_R(CurrentGameObject.transform.GetChild(i).gameObject);
        //    }
        //}

        protected void CrawlThroughAndLinkVirtual_R(GameObject CurrentRealObj, VirtualGameObject CurrentVirtualObj, bool bAcceptAttachables = false)
        {
            // Link both VirtualObject and RealObject together
            CurrentVirtualObj.RunTimeObject = CurrentRealObj;

            // Link Pre-Existing
            if (CurrentVirtualObj.GetPreExistingCount() != 0)
            {
                Component[] newComps = CurrentRealObj.GetComponents(typeof(Component));

                // Link Pre-Existing Infos
                for (int i = 0; i < CurrentVirtualObj.GetPreExistingCount(); i++)
                {
                    PreExistingComponentOperation preExistingCompOp = CurrentVirtualObj.GetPreExisting(i);

                    // Link to newly created component in CurrentRealObj
                    int currentTypeIndex = 0;
                    for (int j = 0; j < newComps.Length; j++)
                    {
                        if (newComps[j].GetType() == preExistingCompOp.ComponentType)
                        {
                            if (currentTypeIndex == preExistingCompOp.ComponentTypeIndex)
                            {
                                preExistingCompOp.RunTimeComponent = newComps[j];
                                break;
                            }

                            currentTypeIndex++;
                        }
                    }
                }
            }

            // Link or Create Overridable Component Operations
            if (CurrentVirtualObj.GetEnabledOverridingCount() != 0)
            {
                for (int i = 0; i < CurrentVirtualObj.GetOverridingCount(); i++)
                {
                    OverridingComponentOperation overridingCompOp = CurrentVirtualObj.GetOverriding(i);

                    if (overridingCompOp.IsFullyEnabled() == false)
                    {
                        continue;
                    }

                    Component pasteToComponent = null;
                    if (overridingCompOp.IsReplacing)
                    {
                        pasteToComponent = overridingCompOp.ReplacingPreExistingCompOp.RunTimeComponent;
                    }
                    else
                    {
                        pasteToComponent =  Undo.AddComponent(CurrentRealObj, overridingCompOp.ComponentType);
                    }

                    UnityEditorInternal.ComponentUtility.CopyComponent(overridingCompOp.OriginComponent);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(pasteToComponent);

                    overridingCompOp.RunTimeComponent = pasteToComponent;
                }
            }


            // Find and Link RealObj and VirtualObj by name.
            for (int ChildernIndex = 0; ChildernIndex < CurrentRealObj.transform.childCount; ChildernIndex++)
            {
                VirtualGameObject FoundVirtualGameObject = null;
                for (int VirtualChildIndex = 0; VirtualChildIndex < CurrentVirtualObj.GetChildCount(); VirtualChildIndex++)
                {
                    if (!bAcceptAttachables && CurrentVirtualObj.GetChild(VirtualChildIndex).bIsAttachable)
                    {
                        continue;
                    }

                    if (CurrentVirtualObj.GetChild(VirtualChildIndex).Name == CurrentRealObj.transform.GetChild(ChildernIndex).gameObject.name)
                    {
                        if (CurrentVirtualObj.GetChild(VirtualChildIndex).RunTimeObject == null)
                        {
                            FoundVirtualGameObject = CurrentVirtualObj.GetChild(VirtualChildIndex);
                            break;
                        }
                    }
                }

                // Crawl through the childern of the linked GameObject and VirtualGameObject
                if (FoundVirtualGameObject != null)
                {
                    CrawlThroughAndLinkVirtual_R(CurrentRealObj.transform.GetChild(ChildernIndex).gameObject, FoundVirtualGameObject, bAcceptAttachables);
                }
                else
                {
                    Debug.LogWarning("CANNOT FIND VIRTUALGAMEOBJECT: " + CurrentRealObj.transform.GetChild(ChildernIndex).name);
                }
            }
        }

        public void SavePreserveableProperties()
        {
            foreach (Component CompKey in ComponentOperationHandler.GetInstance().ComponentOperations.Keys)
            {
                PreExistingComponentOperation compOp = ComponentOperationHandler.GetInstance().ComponentOperations[CompKey] as PreExistingComponentOperation;

                if (compOp == null || !compOp.IsFullyEnabled())
                {
                    continue;
                }

                // Cache Properties/Fields
                compOp.RunTimePreservedProperties.Clear();
                //List<PreservedPropertyData> combinedPreservePropertyDataList = new List<PreservedPropertyData>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties());
                //List<PreservedPropertyData> PreservedProps = CopierSettingsHandler.GetInstance().GetObjectDataValue("PreservedProperties") as List<PreservedPropertyData>;
                //CombinedPreservePropertyDataList.AddRange(PreservedProps.ToArray());

                for (int propDataIndex = 0; propDataIndex < PreservedPropertyHandler.GetInstance().GetPreservedPropertyCount(); propDataIndex++)
                {
                    PreservedPropertyData propData = PreservedPropertyHandler.GetInstance().GetPreservedPropertyData(propDataIndex);

                    if (propData.GetPreservedComponentType() == compOp.ComponentType && propData.bEnabled)
                    {
                        System.Reflection.PropertyInfo[] propertyInfos = compOp.ComponentType.GetProperties();
                        object propertyValue = null;

                        bool bFoundAsProperty = false;
                        for (int j = 0; j < propertyInfos.Length; j++)
                        {
                            if (propertyInfos[j].Name == propData.GetPropertyName())
                            {
                                propertyValue = propertyInfos[j].GetValue(compOp.OriginComponent);
                                bFoundAsProperty = true;
                                break;
                            }
                        }

                        if (!bFoundAsProperty)
                        {
                            System.Reflection.FieldInfo[] fields = compOp.ComponentType.GetFields();
                            for (int j = 0; j < fields.Length; j++)
                            {
                                if (fields[j].Name == propData.GetPropertyName())
                                {
                                    propertyValue = fields[j].GetValue(compOp.OriginComponent);
                                    break;
                                }
                            }
                        }

                        if (propertyValue != null)
                        {
                            //if (typeof(Transform).IsAssignableFrom(PropertyValue.GetType()))
                            //{
                            //    Debug.Log("Assigning PropertyValue thats a Transform. Root: " + (PropertyValue as Transform).root.name);
                            //}

                            compOp.RunTimePreservedProperties.Add(new KeyValuePair<PreservedPropertyData, object>(propData, propertyValue));
                        }
                    }
                }
            }
        }

        public void ApplyPreserveableProperties()
        {
            foreach (Component compKey in ComponentOperationHandler.GetInstance().ComponentOperations.Keys)
            {
                PreExistingComponentOperation compOp = ComponentOperationHandler.GetInstance().ComponentOperations[compKey] as PreExistingComponentOperation;

                if (compOp == null || !compOp.IsFullyEnabled() || compOp.RunTimePreservedProperties.Count <= 0)
                {
                    continue;
                }

                // Now apply the Preserved Properties
                foreach (KeyValuePair<PreservedPropertyData, object> pair in compOp.RunTimePreservedProperties)
                {
                    bool bFoundAsProperty = false;
                    System.Reflection.PropertyInfo[] propertyInfos = compOp.ComponentType.GetProperties();
                    for (int j = 0; j < propertyInfos.Length; j++)
                    {
                        if (propertyInfos[j].Name == pair.Key.GetPropertyName())
                        {
                            propertyInfos[j].SetValue(compOp.RunTimeComponent, pair.Value);
                            bFoundAsProperty = true;


                            // If preserved object is a GameObject or Component, match it into newly created avatar
                            if (typeof(GameObject).IsAssignableFrom(propertyInfos[j].PropertyType) || typeof(Transform).IsAssignableFrom(propertyInfos[j].PropertyType))
                            {
                                GameObject gObj = null;
                                if (typeof(Transform).IsAssignableFrom(propertyInfos[j].PropertyType))
                                {
                                    Transform asTransform = propertyInfos[j].GetValue(compOp.RunTimeComponent) as Transform;
                                    gObj = asTransform.gameObject;
                                }
                                else
                                {
                                    gObj = propertyInfos[j].GetValue(compOp.RunTimeComponent) as GameObject;
                                }

                                VirtualGameObject virtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromObject(gObj);
                                if (virtualObj != null)
                                {
                                    if (typeof(Transform).IsAssignableFrom(propertyInfos[j].PropertyType))
                                    {
                                        propertyInfos[j].SetValue(compOp.RunTimeComponent, virtualObj.RunTimeObject.transform);
                                    }
                                    else
                                    {
                                        propertyInfos[j].SetValue(compOp.RunTimeComponent, virtualObj.RunTimeObject);
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (!bFoundAsProperty)
                    {
                        System.Reflection.FieldInfo[] fields = compOp.ComponentType.GetFields();
                        for (int j = 0; j < fields.Length; j++)
                        {
                            if (fields[j].IsPublic && fields[j].Name == pair.Key.GetPropertyName())
                            {
                                fields[j].SetValue(compOp.RunTimeComponent, pair.Value);

                                // If preserved object is a GameObject or Component, match it into newly created avatar
                                if (typeof(GameObject).IsAssignableFrom(fields[j].FieldType) || typeof(Transform).IsAssignableFrom(fields[j].FieldType))
                                {
                                    GameObject gObj = null;
                                    if (typeof(Transform).IsAssignableFrom(fields[j].FieldType))
                                    {
                                        Transform AsTransform = fields[j].GetValue(compOp.RunTimeComponent) as Transform;
                                        gObj = AsTransform.gameObject;
                                    }
                                    else
                                    {
                                        gObj = fields[j].GetValue(compOp.RunTimeComponent) as GameObject;
                                    }

                                    VirtualGameObject virtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromObject(gObj);
                                    if (virtualObj != null)
                                    {
                                        if (typeof(Transform).IsAssignableFrom(fields[j].FieldType))
                                        {
                                            fields[j].SetValue(compOp.RunTimeComponent, virtualObj.RunTimeObject.transform);
                                        }
                                        else
                                        {
                                            fields[j].SetValue(compOp.RunTimeComponent, virtualObj.RunTimeObject);
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        protected void VerySpecificPreCompOperations()
        {
            SkinnedMeshRendererBlendshapes = new Dictionary<ComponentOperation, Dictionary<string, float>>();
            //RendererSavedMaterial = new Dictionary<ComponentOperation, Material[]>();
            EyeLookSettingDict = new Dictionary<OverridingComponentOperation, EyeLookSetting>();

            // Save Skinned Mesh Renderer Blendshapes
            if (ComponentOperationHandler.GetInstance().TypeCompOpLists.ContainsKey(typeof(SkinnedMeshRenderer)))
            {
                TypeComponentOperationList typeCopyList = ComponentOperationHandler.GetInstance().TypeCompOpLists[typeof(SkinnedMeshRenderer)];
                for (int typeCompOpIndex = 0; typeCompOpIndex < typeCopyList.GetPreExistingCount(); typeCompOpIndex++)
                {
                    PreExistingComponentOperation preExistingCompOp = typeCopyList.GetPreExisting(typeCompOpIndex);
                    Dictionary<string, float> blendShapeValueDict = new Dictionary<string, float>();

                    // Add Blendshapse from the Copy From Skinned Mesh Renderer
                    if (preExistingCompOp.IsBeingOverriden)
                    {
                        SkinnedMeshRenderer copyFromSMR = preExistingCompOp.OverridingCompOp.OriginComponent as SkinnedMeshRenderer;
                        if (copyFromSMR.sharedMesh != null)
                        {
                            for (int BlendShapeIndex = 0; BlendShapeIndex < copyFromSMR.sharedMesh.blendShapeCount; BlendShapeIndex++)
                            {
                                blendShapeValueDict.Add(copyFromSMR.sharedMesh.GetBlendShapeName(BlendShapeIndex), copyFromSMR.GetBlendShapeWeight(BlendShapeIndex));
                            }
                        }
                    }

                    // Add Blendshapse from the Copy To Skinned Mesh Renderer if they don't already exist
                    SkinnedMeshRenderer copyToSMR = preExistingCompOp.OriginComponent as SkinnedMeshRenderer;
                    if (copyToSMR != null && copyToSMR.sharedMesh != null)
                    {
                        for (int BlendShapeIndex = 0; BlendShapeIndex < copyToSMR.sharedMesh.blendShapeCount; BlendShapeIndex++)
                        {
                            // Don't add if they already exist. CopyFrom blendshape values are prioritized
                            string BlendShapeName = copyToSMR.sharedMesh.GetBlendShapeName(BlendShapeIndex);
                            if (blendShapeValueDict.ContainsKey(BlendShapeName) == false)
                            {
                                blendShapeValueDict.Add(BlendShapeName, copyToSMR.GetBlendShapeWeight(BlendShapeIndex));
                            }
                        }
                    }

                    if (blendShapeValueDict.Count > 0)
                    {
                        SkinnedMeshRendererBlendshapes.Add(preExistingCompOp, blendShapeValueDict);
                    }
                }
            }

            // Save Renderer Materials - Save regardless of settings
            //foreach (System.Type t in ComponentOperationHandler.GetInstance().TypeCompOpLists.Keys)
            //{
            //    if (typeof(Renderer).IsAssignableFrom(t))
            //    {
            //        for (int typeCompOpIndex = 0; typeCompOpIndex < ComponentOperationHandler.GetInstance().TypeCompOpLists[t].GetPreExistingCount(); typeCompOpIndex++)
            //        {
            //            PreExistingComponentOperation preExistingCompOp = ComponentOperationHandler.GetInstance().TypeCompOpLists[t].GetPreExisting(typeCompOpIndex);

            //            if (preExistingCompOp.IsBeingOverriden == false)
            //            {
            //                continue;
            //            }

            //            Renderer fromRenderer = preExistingCompOp.OverridingCompOp.OriginComponent as Renderer;

            //            RendererSavedMaterial.Add(preExistingCompOp, fromRenderer.sharedMaterials.Clone() as Material[]);
            //        }
            //    }
            //}

            // Save VRC Descriptor Eye Looking Blendshape setting names
            foreach (System.Type t in ComponentOperationHandler.GetInstance().TypeCompOpLists.Keys)
            {
                if (t == typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor))
                {
                    TypeComponentOperationList typeList = ComponentOperationHandler.GetInstance().TypeCompOpLists[t];

                    for (int i = 0; i < typeList.GetOverridingCount(); i++)
                    {
                        OverridingComponentOperation overridingCompOp = typeList.GetOverriding(i);

                        if (overridingCompOp.IsFullyEnabled() == false)
                        {
                            continue;
                        }

                        VRC.SDK3.Avatars.Components.VRCAvatarDescriptor descriptor = overridingCompOp.OriginComponent as VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

                        SkinnedMeshRenderer smr = descriptor.VisemeSkinnedMesh;
                        if (smr == null || smr.sharedMesh == null)
                        {
                            continue;
                        }

                        EyeLookSetting eyeLookSetting = new EyeLookSetting();

                        // Blink [0] Index
                        int blinkIndex = descriptor.customEyeLookSettings.eyelidsBlendshapes[0];
                        if (blinkIndex >= smr.sharedMesh.blendShapeCount)
                        {
                            blinkIndex = -1;
                        }

                        if (blinkIndex != -1)
                        {
                            eyeLookSetting.BlinkName = smr.sharedMesh.GetBlendShapeName(blinkIndex);
                        }


                        // Looking Up [1] Index
                        int lookingUpIndex = descriptor.customEyeLookSettings.eyelidsBlendshapes[1];
                        if (lookingUpIndex >= smr.sharedMesh.blendShapeCount)
                        {
                            lookingUpIndex = -1;
                        }

                        if (lookingUpIndex != -1)
                        {
                            eyeLookSetting.LookingUpName = smr.sharedMesh.GetBlendShapeName(lookingUpIndex);
                        }


                        // Looking Up [1] Index
                        int lookingDownIndex = descriptor.customEyeLookSettings.eyelidsBlendshapes[2];
                        if (lookingDownIndex >= smr.sharedMesh.blendShapeCount)
                        {
                            lookingDownIndex = -1;
                        }

                        if (lookingDownIndex != -1)
                        {
                            eyeLookSetting.LookingDownName = smr.sharedMesh.GetBlendShapeName(lookingDownIndex);
                        }


                        EyeLookSettingDict.Add(overridingCompOp, eyeLookSetting);
                    }

                    break;
                }
            }
        }

        // For things like setting up bone structure on SkinnedMeshRenderers
        protected void VerySpecificPostCompOperations()
        {
            // Apply Skinned Mesh Renderer Blendshapes
            if (ComponentOperationHandler.GetInstance().TypeCompOpLists.ContainsKey(typeof(SkinnedMeshRenderer)))
            {
                TypeComponentOperationList typeCopyList = ComponentOperationHandler.GetInstance().TypeCompOpLists[typeof(SkinnedMeshRenderer)];
                for (int typeCompOpListIndex = 0; typeCompOpListIndex < typeCopyList.GetPreExistingCount(); typeCompOpListIndex++)
                {
                    PreExistingComponentOperation preExistingCompOp = typeCopyList.GetPreExisting(typeCompOpListIndex);
                    if (preExistingCompOp.IsBeingOverriden == false)
                    {
                        continue;
                    }

                    // Correct BlendShapes
                    if (SkinnedMeshRendererBlendshapes.ContainsKey(preExistingCompOp))
                    {
                        SkinnedMeshRenderer SMR = preExistingCompOp.RunTimeComponent as SkinnedMeshRenderer;

                        if (SMR.sharedMesh != null)
                        {

                            Dictionary<string, float> BlendShapeValueDict = SkinnedMeshRendererBlendshapes[preExistingCompOp];

                            for (int BlendShapeIndex = 0; BlendShapeIndex < SMR.sharedMesh.blendShapeCount; BlendShapeIndex++)
                            {

                                string BlendShapeName = SMR.sharedMesh.GetBlendShapeName(BlendShapeIndex);
                                if (BlendShapeValueDict.ContainsKey(BlendShapeName) == false)
                                {
                                    continue;
                                }

                                SMR.SetBlendShapeWeight(BlendShapeIndex, BlendShapeValueDict[BlendShapeName]);
                            }
                        }
                    }

                    // Set up Skinned Mesh Renderer Bones
                    if (preExistingCompOp.ComponentType == typeof(SkinnedMeshRenderer))
                    {
                        SkinnedMeshRenderer SRenderer = preExistingCompOp.RunTimeComponent as SkinnedMeshRenderer;

                        Transform[] OldBones = SRenderer.bones.Clone() as Transform[];
                        Transform[] NewBones = new Transform[OldBones.Length];
                        for (int j = 0; j < OldBones.Length; j++)
                        {
                            foreach (var NewBone in SRenderer.rootBone.GetComponentsInChildren<Transform>(true))
                            {
                                if (NewBone.name == OldBones[j].name)
                                {
                                    NewBones[j] = NewBone;
                                    continue;
                                }
                            }
                        }

                        SRenderer.bones = NewBones;
                    }
                }
            }


            // Copying Materials
            //if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bCopyMaterials"))
            //{
            //    foreach (System.Type t in ComponentOperationHandler.GetInstance().TypeCompOpLists.Keys)
            //    {
            //        if (typeof(Renderer).IsAssignableFrom(t))
            //        {
            //            for (int typeCompOpIndex = 0; typeCompOpIndex < ComponentOperationHandler.GetInstance().TypeCompOpLists[t].GetPreExistingCount(); typeCompOpIndex++)
            //            {
            //                PreExistingComponentOperation preExistingCompOp = ComponentOperationHandler.GetInstance().TypeCompOpLists[t].GetPreExisting(typeCompOpIndex);

            //                if (preExistingCompOp.IsBeingOverriden == false)
            //                {
            //                    continue;
            //                }

            //                if (RendererSavedMaterial.ContainsKey(preExistingCompOp) == false)
            //                {
            //                    continue;
            //                }

            //                if (CopierSettingsHandler.GetInstance().GetBoolDataValue("bSmartCopyMaterials"))
            //                {
            //                    Renderer runtimeRenderer = preExistingCompOp.RunTimeComponent as Renderer;
            //                    LinkedSmartMaterialList linkedMatList = new LinkedSmartMaterialList(runtimeRenderer.sharedMaterials, RendererSavedMaterial[preExistingCompOp]);

            //                    runtimeRenderer.sharedMaterials = linkedMatList.CreateSmartMaterialList();
            //                }
            //                // if not smartly, just copy over based on indexes
            //                else
            //                {
            //                    Renderer runtimeRenderer = preExistingCompOp.RunTimeComponent as Renderer;
            //                    Material[] savedMaterials = RendererSavedMaterial[preExistingCompOp];
            //                    Material[] newMaterials = runtimeRenderer.sharedMaterials.Clone() as Material[];

            //                    for (int i = 0; i < runtimeRenderer.sharedMaterials.Length && i < RendererSavedMaterial[preExistingCompOp].Length; i++)
            //                    {
            //                        newMaterials[i] = savedMaterials[i];
            //                    }

            //                    runtimeRenderer.sharedMaterials = newMaterials;
            //                }
            //            }
            //        }
            //    }
            //}

            // Applying Avatar Descriptor Eye Blendshapes
            foreach (OverridingComponentOperation overridingCompOp in EyeLookSettingDict.Keys)
            {
                VRC.SDK3.Avatars.Components.VRCAvatarDescriptor descriptor = overridingCompOp.RunTimeComponent as VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

                SkinnedMeshRenderer smr = descriptor.VisemeSkinnedMesh;

                if (smr == null)
                {
                    continue;
                }

                EyeLookSetting eyeLookSetting = EyeLookSettingDict[overridingCompOp];

                if (eyeLookSetting.BlinkName != "")
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[0] = smr.sharedMesh.GetBlendShapeIndex(eyeLookSetting.BlinkName);
                }
                else
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[0] = -1;
                }

                if (eyeLookSetting.LookingUpName != "")
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[1] = smr.sharedMesh.GetBlendShapeIndex(eyeLookSetting.LookingUpName);
                }
                else
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[1] = -1;
                }

                if (eyeLookSetting.LookingDownName != "")
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[2] = smr.sharedMesh.GetBlendShapeIndex(eyeLookSetting.LookingDownName);
                }
                else
                {
                    descriptor.customEyeLookSettings.eyelidsBlendshapes[2] = -1;
                }
            }
        }

        protected void ApplyMaterialOperations()
        {
            for (int i = 0; i < MaterialOperationHandler.GetInstance().GetMaterialOperationCount(); i++)
            {
                MaterialOperation materialOp = MaterialOperationHandler.GetInstance().GetMaterialOperation(i);

                if (!materialOp.IsFullyEnabled())
                {
                    continue;
                }

                materialOp.ApplyMaterialOperation();
            }
        }

        protected void ApplyScaleOperations()
        {
            for (int i = 0; i < ScaleOperationHandler.GetInstance().GetScaleOperationCount(); i++)
            {
                ScaleOperation scaleOp = ScaleOperationHandler.GetInstance().GetScaleOperation(i);

                if (!scaleOp.IsFullyEnabled())
                {
                    continue;
                }

                _AdjustedScales++;

                scaleOp.VirtualGameObj.RunTimeObject.transform.localScale = scaleOp.GetScale();
            }
        }

        protected void ApplyEnabledDisabledOperations()
        {
            /*if (!MergerSettingsHandler.GetInstance().bCopyEnabledDisabledStatus)
            {
                return;
            }*/

            for (int i = 0; i < EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperationCount(); i++)
            {
                EnabledDisabledOperation enabledDisabledOperation = EnabledDisabledOperationHandler.GetInstance().GetEnabledDisabledOperation(i);

                if (!enabledDisabledOperation.IsFullyEnabled())
                {
                    continue;
                }

                _EnabledStatusesChanged++;

                enabledDisabledOperation.GetVirtualObject().RunTimeObject.SetActive(enabledDisabledOperation.EnabledStatus);
            }
        }

        protected void ApplyMiscOperationsPreAttachablesAttachment()
        {
            // Reposition Op
            if (MiscOperationHandler.GetInstance().RepositionOp != null && MiscOperationHandler.GetInstance().RepositionOp.IsFullyEnabled())
            {
                // Position
                if (MiscOperationHandler.GetInstance().RepositionOp.bUserSetPositionEnabled)
                {
                    MergeTreeHandler.GetInstance().VirtualTreeRoot.RunTimeObject.transform.localPosition = MiscOperationHandler.GetInstance().RepositionOp.RepositionLocation;
                }

                // Rotation
                if (MiscOperationHandler.GetInstance().RepositionOp.bUserSetRotationEnabled)
                {
                    MergeTreeHandler.GetInstance().VirtualTreeRoot.RunTimeObject.transform.localEulerAngles = MiscOperationHandler.GetInstance().RepositionOp.RepositionRotation;
                }

                // Scale
                if (MiscOperationHandler.GetInstance().RepositionOp.bUserSetScaleEnabled)
                {
                    MergeTreeHandler.GetInstance().VirtualTreeRoot.RunTimeObject.transform.localScale = MiscOperationHandler.GetInstance().RepositionOp.RepositionScale;
                }
            }
        }

        protected void ApplyMiscOperationsPostAttachableAttachment()
        {
            if (MiscOperationHandler.GetInstance().AvatarIDOp != null)
            {
                // Avatar Blueprint ID
                VRC.Core.PipelineManager pipelineManager = MergeTreeHandler.GetInstance().VirtualTreeRoot.RunTimeObject.GetComponentInChildren<VRC.Core.PipelineManager>(true);
                // If doesnt exist, create one... I think this is ok to do! x'D
                if (pipelineManager == null)
                {
                    pipelineManager = Undo.AddComponent<VRC.Core.PipelineManager>(MergeTreeHandler.GetInstance().VirtualTreeRoot.RunTimeObject);
                }

                if (MiscOperationHandler.GetInstance().AvatarIDOp.IsFullyEnabled())
                {
                    pipelineManager.blueprintId = MiscOperationHandler.GetInstance().AvatarIDOp.AvatarID;
                }
                else
                {
                    pipelineManager.blueprintId = "";
                }
            }
        }


    }
}
#endif