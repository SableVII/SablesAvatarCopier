#if (UNITY_EDITOR)
using SablesTools.AvatarCopier.Warnings;
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.Data
{
    public class RegisteredReferenceElement
    {
        protected bool _bIsGameObjectType = false;
        public bool bIsNotVirtualGameObjectType { get { return _bIsGameObjectType; } }

        //public bool bMissingReference = false;
        //public bool bNoneReference = false;
        public bool bIsUserSet = false;

        public VirtualGameObject OriginalVirtualReference { get; } = null;
        protected VirtualGameObject _VirutalReference = null;
        public VirtualGameObject VirtualReference { get { return _VirutalReference; } set { SetVirtualReference(value); } }

        public GameObject OriginalGameObjectReference { get; } = null;
        protected GameObject _GameObjectReference = null;
        public GameObject GameObjectReference { get { return _GameObjectReference; } set { SetGameObjectReference(value); } }

        public System.Type ReferenceType { get; } = typeof(GameObject);

        protected bool _bIsOutOfSceneGameObject = false;
        public bool bIsOutOfSceneGameObject { get { return _bIsOutOfSceneGameObject; } }

        //protected GameObject _specialOutOfAvatarRef = null;
        //public GameObject SpecialOutOfAvatarRef { get { return _specialOutOfAvatarRef; }  set { SetSpecialReference(value); } }
        //public GameObject OverridenSpecialOutOfAvatarRef = null;

        public bool bFailedToMatch { get; } = false;

        public RegisteredReference RegisteredDataRef { get; }

        protected AvatarCopierWarning _refWarning = null;
        public AvatarCopierWarning RefWarning { get { return _refWarning; } }

        //public GameObject ExpectedSourceGameObject { get; } = null;
        public GameObject ExpectedGameObject { get; } = null;

        // None Ref
        public RegisteredReferenceElement(RegisteredReference inReferenceData)
        {
            RegisteredDataRef = inReferenceData;
            ReferenceType = RegisteredDataRef.ReferenceType;

            _bIsGameObjectType = false;
        }

        // Normal Virtual Game Object Ref
        public RegisteredReferenceElement(RegisteredReference inReferenceData, VirtualGameObject inVirtualRef, GameObject expectedRefedGameObject)
        {
            RegisteredDataRef = inReferenceData;
            ReferenceType = RegisteredDataRef.ReferenceType;

            _bIsGameObjectType = false;

            OriginalVirtualReference = inVirtualRef;
            _VirutalReference = OriginalVirtualReference;

            ExpectedGameObject = expectedRefedGameObject;

            if (inVirtualRef == null)
            {
                bFailedToMatch = true;
            }
        }

        // Game Object Ref
        public RegisteredReferenceElement(RegisteredReference inReferenceData, GameObject inGameObjectRef)
        {
            RegisteredDataRef = inReferenceData;
            ReferenceType = RegisteredDataRef.ReferenceType;

            _bIsGameObjectType = true;

            OriginalGameObjectReference = inGameObjectRef;
            _GameObjectReference = OriginalGameObjectReference;

            if (_GameObjectReference != null)
            {
                if (_GameObjectReference.scene != Handlers.CopierSettingsHandler.GetInstance().Destination.scene)
                {
                    _bIsOutOfSceneGameObject = true;
                }
            }
        }

        //public RegisteredReferenceElement(RegisteredReference inReferenceData, VirtualGameObject inOriginalVirtualReference, GameObject expectedSourceObject, bool inFailedToMatch = false)
        //{
        //    RegisteredDataRef = inReferenceData;
        //    ReferenceType = RegisteredDataRef.ReferenceType;
        //    OriginalVirtualReference = inOriginalVirtualReference;
        //    _VirutalReference = inOriginalVirtualReference;
        //    ExpectedSourceGameObject = expectedSourceObject;
        //    bFailedToMatch = inFailedToMatch;

        //    /*if (VirtualReference == null && bFailedToMatch == true)
        //    {
        //        WarningHandler.GetInstance().AddWarning(new MissingReferenceWarning(this));
        //    }*/
        //}

        //public RegisteredReferenceElement(RegisteredReference inReferenceData, GameObject inSpecialOutOfAvatarRef)
        //{
        //    RegisteredDataRef = inReferenceData;
        //    ReferenceType = RegisteredDataRef.ReferenceType;
        //    _specialOutOfAvatarRef = inSpecialOutOfAvatarRef;
        //    //bFailedToMatch = inFailedToMatch;

        //    /*if (SpecialOutOfAvatarRef == null)
        //    {
        //        WarningHandler.GetInstance().AddWarning(new MissingReferenceWarning(this));
        //    }*/
        //}

        protected void SetVirtualReference(VirtualGameObject virtualObj)
        {
            _VirutalReference = virtualObj;

            _GameObjectReference = null;
            _bIsGameObjectType = false;

            bIsUserSet = true;
        }

        protected void SetGameObjectReference(GameObject gameObj)
        {
            _GameObjectReference = gameObj;
            _bIsGameObjectType = true;

            _VirutalReference = null;

            bIsUserSet = true;
        }

        public void Reset_Inputs()
        {
            bIsUserSet = false;

            if (OriginalVirtualReference != null)
            {
                _VirutalReference = OriginalVirtualReference;
                _bIsGameObjectType = false;
            }
            else
            {
                _GameObjectReference = OriginalGameObjectReference;
                _bIsGameObjectType = true;
            }
        }

        public bool IsNoneValue()
        {
            if (_VirutalReference != null)
            {
                return false;
            }

            if (_GameObjectReference != null)
            {
                return false;
            }

            return true;
        }

        // Creates Warning if checked and it does need one. Returns true if Warning Exists and is still needed.
        public bool CheckNeedWarning()
        {
            // Clean out Old-Warning
            if (_refWarning != null)
            {
                Handlers.WarningHandler.GetInstance().RemoveWarning(_refWarning);
                _refWarning = null;
            }

            // Dont Create Warning if CompOp's VirtualObject is the RegRef's VirtualObject
            if (bIsNotVirtualGameObjectType == false && _VirutalReference == RegisteredDataRef.CompOp.VirtualGameObjectRef)
            {
                return false;
            }

            AvatarCopierWarning newWarning = null;

            // Just straight up not found on Destination Avatar
            if (_VirutalReference == null && !bIsUserSet && bFailedToMatch)
            {
                newWarning = new AvatarCopierMissingReferenceWarning(this);
            }

            if (_VirutalReference != null)
            {
                // Attachable is not attached/fully enabled
                if (newWarning == null && _VirutalReference != null && _VirutalReference.bIsAttachable && !_VirutalReference.AttachmentOp.bUserSetEnabled)
                {
                    newWarning = new AvatarCopierReferenceAttachableDisabledWarning(this);
                }

                // Attachable is not attached
                if (newWarning == null && _VirutalReference != null && _VirutalReference.bIsAttachable && _VirutalReference.AttachmentOp.AttachmentPoint == null)
                {
                    newWarning = new AvatarCopierReferenceAttachableUnattachedWarning(this);
                }
            }

            if (IsNoneValue() == false)
            {
                // If Reference type is of a Component Type, check to see if there is a ComponentOperation that is of type is fully active
                if (newWarning == null && typeof(Component).IsAssignableFrom(ReferenceType) && ReferenceType != typeof(Transform))
                {
                    // Virtual Reference
                    bool bFoundCompOp = false;
                    if (_VirutalReference != null)
                    {
                        for (int i = 0; i < _VirutalReference.GetPreExistingCount(); i++)
                        {
                            if (_VirutalReference.GetPreExisting(i).ComponentType == ReferenceType)
                            {
                                bFoundCompOp = true;
                                break;
                            }
                        }

                        if (!bFoundCompOp)
                        {
                            for (int i = 0; i < _VirutalReference.GetOverridingCount(); i++)
                            {
                                if (_VirutalReference.GetOverriding(i).ComponentType == ReferenceType && _VirutalReference.GetOverriding(i).IsFullyEnabled())
                                {
                                    bFoundCompOp = true;
                                    break;
                                }
                            }
                        }
                    }

                    // GameObject Reference
                    if (_GameObjectReference != null)
                    {
                        if (_GameObjectReference.GetComponent(ReferenceType) != null)
                        {
                            bFoundCompOp = true;
                        }
                    }


                    if (!bFoundCompOp)
                    {
                        newWarning = new AvatarCopierExpectedComponentMissingWarning(this);
                    }
                }
            }

            // Add Warning if needed
            if (newWarning != null)
            {
                _refWarning = newWarning;
                Handlers.WarningHandler.GetInstance().AddWarning(_refWarning);

                // Suppress if Warning is not Fully Enabled
                if (RegisteredDataRef.CompOp.IsFullyEnabled() == false)
                {
                    _refWarning.bSurpressed = true;
                }

                return true;
            }

            return false;
        }
    }
}

#endif