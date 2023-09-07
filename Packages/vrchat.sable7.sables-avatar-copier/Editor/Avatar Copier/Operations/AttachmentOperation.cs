#if (UNITY_EDITOR)
using UnityEngine;
using SablesTools.AvatarCopier.Data;
using SablesTools.AvatarCopier.Warnings;

namespace SablesTools.AvatarCopier.Operations
{
    public class AttachmentOperation : Operation
    {
        //public new bool bUserSetEnabled { get { return base.bUserSetEnabled; } set {  }; }

        //protected bool bEnabled = false;

        public GameObject SourceAttachableObject { get; }
        //public GameObject DestinationAttachableObject = null;

        // The Original Attachment Point. Can not be overriden.
        //public GameObject MatchedAttachmentPoint = null;

        // The Attachment Point of the Attachable for use. Can be overriden.
        protected VirtualGameObject _AttachmentPoint = null;
        public VirtualGameObject AttachmentPoint { get { return _AttachmentPoint; } }

        public VirtualGameObject OriginalAttachmentPoint { get; }

        public VirtualGameObject VirtualObject { get; }

        public bool bSubAttachablesOpenInUI { get; set; }

        private AvatarCopierAttachableNoAttachmentPointWarning _Warning = null;
        public AvatarCopierAttachableNoAttachmentPointWarning Warning { get { return _Warning; } }

        public AttachmentOperation(GameObject inSourceAttachableObject)
        {
            SourceAttachableObject = inSourceAttachableObject;

            OriginalAttachmentPoint = Handlers.AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(SourceAttachableObject.transform.parent.gameObject);
            _AttachmentPoint = OriginalAttachmentPoint;
                
            if (OriginalAttachmentPoint == null)
            {
                _Warning = new AvatarCopierAttachableNoAttachmentPointWarning(this);
                Handlers.WarningHandler.GetInstance().AddWarning(_Warning);
            }

            VirtualObject = Handlers.MergeTreeHandler.GetInstance().CreateAttachmentOpVirtualObjects(this);

            bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseAttachableOperations");
        }

        public override bool IsFullyEnabled()
        {
            if (_AttachmentPoint == null)
            {
                return false;
            }

            return bUserSetEnabled;
        }

        public bool bHasWarnings()
        {
            return _Warning != null;
        }

        public void SetAttachmentPoint(VirtualGameObject inAttachmentPoint)
        {
            if (inAttachmentPoint != null && _AttachmentPoint == null)
            {
                Handlers.WarningHandler.GetInstance().RemoveWarning(_Warning);
                _Warning = null;
            }

            if (inAttachmentPoint == null && _Warning != null)
            {
                _Warning = new AvatarCopierAttachableNoAttachmentPointWarning(this);
                Handlers.WarningHandler.GetInstance().AddWarning(_Warning);
            }

            _AttachmentPoint = inAttachmentPoint;
        }

        protected override void _SetUserSetEnabled(bool value)
        {
            bool bPrevEnabled = _bUserSetEnabled;
            base._SetUserSetEnabled(value);
            
            if (bPrevEnabled != _bUserSetEnabled)
            {
                if (bUserSetEnabled)
                {
                    if (_Warning != null)
                    {
                        Handlers.WarningHandler.GetInstance().AddWarning(_Warning);
                    }
                }
                else
                {
                    if (_Warning != null)
                    {
                        Handlers.WarningHandler.GetInstance().RemoveWarning(_Warning);
                    }
                }
            }
        }
    }
}
#endif