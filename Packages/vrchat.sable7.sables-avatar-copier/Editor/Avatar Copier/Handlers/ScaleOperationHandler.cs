#if (UNITY_EDITOR)
using System.Collections.Generic;
using SablesTools.AvatarCopier.Operations;
using System;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{  
    public class ScaleOperationHandler : OperationHandler
    {
        private static ScaleOperationHandler _Instance = null;

        public static ScaleOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new ScaleOperationHandler();
            }

            return _Instance;
        }

        private ScaleOperationHandler()
        {

        }

        protected List<ScaleOperation> ScaleOperations = new List<ScaleOperation>();
        protected List<ScaleOperation> SavedScaleOperations = new List<ScaleOperation>();

        public void Reset()
        {
            ScaleOperations = new List<ScaleOperation>();
            SavedScaleOperations = new List<ScaleOperation>();
        }

        public void SavedReset()
        {
            SavedScaleOperations = new List<ScaleOperation>(ScaleOperations);
            ScaleOperations = new List<ScaleOperation>();
        }

        public void ApplySavedData()
        {
            foreach (ScaleOperation SavedScaleOp in SavedScaleOperations)
            {
                foreach (ScaleOperation ScaleOp in ScaleOperations)
                {
                    if (SavedScaleOp.SourceTransform == ScaleOp.SourceTransform)
                    {
                        ScaleOp.bUserSetEnabled = SavedScaleOp.bUserSetEnabled;
                        if (SavedScaleOp.bUserChanged)
                        {
                            ScaleOp.SetScale(SavedScaleOp.Scale);
                        }
                        break;
                    }
                }
            }
        }

        public void CreateScaleOperations()
        {
            ScaleOperations = new List<ScaleOperation>();

            if (CopierSettingsHandler.GetInstance().Destination == null || CopierSettingsHandler.GetInstance().Source == null)
            {
                return;
            }

            for (int i = 0; i < AvatarMatchHandler.GetInstance().DestinationGameObjectsCount; i++)
            {
                Data.VirtualGameObject virtualObj = AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromDestinationObject(AvatarMatchHandler.GetInstance().GetDestinationGameObject(i));
                if (virtualObj == null || virtualObj.LinkedSource == null || virtualObj.LinkedSource == CopierSettingsHandler.GetInstance().Source)
                {
                    continue;
                }

                bool Matches = true;
                if (Math.Abs(virtualObj.LinkedSource.transform.localScale.x - virtualObj.LinkedDestination.transform.localScale.x) > CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
                {
                    Matches = false;
                }
                else if (Math.Abs(virtualObj.LinkedSource.transform.localScale.y - virtualObj.LinkedDestination.transform.localScale.y) > CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
                {
                    Matches = false;
                }
                else if (Math.Abs(virtualObj.LinkedSource.transform.localScale.z - virtualObj.LinkedDestination.transform.localScale.z) > CopierSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
                {
                    Matches = false;
                }

                if (!Matches)
                {
                    ScaleOperations.Add(new ScaleOperation(virtualObj.LinkedSource.transform, virtualObj));
                }
            }

            //foreach (GameObject DestinationObj in AvatarMatchHandler.GetInstance().DestinationMatchedData.Keys)
            //{
            //    MatchedObjectData MatchedData = AvatarMatchHandler.GetInstance().DestinationMatchedData[DestinationObj];

            //    // Make MatchedData.SourceGameObject is also not the Source Avatar. Scale gets handled by its own Misc Operation
            //    if (MatchedData.SourceGameObject != null && MatchedData.SourceGameObject != MergerSettingsHandler.GetInstance().SourceAvatar)
            //    {
            //        // If Scales don't match along epsilon create a ScaleOperation
            //        /*if (Math.Abs(MatchedData.SourceGameObject.transform.localScale.sqrMagnitude - MatchedData.DestinationGameObject.transform.localScale.sqrMagnitude) > MergerSavedData.GetInstance().ScaleEpsilon * MergerSavedData.GetInstance().ScaleEpsilon)
            //        {
            //            ScaleOperations.Add(new ScaleOperation(MatchedData.SourceGameObject.transform, MatchedData.VirtualObj));
            //        }*/

            //        bool Matches = true;
            //        if (Math.Abs(MatchedData.SourceGameObject.transform.localScale.x - MatchedData.DestinationGameObject.transform.localScale.x) > MergerSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
            //        {
            //            Matches = false;
            //        }
            //        else if (Math.Abs(MatchedData.SourceGameObject.transform.localScale.y - MatchedData.DestinationGameObject.transform.localScale.y) > MergerSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
            //        {
            //            Matches = false;
            //        }
            //        else if (Math.Abs(MatchedData.SourceGameObject.transform.localScale.z - MatchedData.DestinationGameObject.transform.localScale.z) > MergerSettingsHandler.GetInstance().GetFloatDataValue("ScaleEpsilon"))
            //        {
            //            Matches = false;
            //        }

            //        if (!Matches)
            //        {
            //            ScaleOperations.Add(new ScaleOperation(MatchedData.SourceGameObject.transform, MergeTreeHandler.GetInstance().GetVirtualGameObjectByDestinationGameObject(DestinationObj)));
            //        }
            //    }
            //}           
        }

        public int GetScaleOperationCount()
        {
            return ScaleOperations.Count;
        }

        public int GetEnabledScaledOperationCount()
        {
            int count = 0;
            foreach (ScaleOperation ScaleOp in ScaleOperations)
            {
                if (ScaleOp.bUserSetEnabled)
                {
                    count++;
                }
            }

            return count;
        }

        public ScaleOperation GetScaleOperation(int index)
        {
            if (index >= 0 && index < ScaleOperations.Count)
            {
                return ScaleOperations[index];
            }

            return null;
        }
    }
}

#endif