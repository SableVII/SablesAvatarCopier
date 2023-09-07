#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.Handlers
{
    public class MergeTreeHandler
    {
        private static MergeTreeHandler _Instance = null;

        public VirtualGameObject SelectedVirtualGameObject = null;

        // Contains references to the TopLevel Attachable Virtual Objects for easy recall when updating  
        protected List<VirtualGameObject> _TopLevelAttachableVirtualObjects = new List<VirtualGameObject>();

        protected HashSet<KeyValuePair<GameObject, GameObject>> SavedOpenedVirtualObjNames = new HashSet<KeyValuePair<GameObject, GameObject>>();

        // Ordered based off depth
        protected List<VirtualGameObject> _OrderedVirtualObjects = new List<VirtualGameObject>();

        public static MergeTreeHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new MergeTreeHandler();
            }

            return _Instance;
        }

        public VirtualGameObject VirtualTreeRoot = null;

        public void Reset()
        {
            SavedOpenedVirtualObjNames = new HashSet<KeyValuePair<GameObject, GameObject>>();
            SelectedVirtualGameObject = null;
            VirtualTreeRoot = null;
        }

        public void SavedReset()
        {
            CreateSavedOpenedVirtualObjects();

            SelectedVirtualGameObject = null;
            VirtualTreeRoot = null;
        }

        public void ApplySavedData()
        {
            ApplyOpenedSavedData_R(VirtualTreeRoot);
        }

        public void ApplyOpenedSavedData_R(VirtualGameObject CurrentVirtualGameObject)
        {
            if (CurrentVirtualGameObject == null)
            {
                return;
            }

            if (SavedOpenedVirtualObjNames.Contains(new KeyValuePair<GameObject, GameObject>(CurrentVirtualGameObject.LinkedDestination, CurrentVirtualGameObject.LinkedSource)))
            {
                CurrentVirtualGameObject.bIsOpenInMergeTree = true;
            }

            for (int i = 0; i < CurrentVirtualGameObject.GetChildCount(); i++)
            {
                ApplyOpenedSavedData_R(CurrentVirtualGameObject.GetChild(i));
            }
        }

        public void CreateSavedOpenedVirtualObjects()
        {
            SavedOpenedVirtualObjNames = new HashSet<KeyValuePair<GameObject, GameObject>>();

            CreateSavedOpenedVirtualObjects_R(VirtualTreeRoot);
        }

        public void CreateSavedOpenedVirtualObjects_R(VirtualGameObject CurrentVirtualGameObject)
        {
            if (CurrentVirtualGameObject == null)
            {
                return;
            }

            if (CurrentVirtualGameObject.bIsOpenInMergeTree)
            {
                SavedOpenedVirtualObjNames.Add(new KeyValuePair<GameObject, GameObject>(CurrentVirtualGameObject.LinkedDestination, CurrentVirtualGameObject.LinkedSource));
            }

            for (int i = 0; i < CurrentVirtualGameObject.GetChildCount(); i++)
            {
                CreateSavedOpenedVirtualObjects_R(CurrentVirtualGameObject.GetChild(i));
            }
        }

        public void CreateVirtualTreeFromDestinationAvatar()
        {
            VirtualTreeRoot = CreateVirtualTreeFromDestinationAvatar_R(CopierSettingsHandler.GetInstance().Destination);
        }

        public VirtualGameObject CreateVirtualTreeFromDestinationAvatar_R(GameObject currentDestObj)
        {
            VirtualGameObject newVirtualGameObject = new VirtualGameObject(currentDestObj);

            for (int i = 0; i < currentDestObj.transform.childCount; i++)
            {
                newVirtualGameObject.AddVirtualChild(CreateVirtualTreeFromDestinationAvatar_R(currentDestObj.transform.GetChild(i).gameObject));
            }

            AvatarMatchHandler.GetInstance().LinkVirtualObj(newVirtualGameObject);

            return newVirtualGameObject;
        }

        public VirtualGameObject CreateAttachmentOpVirtualObjects(AttachmentOperation attachmentOp)
        {
            VirtualGameObject topLevelVirtualObject = AddAttachableVirtualObjects_R(attachmentOp.AttachmentPoint, attachmentOp.SourceAttachableObject, attachmentOp);
            _TopLevelAttachableVirtualObjects.Add(topLevelVirtualObject);
            
            return topLevelVirtualObject;
        }

        protected VirtualGameObject AddAttachableVirtualObjects_R(VirtualGameObject parentVirtualGameObject, GameObject currentAttachableSourceObject, AttachmentOperation attachmentOp)
        {
            VirtualGameObject newVirtualObject = new VirtualGameObject(currentAttachableSourceObject, attachmentOp);

            if (parentVirtualGameObject != null)
            {
                newVirtualObject.Parent = parentVirtualGameObject;
            }

            AvatarMatchHandler.GetInstance().LinkVirtualObj(newVirtualObject);

            // Loop through childern

            for (int i = 0; i < currentAttachableSourceObject.transform.childCount; i++)
            {
                AddAttachableVirtualObjects_R(newVirtualObject, currentAttachableSourceObject.transform.GetChild(i).gameObject, attachmentOp);
            }

            return newVirtualObject;
        }

        public void UpdateAttachables()
        {

            foreach (VirtualGameObject TopLevelVirtualGameObject in _TopLevelAttachableVirtualObjects)
            {
                VirtualGameObject NewVirtualAttachtmentParent = TopLevelVirtualGameObject.AttachmentOp.AttachmentPoint;
                if (TopLevelVirtualGameObject.Parent != NewVirtualAttachtmentParent)
                {
                    TopLevelVirtualGameObject.Parent = NewVirtualAttachtmentParent;
                }
            }
        }

        public void UpOpenParents(VirtualGameObject VirtualObj)
        {
            VirtualGameObject CurrentVirtualObj = VirtualObj;
            while (true)
            {
                if (CurrentVirtualObj.Parent == null)
                {
                    break;
                }

                CurrentVirtualObj = CurrentVirtualObj.Parent;
                CurrentVirtualObj.bIsOpenInMergeTree = true;
            }
        }

        public void SetSelectedVirtualObj(VirtualGameObject newSelectedVirtualObj, bool inOpenToChildern = false, bool bUnselectIfSame = false)
        {
            if (bUnselectIfSame && SelectedVirtualGameObject == newSelectedVirtualObj)
            {
                SelectedVirtualGameObject = null;
            }
            else
            {
                SelectedVirtualGameObject = newSelectedVirtualObj;

                if (inOpenToChildern)
                {
                    newSelectedVirtualObj.bIsOpenInMergeTree = true;
                }

                UpOpenParents(newSelectedVirtualObj);
            }
        }

        public void ResetOrderedVirtualObjects()
        {
            _OrderedVirtualObjects = new List<VirtualGameObject>();

            if (VirtualTreeRoot == null)
            {
                return;
            }

            _OrderedVirtualObjects.Add(VirtualTreeRoot);
            VirtualTreeRoot.VirtualTreeOrder = 0;

            int currentIndex = 0;
            while (currentIndex < _OrderedVirtualObjects.Count)
            {
                // Go through and add childern of the current index to the ordered list
                for (int i = 0; i < _OrderedVirtualObjects[currentIndex].GetChildCount(); i++)
                {
                    _OrderedVirtualObjects.Add(_OrderedVirtualObjects[currentIndex].GetChild(i));
                    _OrderedVirtualObjects[currentIndex].VirtualTreeOrder = _OrderedVirtualObjects.Count - 1;
                }

                currentIndex++;
            }

            ComponentOperationHandler.GetInstance().RefreshCompOps();
        }

        public int GetOrderedVirtualObjectsCount()
        {
            return _OrderedVirtualObjects.Count;
        }

        public VirtualGameObject GetOrderedVirtualObject(int index)
        {
            return _OrderedVirtualObjects[index];
        }

        /*public VirtualGameObject GetVirtualGameObjectBySourceGameObject(GameObject SourceGameObject)
        {
            return GetVirtualGameObjectBySourceGameObject_R(VirtualTreeRoot, SourceGameObject);
        }

        public VirtualGameObject GetVirtualGameObjectBySourceGameObject_R(VirtualGameObject CurrentVirtualGameObject, GameObject SourceGameObject)
        {
            if (CurrentVirtualGameObject.LinkedSource == SourceGameObject)
            {
                return CurrentVirtualGameObject;
            }

            for (int i = 0; i < CurrentVirtualGameObject.GetChildCount(); i++)
            {
                VirtualGameObject FoundVirtualGameObject = GetVirtualGameObjectBySourceGameObject_R(CurrentVirtualGameObject.GetChild(i), SourceGameObject);

                // If null, then not found
                if (FoundVirtualGameObject != null)
                {
                    return FoundVirtualGameObject;
                }
            }

            return null;
        }

        public VirtualGameObject GetVirtualGameObjectByDestinationGameObject(GameObject DestinationGameObject)
        {
            return GetVirtualGameObjectByDestinationGameObject_R(VirtualTreeRoot, DestinationGameObject);
        }

        public VirtualGameObject GetVirtualGameObjectByDestinationGameObject_R(VirtualGameObject CurrentVirtualGameObject, GameObject DestinationGameObject)
        {
            if (CurrentVirtualGameObject.LinkedDestination == DestinationGameObject)
            {
                return CurrentVirtualGameObject;
            }

            for (int i = 0; i < CurrentVirtualGameObject.GetChildCount(); i++)
            {
                VirtualGameObject FoundVirtualGameObject = GetVirtualGameObjectByDestinationGameObject_R(CurrentVirtualGameObject.GetChild(i), DestinationGameObject);

                // If null, then not found
                if (FoundVirtualGameObject != null)
                {
                    return FoundVirtualGameObject;
                }
            }

            return null;
        }*/
    }
}

#endif