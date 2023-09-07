#if (UNITY_EDITOR)
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Operations
{
    public class AvatarIDOperation : Operation
    {
        //public bool bUserSetEnabled = true;
        protected bool _bUnableToFindComp = false;
        public bool bUnableToFindComp { get { return _bUnableToFindComp; } }
        protected string OriginalAvatarID { get; }
        public string AvatarID { get; set; }

        //protected VRC.Core.PipelineManager _PipelineManagerRef = null;
        //public VRC.Core.PipelineManager PipeLineManagerRef { get { return _PipelineManagerRef; } }

        public AvatarIDOperation(string inAvatarID)
        {
            OriginalAvatarID = inAvatarID;
            AvatarID = inAvatarID;

            //CheckDoesComponentExist();
            bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations");
        }
    }
}
#endif