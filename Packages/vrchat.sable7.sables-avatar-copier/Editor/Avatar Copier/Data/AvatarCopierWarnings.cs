#if (UNITY_EDITOR)
using SablesTools.AvatarCopier.Operations;
using System.Collections.Generic;
using SablesTools.AvatarCopier.Data;
using UnityEngine;

namespace SablesTools.AvatarCopier.Warnings
{
    public class AvatarCopierWarning
    {
        //public bool bIgnored { get; set; }
        //public virtual int Order { get; }
        //public bool bFixed { get; set; }

        //public virtual string GetName()
        //{
        //    return "<none>";
        //}

        public bool bSurpressed { get; set; } = false;
    }

    public class AvatarCopierMissingSkeletonNameWarning : AvatarCopierWarning
    {
        //public override int Order { get { return 0; } }
    }

    public class AvatarCopierDuplicateObjectNamesWarning : AvatarCopierWarning
    {
        //public override int Order { get { return 0; } }

        public bool bIsFromSource { get; }

        public string DuplicateName { get; }

        protected List<GameObject> DuplicateObjects = new List<GameObject>();

        public AvatarCopierDuplicateObjectNamesWarning(string inDuplicateName, bool isFromSource)
        {
            DuplicateName = inDuplicateName;
            bIsFromSource = isFromSource;
        }

        public int GetDuplicateCount()
        {
            return DuplicateObjects.Count;
        }

        public GameObject GetDuplicate(int index)
        {
            return DuplicateObjects[index];
        }

        public void AddDuplicate(GameObject inDuplicate)
        {
            DuplicateObjects.Add(inDuplicate);
        }
    }

    //public class MissingComponentHome : Warning
    //{
    //    public GameObject SourceGameObject { get; }

    //    public MissingComponentHome(GameObject inSourceGameObject)
    //    {
    //        SourceGameObject = SourceGameObject;
    //    }
    //}

    public class AvatarCopierMissingReferenceWarning : AvatarCopierWarning
    {
        protected RegisteredReferenceElement _RegRefElement;
        public RegisteredReferenceElement RegRefElement { get { return _RegRefElement; } }

        public AvatarCopierMissingReferenceWarning(RegisteredReferenceElement regRefElement)
        {
            _RegRefElement = regRefElement;
        }
    }

    public class AvatarCopierReferenceAttachableDisabledWarning : AvatarCopierWarning
    {
        protected RegisteredReferenceElement _RegRefElement;
        public RegisteredReferenceElement RegRefElement { get { return _RegRefElement; } }

        public AvatarCopierReferenceAttachableDisabledWarning(RegisteredReferenceElement regRefElement)
        {
            _RegRefElement = regRefElement;
        }
    }

    public class AvatarCopierReferenceAttachableUnattachedWarning : AvatarCopierWarning
    {
        protected RegisteredReferenceElement _RegRefElement;
        public RegisteredReferenceElement RegRefElement { get { return _RegRefElement; } }

        public AvatarCopierReferenceAttachableUnattachedWarning(RegisteredReferenceElement regRefElement)
        {
            _RegRefElement = regRefElement;
        }
    }

    public class AvatarCopierExpectedComponentMissingWarning : AvatarCopierWarning
    {
        protected RegisteredReferenceElement _RegRefElement;
        public RegisteredReferenceElement RegRefElement { get { return _RegRefElement; } }

        public AvatarCopierExpectedComponentMissingWarning(RegisteredReferenceElement regRefElement)
        {
            _RegRefElement = regRefElement;
        }
    }

    public class AvatarCopierAttachableNoAttachmentPointWarning : AvatarCopierWarning
    {
        protected AttachmentOperation _AttachmentOp;
        public AttachmentOperation AttachmentOp { get { return _AttachmentOp; } }

        public AvatarCopierAttachableNoAttachmentPointWarning(AttachmentOperation attachmentOp)
        {
            _AttachmentOp = attachmentOp;
        }
    }

    public class AvatarCopierUnusedWarning : AvatarCopierWarning
    {

    }
}
#endif