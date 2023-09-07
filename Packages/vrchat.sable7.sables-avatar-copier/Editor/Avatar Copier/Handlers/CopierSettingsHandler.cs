#if (UNITY_EDITOR)
using UnityEngine;

using System.Collections.Generic;
using UnityEditor;
using System.IO;//Debug


namespace SablesTools.AvatarCopier.Handlers
{
    [System.Serializable]
    public class SettingsData
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

        protected SettingsData Data = new SettingsData();

        public static CopierSettingsHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new CopierSettingsHandler();
                _Instance.LoadData();

                // Check to see if DefaultParameters needs initializing
                if (_Instance.GetBoolDataValue("bFirstTimeOpened"))
                {
                    //Debug.Log("FIRST TIME BEING OPENED!!!");
                    //_Instance.PreservedParameters = new List<PreservedPropertyData>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedParameters());
                    for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
                    {
                        _Instance.Data.DefaultPreservedPropertiesEnabledStatuses.Add(true);
                    }
                    _Instance.TrySetDataField("bFirstTimeOpened", false);
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

            System.Reflection.FieldInfo[] FieldInfos = Data.GetType().GetFields();
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
            SaveJsonData(JsonUtility.ToJson(Data));
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

            if (Data.DefaultPreservedPropertiesEnabledStatuses.Count != PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length)
            {
                Data.DefaultPreservedPropertiesEnabledStatuses.Clear();
                for (int i = 0; i < PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length; i++)
                {
                    Data.DefaultPreservedPropertiesEnabledStatuses.Add(true);
                }
                //DefaultPreservedPropertiesEnabledStatuses.Add = new List<bool>(PreservedPropertyHandler.GetInstance().GetDefaultPreservedProperties().Length);
            }

            for (int i = 0; i < Data.DefaultPreservedPropertiesEnabledStatuses.Count; i++)
            {
                PreservedPropertyHandler.GetInstance().SetDefaultPropertyEnabled(i, Data.DefaultPreservedPropertiesEnabledStatuses[i]);
            }

            //Debug.Log("LOADED DATA");
        }

        private void FromJson()
        {
            TextAsset LoadedData = LoadJsonData();
            if (LoadedData != null)
            {
                Data = JsonUtility.FromJson<SettingsData>(LoadJsonData().text);
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

        private TextAsset LoadJsonData()
        {
            AssetDatabase.Refresh();

            string DataString = "";
            string FullPath = DataDirectory + "/" + DataFileName + ".json";
            if (!File.Exists(FullPath))
            {
                SaveJsonData(JsonUtility.ToJson(Data));
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
            foreach (PreservedPropertyData PropData in Data.PreservedProperties)
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

            if (!FieldInfo.GetValue(Data).Equals(newValue))
            {
                FieldInfo.SetValue(Data, newValue);
                RegisterSave();

                return true;
            }

            return false;
        }

        public bool GetBoolDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return false;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];


            return (bool)FieldInfo.GetValue(Data);
        }

        public float GetFloatDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return 0.0f;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return (float)FieldInfo.GetValue(Data);
        }

        public string GetStringDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return "";
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return (string)FieldInfo.GetValue(Data);
        }

        public int GetIntDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return -1;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return (int)FieldInfo.GetValue(Data);
        }

        public object GetObjectDataValue(string dataFieldName)
        {
            if (!_FieldInfoDictionary.ContainsKey(dataFieldName))
            {
                return null;
            }

            System.Reflection.FieldInfo FieldInfo = _FieldInfoDictionary[dataFieldName];

            return FieldInfo.GetValue(Data);
        }
    }
}
#endif