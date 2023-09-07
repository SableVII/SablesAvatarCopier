#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{
    public class InputErrorHandler
    {
        private static InputErrorHandler _Instance = null;

        public static InputErrorHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new InputErrorHandler();
            }

            return _Instance;
        }

        private InputErrorHandler()
        {

        }

        // Dictionary to check for duplicate non-attachable source game objects
        //protected HashSet<string> NamedSourceGameObjects = new HashSet<string>();
        //protected HashSet<string> NamedDestinationGameObjects = new HashSet<string>();

        //protected bool[] DestinationSkeletonFoundMatchArray;
        //protected bool[] SourceSkeletonFoundMatchArray;
        //protected bool[] SkeletonFoundMatchArray;
        protected Dictionary<string, CanIDo> SkeletonDictionaryInfo;

        // Using a list as repeated access to the humanoid skeleton array appears to be *very* slow
        protected List<string> SkeletonNameList;

        protected class CanIDo
        {
            public int Index { get; set; } = -1;
            public bool bFound { get; set; } = false;

            public CanIDo(int inIndex)
            {
                Index = inIndex;
            }
        }

        // Returns False if Error was encountered
        public bool CheckInputsForErrors()
        {
            WarningHandler.GetInstance().ResetErrors();
            //MergerSettingsHandler.GetInstance().bCantSearchForAttachables = false;

            if (CopierSettingsHandler.GetInstance().Source == null && CopierSettingsHandler.GetInstance().Destination == null)
            {
                return true;
            }

            // Same Input Checking
            if (CopierSettingsHandler.GetInstance().Source == CopierSettingsHandler.GetInstance().Destination)
            {
                WarningHandler.GetInstance().AddError(new Errors.AvatarCopierSameInputsError());
                return false;
            }

            // Destination Avatar check for missing skeleton match
            if (CopierSettingsHandler.GetInstance().Destination != null)
            {
                Animator AnimatorComponent = CopierSettingsHandler.GetInstance().Destination.GetComponent<Animator>();
                Avatar AnimatorAvatar = null;

                if (AnimatorComponent != null)
                {
                    AnimatorAvatar = AnimatorComponent.avatar;
                }

                if (AnimatorAvatar == null)
                {
                    //MergerWarningHandler.GetInstance().AddError(new MergerErrorNeedsAvatarComponent(false));
                    //MergerSettingsHandler.GetInstance().bCantSearchForAttachables = true;
                    //return false;
                }
            }

            // Source Avatar check for missing skeleton match
            if (CopierSettingsHandler.GetInstance().Source != null) 
            {
                Animator AnimatorComponent = CopierSettingsHandler.GetInstance().Source.GetComponent<Animator>();
                Avatar AnimatorAvatar = null;

                if (AnimatorComponent != null)
                {
                    AnimatorAvatar = AnimatorComponent.avatar;
                }

                if (AnimatorAvatar == null)
                {
                    //MergerSettingsHandler.GetInstance().bCantSearchForAttachables = true;
                    //return false;
                }
                else if (!CheckSourceAvatarSkeletonCompatability(CopierSettingsHandler.GetInstance().Source, AnimatorAvatar))
                {
                    return false;
                }
            }

            // Make sure Inputs arn't childern of one another
            if (CopierSettingsHandler.GetInstance().Destination != null && CopierSettingsHandler.GetInstance().Source != null)
            {
                if (CopierSettingsHandler.GetInstance().Source.transform.IsChildOf(CopierSettingsHandler.GetInstance().Destination.transform))
                {
                    WarningHandler.GetInstance().AddError(new Errors.AvatarCopierInputsAreChildernError(true));
                    return false;
                }

                if (CopierSettingsHandler.GetInstance().Destination.transform.IsChildOf(CopierSettingsHandler.GetInstance().Source.transform))
                {
                    WarningHandler.GetInstance().AddError(new Errors.AvatarCopierInputsAreChildernError(false));
                    return false;
                }
            }

            return true;
        }

        public bool CheckSourceAvatarSkeletonCompatability(GameObject InAvatarRoot, Avatar InAvatar)
        {
            SkeletonNameList = new List<string>();

            //SkeletonFoundMatchArray = bool[]{ 0};
            SkeletonDictionaryInfo = new Dictionary<string, CanIDo>();
            for (int i = 0; i < InAvatar.humanDescription.skeleton.Length; i++)
            {
                string name = InAvatar.humanDescription.skeleton[i].name;
                SkeletonDictionaryInfo.Add(name, new CanIDo(i));
                SkeletonNameList.Add(name);
            }

            // Always match the first bone in the skeleton to the root
            SkeletonDictionaryInfo[InAvatar.humanDescription.skeleton[0].name].bFound = true;

            // Go through childern
            for (int i = 0; i < InAvatarRoot.transform.childCount; i++)
            {
                CheckSourceAvatarSkeletonCompatability_R(InAvatarRoot.transform.GetChild(i).gameObject, InAvatar);
            }

            // Check to see if any are remained false, if false, return false
            Errors.AvatarCopierSourceMissingSkeletalReferenceError missingError = null;
            for (int i = 0; i < SkeletonNameList.Count; i++)
            {
                if (SkeletonDictionaryInfo[SkeletonNameList[i]].bFound == false)
                {
                    if (missingError == null)
                    {
                        missingError = new Errors.AvatarCopierSourceMissingSkeletalReferenceError();
                        WarningHandler.GetInstance().AddError(missingError);
                    }

                    missingError.AddMissingSkeletalBoneName(SkeletonNameList[i]);
                }
            }

            return missingError == null;
        }

        public void CheckSourceAvatarSkeletonCompatability_R(GameObject CurrentGameObject, Avatar inAvatar)
        {
            // Loop through and find Index of the humanoid bone
            int foundIndex = -1;
            for (int i = 0; i < SkeletonNameList.Count; i++)
            {
                if (CurrentGameObject.name == SkeletonNameList[i])
                {
                    foundIndex = i;
                    break;
                }
            }

            // Don't continue if this GameObject is not found as a bone.
            if (foundIndex == -1)
            {               
                return;
            }

            // Set index in SkeletonFoundMatchArray to True
            SkeletonDictionaryInfo[SkeletonNameList[foundIndex]].bFound = true;

            // Go through childern if found
            for (int i = 0; i < CurrentGameObject.transform.childCount; i++)
            {
                CheckSourceAvatarSkeletonCompatability_R(CurrentGameObject.transform.GetChild(i).gameObject, inAvatar);
            }
        }
    }
}

#endif