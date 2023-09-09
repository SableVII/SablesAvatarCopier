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
                _Instance.Initialize();
            }

            return _Instance;
        }

        //protected List<PreservedPropertyData> DefaultPreservedParameters = new List<PreservedPropertyData>();
        protected List<PreservedPropertyData> PreserevedProperties = new List<PreservedPropertyData>();

        protected Dictionary<System.Type, ComponentPreserveablePropertyInfo> ComponentTypeToPropertiesDictionary = new Dictionary<System.Type, ComponentPreserveablePropertyInfo>();
        protected List<string> AllowedCopyTypeFriendlyNames = new List<string>();
        protected List<System.Type> TypesListed = new List<System.Type>();

        public string[] GetAllowedCopyTypeFriendlyNameArray()
        {
            return AllowedCopyTypeFriendlyNames.ToArray();
        }

        public void Initialize()
        {
            // Determine all possible properties that can be preserved and record indexes
            foreach (System.Type AllowedType in AvatarCopierUtils.AllowedCopyTypes)
            {
                ComponentTypeToPropertiesDictionary.Add(AllowedType, new ComponentPreserveablePropertyInfo(AllowedType));
                AllowedCopyTypeFriendlyNames.Add(AvatarCopierUtils.TypeToFriendlyName(AllowedType));
                TypesListed.Add(AllowedType);
            }

            AddNewPreservedProperty(typeof(SkinnedMeshRenderer), "sharedMesh", true);
            AddNewPreservedProperty(typeof(SkinnedMeshRenderer), "bones", true);
            AddNewPreservedProperty(typeof(SkinnedMeshRenderer), "rootBone", true);
            //AddNewPreservedProperty(typeof(SkinnedMeshRenderer), "sharedMaterial", true);
            //AddNewPreservedProperty(typeof(SkinnedMeshRenderer), "sharedMaterials", true);
            //AddNewPreservedProperty(typeof(MeshRenderer), "sharedMaterial", true);
            //AddNewPreservedProperty(typeof(MeshRenderer), "sharedMaterials", true);
            AddNewPreservedProperty(typeof(MeshRenderer), "sharedMesh", true);
            //AddNewPreservedProperty(typeof(TrailRenderer), "sharedMaterial", true);
            //AddNewPreservedProperty(typeof(TrailRenderer), "sharedMaterials", true);
            AddNewPreservedProperty(typeof(Animator), "avatar", true);
        }

        // Adds default Preserved Property
        public void AddNewPreservedProperty()
        {
            PreserevedProperties.Add(new PreservedPropertyData());
        }

        // Returns True if successfully added
        public bool AddNewPreservedProperty(System.Type componentType, string propertyName, bool bIsDefault = false)
        {
            int typeIndex = GetIndexFromSystemType(componentType);
            int componentPropertyIndex = GetIndexFromPropertyName(componentType, propertyName);

            if (typeIndex == -1 || componentPropertyIndex == -1)
            {
                return false;
            }

            PreservedPropertyData newPropData = new PreservedPropertyData(typeIndex, componentPropertyIndex, bIsDefault);

            // No need to check for duplicates as duplicates don't matter
            PreserevedProperties.Add(newPropData);

            return true;
        }

        public string[] GetPropertyArrayOfType(System.Type componentType)
        {
            string[] failedArray = { "None" };

            if (componentType != null && ComponentTypeToPropertiesDictionary.ContainsKey(componentType))
            {
                return ComponentTypeToPropertiesDictionary[componentType].PropertyNames.ToArray();
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

            return -1;
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

            return -1;
        }

        public bool GetIsPropertyPreserved(System.Type compType, string propertyName)
        {
            foreach (PreservedPropertyData propData in PreserevedProperties)
            {
                if (propData.bEnabled && propData.GetPreservedComponentType() == compType && propertyName == propData.GetPropertyName())
                {
                    return true;
                }
            }

            return false;
        }

        public int GetEnabledPereservedPropertyCount()
        {
            int count = 0;

            foreach (PreservedPropertyData propData in PreserevedProperties)
            {
                if (propData.bEnabled)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetPreservedPropertyCount()
        {
            return PreserevedProperties.Count;
        }

        public PreservedPropertyData GetPreservedPropertyData(int index)
        {
            return PreserevedProperties[index];
        }
    }
}

#endif