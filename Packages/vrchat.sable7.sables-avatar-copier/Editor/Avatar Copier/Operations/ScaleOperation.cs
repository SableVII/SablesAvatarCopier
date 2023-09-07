#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SablesTools.AvatarCopier.Operations
{
    public class ScaleOperation : Operation
    {
        //public bool bUserSetEnabled = true;
        public Transform SourceTransform { get; }
        
        protected bool _bUserChanged = false;
        public bool bUserChanged { get { return _bUserChanged; } }

        protected Vector3 _Scale = new Vector3();
        public Vector3 Scale { get { return GetScale(); } set { SetScale(value); } }
        public Data.VirtualGameObject VirtualGameObj { get; }

        public ScaleOperation(Transform inSourceTransform, Data.VirtualGameObject inVirtualGameObj)
        {
            SourceTransform = inSourceTransform;
            VirtualGameObj = inVirtualGameObj;

            _bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseScaleOperations");
        }

        public void SetScale(Vector3 inScale)
        {
            if (inScale.Equals(SourceTransform.localScale) && !_bUserChanged)
            {
                return;
            }
            _Scale = inScale;
            _bUserChanged = true;
        }

        public void Reset()
        {
            _bUserChanged = false;
            _Scale = SourceTransform.transform.localScale;
        }

        public Vector3 GetScale()
        {
            if (_bUserChanged)
            {
                return _Scale;
            }

            return SourceTransform.localScale;
        }
    }
}
#endif