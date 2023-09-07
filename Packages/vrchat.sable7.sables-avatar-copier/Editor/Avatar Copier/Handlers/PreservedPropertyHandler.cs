#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using System;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{

    [Serializable]
    public class PreservedPropertyData
    {
        // The type of component this Property is from
        protected System.Type PreservedComponentType = typeof(SkinnedMeshRenderer);

        [SerializeField]
        protected string PropertyName = "";

        // The type of the property
        protected System.Type PropertyType = typeof(GameObject);

        public bool bEnabled = true;
        [SerializeField]
        protected bool bIsDefault = false;

        [SerializeField]
        protected int SelectedPropertyIndex  = 0;
        [SerializeField]
        protected int SelectedTypeIndex = 0;

        public PreservedPropertyData(bool inIsDefault = false)
        {
            SetSelectedTypeIndex(0);

            bIsDefault = inIsDefault;
        }

        public PreservedPropertyData(int inSelectedTypeIndex, int inSelectedPropertyIndex, bool inIsDefault = false)
        {
            SetSelectedTypeIndex(inSelectedTypeIndex);
            SetSelectedPropertyIndex(inSelectedPropertyIndex);

            bIsDefault = inIsDefault;
        }

        // Resets Index fields to ensure PreservedComponentType and PropertyType are set correctly
        public void Ensure()
        {
            int prevPropertyTypeIndex = SelectedPropertyIndex; // Gets auto-reset to 0 in SetSelectedTypeIndex as designed
            SetSelectedTypeIndex(SelectedTypeIndex);
            SetSelectedPropertyIndex(prevPropertyTypeIndex);
        }

        public System.Type GetPreservedComponentType()
        {
            return PreservedComponentType;
        }

        public string GetPropertyName()
        {
            return PropertyName;
        }

        public int GetSelectedPropertyIndex()
        {
            return SelectedPropertyIndex;
        }

        public int GetSelectedTypeIndex()
        {
            return SelectedTypeIndex;
        }

        public void SetSelectedPropertyIndex(int inPropertyIndex)
        {
            ComponentPreserveablePropertyInfo PropertyInfo = PreservedPropertyHandler.GetInstance().GetComponentPropertyInfo(PreservedComponentType);

            if (inPropertyIndex >= 0 && PropertyInfo != null && inPropertyIndex < PropertyInfo.PropertyNames.Count)
            {
                SelectedPropertyIndex = inPropertyIndex;
                PropertyName = PropertyInfo.PropertyNames[SelectedPropertyIndex];
                PropertyType = PropertyInfo.PropertyTypes[SelectedPropertyIndex];
            }
            //MergerSettingsHandler.GetInstance().Save();
        }

        public void SetSelectedTypeIndex(int inTypeIndex)
        {
            System.Type newType = PreservedPropertyHandler.GetInstance().GetSystemTypeFromIndex(inTypeIndex);

            if (newType != null)
            {
                SelectedTypeIndex = inTypeIndex;
                PreservedComponentType = newType;

                // Reset SelectedPropertyIndex
                SetSelectedPropertyIndex(0);
            }
            //MergerSettingsHandler.GetInstance().Save();
        }

        public bool GetIsDefault()
        {
            return bIsDefault;
        }

        public System.Type GetPropertyType()
        {
            return PropertyType;
        }
    }

    // Stores all the Preservable Parameters in nice lists. Along with corresponding types.
    public class ComponentPreserveablePropertyInfo
    {
        public System.Type ComponentType { get; }

        public List<string> PropertyNames { get; } = new List<string>();
        public List<System.Type> PropertyTypes { get; } = new List<System.Type>();

        public ComponentPreserveablePropertyInfo(System.Type inComponentType)
        {
            ComponentType = inComponentType;

            if (ComponentType == null)
            {
                return;
            }

            System.Reflection.PropertyInfo[] PropertyInfos = ComponentType.GetProperties();
            for (int i = 0; i < PropertyInfos.Length; i++)
            {
                if (!PropertyInfos[i].CanRead || !PropertyInfos[i].CanWrite || AvatarCopierUtils.IgnoredPropertyAndFieldNames.Contains(PropertyInfos[i].Name))
                {
                    continue;
                }

                PropertyNames.Add(PropertyInfos[i].Name);
                PropertyTypes.Add(PropertyInfos[i].PropertyType);
            }

            System.Reflection.FieldInfo[] fields = ComponentType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsPublic)
                {
                    PropertyNames.Add(fields[i].Name);
                    PropertyTypes.Add(fields[i].FieldType);
                }
            }
        }
    }

    public class PreservedPropertyHandler
    {
        private static PreservedPropertyHandler _Instance = null;

        public static PreservedPropertyHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new PreservedPropertyHandler();
                _Instance.InitializeDefaults();
            }

            return _Instance;
        }

        protected List<PreservedPropertyData> DefaultPreservedParameters = new List<PreservedPropertyData>();

        protected Dictionary<System.Type, ComponentPreserveablePropertyInfo> ComponentTypeToPropertiesDictionary = new Dictionary<System.Type, ComponentPreserveablePropertyInfo>();
        protected List<string> AllowedCopyTypeFriendlyNames = new List<string>();
        protected List<System.Type> TypesListed = new List<System.Type>();

        public PreservedPropertyData[] GetDefaultPreservedProperties()
        {
            return DefaultPreservedParameters.ToArray();
        }

        public void SetDefaultPropertyEnabled(int inIndex, bool bInValue)
        {
            if (inIndex < 0 || inIndex >= DefaultPreservedParameters.Count)
            {
                return;
            }

            DefaultPreservedParameters[inIndex].bEnabled = bInValue;
            List<bool> DefaultStatuses = CopierSettingsHandler.GetInstance().GetObjectDataValue("DefaultPreservedPropertiesEnabledStatuses") as List<bool>;
            DefaultStatuses[inIndex] = bInValue;
            CopierSettingsHandler.GetInstance().RegisterSave();
        }

        public string[] GetAllowedCopyTypeFriendlyNameArray()
        {
            return AllowedCopyTypeFriendlyNames.ToArray();
        }

        public void InitializeDefaults()
        {
            int tempIndex = 0;
            foreach (System.Type AllowedType in AvatarCopierUtils.AllowedCopyTypes)
            {
                ComponentTypeToPropertiesDictionary.Add(AllowedType, new ComponentPreserveablePropertyInfo(AllowedType));
                AllowedCopyTypeFriendlyNames.Add(AvatarCopierUtils.TypeToFriendlyName(AllowedType));
                TypesListed.Add(AllowedType);

                tempIndex++;
            }

            DefaultPreservedParameters = new List<PreservedPropertyData>
            {
                new PreservedPropertyData(GetIndexFromSystemType(typeof(SkinnedMeshRenderer)), GetIndexFromPropertyName(typeof(SkinnedMeshRenderer), "sharedMesh"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(SkinnedMeshRenderer)), GetIndexFromPropertyName(typeof(SkinnedMeshRenderer), "bones"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(SkinnedMeshRenderer)), GetIndexFromPropertyName(typeof(SkinnedMeshRenderer), "rootBone"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(SkinnedMeshRenderer)), GetIndexFromPropertyName(typeof(SkinnedMeshRenderer), "sharedMaterial"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(SkinnedMeshRenderer)), GetIndexFromPropertyName(typeof(SkinnedMeshRenderer), "sharedMaterials"), true),
                //new PreservedPropertyData(GetIndexFromSystemType(typeof(MeshRenderer)), GetIndexFromPropertyName(typeof(MeshRenderer), "sharedMaterial"), true),
                //new PreservedPropertyData(GetIndexFromSystemType(typeof(MeshRenderer)), GetIndexFromPropertyName(typeof(MeshRenderer), "sharedMaterials"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(MeshRenderer)), GetIndexFromPropertyName(typeof(MeshRenderer), "sharedMesh"), true),
                //new PreservedPropertyData(GetIndexFromSystemType(typeof(TrailRenderer)), GetIndexFromPropertyName(typeof(TrailRenderer), "sharedMaterial"), true),
                //new PreservedPropertyData(GetIndexFromSystemType(typeof(TrailRenderer)), GetIndexFromPropertyName(typeof(TrailRenderer), "sharedMaterials"), true),
                new PreservedPropertyData(GetIndexFromSystemType(typeof(Animator)), GetIndexFromPropertyName(typeof(Animator), "avatar"), true)
            };

            //foreach (PreservedPropertyData DefaultPropData in DefaultPreservedParameters)
            //{
            //    Debug.Log("Default Preserved Property Data: " + DefaultPropData.GetPreservedComponentType() + "[" + DefaultPropData.GetSelectedPropertyIndex() + "] --  " + DefaultPropData.GetPropertyName() + " [" + DefaultPropData.GetSelectedPropertyIndex() + "]");
            //}

        }


        public string[] GetPropertyArrayOfType(System.Type ComponentType)
        {
            string[] failedArray = { "None" };

            if (ComponentType != null && ComponentTypeToPropertiesDictionary.ContainsKey(ComponentType))
            {
                return ComponentTypeToPropertiesDictionary[ComponentType].PropertyNames.ToArray();
            }

            return failedArray;
        }

        public ComponentPreserveablePropertyInfo GetComponentPropertyInfo(System.Type inType)
        {
            if (ComponentTypeToPropertiesDictionary.ContainsKey(inType))
            {
                return ComponentTypeToPropertiesDictionary[inType];
            }

            return null;
        }

        public System.Type GetSystemTypeFromIndex(int inIndex)
        {
            if (inIndex >= 0 && inIndex < TypesListed.Count)
            {
                return TypesListed[inIndex];
            }

            return null;
        }

        public int GetIndexFromSystemType(System.Type inType)
        {
            for (int i = 0; i < TypesListed.Count; i++)
            {
                if (inType == TypesListed[i])
                {
                    return i;
                }
            }

            return 0;
        }

        public int GetIndexFromPropertyName(System.Type inType, string inName)
        {
            if (ComponentTypeToPropertiesDictionary.ContainsKey(inType))
            {
                for (int i = 0; i < ComponentTypeToPropertiesDictionary[inType].PropertyNames.Count; i++)
                {
                    if (inName == ComponentTypeToPropertiesDictionary[inType].PropertyNames[i])
                    {
                        return i;
                    }
                }
            }

            return 0;
        }

        public bool GetIsPropertyPreserved(System.Type CompType, string PropertyName)
        {
            if (!CopierSettingsHandler.GetInstance().GetBoolDataValue("bPreserveProperties"))
            {
                return false;
            }

            foreach (PreservedPropertyData PropData in DefaultPreservedParameters)
            {
                if (PropData.bEnabled && PropData.GetPreservedComponentType() == CompType && PropertyName == PropData.GetPropertyName())
                {
                    return true;
                }
            }

            List<PreservedPropertyData> PreservedProps = CopierSettingsHandler.GetInstance().GetObjectDataValue("PreservedProperties") as List<PreservedPropertyData>;
            foreach (PreservedPropertyData PropData in PreservedProps)
            {
                if (PropData.bEnabled && PropData.GetPreservedComponentType() == CompType && PropertyName == PropData.GetPropertyName())
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#endif