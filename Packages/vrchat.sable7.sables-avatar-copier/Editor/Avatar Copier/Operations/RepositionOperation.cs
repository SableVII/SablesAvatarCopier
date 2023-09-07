#if (UNITY_EDITOR)
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Operations
{
    public class RepositionAvatarOperation : Operation
    {
        //public bool bUserSetEnabled = true;
        protected Transform OriginalRepositionTransform { get; }

        protected bool _bUserLocationChanged = false;
        public bool bUserLocationChanged { get { return _bUserLocationChanged; } }

        protected bool _bUserRotationChanged = false;
        public bool bUserRotationChanged { get { return _bUserRotationChanged; } }

        protected bool _bUserScaleChanged = false;
        public bool bUserScaleChanged { get { return _bUserScaleChanged; } }

        public bool bUserSetPositionEnabled { get; set; } = true;
        protected Vector3 _RepositionLocation = new Vector3();
        public Vector3 RepositionLocation
        {
            get
            {
                if (_bUserLocationChanged)
                {
                    return _RepositionLocation;
                }
                else
                {
                    return OriginalRepositionTransform.transform.localPosition;
                }
            }
            set
            {
                if (_bUserLocationChanged || value != OriginalRepositionTransform.localPosition)
                {
                    _bUserLocationChanged = true;
                    _RepositionLocation = value;
                }
            }
        }

        public bool bUserSetRotationEnabled { get; set; } = true;
        protected Vector3 _RepositionRotation = new Vector3();
        public Vector3 RepositionRotation
        {
            get
            {
                if (_bUserRotationChanged)
                {
                    return _RepositionRotation;
                }
                else
                {
                    return OriginalRepositionTransform.transform.localEulerAngles;
                }
            }
            set
            {
                if (_bUserRotationChanged || value != OriginalRepositionTransform.localEulerAngles)
                {
                    _bUserRotationChanged = true;
                    _RepositionRotation = value;
                }
            }
        }

        public bool bUserSetScaleEnabled { get; set; } = true;
        protected Vector3 _RepositionScale = new Vector3();
        public Vector3 RepositionScale
        {
            get
            {
                if (_bUserScaleChanged)
                {
                    return _RepositionScale;
                }
                else
                {
                    return OriginalRepositionTransform.transform.localScale;
                }
            }
            set
            {
                if (_bUserScaleChanged || value != OriginalRepositionTransform.localScale)
                {
                    _bUserScaleChanged = true;
                    _RepositionScale = value;
                }
            }
        }

        public RepositionAvatarOperation(Transform inTransform)
        {
            OriginalRepositionTransform = inTransform;

            //_RepositionLocation = inTransform.localPosition;
            //_RepositionRotation = inTransform.localEulerAngles;
            //_RepositionScale = inTransform.localScale;

            bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations");

            bUserSetPositionEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations");
            bUserSetRotationEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations");
            bUserSetScaleEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMiscOperations");
        }
    }
}
#endif