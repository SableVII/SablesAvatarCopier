#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;

namespace SablesTools.AvatarCopier.Handlers
{
    // Organzed collection of CompOps by component type
    public class TypeComponentOperationList
    {
        protected List<PreExistingComponentOperation> _PreExistingOps = new List<PreExistingComponentOperation>();
        protected List<OverridingComponentOperation> _OverridingOps = new List<OverridingComponentOperation>();
        public bool bPreExistingOpenInUI { get; set; } = false;
        public bool bOverridingOpenInUI { get; set; } = false;
        public int GetTotalCompOpCount()
        {
            return _PreExistingOps.Count + _OverridingOps.Count;
        }

        public int GetPreExistingCount()
        {
            return _PreExistingOps.Count;
        }

        public int GetOverridingCount()
        {
            return _OverridingOps.Count;
        }

        // Potential Optimization: Store this count
        public int GetPreExistingPreExistsCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingOp in _PreExistingOps)
            {
                if (preExistingOp.IsBeingOverriden == false)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetPreExistingPreExistsEnabledCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingOp in _PreExistingOps)
            {
                if (preExistingOp.IsFullyEnabled() && preExistingOp.IsBeingOverriden == false)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetPreExistingReplacedCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingOp in _PreExistingOps)
            {
                if (preExistingOp.IsBeingOverriden)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetPreExistingReplacedEnabledCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingOp in _PreExistingOps)
            {
                if (preExistingOp.IsFullyEnabled() && preExistingOp.IsBeingOverriden)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetOverridingReplacingEnabledCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation overridingOp in _OverridingOps)
            {
                if (overridingOp.IsFullyEnabled() && overridingOp.IsReplacing)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetOverridingNewEnabledCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation overridingOp in _OverridingOps)
            {
                if (overridingOp.IsFullyEnabled() && overridingOp.IsReplacing == false)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetOverridingReplacingCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation overridingOp in _OverridingOps)
            {
                if (overridingOp.IsReplacing)
                {
                    count++;
                }
            }

            return count;
        }

        // Potential Optimization: Store this count
        public int GetOverridingNewCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation overridingOp in _OverridingOps)
            {
                if (overridingOp.IsReplacing == false)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetEnabledPreExistingCount()
        {
            int count = 0;
            for (int i = 0; i < _PreExistingOps.Count; i++)
            {
                if (_PreExistingOps[i].IsFullyEnabled())
                {
                    count++;
                }
            }

            return count;
        }

        public int GetEnabledOverridingCount()
        {
            int count = 0;
            for (int i = 0; i < _OverridingOps.Count; i++)
            {
                if (_OverridingOps[i].IsFullyEnabled())
                {
                    count++;
                }
            }

            return count;
        }


        public void AddPreExisting(PreExistingComponentOperation preExisting)
        {
            if (preExisting.ComponentType != ComponentType)
            {
                return;
            }

            _PreExistingOps.Add(preExisting);
        }

        public void AddOverriding(OverridingComponentOperation overriding)
        {
            if (overriding.ComponentType != ComponentType)
            {
                return;
            }

            _OverridingOps.Add(overriding);
        }

        public PreExistingComponentOperation GetPreExisting(int index)
        {
            return _PreExistingOps[index];
        }

        public OverridingComponentOperation GetOverriding(int index)
        {
            return _OverridingOps[index];
        }


        // Returns True if the Overridable Component Operation was found and removed.
        public bool RemoveOverrideable(OverridingComponentOperation overriding)
        {
            for (int i = 0; i < _OverridingOps.Count; i++)
            {
                if (_OverridingOps[i] == overriding)
                {
                    _OverridingOps.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public System.Type ComponentType { get; }

        public TypeComponentOperationList(System.Type compType)
        {
            ComponentType = compType;
        }

        public bool PreExistingHasWarning()
        {
            foreach (PreExistingComponentOperation preExistingCompOp in _PreExistingOps)
            {
                if (preExistingCompOp.HasUnSuppressedWarning())
                {
                    return true;
                }
            }

            return false;
        }

        public bool OverridingHasWarning()
        {
            foreach (OverridingComponentOperation overiddingCompOp in _OverridingOps)
            {
                if (overiddingCompOp.HasUnSuppressedWarning())
                {
                    return true;
                }
            }

            return false;
        }

        public void ReOrganize()
        {
            List<PreExistingComponentOperation> organizedPreExisting = new List<PreExistingComponentOperation>();
            List<OverridingComponentOperation> organizedOverriding = new List<OverridingComponentOperation>();

            // Organize Pre-Existing
            for (int preExistIndex = 0; preExistIndex < _PreExistingOps.Count; preExistIndex++)
            {
                int organizedIndex = 0;
                bool bPlaced = false;
                while (organizedIndex < organizedPreExisting.Count)
                {
                    if (_PreExistingOps[preExistIndex].VirtualGameObjectRef.VirtualTreeOrder < organizedPreExisting[organizedIndex].VirtualGameObjectRef.VirtualTreeOrder)
                    {
                        organizedPreExisting.Insert(organizedIndex, _PreExistingOps[preExistIndex]);
                        bPlaced = true;
                        break;
                    }

                    organizedIndex++;
                }

                if (!bPlaced)
                {
                    organizedPreExisting.Add(_PreExistingOps[preExistIndex]);
                }
            }

            _PreExistingOps = organizedPreExisting;

            // Organize Overriding
            for (int overridingIndex = 0; overridingIndex < _OverridingOps.Count; overridingIndex++)
            {
                int organizedIndex = 0;
                bool bPlaced = false;
                while (organizedIndex < organizedOverriding.Count)
                {
                    if (_OverridingOps[overridingIndex].VirtualGameObjectRef.VirtualTreeOrder < organizedOverriding[organizedIndex].VirtualGameObjectRef.VirtualTreeOrder)
                    {
                        organizedOverriding.Insert(organizedIndex, _OverridingOps[overridingIndex]);
                        bPlaced = true;
                        break;
                    }

                    organizedIndex++;
                }

                if (!bPlaced)
                {
                    organizedOverriding.Add(_OverridingOps[overridingIndex]);
                }
            }

            _OverridingOps = organizedOverriding;
        }
        //public bool IsFullyEnabled()
        //{
        //    if (!MergerSettingsHandler.GetInstance().bCopyComponents)
        //    {
        //        return false;
        //    }

        //    return bUserSetEnabled;
        //}
    }


    // Handles everything ComponentOperation
    public class ComponentOperationHandler : OperationHandler
    {
        private static ComponentOperationHandler _Instance = null;

        public static ComponentOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new ComponentOperationHandler();
            }

            return _Instance;
        }

        private ComponentOperationHandler()
        {

        }

        public Dictionary<System.Type, TypeComponentOperationList> TypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>();
        public Dictionary<System.Type, TypeComponentOperationList> SavedTypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>();
        public List<OverridingComponentOperation> UnusedComponentOperations = new List<OverridingComponentOperation>();

        public Dictionary<Component, ComponentOperation> ComponentOperations = new Dictionary<Component, ComponentOperation>();
        public Dictionary<Component, ComponentOperation> SavedComponentOperations = new Dictionary<Component, ComponentOperation>();

        public List<PreExistingComponentOperation> PreExistingCompOperations = new List<PreExistingComponentOperation>();
        public List<OverridingComponentOperation> OverridingCompOperations = new List<OverridingComponentOperation>();

        protected Warnings.AvatarCopierUnusedWarning UnusedWarningRef = null;

        protected bool _bRegisteredForRefCheck = false;

        public void Reset()
        {
            TypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>();
            SavedTypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>();
            UnusedComponentOperations = new List<OverridingComponentOperation>();

            ComponentOperations = new Dictionary<Component, ComponentOperation>();
            SavedComponentOperations = new Dictionary<Component, ComponentOperation>();

            PreExistingCompOperations = new List<PreExistingComponentOperation>();
            OverridingCompOperations = new List<OverridingComponentOperation>();

            UnusedWarningRef = null;
        }

        public void SavedReset()
        {
            SavedTypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>(TypeCompOpLists);
            SavedComponentOperations = new Dictionary<Component, ComponentOperation>(ComponentOperations);

            TypeCompOpLists = new Dictionary<System.Type, TypeComponentOperationList>();
            UnusedComponentOperations = new List<OverridingComponentOperation>();
            ComponentOperations = new Dictionary<Component, ComponentOperation>();
            PreExistingCompOperations = new List<PreExistingComponentOperation>();
            OverridingCompOperations = new List<OverridingComponentOperation>();
            UnusedWarningRef = null;
        }

        public void ApplySavedData()
        {
            // Apply Saved CompOps
            foreach (Component savedCompKey in SavedComponentOperations.Keys)
            {
                if (ComponentOperations.ContainsKey(savedCompKey) == false)
                {
                    continue;
                }

                ComponentOperation savedCompOp = SavedComponentOperations[savedCompKey];
                ComponentOperation compOp = ComponentOperations[savedCompKey];

                if (savedCompOp.OriginComponent == compOp.OriginComponent)
                {
                    // Comp Op Settings
                    compOp.bUserSetEnabled = savedCompOp.bUserSetEnabled;
                    compOp.RegisteredRefCollection.bOpenInUI = savedCompOp.RegisteredRefCollection.bOpenInUI;

                    // Correcting Attached to VirtualObject Parent. 
                    if (savedCompOp.VirtualGameObjectRef != null)
                    {
                        // If the new Component Operation has no virtual reference or if it's virtual reference is differerent from what is saved
                        if ((compOp.VirtualGameObjectRef == null && savedCompOp.VirtualGameObjectRef.GetOriginGameObject() != null) || compOp.VirtualGameObjectRef.GetOriginGameObject() != savedCompOp.VirtualGameObjectRef.GetOriginGameObject())
                        {
                            VirtualGameObject NewVirtualAttachParent = null;
                            if (savedCompOp.VirtualGameObjectRef.bSourceIsOriginGameObject)
                            {
                                NewVirtualAttachParent = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(savedCompOp.VirtualGameObjectRef.LinkedSource); // ? Use Get Origin ?
                            }
                            else
                            {
                                NewVirtualAttachParent = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromDestinationObject(savedCompOp.VirtualGameObjectRef.LinkedDestination); // ? Use Get Origin ?
                            }

                            ChangeCompOpVirtualObjectParent(compOp as OverridingComponentOperation, NewVirtualAttachParent);
                            RefreshCompOps();
                        }
                    }

                    // Set Show CompOp
                    if (compOp.VirtualGameObjectRef != null && savedCompOp.VirtualGameObjectRef != null)
                    {
                        compOp.VirtualGameObjectRef.bShowPreExistingCompOps = savedCompOp.VirtualGameObjectRef.bShowPreExistingCompOps;
                        compOp.VirtualGameObjectRef.bShowOverridingCompOps = savedCompOp.VirtualGameObjectRef.bShowOverridingCompOps;
                    }

                    // Registered References
                    foreach (RegisteredReference savedRegisteredRef in savedCompOp.RegisteredRefCollection.RegisteredReferences)
                    {
                        foreach (RegisteredReference registeredRef in compOp.RegisteredRefCollection.RegisteredReferences)
                        {
                            if (savedRegisteredRef.PropFieldName != registeredRef.PropFieldName)
                            {
                                continue;
                            }

                            // Force reset of field/prop if counts don't equal
                            if (registeredRef.GetReferenceCount() != savedRegisteredRef.GetReferenceCount())
                            {
                                continue;
                            }

                            // Set parameters from saved as much as it can
                            for (int k = 0; k < registeredRef.GetReferenceCount(); k++)
                            {
                                RegisteredReferenceElement savedRefElement = savedRegisteredRef.GetReferenceElement(k);
                                RegisteredReferenceElement refElement = registeredRef.GetReferenceElement(k);
                                if (savedRefElement.bIsUserSet == false)
                                {
                                    continue;
                                }

                                if (savedRefElement.bIsNotVirtualGameObjectType)
                                {
                                    if (savedRefElement.GameObjectReference != null)
                                    {
                                        refElement.GameObjectReference = savedRefElement.GameObjectReference;
                                    }
                                }
                                else
                                {
                                    // Set to null if userset to null
                                    if (savedRefElement.VirtualReference == null)
                                    {
                                        refElement.VirtualReference = null;
                                        continue;
                                    }

                                    // Get new Virtual Object based on the LinkedDestination gameobject. If not found, then this old reference is no-longer valid.
                                    VirtualGameObject DestinationLinked = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromDestinationObject(savedRefElement.VirtualReference.LinkedDestination);
                                    if (DestinationLinked == null)
                                    {
                                        continue;
                                    }

                                    // Just incase the Linked Source was originally set to null, take whatever was set in the field
                                    if (savedRefElement.VirtualReference.LinkedSource == null)
                                    {
                                        refElement.VirtualReference = DestinationLinked;
                                        continue;
                                    }

                                    // Get new Virtual Object based on the LinkedSource gameobject.  If not found, then this old reference is no-longer valid.
                                    VirtualGameObject SourceLinked = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(savedRefElement.VirtualReference.LinkedSource);
                                    if (SourceLinked == null)
                                    {
                                        // Failed to link, then ref is no-longer valid
                                        continue;
                                    }

                                    // If both Linked Virtual Objects match, then we know for sure(?) that we have the correct Virtual object from the saved RegisteredRef.
                                    if (DestinationLinked == SourceLinked)
                                    {
                                        refElement.VirtualReference = DestinationLinked;
                                    }
                                }

                                //// Special Ref
                                //if (SavedRefElement.SpecialOutOfAvatarRef != null)
                                //{
                                //    // Easy peasy
                                //    RefElement.SpecialOutOfAvatarRef = SavedRefElement.SpecialOutOfAvatarRef;
                                //    continue;
                                //}





                                /*if (SavedRegisteredRef.GetReferenceElement(k).OriginalVirtualReference != SavedRegisteredRef.GetReferenceElement(k).VirtualReference)
                                {
                                    RegisteredRef.GetReferenceElement(k).VirtualReference = SavedRegisteredRef.GetReferenceElement(k).VirtualReference;
                                }*/
                            }

                            break;
                        }
                    }
                }
            }

            // Type List
            foreach (System.Type savedType in SavedTypeCompOpLists.Keys)
            {
                if (!TypeCompOpLists.ContainsKey(savedType))
                {
                    continue;
                }

                TypeCompOpLists[savedType].bPreExistingOpenInUI = SavedTypeCompOpLists[savedType].bPreExistingOpenInUI;
                TypeCompOpLists[savedType].bOverridingOpenInUI = SavedTypeCompOpLists[savedType].bOverridingOpenInUI;
            }
        }

        public void GenerateComponentOperations()
        {
            GenerateComponentOperations_R(MergeTreeHandler.GetInstance().VirtualTreeRoot);

            // Make sure to generate CompOps for TopLevelAttachableVirtualObjs that arn't attached to the Virtual Tree
            for (int i = 0; i < AvatarMatchHandler.GetInstance().TopLevelAttachableGameObjectsCount; i++)
            {
                VirtualGameObject topLevelVirtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(AvatarMatchHandler.GetInstance().GetTopLevelAttachableGameObject(i));
                if (topLevelVirtualObj.Parent == null)
                {
                    GenerateComponentOperations_R(topLevelVirtualObj);
                }
            }

            // Find Unused Component Operations
            FindUnusedComponentOperations_R(CopierSettingsHandler.GetInstance().Source);
        }

        protected void GenerateComponentOperations_R(VirtualGameObject currentVirtualObj)
        {
            if (currentVirtualObj.bIsAttachable)
            {
                AddAttachableObjectCompOp(currentVirtualObj);
            }
            else
            {
                AddVirtualObjectCompOp(currentVirtualObj);
            }

            for (int i = 0; i < currentVirtualObj.GetChildCount(); i++)
            {
                GenerateComponentOperations_R(currentVirtualObj.GetChild(i));
            }
        }

        protected void FindUnusedComponentOperations_R(GameObject currentSourceGameObject)
        {
            if (AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(currentSourceGameObject) == null)
            {
                List<Component> unmatchedSourceComponents = new List<Component>(currentSourceGameObject.GetComponents(typeof(Component)));

                foreach (Component comp in unmatchedSourceComponents)
                {
                    if (AvatarCopierUtils.AllowedCopyTypes.Contains(comp.GetType()))
                    {
                        OverridingComponentOperation newOverridingCompOp = new OverridingComponentOperation(comp, null);
                        UnusedComponentOperations.Add(newOverridingCompOp);
                        ComponentOperations.Add(comp, newOverridingCompOp);

                        if (comp.GetType() != typeof(VRC.Core.PipelineManager)) // Ignore PipelineManagers as that is handled via Misc Operation
                        {
                            RegisteredReferenceUtils.RegisterReferences(newOverridingCompOp);
                        }

                        if (UnusedWarningRef == null)
                        {
                            UnusedWarningRef = new Warnings.AvatarCopierUnusedWarning();
                            WarningHandler.GetInstance().AddWarning(UnusedWarningRef);
                        }
                    }
                }                         
            }

            for (int i = 0; i < currentSourceGameObject.transform.childCount; i++)
            {
                FindUnusedComponentOperations_R(currentSourceGameObject.transform.GetChild(i).gameObject);
            }
        }

        public void AddAttachableObjectCompOp(VirtualGameObject attachableVirtualObj)
        {
            // Create Pre-Existing from Attachable Source
            List<Component> sourceComponents = new List<Component>();
            if (attachableVirtualObj.LinkedSource != null)
            {
                sourceComponents.AddRange(attachableVirtualObj.LinkedSource.GetComponents(typeof(Component)));
            }

            for (int i = 0; i < sourceComponents.Count; i++)
            {
                Component comp = sourceComponents[i];
                System.Type compType = comp.GetType();

                if (AvatarCopierUtils.AllowedCopyTypes.Contains(compType) == false)
                {
                    continue;
                }

                int compTypeIndex = 0;

                for (int j = 0; j < i; j++)
                {
                    if (compType == sourceComponents[j].GetType())
                    {
                        compTypeIndex++;
                    }
                }

                PreExistingComponentOperation newPreExistingCompOps = CreatePreExistingCompOp(attachableVirtualObj, comp, compTypeIndex);
            }
        }

        public void AddVirtualObjectCompOp(VirtualGameObject virtualObj)
        {
            List<Component> destinationComponents = new List<Component>();
            if (virtualObj.LinkedDestination != null)
            {
                destinationComponents.AddRange(virtualObj.LinkedDestination.GetComponents(typeof(Component)));
            }

            // This list will have components removed from it if a match is found.
            List<Component> sourceComponents = new List<Component>();
            if (virtualObj.LinkedSource != null)
            {
                sourceComponents.AddRange(virtualObj.LinkedSource.GetComponents(typeof(Component)));
            }

            List<PreExistingComponentOperation> preExistingCompOps = new List<PreExistingComponentOperation>();

            /// Add Pre-Existing Component Operations            
            for (int i = 0; i < destinationComponents.Count; i++)
            {
                Component comp = destinationComponents[i];
                System.Type compType = comp.GetType();

                if (AvatarCopierUtils.AllowedCopyTypes.Contains(compType) == false)
                {
                    continue;
                }

                int compTypeIndex = 0;

                for (int j = 0; j < i; j++)
                {
                    if (compType == destinationComponents[j].GetType())
                    {
                        compTypeIndex++;
                    }
                }

                PreExistingComponentOperation newPreExistingCompOp = CreatePreExistingCompOp(virtualObj, comp, compTypeIndex);

                if (newPreExistingCompOp != null)
                {
                    preExistingCompOps.Add(newPreExistingCompOp);
                }
            }

            // Now add all remaining extra components from the Overriding/Source Object
            foreach (Component comp in sourceComponents)
            {
                System.Type compType = comp.GetType();
                if (AvatarCopierUtils.AllowedCopyTypes.Contains(compType) == false)
                {
                    continue;
                }

                OverridingComponentOperation newOverrideableCompOp = CreateOverridingCompOp(virtualObj, comp);
            }

            virtualObj.RefreshWhatIsOverwritten();
        }

        public PreExistingComponentOperation CreatePreExistingCompOp(VirtualGameObject virtualObj, Component comp, int compTypeIndex)
        {
            System.Type compType = comp.GetType();
            if (AvatarCopierUtils.AllowedCopyTypes.Contains(compType) == false)
            {
                return null;
            }

            PreExistingComponentOperation newPreExistingCompOp = new PreExistingComponentOperation(comp, virtualObj, compTypeIndex);

            if (compType != typeof(VRC.Core.PipelineManager)) // Ignore PipelineManagers as that is handled via Misc Operation
            {
                RegisteredReferenceUtils.RegisterReferences(newPreExistingCompOp);
            }

            // Add to TypeCompOperations List
            if (!TypeCompOpLists.ContainsKey(compType))
            {
                TypeCompOpLists.Add(compType, new TypeComponentOperationList(compType));

            }
            TypeCompOpLists[compType].AddPreExisting(newPreExistingCompOp);

            virtualObj.AddPreExisting(newPreExistingCompOp);

            ComponentOperations.Add(comp, newPreExistingCompOp);

            PreExistingCompOperations.Add(newPreExistingCompOp);

            return newPreExistingCompOp;
        }

        public OverridingComponentOperation CreateOverridingCompOp(VirtualGameObject virtualObj, Component comp)
        {
            System.Type compType = comp.GetType();
            if (AvatarCopierUtils.AllowedCopyTypes.Contains(compType) == false)
            {
                return null;
            }

            OverridingComponentOperation newOverrideableCompOp = new OverridingComponentOperation(comp, virtualObj);

            if (compType != typeof(VRC.Core.PipelineManager)) // Ignore PipelineManagers as that is handled via Misc Operation
            {
                RegisteredReferenceUtils.RegisterReferences(newOverrideableCompOp);
            }

            // Add to TypeCompOperations List
            if (!TypeCompOpLists.ContainsKey(compType))
            {
                TypeCompOpLists.Add(compType, new TypeComponentOperationList(compType));

            }

            TypeCompOpLists[compType].AddOverriding(newOverrideableCompOp);

            virtualObj.AddOverriding(newOverrideableCompOp);

            ComponentOperations.Add(comp, newOverrideableCompOp);

            OverridingCompOperations.Add(newOverrideableCompOp);

            return newOverrideableCompOp;
        }

        public int GetEnabledCompOpTotal()
        {
            return GetEnabledOverridingCount() + GetEnabledPreExistingCount();
        }

        public int GetEnabledPreExistingCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingCompOp in PreExistingCompOperations)
            {
                if (preExistingCompOp.IsFullyEnabled())
                {
                    count++;
                }
            }

            return count;
        }

        public int GetEnabledOverridingCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation overridingCompOp in OverridingCompOperations)
            {
                if (overridingCompOp.IsFullyEnabled())
                {
                    count++;
                }
            }

            return count;
        }

        public int GetEnabledPreExistingCountByType(System.Type inType)
        {
            if (!TypeCompOpLists.ContainsKey(inType))
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < TypeCompOpLists[inType].GetPreExistingCount(); i++)
            {
                if (TypeCompOpLists[inType].GetPreExisting(i).IsFullyEnabled())
                {
                    count++;
                }
            }
            return count;
        }

        public int GetEnabledOverridingCountByType(System.Type inType)
        {
            if (!TypeCompOpLists.ContainsKey(inType))
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < TypeCompOpLists[inType].GetOverridingCount(); i++)
            {
                if (TypeCompOpLists[inType].GetOverriding(i).IsFullyEnabled())
                {
                    count++;
                }
            }
            return count;
        }

        public int GetCompOpTotal()
        {
            return ComponentOperations.Count;
        }

        public int GetCompOpTotalByType(System.Type inType)
        {
            if (!TypeCompOpLists.ContainsKey(inType))
            {
                return 0;
            }

            return TypeCompOpLists[inType].GetTotalCompOpCount();
        }

        // 
        public void RegisterForRefRefresh()
        {
            _bRegisteredForRefCheck = true;
        }

        public void TryRefRefresh()
        {
            if (!_bRegisteredForRefCheck)
            {
                return;
            }

            _bRegisteredForRefCheck = false;

            foreach (Component comp in ComponentOperations.Keys)
            {
                ComponentOperation compOp = ComponentOperations[comp];
                foreach (RegisteredReference regRef in compOp.RegisteredRefCollection.RegisteredReferences)
                {
                    for (int j = 0; j < regRef.GetReferenceCount(); j++)
                    {
                        regRef.GetReferenceElement(j).CheckNeedWarning();
                    }
                }
            }
        }

        public void ChangeCompOpVirtualObjectParent(OverridingComponentOperation overridingCompOp, VirtualGameObject newVirtualObjParent, bool bScrollToCompOp = false)
        {
            if (newVirtualObjParent == null || overridingCompOp == null)
            {
                return;
            }

            if (newVirtualObjParent == overridingCompOp.VirtualGameObjectRef)
            {
                return;
            }

            /// 0st Check to see if the CompOp can even fit on VirtualObject
            if (newVirtualObjParent.CanAcceptOverridingCompOp(overridingCompOp) == false)
            {
                return;
            }

            /// 1st Clear from lists
            /// Remove from UnusedList if exists
            if (overridingCompOp.IsUnused && overridingCompOp.VirtualGameObjectRef == null)
            {
                for (int i = 0; i < UnusedComponentOperations.Count; i++)
                {
                    if (UnusedComponentOperations[i] == overridingCompOp)
                    {
                        UnusedComponentOperations.RemoveAt(i);

                        if (UnusedComponentOperations.Count == 0)
                        {
                            WarningHandler.GetInstance().RemoveWarning(UnusedWarningRef);
                            UnusedWarningRef = null;
                        }
                        break;
                    }
                }

                // Now add newly moved Unused to list
                OverridingCompOperations.Add(overridingCompOp);
            }

            if (overridingCompOp.VirtualGameObjectRef != null)
            {
                VirtualGameObject oldVirtualGameObject = overridingCompOp.VirtualGameObjectRef;

                overridingCompOp.VirtualGameObjectRef.RemoveOverriding(overridingCompOp);

                // setting to null Also detaches the linked to Pre-Existing Component Operation
                overridingCompOp.SetToReplace(null);

                oldVirtualGameObject.RefreshWhatIsOverwritten();

                // Remove from TypeCompOperations List if exists
                if (TypeCompOpLists.ContainsKey(overridingCompOp.ComponentType))
                {
                    TypeCompOpLists[overridingCompOp.ComponentType].RemoveOverrideable(overridingCompOp);

                    // Remove old TypeCompOperationsList if Empty
                    if (TypeCompOpLists[overridingCompOp.ComponentType].GetTotalCompOpCount() == 0)
                    {
                        TypeCompOpLists.Remove(overridingCompOp.ComponentType);
                    }
                }
            }

            /// 2nd set new VirtualObject Parent
            overridingCompOp.SetVirtualGameObject(newVirtualObjParent);

            /// 3rd add CompOp back into CompOpLists
            newVirtualObjParent.AddOverriding(overridingCompOp);

            newVirtualObjParent.RefreshWhatIsOverwritten();

            // Type Lists
            if (!TypeCompOpLists.ContainsKey(overridingCompOp.ComponentType))
            {
                TypeCompOpLists.Add(overridingCompOp.ComponentType, new TypeComponentOperationList(overridingCompOp.ComponentType));

            }
            TypeCompOpLists[overridingCompOp.ComponentType].AddOverriding(overridingCompOp);

            // Scroll to changed Comp Op
            if (bScrollToCompOp)
            {
                EditorUI.CopyDetailsUIPanel.GetInstance().ScrollToCompOperation(overridingCompOp);
            }

            // If CompOp is a Renderer, refresh Material Ops
            if (typeof(Renderer).IsAssignableFrom(overridingCompOp.ComponentType))
            {
                MaterialOperationHandler.GetInstance().SavedReset();
                MaterialOperationHandler.GetInstance().CreateMaterialOperations();
                MaterialOperationHandler.GetInstance().ApplySavedData();
            }
        }

        public void RefreshCompOps()
        {
            foreach (System.Type typeKey in TypeCompOpLists.Keys)
            {
                TypeCompOpLists[typeKey].ReOrganize();
            }
        }


        public int GetNonReplacedTotalCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation compOp in PreExistingCompOperations)
            {
                if (compOp.IsBeingOverriden == false)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetNonReplacedEnabledCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation compOp in PreExistingCompOperations)
            {
                if (compOp.IsFullyEnabled() && compOp.IsBeingOverriden == false)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetReplacedTotalCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation compOp in PreExistingCompOperations)
            {
                if (compOp.IsBeingOverriden)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetReplacedEnabledCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation compOp in PreExistingCompOperations)
            {
                if (compOp.IsFullyEnabled() && compOp.IsBeingOverriden)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetNewTotalCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation compOp in OverridingCompOperations)
            {
                if (compOp.IsReplacing == false)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetNewEnabledCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation compOp in OverridingCompOperations)
            {
                if (compOp.IsFullyEnabled() && compOp.IsReplacing == false)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetReplacingTotalCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation compOp in OverridingCompOperations)
            {
                if (compOp.IsReplacing)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetReplacingEnabledCount()
        {
            int count = 0;
            foreach (OverridingComponentOperation compOp in OverridingCompOperations)
            {
                if (compOp.IsFullyEnabled() && compOp.IsReplacing)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
#endif