#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Data;

namespace SablesTools.AvatarCopier.Operations
{
    public class ComponentOperation : Operation
    {
        public System.Type ComponentType { get; }

        // The component on the merged/created avatar that this Component Operation links with
        public Component RunTimeComponent { get; set; }

        // The Component this Component Information was created from
        public Component OriginComponent { get; }

        public VirtualGameObject OriginalVirtualGameObjectRef { get; }
        protected VirtualGameObject _VirtualGameObjectRef = null;
        public VirtualGameObject VirtualGameObjectRef { get { return _VirtualGameObjectRef; } set { SetVirtualGameObject(value); } }

        protected bool _bVirtualIsUserSet = false;
        public bool bIsVirtualUserSet { get { return _bVirtualIsUserSet; } }

        public RegisteredReferenceCollection RegisteredRefCollection { get; } = new RegisteredReferenceCollection();

        public List<KeyValuePair<Handlers.PreservedPropertyData, object>> RunTimePreservedProperties { get; } = new List<KeyValuePair<Handlers.PreservedPropertyData, object>>();
 

        public ComponentOperation(Component originComponent, VirtualGameObject virtualGameObject)
        {
            OriginComponent = originComponent;
            OriginalVirtualGameObjectRef = virtualGameObject;
            _VirtualGameObjectRef = virtualGameObject;

            ComponentType = OriginComponent.GetType();

            bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseCompOperations");
        }

        public void SetVirtualGameObject(VirtualGameObject inVirtualObj)
        {
            if (inVirtualObj != null)
            {
                _VirtualGameObjectRef = inVirtualObj;
                _bVirtualIsUserSet = true;
            }
        }

        public bool HasUnSuppressedWarning()
        {
            return RegisteredRefCollection.HasUnSurpressedWarning();
        }

        //public override bool IsFullyEnabled()
        //{
        //    if (MergerSettingsHandler.GetInstance().bCopyComponents == false)
        //    {
        //        return false;
        //    }

        //    return bUserSetEnabled;
        //}
    }

    public class PreExistingComponentOperation : ComponentOperation
    {
        public bool IsBeingOverriden { get { return _OverridingCompOp != null && _OverridingCompOp.IsFullyEnabled(); } }
        protected OverridingComponentOperation _OverridingCompOp = null;
        public OverridingComponentOperation OverridingCompOp { get { return _OverridingCompOp; } }

        public ComponentOperation ReplacingCompOp { get; }

        public int ComponentTypeIndex { get; } = -1;

        //public bool IsPreExisting { get; }
        public bool bIsAttachable { get { return IsAttachable(); } }

        // Should only be called by another CompOp
        public void SetToBeReplaced(OverridingComponentOperation overridingCompOp)
        {
            _OverridingCompOp = overridingCompOp;
        }

        public PreExistingComponentOperation(Component originComponent, VirtualGameObject virtualGameObject, int componentTypeIndex) : base(originComponent, virtualGameObject)
        {
            ComponentTypeIndex = componentTypeIndex;
        }

        public override bool IsFullyEnabled()
        {
            //if (MergerSettingsHandler.GetInstance().bCopyComponents == false)
            //{
            //    return false;
            //}

            if (VirtualGameObjectRef != null && VirtualGameObjectRef.bIsAttachable)
            {
                if (VirtualGameObjectRef.AttachmentOp.IsFullyEnabled() == false)
                {
                    return false;
                }
            }

            return bUserSetEnabled;
        }

        protected bool IsAttachable()
        {
            if (VirtualGameObjectRef != null)
            {
                return VirtualGameObjectRef.bIsAttachable;
            }

            return false;
        }
    }

    public class OverridingComponentOperation : ComponentOperation
    {
        public bool IsUnused { get { return _VirtualGameObjectRef == null; } }

        public bool IsOriginallyUnused { get { return OriginalVirtualGameObjectRef == null; } }

        protected PreExistingComponentOperation _ReplacingPreExistingCompOp = null;
        public PreExistingComponentOperation ReplacingPreExistingCompOp { get { return _ReplacingPreExistingCompOp; } }
        public bool IsReplacing { get { return _ReplacingPreExistingCompOp != null; } }

        public OverridingComponentOperation(Component originComponent, VirtualGameObject virtualGameObject) : base(originComponent, virtualGameObject)
        {

        }

        public void SetToReplace(PreExistingComponentOperation preExistingCompOp)
        {
            // Ensure Old Replacing CompOp is no longer being set to be replaced
            if (IsReplacing)
            {
                _ReplacingPreExistingCompOp.SetToBeReplaced(null);
            }

            _ReplacingPreExistingCompOp = preExistingCompOp;

            if (preExistingCompOp != null)
            {
                preExistingCompOp.SetToBeReplaced(this);
            }
        }

        public override bool IsFullyEnabled()
        {
            if (IsUnused)
            {
                return false;
            }

            return base.IsFullyEnabled();
        }
    }
}
#endif