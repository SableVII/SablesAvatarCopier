#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SablesTools.AvatarCopier.Operations;

namespace SablesTools.AvatarCopier.Handlers
{
    // Handles
    public class AttachmentOperationHandler : OperationHandler
    {
        private static AttachmentOperationHandler _Instance = null;

        public static AttachmentOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new AttachmentOperationHandler();
            }

            return _Instance;
        }

        private AttachmentOperationHandler()
        {

        }

        public List<AttachmentOperation> AttachmentOperations = new List<AttachmentOperation>();
        public List<AttachmentOperation> SavedAttachmentOperations = new List<AttachmentOperation>();

        public void Reset()
        {
            AttachmentOperations = new List<AttachmentOperation>();
            SavedAttachmentOperations = new List<AttachmentOperation>();
        }

        public void SavedReset()
        {
            SavedAttachmentOperations = new List<AttachmentOperation>(AttachmentOperations);

            AttachmentOperations = new List<AttachmentOperation>();
        }

        public void ApplySavedData()
        {
            foreach (AttachmentOperation SavedAttachmentOp in SavedAttachmentOperations)
            {
                foreach (AttachmentOperation AttachmentOp in AttachmentOperations)
                {
                    if (SavedAttachmentOp.VirtualObject.LinkedDestination == AttachmentOp.VirtualObject.LinkedDestination &&
                        SavedAttachmentOp.VirtualObject.LinkedSource == AttachmentOp.VirtualObject.LinkedSource && SavedAttachmentOp.VirtualObject.Name == AttachmentOp.VirtualObject.Name)
                    {
                        AttachmentOp.bUserSetEnabled = SavedAttachmentOp.bUserSetEnabled;

                        // Set AttachmentPoint if it current still exists 
                        if (SavedAttachmentOp.AttachmentPoint != null)
                        {
                            Data.VirtualGameObject VirtualObj = null;
                            if (SavedAttachmentOp.AttachmentPoint.bSourceIsOriginGameObject)
                            {
                                VirtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromSourceObject(SavedAttachmentOp.AttachmentPoint.LinkedSource);
                            }
                            else
                            {
                                VirtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromDestinationObject(SavedAttachmentOp.AttachmentPoint.LinkedDestination);
                            }

                            if (VirtualObj != null)
                            {
                                AttachmentOp.SetAttachmentPoint(VirtualObj);
                            }
                        }

                        break;
                    }
                }
            }
        }


        public void GenerateAttachmentOperations()
        {
            // Source Objects are already assumed linked with their virtual objects

            AttachmentOperations = new List<AttachmentOperation>();

            if (CopierSettingsHandler.GetInstance().bCantSearchForAttachables)
            {
                return;
            }

            for (int i = 0; i < AvatarMatchHandler.GetInstance().TopLevelAttachableGameObjectsCount; i++)
            {
                // Create Attachment Op. Attachment Ops and Virtual Objects are linked in Attachment Constructor;
                AttachmentOperation attachOp = new AttachmentOperation(AvatarMatchHandler.GetInstance().GetTopLevelAttachableGameObject(i));

                AttachmentOperations.Add(attachOp);
            }
        }

        public int GetEnabledAttachementOperationsCount()
        {
            int i = 0;
            foreach (AttachmentOperation AttachmentOp in AttachmentOperations)
            {
                if (AttachmentOp.bUserSetEnabled)
                {
                    i++;
                }
            }

            return i;
        }

        public AttachmentOperation GetAttachmentOperationBySourceAttachableObject(GameObject SourceAttachableObject)
        {
            // Search Top-Level Attachables
            foreach (AttachmentOperation AttachOp in AttachmentOperations)
            {
                if (AttachOp.SourceAttachableObject == SourceAttachableObject)
                {
                    return AttachOp;
                }
            }

            // Serach Sub-Attachables
            foreach (AttachmentOperation AttachOp in AttachmentOperations)
            {
                if (FindAttachmentOperationBySourceAttachableObject_R(AttachOp.SourceAttachableObject, SourceAttachableObject))
                {
                    return AttachOp;
                }
            }

            return null;
        }

        public bool FindAttachmentOperationBySourceAttachableObject_R(GameObject CurrentSubAttachableObject, GameObject TargetSourceAttachableObject)
        {
            // Not checking CurrentSubAttachable as it is checked within following for-loop

            for (int i = 0; i < CurrentSubAttachableObject.transform.childCount; i++)
            {
                if (TargetSourceAttachableObject == CurrentSubAttachableObject.transform.GetChild(i).gameObject)
                {
                    return true;
                }

                if (FindAttachmentOperationBySourceAttachableObject_R(CurrentSubAttachableObject.transform.GetChild(i).gameObject, TargetSourceAttachableObject))
                {
                    return true;
                }
            }

            return false;
        }

        /*public void AttachAttachablesToDestinationAvatar()
        {
            if (!MergerSavedData.GetInstance().bCopyAttachables)
            {
                return;
            }

            foreach (AttachmentOperation AttachOp in AttachmentOperations)
            {
                if (AttachOp.IsEnabled())
                {
                    GameObject CreatedAttachableObject = GameObject.Instantiate(AttachOp.SourceAttachableObject);
                    CreatedAttachableObject.name = AttachOp.SourceAttachableObject.name;

                    CreatedAttachableObject.transform.SetParent(AttachOp.AttachmentPoint.transform);

                    CreatedAttachableObject.transform.SetSiblingIndex(AttachOp.SourceAttachableObject.transform.GetSiblingIndex());

                    AvatarMatchHandler.GetInstance().AddAttachedAttachablesDataToMatched_R(AttachOp.SourceAttachableObject, CreatedAttachableObject);

                    CleanAttachableOfComponents_R(CreatedAttachableObject);
                    // Clean Attachables of all components so we can add the components that selected to be copied over to be copy over, ignoring the ones that arn't selected

                }
            }
        }*/

        protected void CleanAttachableOfComponents_R(GameObject CurrentAttachable)
        {
            Component[] Components = CurrentAttachable.GetComponents(typeof(Component));
            
            // Start at 1 to ignore 
            for (int i = 1; i < Components.Length; i++)
            {
                Object.DestroyImmediate(Components[i]);
            }

            for (int i = 0; i < CurrentAttachable.transform.childCount; i++)
            {
                CleanAttachableOfComponents_R(CurrentAttachable.transform.GetChild(i).gameObject);
            }
        }

        public bool AttachablesHaveWarnings()
        {
            foreach (AttachmentOperation at in AttachmentOperations)
            {
                if (at.Warning != null && at.bUserSetEnabled == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif