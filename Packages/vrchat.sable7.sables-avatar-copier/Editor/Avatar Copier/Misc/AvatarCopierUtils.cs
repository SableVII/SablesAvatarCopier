#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SablesTools.AvatarCopier
{
    public static class AvatarCopierUtils
    {
        public static HashSet<System.Type> AllowedCopyTypes = new HashSet<System.Type>
        {
            typeof(UnityEngine.Cloth),
            typeof(UnityEngine.Light),
            typeof(UnityEngine.BoxCollider),
            typeof(UnityEngine.SphereCollider),
            typeof(UnityEngine.CapsuleCollider),
            typeof(UnityEngine.Rigidbody),
            //typeof(UnityEngine.CharacterJoint), /// Need Copy Fun // Joints NEED Rigid Body
            //typeof(UnityEngine.ConfigurableJoint), /// Need Copy Fun
            //typeof(UnityEngine.FixedJoint), /// Need Copy Fun
            //typeof(UnityEngine.HingeJoint), /// Need Copy Fun
            //typeof(UnityEngine.SpringJoint), /// Need Copy Fun
            typeof(UnityEngine.Animations.AimConstraint),
            typeof(UnityEngine.Animations.LookAtConstraint),
            typeof(UnityEngine.Animations.ParentConstraint),
            typeof(UnityEngine.Animations.PositionConstraint),
            typeof(UnityEngine.Animations.RotationConstraint),
            typeof(UnityEngine.Animations.ScaleConstraint),
            typeof(UnityEngine.Camera),
            typeof(UnityEngine.AudioSource),
            typeof(UnityEngine.Animator),
            typeof(UnityEngine.SkinnedMeshRenderer),
            typeof(UnityEngine.MeshFilter),
            typeof(UnityEngine.MeshRenderer),
            typeof(UnityEngine.Animation),
            typeof(UnityEngine.ParticleSystem),
            //typeof(UnityEngine.ParticleSystemRenderer), // Ignore this as if there is a Particle System, there is a ParticleSystemRenderer (hidden)
            typeof(UnityEngine.TrailRenderer),
            
            //typeof(VRC.Core.PipelineManager),
            typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor),
            typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone),
            typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider),
            typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactSender),
            typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver),
            typeof(VRC.SDK3.Avatars.Components.VRCHeadChop),

            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCAimConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCLookAtConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCParentConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCPositionConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCRotationConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCScaleConstraint)
        };

        public static HashSet<System.Type> AllowedDuplicateCopyTypes = new HashSet<System.Type>
        {
            typeof(UnityEngine.BoxCollider),
            typeof(UnityEngine.SphereCollider),
            typeof(UnityEngine.CapsuleCollider),
            //typeof(UnityEngine.CharacterJoint), /// Need Copy Fun // Joints NEED Rigid Body
            //typeof(UnityEngine.ConfigurableJoint), /// Need Copy Fun
            //typeof(UnityEngine.FixedJoint), /// Need Copy Fun
            //typeof(UnityEngine.HingeJoint), /// Need Copy Fun
            //typeof(UnityEngine.SpringJoint), /// Need Copy Fun
            typeof(UnityEngine.AudioSource),

            typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone),
            typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider),
            typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactSender),
            typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver),
            typeof(VRC.SDK3.Avatars.Components.VRCHeadChop),

            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCAimConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCLookAtConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCParentConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCPositionConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCRotationConstraint),
            typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCScaleConstraint)
        };

        public static HashSet<string> IgnoredPropertyAndFieldNames = new HashSet<string>
        {
            "hideFlags",
            "name",
            "tag"
        };

        public static string TypeToFriendlyName(System.Type inType)
        {
            if (inType == null)
            {
                return "<none>";
            }

            if (inType == typeof(BoxCollider))
                return "Box Collider";
            if (inType == typeof(SphereCollider))
                return "Sphere Collider";
            if (inType == typeof(CapsuleCollider))
                return "Capsule Collider";
            if (inType == typeof(UnityEngine.Animations.AimConstraint))
                return "Aim Constraint";
            if (inType == typeof(UnityEngine.Animations.LookAtConstraint))
                return "Look At Constraint";
            if (inType == typeof(UnityEngine.Animations.ParentConstraint))
                return "Parent Constraint";
            if (inType == typeof(UnityEngine.Animations.PositionConstraint))
                return "Position Constraint";
            if (inType == typeof(UnityEngine.Animations.RotationConstraint))
                return "Rotation Constraint";
            if (inType == typeof(UnityEngine.Animations.ScaleConstraint))
                return "Scale Constraint";
            if (inType == typeof(AudioSource))
                return "Audio Source";
            if (inType == typeof(SkinnedMeshRenderer))
                return "Skinned Mesh Renderer";
            if (inType == typeof(MeshFilter))
                return "Mesh Filter";
            if (inType == typeof(MeshRenderer))
                return "Mesh Renderer";
            if (inType == typeof(ParticleSystem))
                return "Particle System";
            if (inType == typeof(TrailRenderer))
                return "Trail Renderer";
            //if (inType == typeof(VRC.Core.PipelineManager))
            //    return "Pipeline Manager";
            if (inType == typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor))
                return "VRC Avatar Descriptor";
            if (inType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone))
                return "VRC Phys Bone";
            if (inType == typeof(VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider))
                return "VRC Phys Bone Collider";
            if (inType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactSender))
                return "VRC Contact Sender";
            if (inType == typeof(VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver))
                return "VRC Contact Receiver";
            if (inType == typeof(VRC.SDK3.Avatars.Components.VRCHeadChop))
                return "VRC Head Chop";
            if (inType == typeof(Camera))
                return "Camera";
            if (inType == typeof(Animator))
                return "Animator";
            if (inType == typeof(Animation))
                return "Animation";
            if (inType == typeof(Cloth))
                return "Cloth";
            if (inType == typeof(Light))
                return "Light";
            if (inType == typeof(Rigidbody))
                return "Rigidbody";
            if (inType == typeof(Transform))
                return "Transform";
            if (inType == typeof(GameObject))
                return "Game Object";

            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCAimConstraint))
                return "VRC Aim Constraint";
            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCLookAtConstraint))
                return "VRC Look At Constraint";
            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCParentConstraint))
                return "VRC Parent Constraint";
            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCPositionConstraint))
                return "VRC Position Constraint";
            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCRotationConstraint))
                return "VRC Rotation Constraint";
            if (inType == typeof(VRC.SDK3.Dynamics.Constraint.Components.VRCScaleConstraint))
                return "VRC Scale Constraint";
            return inType.ToString();
        }

        public static string GetXOutOfTotalText(int CurrentCount, int TotalCount)
        {
            string outText = "";
            if (CurrentCount != TotalCount)
            {
                outText = CurrentCount + "/";
            }
            outText += TotalCount;
            return outText;
        }

        public static System.Type[] GetAllowedTypesAsArray()
        {
            System.Type[] outArray = new System.Type[AllowedCopyTypes.Count];
            AllowedCopyTypes.CopyTo(outArray);
            return outArray;
        }


        private static Texture2D _WarningIcon = null;
        private static Texture2D _AttachableIcon = null;
        private static Texture2D _GameObjectIcon = null;
        private static Texture2D _TransformIcon = null;
        private static Texture2D _ClothIcon = null;
        private static Texture2D _LightIcon = null;
        private static Texture2D _BoxColliderIcon = null;
        private static Texture2D _SphereColliderIcon = null;
        private static Texture2D _CapsuleColliderIcon = null;
        private static Texture2D _RigidbodyIcon = null;
        private static Texture2D _CharacterJointIcon = null;
        private static Texture2D _ConfigurableJointIcon = null;
        private static Texture2D _FixedJointIcon = null;
        private static Texture2D _LookAtConstraintIcon = null;
        private static Texture2D _ParentConstraintIcon = null;
        private static Texture2D _PositionConstraintIcon = null;
        private static Texture2D _RotationConstraintIcon = null;
        private static Texture2D _ScaleConstraintIcon = null;
        private static Texture2D _CameraIcon = null;
        private static Texture2D _AudioSourceIcon = null;
        private static Texture2D _AnimatorIcon = null;
        private static Texture2D _SkinnedMeshRendererIcon = null;
        private static Texture2D _MeshFilterIcon = null;
        private static Texture2D _MeshRendererIcon = null;
        private static Texture2D _AnimationIcon = null;
        private static Texture2D _ParticleSystemIcon = null;
        private static Texture2D _TrailRendererIcon = null;
        private static Texture2D _VRCAvatarDescriptorIcon = null;
        private static Texture2D _VRCPhysBoneIcon = null;
        private static Texture2D _VRCHeadChopIcon = null;
        private static Texture2D _VRCPhysBoneColliderIcon = null;
        private static Texture2D _VRCContactSenderIcon = null;
        private static Texture2D _VRCContactReceiverIcon = null;
        private static Texture2D _VRCLookAtConstraintIcon = null;
        private static Texture2D _VRCParentConstraintIcon = null;
        private static Texture2D _VRCPositionConstraintIcon = null;
        private static Texture2D _VRCRotationConstraintIcon = null;
        private static Texture2D _VRCScaleConstraintIcon = null;

        //https://github.com/halak/unity-editor-icons
        public static Texture2D GetIconTexture(string iconName)
        {
            if (iconName == null)
            {
                return null;
            }

            switch (iconName.ToLower())
            {
                case "warning":
                    if (_WarningIcon == null)
                    {
                        _WarningIcon = EditorGUIUtility.IconContent("d_console.warnicon@2x").image as Texture2D;
                    }
                    return _WarningIcon;

                case "attachable":
                    if (_AttachableIcon == null)
                    {
                        _AttachableIcon = EditorGUIUtility.IconContent("sv_icon_dot9_pix16_gizmo").image as Texture2D;
                    }
                    return _AttachableIcon;

                case "game object":
                    if (_GameObjectIcon == null)
                    {
                        _GameObjectIcon = EditorGUIUtility.IconContent("d_GameObject Icon").image as Texture2D;
                    }
                    return _GameObjectIcon;

                case "transform":
                    if (_TransformIcon == null)
                    {
                        _TransformIcon = EditorGUIUtility.IconContent("d_Transform Icon").image as Texture2D;
                    }
                    return _TransformIcon;

                case "cloth":
                    if (_ClothIcon == null)
                    {
                        _ClothIcon = EditorGUIUtility.IconContent("Cloth Icon").image as Texture2D;
                    }
                    return _ClothIcon;

                case "light":
                    if (_LightIcon == null)
                    {
                        _LightIcon = EditorGUIUtility.IconContent("d_Light Icon").image as Texture2D;
                    }
                    return _LightIcon;

                case "box collider":
                    if (_BoxColliderIcon == null)
                    {
                        _BoxColliderIcon = EditorGUIUtility.IconContent("d_BoxCollider Icon").image as Texture2D;
                    }
                    return _BoxColliderIcon;

                case "sphere collider":
                    if (_SphereColliderIcon == null)
                    {
                        _SphereColliderIcon = EditorGUIUtility.IconContent("d_SphereCollider Icon").image as Texture2D;
                    }
                    return _SphereColliderIcon;

                case "capsule collider":
                    if (_CapsuleColliderIcon == null)
                    {
                        _CapsuleColliderIcon = EditorGUIUtility.IconContent("d_CapsuleCollider Icon").image as Texture2D;
                    }
                    return _CapsuleColliderIcon;

                case "rigid body":
                    if (_RigidbodyIcon == null)
                    {
                        _RigidbodyIcon = EditorGUIUtility.IconContent("d_Rigidbody Icon").image as Texture2D;
                    }
                    return _RigidbodyIcon;

                case "character joint":
                    if (_CharacterJointIcon == null)
                    {
                        _CharacterJointIcon = EditorGUIUtility.IconContent("d_CharacterJoint Icon").image as Texture2D;
                    }
                    return _CharacterJointIcon;

                case "configurable joint":
                    if (_ConfigurableJointIcon == null)
                    {
                        _ConfigurableJointIcon = EditorGUIUtility.IconContent("ConfigurableJoint Icon").image as Texture2D;
                    }
                    return _ConfigurableJointIcon;

                case "fixed joint":
                    if (_FixedJointIcon == null)
                    {
                        _FixedJointIcon = EditorGUIUtility.IconContent("d_FixedJoint Icon").image as Texture2D;
                    }
                    return _FixedJointIcon;

                case "look at constraint":
                    if (_LookAtConstraintIcon == null)
                    {
                        _LookAtConstraintIcon = EditorGUIUtility.IconContent("LookAtConstraint Icon").image as Texture2D;
                    }
                    return _LookAtConstraintIcon;

                case "parent constraint":
                    if (_ParentConstraintIcon == null)
                    {
                        _ParentConstraintIcon = EditorGUIUtility.IconContent("ParentConstraint Icon").image as Texture2D;
                    }
                    return _ParentConstraintIcon;

                case "position constraint":
                    if (_PositionConstraintIcon == null)
                    {
                        _PositionConstraintIcon = EditorGUIUtility.IconContent("PositionConstraint Icon").image as Texture2D;
                    }
                    return _PositionConstraintIcon;

                case "rotation constraint":
                    if (_RotationConstraintIcon == null)
                    {
                        _RotationConstraintIcon = EditorGUIUtility.IconContent("RotationConstraint Icon").image as Texture2D;
                    }
                    return _RotationConstraintIcon;

                case "scale constraint":
                    if (_ScaleConstraintIcon == null)
                    {
                        _ScaleConstraintIcon = EditorGUIUtility.IconContent("ScaleConstraint Icon").image as Texture2D;
                    }
                    return _ScaleConstraintIcon;

                case "camera":
                    if (_CameraIcon == null)
                    {
                        _CameraIcon = EditorGUIUtility.IconContent("d_Camera Icon").image as Texture2D;
                    }
                    return _CameraIcon;

                case "audio source":
                    if (_AudioSourceIcon == null)
                    {
                        _AudioSourceIcon = EditorGUIUtility.IconContent("d_AudioSource Icon").image as Texture2D;
                    }
                    return _AudioSourceIcon;

                case "animator":
                    if (_AnimatorIcon == null)
                    {
                        _AnimatorIcon = EditorGUIUtility.IconContent("d_Animator Icon").image as Texture2D;
                    }
                    return _AnimatorIcon;

                case "skinned mesh renderer":
                    if (_SkinnedMeshRendererIcon == null)
                    {
                        _SkinnedMeshRendererIcon = EditorGUIUtility.IconContent("d_SkinnedMeshRenderer Icon").image as Texture2D;
                    }
                    return _SkinnedMeshRendererIcon;

                case "mesh filter":
                    if (_MeshFilterIcon == null)
                    {
                        _MeshFilterIcon = EditorGUIUtility.IconContent("d_MeshFilter Icon").image as Texture2D;
                    }
                    return _MeshFilterIcon;

                case "mesh renderer":
                    if (_MeshRendererIcon == null)
                    {
                        _MeshRendererIcon = EditorGUIUtility.IconContent("d_MeshRenderer Icon").image as Texture2D;
                    }
                    return _MeshRendererIcon;

                case "animation":
                    if (_AnimationIcon == null)
                    {
                        _AnimationIcon = EditorGUIUtility.IconContent("d_Animation Icon").image as Texture2D;
                    }
                    return _AnimationIcon;

                case "particle system":
                    if (_ParticleSystemIcon == null)
                    {
                        _ParticleSystemIcon = EditorGUIUtility.IconContent("d_ParticleSystem Icon").image as Texture2D;
                    }
                    return _ParticleSystemIcon;

                case "trail renderer":
                    if (_TrailRendererIcon == null)
                    {
                        _TrailRendererIcon = EditorGUIUtility.IconContent("d_TrailRenderer Icon").image as Texture2D;
                    }
                    return _TrailRendererIcon;

                case "vrc avatar descriptor":
                    if (_VRCAvatarDescriptorIcon == null)
                    {
                        _VRCAvatarDescriptorIcon = EditorGUIUtility.IconContent("sv_icon_dot1_pix16_gizmo").image as Texture2D;
                    }
                    return _VRCAvatarDescriptorIcon;

                case "vrc phys bone":
                    if (_VRCPhysBoneIcon == null)
                    {
                        _VRCPhysBoneIcon = EditorGUIUtility.IconContent("sv_icon_dot2_pix16_gizmo").image as Texture2D;
                    }
                    return _VRCPhysBoneIcon;

                case "vrc phys bone collider":
                    if (_VRCPhysBoneColliderIcon == null)
                    {
                        _VRCPhysBoneColliderIcon = EditorGUIUtility.IconContent("sv_icon_dot3_pix16_gizmo").image as Texture2D;
                    }
                    return _VRCPhysBoneColliderIcon;

                case "vrc contact sender":
                    if (_VRCContactSenderIcon == null)
                    {
                        _VRCContactSenderIcon = EditorGUIUtility.IconContent("sv_icon_dot4_pix16_gizmo").image as Texture2D;
                    }
                    return _VRCContactSenderIcon;

                case "vrc contact receiver":
                    if (_VRCContactReceiverIcon == null)
                    {
                        _VRCContactReceiverIcon = EditorGUIUtility.IconContent("sv_icon_dot5_pix16_gizmo").image as Texture2D;
                    }
                    return _VRCContactReceiverIcon;

                case "vrc head chop":
                    if (_VRCHeadChopIcon == null)
                    {
                        _VRCHeadChopIcon = EditorGUIUtility.IconContent("HeadZoomSilhouette").image as Texture2D;
                    }
                    return _VRCHeadChopIcon;

                case "vrc look at constraint":
                    if (_VRCLookAtConstraintIcon == null)
                    {
                        _VRCLookAtConstraintIcon = EditorGUIUtility.IconContent("d_LookAtConstraint Icon").image as Texture2D;
                    }
                    return _VRCLookAtConstraintIcon;

                case "vrc parent constraint":
                    if (_VRCParentConstraintIcon == null)
                    {
                        _VRCParentConstraintIcon = EditorGUIUtility.IconContent("d_ParentConstraint Icon").image as Texture2D;
                    }
                    return _VRCParentConstraintIcon;

                case "vrc position constraint":
                    if (_VRCPositionConstraintIcon == null)
                    {
                        _VRCPositionConstraintIcon = EditorGUIUtility.IconContent("d_PositionConstraint Icon").image as Texture2D;
                    }
                    return _VRCPositionConstraintIcon;

                case "vrc rotation constraint":
                    if (_VRCRotationConstraintIcon == null)
                    {
                        _VRCRotationConstraintIcon = EditorGUIUtility.IconContent("d_RotationConstraint Icon").image as Texture2D;
                    }
                    return _VRCRotationConstraintIcon;

                case "vrc scale constraint":
                    if (_VRCScaleConstraintIcon == null)
                    {
                        _VRCScaleConstraintIcon = EditorGUIUtility.IconContent("d_ScaleConstraint Icon").image as Texture2D;
                    }
                    return _VRCScaleConstraintIcon;

            }
            return null;
        }
    }
}
#endif