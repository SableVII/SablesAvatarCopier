#if (UNITY_EDITOR)
using SablesTools.AvatarCopier.Operations;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{
    public class MiscOperationHandler : OperationHandler
    {
        private static MiscOperationHandler _Instance = null;

        public static MiscOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new MiscOperationHandler();
            }

            return _Instance;
        }

        private MiscOperationHandler()
        {

        }

        public RepositionAvatarOperation RepositionOp = null;
        public RepositionAvatarOperation SavedRepositionOp = null;
        public AvatarIDOperation AvatarIDOp = null;
        public AvatarIDOperation SavedAvatarIDOp = null;

        public void Reset()
        {
            RepositionOp = null;
            SavedRepositionOp = null;
            AvatarIDOp = null;
            SavedAvatarIDOp = null;
        }

        public void SavedReset()
        {
            SavedRepositionOp = RepositionOp;
            SavedAvatarIDOp = AvatarIDOp;

            RepositionOp = null;
            SavedAvatarIDOp = null;
        }

        public void ApplySavedData()
        {
            if (SavedRepositionOp != null && RepositionOp != null)
            {
                RepositionOp.bUserSetEnabled = SavedRepositionOp.bUserSetEnabled;

                if (SavedRepositionOp.bUserLocationChanged)
                {
                    RepositionOp.RepositionLocation = SavedRepositionOp.RepositionLocation;
                }

                if (SavedRepositionOp.bUserRotationChanged)
                {
                    RepositionOp.RepositionRotation = SavedRepositionOp.RepositionRotation;
                }

                if (SavedRepositionOp.bUserScaleChanged)
                {
                    RepositionOp.RepositionScale = SavedRepositionOp.RepositionScale;
                }

                RepositionOp.bUserSetPositionEnabled = SavedRepositionOp.bUserSetPositionEnabled;
                RepositionOp.bUserSetRotationEnabled = SavedRepositionOp.bUserSetRotationEnabled;
                RepositionOp.bUserSetScaleEnabled = SavedRepositionOp.bUserSetScaleEnabled;
            }

            if (SavedAvatarIDOp != null && AvatarIDOp != null)
            {
                AvatarIDOp.bUserSetEnabled = SavedAvatarIDOp.bUserSetEnabled;
            }
        }

        public void CreateMiscOperations()
        {

            // Reposition Avatar Operation
            if (CopierSettingsHandler.GetInstance().bMatchInputsTogether)
            {
                RepositionOp = new RepositionAvatarOperation(CopierSettingsHandler.GetInstance().Source.transform);
            }

            // Avatar ID Op
            VRC.Core.PipelineManager PipelineManager = CopierSettingsHandler.GetInstance().Source.GetComponentInChildren<VRC.Core.PipelineManager>(true);
            if (PipelineManager != null)
            {
                AvatarIDOp = new AvatarIDOperation(PipelineManager.blueprintId);
            }
        }

        public int GetTotalMiscOperationCount()
        {
            int count = 0;
            if (RepositionOp != null)
            {
                count++;
            }

            if (AvatarIDOp != null)
            {
                count++;
            }

            return count;
        }

        public int GetTotalMiscEnabledOperationCount()
        {
            int count = 0;
            if (RepositionOp != null && RepositionOp.bUserSetEnabled)
            {
                count++;
            }

            if (AvatarIDOp != null && AvatarIDOp.bUserSetEnabled)
            {
                count++;
            }

            return count;
        }

        public void SetAllEnabledStatus(bool inStatus)
        {
            if (RepositionOp != null)
            {
                RepositionOp.bUserSetEnabled = inStatus;
            }

            if (AvatarIDOp != null)
            {
                AvatarIDOp.bUserSetEnabled = inStatus;
            }
        }
    }
}

#endif