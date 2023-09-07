#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Operations;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Data
{
    public class VirtualGameObject
    {
        public string Name { get; set; }

        protected VirtualGameObject _parent = null;
        public VirtualGameObject Parent { get { return _parent; } set { SetParent(value); } }

        //public int SiblingIndex { get; set; }

        protected bool _bIsRoot = false;
        public bool bIsRoot { get { return _bIsRoot; } }

        //protected bool _bIsAttachable = false;
        public bool bIsAttachable { get { return AttachmentOp != null; } }

        protected bool _bIsTopLevelAttachable = false;
        public bool bIsTopLevelAttachable { get { return _bIsTopLevelAttachable; } }

        public AttachmentOperation AttachmentOp { get; }

        public bool bSourceIsOriginGameObject { get; }

        public GameObject LinkedSource { get; set; } = null;
        public GameObject LinkedDestination { get; set; } = null;

        protected List<PreExistingComponentOperation> _PreExistingCompOps { get; set; } = new List<PreExistingComponentOperation>();
        protected List<OverridingComponentOperation> _OverridingCompOps { get; set; } = new List<OverridingComponentOperation>();

        public bool bShowPreExistingCompOps = false;
        public bool bShowOverridingCompOps = false;

        // The real GameObject that is this Virtual Object's Origin
        //public GameObject OriginGameObject { get; }

        protected List<VirtualGameObject> Childern = new List<VirtualGameObject>();


        public Vector3 TransformPosition { get; set; }
        public Vector3 TransformRoation { get; set; }
        public Vector3 TransformScale { get; set; }

        public bool bIsOpenInMergeTree { get; set; }

        public int VirtualTreeOrder { get; set; } = int.MaxValue;

        // The Created or Linked GameObject set-up at runtime.
        public GameObject RunTimeObject { get; set; }

        public int GetChildCount()
        {
            return Childern.Count;
        }

        public GameObject GetOriginGameObject()
        {
            if (bSourceIsOriginGameObject)
            {
                return LinkedSource;
            }

            return LinkedDestination;
        }

        public VirtualGameObject(GameObject inDestinationObject)
        {
            LinkedDestination = inDestinationObject;

            if (LinkedDestination == Handlers.CopierSettingsHandler.GetInstance().Destination)
            {
                _bIsRoot = true;
            }

            SetTransformData(GetOriginGameObject().transform);
            Name = GetOriginGameObject().name;
        }

        public VirtualGameObject(GameObject inSourceAttachableObject, AttachmentOperation inAttachmentOperation)
        {
            LinkedSource = inSourceAttachableObject;

            if (LinkedSource != null)
            {
                bSourceIsOriginGameObject = true;
            }

            SetTransformData(inSourceAttachableObject.transform);
            Name = inSourceAttachableObject.name;

            if (inAttachmentOperation != null)
            {
                AttachmentOp = inAttachmentOperation;
                _bIsTopLevelAttachable = inAttachmentOperation.SourceAttachableObject == inSourceAttachableObject;
            }
        }

        public void SetTransformData(Transform trans)
        {
            TransformPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, trans.localPosition.z);
            TransformRoation = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, trans.localEulerAngles.z);
            TransformScale = new Vector3(trans.localScale.x, trans.localScale.y, trans.localScale.z);
        }

        public void AddVirtualChild(VirtualGameObject NewChild)
        {
            if (NewChild == null)
            {
                return;
            }

            Childern.Add(NewChild);

            if (NewChild.Parent != this)
            {
                NewChild.Parent = this;
            }
        }

        protected void SetParent(VirtualGameObject inParent)
        {
            // Not super optimized, but this is the only way I can think of it working without infinite loops
            if (inParent == null)
            {
                if (_parent != null && _parent.IsAChild(this))
                {
                    _parent.RemoveChild(this);
                }

                _parent = null;

                return;
            }

            if (inParent != _parent)
            {
                if (_parent != null && _parent.IsAChild(this))
                {
                    _parent.RemoveChild(this);
                }

                _parent = inParent;

                if (!_parent.IsAChild(this))
                {
                    _parent.AddVirtualChild(this);
                }
            }
        }

        public int GetSiblingIndex()
        {
            if (_parent != null)
            {
                return _parent.GetChildIndex(this);
            }

            return 0;
        }

        public int GetChildIndex(VirtualGameObject VirtualObj)
        {
            if (VirtualObj == null)
            {
                return -1;
            }

            return Childern.IndexOf(VirtualObj);
        }

        public VirtualGameObject GetChild(int index)
        {
            if (index < 0 || index >= Childern.Count)
            {
                return null;
            }

            return Childern[index];
        }

        public bool IsAChild(VirtualGameObject inVirtualObj)
        {
            foreach (VirtualGameObject VirtualObj in Childern)
            {
                if (VirtualObj == inVirtualObj)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveChild(VirtualGameObject RemovedChild)
        {
            if (Childern.Remove(RemovedChild))
            {
                RemovedChild.Parent = null;
            }
        }

        public int GetEnabledPreExistingCount()
        {
            int count = 0;
            foreach (PreExistingComponentOperation preExistingCompOp in _PreExistingCompOps)
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
            foreach (OverridingComponentOperation overridingCompOp in _OverridingCompOps)
            {
                if (overridingCompOp.IsFullyEnabled())
                {
                    count++;
                }
            }

            return count;
        }

        public int GetOverridingCount()
        {
            return _OverridingCompOps.Count;
        }

        public int GetPreExistingCount()
        {
            return _PreExistingCompOps.Count;
        }

        public PreExistingComponentOperation GetPreExisting(int index)
        {
            return _PreExistingCompOps[index];
        }

        public OverridingComponentOperation GetOverriding(int index)
        {
            return _OverridingCompOps[index];
        }

        public void AddPreExisting(PreExistingComponentOperation preExisting)
        {
            _PreExistingCompOps.Add(preExisting);
        }

        public void AddOverriding(OverridingComponentOperation overriding)
        {
            _OverridingCompOps.Add(overriding);
        }

        public void RemoveOverriding(int index)
        {
            _OverridingCompOps.RemoveAt(index);
        }

        public void RemoveOverriding(OverridingComponentOperation overriding)
        {
            for (int i = 0; i < _OverridingCompOps.Count; i++)
            {
                if (overriding == _OverridingCompOps[i])
                {
                    RemoveOverriding(i);
                    return;
                }
            }
        }

        public void RefreshWhatIsOverwritten()
        {
            List<OverridingComponentOperation> tempOverridingCompOps = new List<OverridingComponentOperation>(_OverridingCompOps);

            // Reset _PreExistiing
            for (int i = 0; i < _PreExistingCompOps.Count; i++)
            {
                _PreExistingCompOps[i].SetToBeReplaced(null);
            }

            // Reset _OverridingComp
            for (int i = 0; i < _OverridingCompOps.Count; i++)
            {
                _OverridingCompOps[i].SetToReplace(null);
            }

            // Now check to see what PreExisting is gonna be replaced
            for (int i = 0; i < _PreExistingCompOps.Count; i++)
            {
                for (int j = 0; j < tempOverridingCompOps.Count; j++)
                {
                    if (tempOverridingCompOps[j].ComponentType == _PreExistingCompOps[i].ComponentType)
                    {
                        tempOverridingCompOps[j].SetToReplace(_PreExistingCompOps[i]);
                        break;
                    }
                }
            }
        }

        public bool CanAcceptOverridingCompOp(OverridingComponentOperation overriding)
        {
            if (AvatarCopierUtils.AllowedDuplicateCopyTypes.Contains(overriding.ComponentType))
            {
                return true;
            }

            // if (bIsAttachable)
            //{
            //    return false;
            //}

            // Does it already exist in the OverridingCompOps
            foreach (OverridingComponentOperation oCompOp in _OverridingCompOps)
            {
                if (oCompOp.ComponentType == overriding.ComponentType)
                {
                    return false;
                }
            }

            return true;
        }

        public bool PreExistingHasWarning()
        {
            foreach (PreExistingComponentOperation preExisting in _PreExistingCompOps)
            {
                if (preExisting.IsFullyEnabled() && preExisting.HasUnSuppressedWarning())
                {
                    return true;
                }
            }

            return false;
        }

        public bool OverridingHasWarning()
        {
            foreach (OverridingComponentOperation overidding in _OverridingCompOps)
            {
                if (overidding.IsFullyEnabled() && overidding.HasUnSuppressedWarning())
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#endif