#if (UNITY_EDITOR)
using SablesTools.AvatarCopier.Operations;
using SablesTools.AvatarCopier.Data;
using UnityEngine;

namespace SablesTools.AvatarCopier
{
    public class RegisteredReferenceUtils
    {
        public static void RegisterReferences(ComponentOperation compOp)
        {
            if (compOp == null)
            {
                return;
            }

            if (compOp.ComponentType == typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor))
            {
                GetVRCAvatarDescriptorPropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone))
            {
                GetVRCPhysBoneRegisterablePropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider))
            {
                GetVRCPhysBoneColliderRegisterablePropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver))
            {
                GetVRCContactReceiverRegisterablePropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactSender))
            {
                GetVRCContactSenderRegisterablePropFields(compOp);
                return;
            }
            if (typeof(Renderer).IsAssignableFrom(compOp.ComponentType) && compOp.ComponentType != typeof(ParticleSystemRenderer))
            {
                GetRendererPropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(Cloth))
            {
                GetClothPropFields(compOp);
                return;
            }
            if (typeof(UnityEngine.Animations.IConstraint).IsAssignableFrom(compOp.ComponentType))
            {
                GetConstraintPropFields(compOp);
                return;
            }
            if (compOp.ComponentType == typeof(ParticleSystem))
            {
                GetParticleSystemPropFields(compOp);
                return;
            }
        }

        /// VRC Avatar Descriptor 
        public static void GetVRCAvatarDescriptorPropFields(ComponentOperation compOp)
        {
            VRC.SDK3.Avatars.Components.VRCAvatarDescriptor sourceVRCAvatarDescriptor = compOp.OriginComponent as VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

            if (sourceVRCAvatarDescriptor == null)
            {
                return;
            }

            // Face Mesh
            if (sourceVRCAvatarDescriptor.VisemeSkinnedMesh != null)
            {
                RegisteredReference newRefData = new RegisteredReference("VisemeSkinnedMesh", typeof(SkinnedMeshRenderer), compOp);

                //newRefData.AddRef(SourceVRCAvatarDescriptor.VisemeSkinnedMesh != null ? SourceVRCAvatarDescriptor.VisemeSkinnedMesh.gameObject : null);

                newRefData.AddRef(sourceVRCAvatarDescriptor.VisemeSkinnedMesh.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Jaw Bone
            if (sourceVRCAvatarDescriptor.lipSyncJawBone != null)
            {
                RegisteredReference newRefData = new RegisteredReference("lipSyncJawBone", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.lipSyncJawBone.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Left Eye Transform
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.leftEye != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.leftEye", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.leftEye.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Right Eye Transform
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.rightEye != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.rightEye", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.rightEye.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Upper Left Eyelid
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.upperLeftEyelid != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.upperLeftEyelid", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.upperLeftEyelid.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Upper Right Eyelid
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.upperRightEyelid != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.upperRightEyelid", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.upperRightEyelid.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Lower Left Eyelid
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.lowerLeftEyelid != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.lowerLeftEyelid", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.lowerLeftEyelid.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Lower Right Eyelid
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.lowerRightEyelid != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.lowerRightEyelid", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.lowerRightEyelid.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Upper Left Eyelid
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.upperLeftEyelid != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.upperLeftEyelid", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.upperLeftEyelid.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Eyelid Skinned Mesh
            if (sourceVRCAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh != null)
            {
                RegisteredReference newRefData = new RegisteredReference("customEyeLookSettings.eyelidsSkinnedMesh", typeof(SkinnedMeshRenderer), compOp);

                newRefData.AddRef(sourceVRCAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }

        /// VRC Phys Bone
        public static void GetVRCPhysBoneRegisterablePropFields(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone sourceVRCPhysBone = compOp.OriginComponent as VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone;

            if (sourceVRCPhysBone == null)
            {
                return;
            }

            // Root Transform
            if (sourceVRCPhysBone.rootTransform != null)
            {
                RegisteredReference newRefData = new RegisteredReference("rootTransform", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCPhysBone.rootTransform.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Ignored Transform
            if (sourceVRCPhysBone.ignoreTransforms.Count > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("ignoreTransforms", typeof(Transform), compOp);

                foreach (Transform sTrans in sourceVRCPhysBone.ignoreTransforms)
                {
                    newRefData.AddRef(sTrans != null ? sTrans.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Colliders
            if (sourceVRCPhysBone.colliders.Count > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("colliders", typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider), compOp);

                foreach (VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider collider in sourceVRCPhysBone.colliders)
                {
                    newRefData.AddRef(collider != null ? collider.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }

        /// VRC Phys Bone Collider
        public static void GetVRCPhysBoneColliderRegisterablePropFields(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider sourceVRCPhysBoneCollider = compOp.OriginComponent as VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider;

            if (sourceVRCPhysBoneCollider == null)
            {
                return;
            }

            // Root Transform
            if (sourceVRCPhysBoneCollider.rootTransform != null)
            {
                RegisteredReference newRefData = new RegisteredReference("rootTransform", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCPhysBoneCollider.rootTransform.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }

        /// VRC Contact Receiver
        public static void GetVRCContactReceiverRegisterablePropFields(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver sourceVRCContactReceiver = compOp.OriginComponent as VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver;

            if (sourceVRCContactReceiver == null)
            {
                return;
            }

            // Root Transform
            if (sourceVRCContactReceiver.rootTransform != null)
            {
                RegisteredReference newRefData = new RegisteredReference("rootTransform", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCContactReceiver.rootTransform.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }

        /// VRC Contact Sender
        public static void GetVRCContactSenderRegisterablePropFields(ComponentOperation compOp)
        {
            VRC.SDK3.Dynamics.Contact.Components.VRCContactSender sourceVRCContactSender = compOp.OriginComponent as VRC.SDK3.Dynamics.Contact.Components.VRCContactSender;

            if (sourceVRCContactSender == null)
            {
                return;
            }

            // Root Transform
            if (sourceVRCContactSender.rootTransform != null)
            {
                RegisteredReference newRefData = new RegisteredReference("rootTransform", typeof(Transform), compOp);

                newRefData.AddRef(sourceVRCContactSender.rootTransform.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }

        /// Renderer
        public static void GetRendererPropFields(ComponentOperation compOp)
        {
            Renderer sourceRenderer = compOp.OriginComponent as Renderer;

            if (sourceRenderer == null)
            {
                return;
            }

            // Light Probe Proxy Volume Override
            if (sourceRenderer.lightProbeProxyVolumeOverride != null)
            {
                RegisteredReference newRefData = new RegisteredReference("lightProbeProxyVolumeOverride", typeof(GameObject), compOp);

                newRefData.AddRef(sourceRenderer.lightProbeProxyVolumeOverride);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Probe Anchor
            if (sourceRenderer.probeAnchor != null)
            {
                RegisteredReference newRefData = new RegisteredReference("probeAnchor", typeof(Transform), compOp);

                newRefData.AddRef(sourceRenderer.probeAnchor.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Skinned Mesh Renderer
            if (compOp.ComponentType == typeof(SkinnedMeshRenderer))
            {
                SkinnedMeshRenderer SourceSkinnedMeshRenderer = sourceRenderer as SkinnedMeshRenderer;

                if (SourceSkinnedMeshRenderer == null)
                {
                    return;
                }

                // Root Bone
                if (SourceSkinnedMeshRenderer.rootBone != null)
                {
                    RegisteredReference newRefData = new RegisteredReference("rootBone", typeof(Transform), compOp);

                    newRefData.AddRef(SourceSkinnedMeshRenderer.rootBone.gameObject);

                    compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
                }

                return;
            }
        }

        /// Cloth
        public static void GetClothPropFields(ComponentOperation compOp)
        {
            Cloth sourceCloth = compOp.OriginComponent as Cloth;

            if (sourceCloth == null)
            {
                return;
            }

            // Capsule Colliders
            if (sourceCloth.capsuleColliders.Length > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("capsuleColliders", typeof(CapsuleCollider), compOp);

                foreach (CapsuleCollider collider in sourceCloth.capsuleColliders)
                {
                    newRefData.AddRef(collider != null ? collider.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Sphere Colliders [Pairs]
            if (sourceCloth.sphereColliders.Length > 0)
            {
                RegisteredReference newFirstRefData = new RegisteredReference("sphereCollidersFirst", typeof(CapsuleCollider), compOp);
                RegisteredReference newSecondRefData = new RegisteredReference("sphereCollidersSecond", typeof(CapsuleCollider), compOp);

                foreach (ClothSphereColliderPair collPair in sourceCloth.sphereColliders)
                {
                    newFirstRefData.AddRef(collPair.first != null ? collPair.first.gameObject : null);
                    newSecondRefData.AddRef(collPair.second != null ? collPair.second.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newFirstRefData);
                compOp.RegisteredRefCollection.AddRegisteredData(newSecondRefData);
            }
        }

        /// Constraints
        public static void GetConstraintPropFields(ComponentOperation compOp)
        {
            UnityEngine.Animations.IConstraint sourceConstraint = compOp.OriginComponent as UnityEngine.Animations.IConstraint;

            if (sourceConstraint == null)
            {
                return;
            }

            // Sources
            if (sourceConstraint.sourceCount > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("sourceTransform", typeof(Transform), compOp);

                for (int i = 0; i < sourceConstraint.sourceCount; i++)
                {
                    UnityEngine.Animations.ConstraintSource constraintSource = sourceConstraint.GetSource(i);

                    newRefData.AddRef(constraintSource.sourceTransform != null ? constraintSource.sourceTransform.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // World Up Object (for Aim Constraints and Look At Constraints)
            if (compOp.ComponentType == typeof(UnityEngine.Animations.AimConstraint) || compOp.ComponentType == typeof(UnityEngine.Animations.LookAtConstraint))
            {
                Transform sourceWorldUpObject = null;

                if (compOp.ComponentType == typeof(UnityEngine.Animations.AimConstraint))
                {
                    sourceWorldUpObject = (compOp.OriginComponent as UnityEngine.Animations.AimConstraint).worldUpObject;
                }
                else if (compOp.ComponentType == typeof(UnityEngine.Animations.LookAtConstraint))
                {
                    sourceWorldUpObject = (compOp.OriginComponent as UnityEngine.Animations.LookAtConstraint).worldUpObject;
                }

                if (sourceWorldUpObject != null)
                {
                    RegisteredReference newRefData = new RegisteredReference("worldUpObject", typeof(Transform), compOp);

                    newRefData.AddRef(sourceWorldUpObject.gameObject);

                    compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
                }
            }
        }

        /// Particle System
        public static void GetParticleSystemPropFields(ComponentOperation compOp)
        {
            ParticleSystem sourceParticleSystem = compOp.OriginComponent as ParticleSystem;

            if (sourceParticleSystem == null)
            {
                return;
            }

            // Custom Simulation Space
            if (sourceParticleSystem.main.customSimulationSpace != null)
            {
                RegisteredReference newRefData = new RegisteredReference("main.customSimulationSpace", typeof(GameObject), compOp);

                newRefData.AddRef(sourceParticleSystem.main.customSimulationSpace.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Collision
            int planeColliderCount = 0;
            for (int i = 0; i < sourceParticleSystem.collision.maxCollisionShapes; i++)
            {
                if (sourceParticleSystem.collision.GetPlane(i) != null)
                {
                    planeColliderCount++;
                }
            }

            if (planeColliderCount > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("collision.maxCollisionShapes", typeof(Transform), compOp);

                int foundColliderCount = 0;
                for (int i = 0; i < sourceParticleSystem.collision.maxCollisionShapes; i++)
                {
                    Transform SourcePlane = sourceParticleSystem.collision.GetPlane(i);

                    newRefData.AddRef(SourcePlane != null ? SourcePlane.gameObject : null);

                    if (SourcePlane != null)
                    {
                        foundColliderCount++;
                        if (planeColliderCount == foundColliderCount)
                        {
                            break;
                        }
                    }
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Triggers
            int triggerColliderCount = 0;
            for (int i = 0; i < sourceParticleSystem.trigger.colliderCount; i++)
            {
                if (sourceParticleSystem.trigger.GetCollider(i) != null)
                {
                    triggerColliderCount++;
                }
            }

            if (triggerColliderCount > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("trigger.maxColliderCount", typeof(Transform), compOp);

                int foundColliderCount = 0;
                for (int i = 0; i < sourceParticleSystem.trigger.colliderCount; i++)
                {
                    Component sourceCollider = sourceParticleSystem.trigger.GetCollider(i);

                    newRefData.AddRef(sourceCollider != null ? sourceCollider.gameObject : null);
                    
                    if (sourceCollider != null)
                    {
                        foundColliderCount++;
                        if (triggerColliderCount == foundColliderCount)
                        {
                            break;
                        }
                    }
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Sub Emitters
            if (sourceParticleSystem.subEmitters.subEmittersCount > 0)
            {
                RegisteredReference newRefData = new RegisteredReference("subEmitters.subEmitters", typeof(ParticleSystem), compOp);

                for (int i = 0; i < sourceParticleSystem.subEmitters.subEmittersCount; i++)
                {
                    ParticleSystem SourceSubEmitter = sourceParticleSystem.subEmitters.GetSubEmitterSystem(i);

                    newRefData.AddRef(SourceSubEmitter != null ? SourceSubEmitter.gameObject : null);
                }

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Probe Anchor
            // Particle Renderer is a component along-side the ParticleSystem
            ParticleSystemRenderer sourcePartcileRenderer = compOp.OriginComponent.gameObject.GetComponent<ParticleSystemRenderer>();
            if (sourcePartcileRenderer == null)
            {
                return;
            }

            if (sourcePartcileRenderer.probeAnchor != null)
            {
                RegisteredReference newRefData = new RegisteredReference("renderer.probeAnchor", typeof(Transform), compOp);

                newRefData.AddRef(sourcePartcileRenderer.probeAnchor.gameObject);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }

            // Light Probe Proxy Volume Override
            if (sourcePartcileRenderer.lightProbeProxyVolumeOverride != null)
            {
                RegisteredReference newRefData = new RegisteredReference("renderer.lightProbeProxyVolumeOverride", typeof(GameObject), compOp);

                newRefData.AddRef(sourcePartcileRenderer.lightProbeProxyVolumeOverride);

                compOp.RegisteredRefCollection.AddRegisteredData(newRefData);
            }
        }
    }
}

#endif