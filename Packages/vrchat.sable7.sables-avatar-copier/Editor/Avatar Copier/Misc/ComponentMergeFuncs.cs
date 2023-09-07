#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Handlers;
using SablesTools.AvatarCopier.Operations;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier
{
    public class ComponentMergeFuncs
    {
        public static void SetRegisteredReferences()
        {
            SetRegisteredReferences_R(MergeTreeHandler.GetInstance().VirtualTreeRoot);
        }

        protected static void SetRegisteredReferences_R(Data.VirtualGameObject currentVirtualObj)
        {
            for (int i = 0; i < currentVirtualObj.GetPreExistingCount(); i++)
            {
                SetCompOpRegisteredOp(currentVirtualObj.GetPreExisting(i));
            }

            for (int i = 0; i < currentVirtualObj.GetOverridingCount(); i++)
            {
                SetCompOpRegisteredOp(currentVirtualObj.GetOverriding(i));
            }

            for (int i = 0; i < currentVirtualObj.GetChildCount(); i++)
            {
                SetRegisteredReferences_R(currentVirtualObj.GetChild(i));
            }
        }

        protected static void SetCompOpRegisteredOp(ComponentOperation compOp)
        {
            if (compOp.IsFullyEnabled() == false)
            {
                return;
            }

            if (compOp.ComponentType == typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor))
            {
                CopyVRCAvatarDescriptorParameters(compOp);
            }
            else if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone))
            {
                CopyVRCPhysBoneParameters(compOp);
            }
            else if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider))
            {
                CopyVRCPhysBoneColliderParameters(compOp);
            }
            else if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver))
            {
                CopyVRCContactReceiverParameters(compOp);
            }
            else if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactSender))
            {
                CopyVRCContactSenderParameters(compOp);
            }
            else if (typeof(Renderer).IsAssignableFrom(compOp.ComponentType))
            {
                CopyRendererParameters(compOp);
            }
            else if (typeof(UnityEngine.Animations.IConstraint).IsAssignableFrom(compOp.ComponentType))
            {
                CopyConstraintParameters(compOp);
            }
            else if (compOp.ComponentType == typeof(ParticleSystem))
            {
                CopyParticleParameters(compOp);
            }
        }

        protected static void ShowRegisterablePropertiesAndFields(System.Type inType)
        {
            //System.Type inType = inComponent.GetType();
            System.Reflection.FieldInfo[] fields = inType.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.IsPublic && typeof(Component).IsAssignableFrom(field.FieldType) || typeof(GameObject).IsAssignableFrom(field.FieldType))
                {
                    //Debug.Log(" ~ Found Field: " + field.Name + " of type: " + field.FieldType);
                }
            }

            //System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.GetField;
            System.Reflection.PropertyInfo[] pinfos = inType.GetProperties();
            foreach (System.Reflection.PropertyInfo pinfo in pinfos)
            {
                if (pinfo.CanRead && pinfo.CanWrite && !AvatarCopierUtils.IgnoredPropertyAndFieldNames.Contains(pinfo.Name) && (typeof(Component).IsAssignableFrom(pinfo.PropertyType) || typeof(GameObject).IsAssignableFrom(pinfo.PropertyType)))
                {
                    //Debug.Log(" ~ Found Property: " + pinfo.Name + " of type: " + pinfo.PropertyType);
                }
                //else
                //{
                //    Debug.Log("Non-writtable: " + pinfo);
                //}
            }
        }

        protected static Component GetSingleComponentRef(ComponentOperation compOp, string propFieldName, int index = 0)
        {
            Data.RegisteredReference refData = compOp.RegisteredRefCollection.GetRegisteredDataOfName(propFieldName);
            if (refData != null)
            {
                return refData.GetRunTimeComponentRef(index);
            }
            return null;
        }

        protected static GameObject GetSingleGameObjectRef(ComponentOperation compOp, string propFieldName, int index = 0)
        {
            Data.RegisteredReference refData = compOp.RegisteredRefCollection.GetRegisteredDataOfName(propFieldName);
            if (refData != null)
            {
                return refData.GetRunTimeGameObjectRef(index);
            }
            return null;
        }

        protected static void CopyVRCAvatarDescriptorParameters(ComponentOperation compOp)
        {
            VRC.SDK3.Avatars.Components.VRCAvatarDescriptor destinationDescriptor = compOp.RunTimeComponent as VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

            // VisemeSkinnedMesh
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "VisemeSkinnedMesh"))
            {
                destinationDescriptor.VisemeSkinnedMesh = GetSingleComponentRef(compOp, "VisemeSkinnedMesh") as SkinnedMeshRenderer;
            }

            // lipSyncJawBone
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "lipSyncJawBone"))
            {
                destinationDescriptor.lipSyncJawBone = GetSingleComponentRef(compOp, "lipSyncJawBone") as Transform;
            }

            // customEyeLookSettings:leftEye
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.leftEye"))
            {
                destinationDescriptor.customEyeLookSettings.leftEye = GetSingleComponentRef(compOp, "customEyeLookSettings.leftEye") as Transform;
            }

            // customEyeLookSettings:RightEye
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.rightEye"))
            {
                destinationDescriptor.customEyeLookSettings.rightEye = GetSingleComponentRef(compOp, "customEyeLookSettings.rightEye") as Transform;
            }
            // customEyeLookSettings:upperLeftEyelid
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.upperLeftEyelid"))
            {
                destinationDescriptor.customEyeLookSettings.upperLeftEyelid = GetSingleComponentRef(compOp, "customEyeLookSettings.upperLeftEyelid") as Transform;
            }

            // customEyeLookSettings:upperRightEyelid
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.upperRightEyelid"))
            {
                destinationDescriptor.customEyeLookSettings.upperRightEyelid = GetSingleComponentRef(compOp, "customEyeLookSettings.upperRightEyelid") as Transform;
            }

            // customEyeLookSettings:lowerLeftEyelid
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.lowerLeftEyelid"))
            {
                destinationDescriptor.customEyeLookSettings.lowerLeftEyelid = GetSingleComponentRef(compOp, "customEyeLookSettings.lowerLeftEyelid") as Transform;
            }

            // customEyeLookSettings:lowerRightEyelid
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.lowerRightEyelid"))
            {
                destinationDescriptor.customEyeLookSettings.lowerRightEyelid = GetSingleComponentRef(compOp, "customEyeLookSettings.lowerRightEyelid") as Transform;
            }

            // customEyeLookSettings.eyelidsSkinnedMesh
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "customEyeLookSettings.eyelidsSkinnedMesh"))
            {
                destinationDescriptor.customEyeLookSettings.eyelidsSkinnedMesh = GetSingleComponentRef(compOp, "customEyeLookSettings.eyelidsSkinnedMesh") as SkinnedMeshRenderer;
            }
        }

        static void CopyVRCPhysBoneParameters(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone destinationPhysBone = compOp.RunTimeComponent as VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone;

            // rootTransform
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "rootTransform"))
            {
                destinationPhysBone.rootTransform = GetSingleComponentRef(compOp, "rootTransform") as Transform;
            }

            // ignoredTransforms
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "ignoreTransforms"))
            {
                destinationPhysBone.ignoreTransforms = new List<Transform>();
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("ignoreTransforms"); i++)
                {
                    destinationPhysBone.ignoreTransforms.Add(GetSingleComponentRef(compOp, "ignoreTransforms", i) as Transform);
                }
            }

            // colliders
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "colliders"))
            {
                destinationPhysBone.colliders = new List<VRC.Dynamics.VRCPhysBoneColliderBase>();
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("colliders"); i++)
                {
                    destinationPhysBone.colliders.Add(GetSingleComponentRef(compOp, "colliders", i) as VRC.Dynamics.VRCPhysBoneColliderBase);
                }
            }
        }

        static void CopyVRCPhysBoneColliderParameters(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider destinationPhysBoneCollider = compOp.RunTimeComponent as VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider;

            // rootTransform
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "rootTransform"))
            {
                destinationPhysBoneCollider.rootTransform = GetSingleComponentRef(compOp, "rootTransform") as Transform;
            }
        }

        static void CopyVRCContactReceiverParameters(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver destinationContactReceiver = compOp.RunTimeComponent as VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver;

            // rootTransform
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "rootTransform"))
            {
                destinationContactReceiver.rootTransform = GetSingleComponentRef(compOp, "rootTransform") as Transform;
            }
        }

        static void CopyVRCContactSenderParameters(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.Contact.Components.VRCContactSender destinationContactSender = compOp.RunTimeComponent as VRC.SDK3.Dynamics.Contact.Components.VRCContactSender;

            // rootTransform
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "rootTransform"))
            {
                destinationContactSender.rootTransform = GetSingleComponentRef(compOp, "rootTransform") as Transform;
            }
        }

        static void CopyClothParameters(ComponentOperation compOp)
        {
            Cloth destinationCloth = compOp.RunTimeComponent as Cloth;

            // capsuleColliders
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "capsuleColliders"))
            {
                List<CapsuleCollider> List = new List<CapsuleCollider>();
                
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("capsuleColliders"); i++)
                {
                    List.Add(GetSingleComponentRef(compOp, "capsuleColliders", i) as CapsuleCollider);
                }
                destinationCloth.capsuleColliders = List.ToArray();
            }

            // sphereColliders   // Special case as its a pair of two SphereColliders
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "sphereColliders"))
            {
                List<ClothSphereColliderPair> list = new List<ClothSphereColliderPair>();

                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("sphereCollidersFirst"); i++)
                {
                    list.Add(new ClothSphereColliderPair(GetSingleComponentRef(compOp, "sphereCollidersFirst", i) as SphereCollider, GetSingleComponentRef(compOp, "sphereCollidersSecond", i) as SphereCollider));
                }
                destinationCloth.sphereColliders = list.ToArray();
            }
        }

        static void CopyRendererParameters(ComponentOperation compOp)
        {
            Renderer destinationRenderer = compOp.RunTimeComponent as Renderer;

            // lightProbeProxyVolumeOverride
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "lightProbeProxyVolumeOverride"))
            {
                destinationRenderer.lightProbeProxyVolumeOverride = GetSingleGameObjectRef(compOp, "lightProbeProxyVolumeOverride");
            }

            // probeAnchor
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "probeAnchor"))
            {
                destinationRenderer.probeAnchor = GetSingleComponentRef(compOp, "probeAnchor") as Transform;
            }

            // Skinned Mesh Renderers
            if (compOp.ComponentType == typeof(SkinnedMeshRenderer))
            {
                SkinnedMeshRenderer DestinationSkinnedMeshRenderer = destinationRenderer as SkinnedMeshRenderer;

                // rootBone
                if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "rootBone"))
                {
                    DestinationSkinnedMeshRenderer.rootBone = GetSingleComponentRef(compOp, "rootBone") as Transform;
                }
            }
        }

        static void CopyConstraintParameters(ComponentOperation compOp)
        {
            UnityEngine.Animations.IConstraint destinationConstraint = compOp.RunTimeComponent as UnityEngine.Animations.IConstraint;

            // sourceTransform
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "sourceTransform"))
            {
                for (int i = 0; i < destinationConstraint.sourceCount; i++)
                {
                    float cachedWeight = destinationConstraint.GetSource(i).weight;
                    UnityEngine.Animations.ConstraintSource newConstraintSource = new UnityEngine.Animations.ConstraintSource();
                    newConstraintSource.weight = cachedWeight;
                    newConstraintSource.sourceTransform = GetSingleComponentRef(compOp, "sourceTransform", i) as Transform;

                    destinationConstraint.SetSource(i, newConstraintSource);
                }
            }

            // Aim Constraint
            if (compOp.ComponentType == typeof(UnityEngine.Animations.AimConstraint))
            {
                UnityEngine.Animations.AimConstraint aimConstraint = compOp.RunTimeComponent as UnityEngine.Animations.AimConstraint;

                // worldUpObject
                if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "worldUpObject"))
                {
                    aimConstraint.worldUpObject = GetSingleComponentRef(compOp, "worldUpObject") as Transform;
                }
            }
        }

        static void CopyParticleParameters(ComponentOperation compOp)
        {
            ParticleSystem destinationParticleSystem = compOp.RunTimeComponent as ParticleSystem;

            // main.customSimulationSpace
            var main = destinationParticleSystem.main;
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "main.customSimulationSpace"))
            {
                main.customSimulationSpace = GetSingleComponentRef(compOp, "main.customSimulationSpace") as Transform;
            }

            // collision.maxCollisionShapes
            var collision = destinationParticleSystem.collision;
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "collision.maxCollisionShapes"))
            {
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("collision.maxCollisionShapes"); i++)
                {
                    collision.SetPlane(i, GetSingleComponentRef(compOp, "collision.maxCollisionShapes", i) as Transform);
                }
            }

            // trigger.maxColliderCount
            var trigger = destinationParticleSystem.trigger;
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "trigger.maxColliderCount"))
            {
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("trigger.maxColliderCount"); i++)
                {
                    trigger.SetCollider(i, GetSingleComponentRef(compOp, "trigger.maxColliderCount", i));
                }
            }

            // subEmitters.subEmittersCount
            var subEmitters = destinationParticleSystem.subEmitters;
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "subEmitters.subEmittersCount"))
            {               
                for (int i = 0; i < compOp.RegisteredRefCollection.GetRegisteredDataCountOfName("subEmitters.subEmittersCount"); i++)
                {
                    subEmitters.SetSubEmitterSystem(i, GetSingleComponentRef(compOp, "subEmitters.subEmittersCount", i) as ParticleSystem);
                }
            }

            // renderer.probeAnchor
            var renderer = destinationParticleSystem.gameObject.GetComponent<ParticleSystemRenderer>();
            if (!PreservedPropertyHandler.GetInstance().GetIsPropertyPreserved(compOp.ComponentType, "renderer.probeAnchor") && renderer != null)
            {
                renderer.probeAnchor = GetSingleComponentRef(compOp, "renderer.probeAnchor") as Transform;
            }

            // Renderer
            // So the renderer module is actually a ParticleSystemRenderer component attached to the GameObject
            /*ParticleSystemRenderer SourcePartcileRenderer = CompOp.GetSourceComponent().gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer DestinationParticleRenderer = CompOp.GetDestinationGameObject().GetComponent<ParticleSystemRenderer>();
            if (SourcePartcileRenderer != null && DestinationParticleRenderer != null)
            {
                if (SourcePartcileRenderer.probeAnchor != null)
                {
                    MatchedObjectData DestinationAnchor = AvatarMatchHandler.GetInstance().GetMatchedObjectData(SourcePartcileRenderer.probeAnchor.gameObject);
                    if (DestinationAnchor == null)
                    {
                        DestinationParticleRenderer.probeAnchor = DestinationAnchor.DestinationGameObject.transform;
                    }
                }
            }*/
        }
    }
}
#endif