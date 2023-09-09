#if (UNITY_EDITOR)
using UnityEngine;

using System.Collections.Generic;
using UnityEditor;
using System.IO;//Debug


namespace SablesTools.AvatarCopier.Handlers
{
    [System.Serializable]
    public class DefaultSettingsData
    {
        public float ScaleEpsilon = 0.0001f;
        public bool bCopyMaterials = true;
        public bool bSmartCopyMaterials = false;
        public bool bUnpackPrefab = true;
        public bool bShowAdvancedSettings = false;
        public bool bFirstTimeOpened = true;
        public bool bCreateDestinationClone = false;
        //public bool bPreserveProperties = true;
        public bool bDefaultUseSmartMaterialCopy = false;

        public bool bDefaultUseAttachableOperations = true;
        public bool bDefaultUseCompOperations = true;
        public bool bDefaultUseScaleOperations = true;
        public bool bDefaultUseEnabledDisabledOperations = true;
        public bool bDefaultUseMaterialOperations = true;
        public bool bDefaultUseMiscOperations = true;
    }

    //[System.Serializable]
    public class CopierSettingsHandler
    {
        private static CopierSettingsHandler _Instance = null;

        //private static string DataDirectory = "Assets/Resources";
        //private static string DataFileName = "AvatarCopierSettings";

        private string _PackageID = "vrchat.sable7.sables-avatar-copier";

        protected DefaultSettingsData _DefaultData = new DefaultSettingsData();

        //public List<PreservedPropertyData> PreservedProperties = new List<PreservedPropertyData>();
        //public List<bool> DefaultPreservedPropertiesEnabledStatuses = new List<bool>();

        public static CopierSettingsHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new CopierSettingsHandler();
                //_Instance.LoadData();

                // Clean out Destination and Source Avatars as sometimes it just returns with random junk x.x
                _Instance.Destination = null;
                _Instance.Source = null;

                //_Instance.RegisterSave();
            }

            return _Instance;
        }

        private CopierSettingsHandler()
        {
            //_PublicPropertyInfos = GetType().GetProperties(System.Reflection.BindingFlags.Public);
            //_FieldInfoDictionary = new Dictionary<string, System.Reflection.FieldInfo>();

            CheckAndSetEditorPrefsDefaults();

            System.Reflection.FieldInfo[] FieldInfos = _DefaultData.GetType().GetFields();
            for (int i = 0; i < FieldInfos.Length; i++)
            {
                if (FieldInfos[i].IsPublic)
                {
                    _FieldInfoDictionary.Add(FieldInfos[i].Name, FieldInfos[i]);
                }
            }
        }

        private void CheckAndSetEditorPrefsDefaults()
        {
            // Get default settings fields
            System.Reflection.FieldInfo[] fieldInfos = _DefaultData.GetType().GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                System.Reflection.FieldInfo fieldInfo = fieldInfos[i];
                if (fieldInfo.IsPublic)
                {
                    string prefName = _PackageID + "." + fieldInfo.Name;
                    if (fieldInfo.FieldType == typeof(bool))
                    {
                        if (EditorPrefs.HasKey(prefName) == false)
                        {
                            //Debug.Log("Setting Editor Pref: " + prefName + " to value " + (bool)fieldInfo.GetValue(_DefaultData));
                            EditorPrefs.SetBool(prefName, (bool)fieldInfo.GetValue(_DefaultData));
                        }
                    }
                    else if (fieldInfo.FieldType == typeof(int))
                    {
                        if (EditorPrefs.HasKey(prefName) == false)
                        {
                            //Debug.Log("Setting Editor Pref: " + prefName + " to value " + (int)fieldInfo.GetValue(_DefaultData));
                            EditorPrefs.SetInt(prefName, (int)fieldInfo.GetValue(_DefaultData));
                        }
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        if (EditorPrefs.HasKey(prefName) == false)
                        {
                            //Debug.Log("Setting Editor Pref: " + prefName + " to value " + (float)fieldInfo.GetValue(_DefaultData));
                            EditorPrefs.SetFloat(prefName, (float)fieldInfo.GetValue(_DefaultData));
                        }
                    }
                    else if (fieldInfo.FieldType == typeof(string))
                    {
                        if (EditorPrefs.HasKey(prefName) == false)
                        {
                            //Debug.Log("Setting Editor Pref: " + prefName + " to value " + (string)fieldInfo.GetValue(_DefaultData));
                            EditorPrefs.SetString(prefName, (string)fieldInfo.GetValue(_DefaultData));
                        }
                    }
                    //_FieldInfoDictionary.Add(FieldInfos[i].Name, FieldInfos[i]);
                }
            }
        }

        protected bool RegisteredForSave = false;

        public GameObject Destination = null;
        public GameObject Source = null;

        private Dictionary<string, System.Reflection.FieldInfo> _FieldInfoDictionary = new Dictionary<string, System.Reflection.FieldInfo>();


        // These are temporary and only persist for the session
        public bool bOrganizeCompOpsByType { get; set; } = true;
        public bool bHideUnusedCompOpTypes { get; set; } = true;

        public bool bDestinationIsAnAvatar { get; set; } = false;
        public bool bSourceIsAnAvatar { get; set; } = false;

        public bool bScannedAvatars { get; set; } = false;

        public string CloneName { get; set; } = "";

        public int SelectedTabIndex { get; set; } = 1;

        public bool bCantSearchForAttachables { get; set; } = true;

        public bool bMatchInputsTogether { get; set; } = true;

        // Safely sets the EditorPrefs if the given boolFieldName already exists. Returns True if succesfully set.
        public bool TrySetBoolDataField(string boolFieldName, bool bValue)
        {
            string prefName = _PackageID + "." + boolFieldName;
            if (EditorPrefs.HasKey(prefName))
            {
                EditorPrefs.SetBool(prefName, bValue);
                return true;
            }

            return false;
        }

        // Safely sets the EditorPrefs if the given intFieldName already exists. Returns True if succesfully set.
        public bool TrySetIntDataField(string intFieldName, int value)
        {
            string prefName = _PackageID + "." + intFieldName;
            if (EditorPrefs.HasKey(prefName))
            {
                EditorPrefs.SetInt(prefName, value);
                return true;
            }

            return false;
        }

        // Safely sets the EditorPrefs if the given floatFieldName already exists. Returns True if succesfully set.
        public bool TrySetFloatDataField(string floatFieldName, float value)
        {
            string prefName = _PackageID + "." + floatFieldName;
            if (EditorPrefs.HasKey(prefName))
            {
                EditorPrefs.SetFloat(prefName, value);
                return true;
            }

            return false;
        }

        // Safely sets the EditorPrefs if the given stringFieldName already exists. Returns True if succesfully set.
        public bool TrySetStringDataField(string stringFieldName, string value)
        {
            string prefName = _PackageID + "." + stringFieldName;
            if (EditorPrefs.HasKey(prefName))
            {
                EditorPrefs.SetString(prefName, value);
                return true;
            }

            return false;
        }

        public bool GetBoolDataValue(string dataFieldName)
        {
            if (UnityEditor.EditorPrefs.HasKey(dataFieldName))
            {
                return EditorPrefs.GetBool(_PackageID + "." + dataFieldName);
            }

            return false;
        }

        public int GetIntDataValue(string dataFieldName)
        {
            if (UnityEditor.EditorPrefs.HasKey(dataFieldName))
            {
                return EditorPrefs.GetInt(_PackageID + "." + dataFieldName);
            }

            return 0;
        }

        public float GetFloatDataValue(string dataFieldName)
        {
            if (UnityEditor.EditorPrefs.HasKey(dataFieldName))
            {
                return EditorPrefs.GetFloat(_PackageID + "." + dataFieldName);
            }

            return 0.0f;
        }

        public string GetStringDataValue(string dataFieldName)
        {
            if (UnityEditor.EditorPrefs.HasKey(dataFieldName))
            {
                return EditorPrefs.GetString(_PackageID + "." + dataFieldName);
            }

            return "";
        }
    }
}
#endif