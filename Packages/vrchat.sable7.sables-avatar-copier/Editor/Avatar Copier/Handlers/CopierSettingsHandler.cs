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
        public List<PreservedPropertyData> PreservedProperties = new List<PreservedPropertyData>();
        public bool bCreateDestinationClone = false;
        public bool bPreserveProperties = true;
        public List<bool> DefaultPreservedPropertiesEnabledStatuses = new List<bool>();
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

        private static string DataDirectory = "Assets/Resources";
        private static string DataFileName = "AvatarCopierSettings";

        private string _PackageID = "vrchat.sable7.sables-avatar-copier";

        protected DefaultSettingsData _DefaultData = new DefaultSettingsData();

        public static CopierSettingsHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new CopierSettingsHandler();
                _Instance.LoadData();

                // Check to see if DefaultParameters needs initializing
                if (_Instance.GetBoolDataValue("bFirstTimeOpened"))
                {
                    _Instance.OnFirstTimeBeingOpened();

                    _Instance.TrySetBoolDataField("bFirstTimeOpened", false);
                }

                // Clean out Destination and Source Avatars as sometimes it just returns with random junk x.x
                _Instance.Destination = null;
                _Instance.Source = null;

                _Instance.RegisterSave();
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

        public void SaveIfRegistered()
        {
            if (RegisteredForSave)
            {
                Save();
            }
        }

        protected void Save()
        {
            SaveJsonData(JsonUtility.ToJson(_DefaultData));
            RegisteredForSave = false;
            //Debug.Log("SAVED DATA");
        }

        public void RegisterSave()
        {
            RegisteredForSave = true;
        }

        public void LoadData()
        {
            FromJson();

            if (_DefaultData.DefaultPreservedPropertiesEnabledStatuses.Count != PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length)
            {
                _DefaultData.DefaultPreservedPropertiesEnabledStatuses.Clear();
                for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
                {
                    _DefaultData.DefaultPreservedPropertiesEnabledStatuses.Add(true);
                }
                //DefaultPreservedPropertiesEnabledStatuses.Add = new List<bool>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length);
            }

            for (int i = 0; i < _DefaultData.DefaultPreservedPropertiesEnabledStatuses.Count; i++)
            {
                PreservedPropertyHandler.GetInstance().SetDefaultPropertyEnabled(i, _DefaultData.DefaultPreservedPropertiesEnabledStatuses[i]);
            }

            //Debug.Log("LOADED DATA");
        }

        private void FromJson()
        {
            TextAsset LoadedData = LoadJsonData();
            if (LoadedData != null)
            {
                _DefaultData = JsonUtility.FromJson<DefaultSettingsData>(LoadJsonData().text);
            }
        }

        private void SaveJsonData(string JsonText)
        {
            string FullPath = DataDirectory + "/" + DataFileName + ".json";

            if (!System.IO.Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            if (!System.IO.Directory.Exists(DataDirectory))
            {
                Debug.LogError("Failed to create Directory for saving the Data json");
                return;
            }

            using (File.Create(FullPath))
            {

            }

            using (StreamWriter SW = new StreamWriter(FullPath))
            {
                //Debug.Log("Saving -> " + JsonUtility.ToJson(MaterialSwapperData));
                SW.WriteLine(JsonText);
            }
        }

        private void OnFirstTimeBeingOpened()
        {
            //Debug.Log("FIRST TIME BEING OPENED!!!");

            // Set EditorPrefs with Defaults
            //SetEditorPrefsDefaults();

            // Set Default Preserved Properties
            for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
            {
                _Instance._DefaultData.DefaultPreservedPropertiesEnabledStatuses.Add(true);
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

        private TextAsset LoadJsonData()
        {
            AssetDatabase.Refresh();

            string DataString = "";
            string FullPath = DataDirectory + "/" + DataFileName + ".json";
            if (!File.Exists(FullPath))
            {
                SaveJsonData(JsonUtility.ToJson(_DefaultData));
            }

            if (File.Exists(FullPath))
            {
                DataString = File.ReadAllText(FullPath);
            }

            if (DataString == "")
            {
                Debug.LogError("Failed to load data json");
                return null;
            }

            TextAsset DataTextFile = AssetDatabase.LoadAssetAtPath<TextAsset>(FullPath);
            if (DataTextFile == null)
            {
                Debug.Log("Loaded Data File is null FileExist(?): " + File.Exists(FullPath));
            }

            return DataTextFile;
            //CopierData = JsonUtility.FromJson<AvatarCopierData>(DataTextFile.text);
        }

        // Called to make sure each property data is properly set
        public void EnsurePreservedPropertyData()
        {
            foreach (PreservedPropertyData PropData in _DefaultData.PreservedProperties)
            {
                PropData.Ensure();
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


        // Returns True if in Value is new. False otherwise or if Field of in FieldName is not found. This is the prefered method of changing 
        public bool TrySetDataField(string dataFieldName, object newValue)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return false;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            if (FieldInfo.FieldType != newValue.GetType())
            {
                return false;
            }

            if (!FieldInfo.GetValue(_DefaultData).Equals(newValue))
            {
                FieldInfo.SetValue(_DefaultData, newValue);
                RegisterSave();

                return true;
            }

            return false;
        }

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

        /*public bool GetBoolDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return false;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];


            return (bool)FieldInfo.GetValue(Data);
        }*/

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

        //public float GetFloatDataValue(string dataFieldName)
        //{
        //    if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
        //    {
        //        return 0.0f;
        //    }

        //    System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

        //    return (float)FieldInfo.GetValue(Data);
        //}

        /*public string GetStringDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return "";
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return (string)FieldInfo.GetValue(_DefaultData);
        }*/

        //public int GetIntDataValue(string dataFieldName)
        //{
        //    if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
        //    {
        //        return -1;
        //    }

        //    System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

        //    return (int)FieldInfo.GetValue(Data);
        //}

        public object GetObjectDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return null;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return FieldInfo.GetValue(_DefaultData);
        }
    }
}
#endif