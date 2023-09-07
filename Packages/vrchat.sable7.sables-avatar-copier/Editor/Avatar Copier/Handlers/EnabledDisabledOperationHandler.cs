#if (UNITY_EDITOR)
using System.Collections.Generic;
using SablesTools.AvatarCopier.Handlers;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Operations
{
    public class EnabledDisabledOperation : Operation
    {
        protected bool OriginalEnabledStatus = false;
        public bool EnabledStatus = false;
        public Data.VirtualGameObject VirtualObj { get; }

        //public bool bUserSetEnabled = true;

        public EnabledDisabledOperation(Data.VirtualGameObject inVirtualObj, bool inEnabledStatus)
        {
            VirtualObj = inVirtualObj;
            OriginalEnabledStatus = inEnabledStatus;
            EnabledStatus = inEnabledStatus;

            bUserSetEnabled = CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseEnabledDisabledOperations");
        }

        public bool HasUpdatedEnabledStatus()
        {
            return OriginalEnabledStatus != EnabledStatus;
        }

        public bool GetOriginalEnabledStatus()
        {
            return OriginalEnabledStatus;
        }

        public Data.VirtualGameObject GetVirtualObject()
        {
            return VirtualObj;
        }
    }


    public class EnabledDisabledOperationHandler : OperationHandler
    {
        private static EnabledDisabledOperationHandler _Instance = null;

        public static EnabledDisabledOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new EnabledDisabledOperationHandler();
            }

            return _Instance;
        }

        private EnabledDisabledOperationHandler()
        {

        }

        protected List<EnabledDisabledOperation> EnabledDisabledOperations = new List<EnabledDisabledOperation>();
        protected List<EnabledDisabledOperation> SavedEnabledDisabledOperations = new List<EnabledDisabledOperation>();

        public void Reset()
        {
            EnabledDisabledOperations = new List<EnabledDisabledOperation>();
            SavedEnabledDisabledOperations = new List<EnabledDisabledOperation>();
        }

        public void SavedReset()
        {
            SavedEnabledDisabledOperations = new List<EnabledDisabledOperation>(EnabledDisabledOperations);
            EnabledDisabledOperations = new List<EnabledDisabledOperation>();
        }

        public void ApplySavedData()
        {
            foreach (EnabledDisabledOperation SavedOp in SavedEnabledDisabledOperations)
            {
                foreach (EnabledDisabledOperation NewOp in EnabledDisabledOperations)
                {
                    // Linked Source should never be null
                    if (NewOp.VirtualObj.LinkedSource == SavedOp.VirtualObj.LinkedSource && NewOp.VirtualObj.LinkedSource.name == SavedOp.VirtualObj.LinkedSource.name)
                    {
                        NewOp.bUserSetEnabled = SavedOp.bUserSetEnabled;
                        NewOp.EnabledStatus = SavedOp.EnabledStatus;
                        break;
                    }
                }
            }
        }

        public void CreateEnabledDisabledOperations()
        {
            EnabledDisabledOperations = new List<EnabledDisabledOperation>();

            /*if (MergerSettingsHandler.GetInstance().DestinationAvatar == null || MergerSettingsHandler.GetInstance().SourceAvatar == null)
            {
                return;
            }*/

            for (int i = 0; i < AvatarMatchHandler.GetInstance().DestinationGameObjectsCount; i++)
            {
                Data.VirtualGameObject virtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromDestinationObject(AvatarMatchHandler.GetInstance().GetDestinationGameObject(i));
                if (virtualObj == null || virtualObj.LinkedSource == null)
                {
                    continue;
                }

                if (virtualObj.LinkedDestination.activeSelf != virtualObj.LinkedSource.activeSelf)
                {
                    EnabledDisabledOperations.Add(new EnabledDisabledOperation(virtualObj, virtualObj.LinkedSource.activeSelf));
                }
            }

            //foreach (GameObject DestinationObj in AvatarMatchHandler.GetInstance().Des.Keys)
            //{
            //    MatchedObjectData MatchedData = AvatarMatchHandler.GetInstance().DestinationMatchedData[DestinationObj];

            //    if (MatchedData.SourceGameObject != null)
            //    {
            //        // If activeSelf don't match create an Enabled/Disabled Operation
            //        if (MatchedData.SourceGameObject.activeSelf != DestinationObj.activeSelf)
            //        {
            //            EnabledDisabledOperations.Add(new EnabledDisabledOperation(MergeTreeHandler.GetInstance().GetVirtualGameObjectByDestinationGameObject(DestinationObj), MatchedData.SourceGameObject.activeSelf));        
            //        }
            //    }
            //}           
        }

        public int GetEnabledDisabledOperationCount()
        {
            return EnabledDisabledOperations.Count;
        }

        public int GetEnabledEnabledDisabledOperationCount()
        {
            int count = 0;
            foreach (EnabledDisabledOperation EnabledDisabledOp in EnabledDisabledOperations)
            {
                if (EnabledDisabledOp.bUserSetEnabled)
                {
                    count++;
                }
            }

            return count;
        }

        public EnabledDisabledOperation GetEnabledDisabledOperation(int index)
        {
            if (index >= 0 && index < EnabledDisabledOperations.Count)
            {
                return EnabledDisabledOperations[index];
            }

            return null;
        }
    }
}

#endif