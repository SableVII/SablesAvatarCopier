#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.Data
{
    public class RegisteredReference
    {
        public string PropFieldName { get; }
        public System.Type ReferenceType { get; }
        protected List<RegisteredReferenceElement> _references = new List<RegisteredReferenceElement>();

        public Operations.ComponentOperation CompOp { get; }

        public bool GetIsList()
        {
            return _references.Count > 1;
        }

        public RegisteredReference(string propFieldName, System.Type referenceType, Operations.ComponentOperation compOpRef)
        {
            PropFieldName = propFieldName;
            ReferenceType = referenceType;
            CompOp = compOpRef;
        }

        public void AddRef(GameObject inGameObjectRef)
        {
            VirtualGameObject virtualObj = null;
            bool bIsAPreExistingGameObj = false;
            bool bSpecialOutofSceneRef = false;
            bool bNoMatchesFOund = false;

            if (inGameObjectRef != null)
            {
                if (inGameObjectRef.scene != Handlers.CopierSettingsHandler.GetInstance().Destination.scene)
                {
                    bSpecialOutofSceneRef = true;
                }
                else
                {
                    virtualObj = Handlers.AvatarMatchHandler.GetInstance().GetVirtualGameObjectFromObject(inGameObjectRef);

                    // If can't find a Match, then try manually finding the first matching VirtualGameObject
                    if (virtualObj == null)
                    {
                        // Attempt to find a VirtualObj that matches name
                        virtualObj = Handlers.AvatarMatchHandler.GetInstance().LinkSourceGameObjectToVirtual(inGameObjectRef);
                    }

                    //  if STILL no VirtualObject found... then set this Reference as a GameObject reference only if this is going on an PreExistingCompOp.
                    if (virtualObj == null)
                    {
                        if (CompOp.GetType() == typeof(Operations.PreExistingComponentOperation))
                        {
                            bIsAPreExistingGameObj = true;
                        }
                        else
                        {
                            bNoMatchesFOund = true;
                        }
                    }
                }
            }


            RegisteredReferenceElement RefData = null;
            if (inGameObjectRef == null)
            {
                RefData = new RegisteredReferenceElement(this, null);
            }
            else if (bNoMatchesFOund)
            {
                RefData = new RegisteredReferenceElement(this, null, inGameObjectRef);
            }
            else if (virtualObj != null)
            {
                RefData = new RegisteredReferenceElement(this, virtualObj, inGameObjectRef);
            }
            else if (bIsAPreExistingGameObj || bSpecialOutofSceneRef)
            {
                RefData = new RegisteredReferenceElement(this, inGameObjectRef);
            }

            _references.Add(RefData);
        }

        // Gets the Original Reference or Overriten Reference GameObject
        public GameObject GetRunTimeGameObjectRef(int index = 0)
        {
            if (index >= _references.Count || index < 0)
            {
                return null;
            }

            // If Game Object Ref Type
            if (_references[index].bIsNotVirtualGameObjectType)
            {
                if (_references[index].GameObjectReference != null)
                {
                    return _references[index].GameObjectReference;
                }

                return null;
            }

            // If Virtual Object Ref Type
            if (_references[index].VirtualReference != null)
            {
                return _references[index].VirtualReference.RunTimeObject;
            }

            return null;
        }

        // Gets the Original Reference or Overriten Reference as this RegisteredFieldPropRefData's ReferenceType
        public Component GetRunTimeComponentRef(int index = 0)
        {
            if (index < 0 || index >= _references.Count)
            {
                return null;
            }

            //if (References[index].bMissingReference)
            //{
            //    return null;
            //}

            //if (References[index].VirtualReference != null)
            //{
            //    return References[index].VirtualReference.RunTimeObject.GetComponent(ReferenceType);
            //}

            //if (_references[index].SpecialOutOfAvatarRef != null)
            //{
            //    return _references[index].SpecialOutOfAvatarRef.transform;
            //}

            //if (_references[index].VirtualReference != null && _references[index].VirtualReference.RunTimeObject != null)
            //{
            //    return _references[index].VirtualReference.RunTimeObject.GetComponent(ReferenceType);
            //}

            // If Game Object Ref Type
            if (_references[index].bIsNotVirtualGameObjectType)
            {
                if (_references[index].GameObjectReference != null)
                {
                    return _references[index].GameObjectReference.GetComponent(ReferenceType);
                }

                return null;
            }

            // If Virtual Object Ref Type
            if (_references[index].VirtualReference != null)
            {
                return _references[index].VirtualReference.RunTimeObject.GetComponent(ReferenceType);
            }

            return null;
        }
        
        public int GetReferenceCount()
        {
            return _references.Count;
        }

        public RegisteredReferenceElement GetReferenceElement(int index)
        {
            if (index < 0 || index >= _references.Count)
            {
                return null;
            }
            return _references[index];
        }
    }
}

#endif