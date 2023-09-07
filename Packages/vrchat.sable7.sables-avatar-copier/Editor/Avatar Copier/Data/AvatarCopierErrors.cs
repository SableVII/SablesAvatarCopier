#if (UNITY_EDITOR)
using SablesTools.AvatarCopier.Operations;
using System.Collections.Generic;
using UnityEngine;

namespace SablesTools.AvatarCopier.Errors
{
    public class AvatarCopierError
    {
        public virtual string ErrorTitle { get { return "<Default Error Title>"; } }
        public virtual string ErrorMessage { get { return "<Default Error Message>"; } }

        protected bool _bHaultCopy = true;
        public bool bHaultCopy { get { return _bHaultCopy; } }
    }

    public class AvatarCopierSameInputsError : AvatarCopierError
    {
        public override string ErrorTitle { get { return "Matching Destination and Source Inputs"; } }
        public override string ErrorMessage { get { return "Destination and Source inputs must be different objects."; } }
    }

    public class AvatarCopierSourceMissingSkeletalReferenceError : AvatarCopierError
    {
        public override string ErrorTitle { get { return "Misisng Skeletal Transforms in Source Avatar"; } }
        public override string ErrorMessage { get { return GetErrorMessage(); } }
        protected List<string> _MissingSkeletalBoneNames = new List<string>();

        public AvatarCopierSourceMissingSkeletalReferenceError()
        {
            _bHaultCopy = false;
        }

        public void AddMissingSkeletalBoneName(string skeletalBoneName)
        {
            _MissingSkeletalBoneNames.Add(skeletalBoneName);
        }

        public string GetMissingSkeletalBoneName(int index)
        {
            return _MissingSkeletalBoneNames[index];
        }

        public int GetMissingSkeletalBoneCount()
        {
            return _MissingSkeletalBoneNames.Count;
        }

        protected string GetErrorMessage()
        {
            string outErrorMessage = "Unable to match up all imported Avatar Transforms with the transforms found currently within the Source Avatar." +
                "This can happen if one of the Transforms have been renamed and/or deleted from its original import state.";
        
            return outErrorMessage;
        }
    }

    public class AvatarCopierInputsAreChildernError : AvatarCopierError
    {
        public override string ErrorTitle { get { return "Misisng Skeletal Transforms in Source Avatar"; } }
        public override string ErrorMessage { get { return GetErrorMessage(); } }
        public bool bSourceAvatarIsChild = false;

        public AvatarCopierInputsAreChildernError(bool isSourceAvatarTheChild = false)
        {
            bSourceAvatarIsChild = isSourceAvatarTheChild;
        }

        protected string GetErrorMessage()
        {
            string outErrorMessage = " is a child of the ";

            if (bSourceAvatarIsChild)
            {
                outErrorMessage = "Source Avatar " + outErrorMessage;
                outErrorMessage += "Destination Avatar";
            }
            else
            {
                outErrorMessage = "Destination Avatar" + outErrorMessage;
                outErrorMessage += "Source Avatar";
            }

            return outErrorMessage;
        }
    }

    /*public class MergerErrorNeedsAvatarComponent : MergerError
    {
        public override string ErrorTitle { get { return "Misisng Avatar Component in " + (bIsSourceAvatar ? "Source Avatar" : "Destination Avatar") + " Input"; } }
        public override string ErrorMessage { get { return GetErrorMessage(); } }
        public bool bIsSourceAvatar { get; } = false;

        public MergerErrorNeedsAvatarComponent(bool inIsSourceAvatar)
        {
            bIsSourceAvatar = inIsSourceAvatar;
        }

        protected string GetErrorMessage()
        {
            string outErrorMessage = "Unable to find an Avatar Component on the inputted " +
                (bIsSourceAvatar ? "Source Avatar." : "Destination Avatar.") +
                "This component is required for the Avatar Merger to function atm";

            return outErrorMessage;
        }
    }*/

   
}
#endif