#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using SablesTools.AvatarCopier.Data;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{
    public class AvatarMatchHandler
    {
        private static AvatarMatchHandler _Instance = null;

        public static AvatarMatchHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new AvatarMatchHandler();
            }

            return _Instance;
        }

        private AvatarMatchHandler()
        {

        }

        // Dictionary to check for duplicate non-attachable source game objects
        protected Dictionary<string, List<GameObject>> _NamedDestinationGameObjects = new Dictionary<string, List<GameObject>>();
        protected Dictionary<string, List<GameObject>> _NamedSourceGameObjects = new Dictionary<string, List<GameObject>>();

        protected Dictionary<string, List<VirtualGameObject>> _NameToVirtualObjListDictionary = new Dictionary<string, List<VirtualGameObject>>();

        protected Dictionary<GameObject, VirtualGameObject> _DestinationGameObjToVirtualObjDictionary = new Dictionary<GameObject, VirtualGameObject>();
        protected Dictionary<GameObject, VirtualGameObject> _SourceGameObjToVirtualObjDictionary = new Dictionary<GameObject, VirtualGameObject>();

        protected List<GameObject> _TopLevelAttachableGameObjects = new List<GameObject>();
        public int TopLevelAttachableGameObjectsCount { get { return _TopLevelAttachableGameObjects.Count; } }

        protected List<string> _DestinationDuplicateNames = new List<string>();
        protected List<string> _SourceDuplicateNames = new List<string>();

        // List of all GameObjects currently under the Destination Avatar
        protected List<GameObject> _DestinationGameObjects = new List<GameObject>();
        public int DestinationGameObjectsCount { get { return _DestinationGameObjects.Count; } }

        // List of all GameObjects currently under the Source Avatar
        protected List<GameObject> _SourceGameObjects = new List<GameObject>();
        public int SourceGameObjectsCount { get { return _SourceGameObjects.Count; } }

        protected HashSet<string> _SkeletonBoneNameSet = new HashSet<string>();

        public bool bDebug = false;

        public bool bDestinationDuplicatesUIOpen = false;
        public bool bSourceDuplicatesUIOpen = false;

        public Vector2 DuplicateMenuScrollPosition = new Vector2();

        public void Reset()
        {
            _NamedDestinationGameObjects = new Dictionary<string, List<GameObject>>();
            _NamedSourceGameObjects = new Dictionary<string, List<GameObject>>();

            _NameToVirtualObjListDictionary = new Dictionary<string, List<VirtualGameObject>>();

            _DestinationGameObjToVirtualObjDictionary = new Dictionary<GameObject, VirtualGameObject>();
            _SourceGameObjToVirtualObjDictionary = new Dictionary<GameObject, VirtualGameObject>();

            _TopLevelAttachableGameObjects = new List<GameObject>();

            _DestinationDuplicateNames = new List<string>();
            _SourceDuplicateNames = new List<string>();

            _DestinationGameObjects = new List<GameObject>();

            _SourceGameObjects = new List<GameObject>();

            _SkeletonBoneNameSet = new HashSet<string>();

            bDestinationDuplicatesUIOpen = false;
            bSourceDuplicatesUIOpen = false;

            DuplicateMenuScrollPosition = new Vector2();
        }


        public void CreateMappings()
        {
            Reset();

            MapDestinationAvatar();
            MapSourceAvatar();

            CreateDuplicateWarnings();

            FindTopLevelAttachables();
        }

        protected void MapDestinationAvatar()
        {
            MapDestinationAvatar_R(CopierSettingsHandler.GetInstance().Destination);
        }

        protected void MapDestinationAvatar_R(GameObject currentDestinationObj)
        {
            // Add to whole list
            _DestinationGameObjects.Add(currentDestinationObj);

            // Add to Named Dict for duplicate tracking
            if (_NamedDestinationGameObjects.ContainsKey(currentDestinationObj.name))
            {
                // Add Name to _DestinationDuplicateNames if there is exactly one already in the dictionary's list
                List<GameObject> l = _NamedDestinationGameObjects[currentDestinationObj.name];
                if (l.Count == 1)
                {
                    _DestinationDuplicateNames.Add(currentDestinationObj.name);
                }

                l.Add(currentDestinationObj);
            }
            else
            {
                // Create list and add it to the Dictionary
                List<GameObject> l = new List<GameObject>();
                l.Add(currentDestinationObj);
                _NamedDestinationGameObjects.Add(currentDestinationObj.name, l);
            }

            // Loop through childern
            for (int i = 0; i < currentDestinationObj.transform.childCount; i++)
            {
                MapDestinationAvatar_R(currentDestinationObj.transform.GetChild(i).gameObject);
            }
        }

        protected void MapSourceAvatar()
        {
            MapSourceAvatar_R(CopierSettingsHandler.GetInstance().Source);
        }

        protected void MapSourceAvatar_R(GameObject currentSourceObj)
        {
            // Add to whole list
            _SourceGameObjects.Add(currentSourceObj);

            // Add to Named Dict for duplicate tracking
            if (_NamedSourceGameObjects.ContainsKey(currentSourceObj.name))
            {
                // Add Name to _SourceDuplicateNames if there is exactly one already in the dictionary's list
                List<GameObject> l = _NamedSourceGameObjects[currentSourceObj.name];
                if (l.Count == 1)
                {
                    _SourceDuplicateNames.Add(currentSourceObj.name);
                }

                l.Add(currentSourceObj);
            }
            else
            {
                // Create list and add it to the Dictionary
                List<GameObject> l = new List<GameObject>();
                l.Add(currentSourceObj);
                _NamedSourceGameObjects.Add(currentSourceObj.name, l);
            }

            // Loop through childern
            for (int i = 0; i < currentSourceObj.transform.childCount; i++)
            {
                MapSourceAvatar_R(currentSourceObj.transform.GetChild(i).gameObject);
            }
        }

        protected void FindTopLevelAttachables()
        {
            if (CopierSettingsHandler.GetInstance().bCantSearchForAttachables)
            {
                return;
            }

            GetHashSetFromSourceHumanoidData();

            FindTopLevelAttachable_R(CopierSettingsHandler.GetInstance().Source);
        }

        protected void FindTopLevelAttachable_R(GameObject currentSourceObj)
        {
            // If the source object is not in the Skeleton Bone Name Set, then it should be an attachable.
            if (_SkeletonBoneNameSet.Contains(currentSourceObj.name) == false && currentSourceObj != CopierSettingsHandler.GetInstance().Source)
            {
                _TopLevelAttachableGameObjects.Add(currentSourceObj);

                return;
            }

            // Loop through childern
            for (int i = 0; i < currentSourceObj.transform.childCount; i++)
            {
                FindTopLevelAttachable_R(currentSourceObj.transform.GetChild(i).gameObject);
            }
        }
        ///
        /// Mapping
        ///

        protected void CreateDuplicateWarnings()
        {
            // Destination Duplicate Warnings
            foreach (string destDuplicateName in _DestinationDuplicateNames)
            {
                Warnings.AvatarCopierDuplicateObjectNamesWarning newDuplicateWarning = new Warnings.AvatarCopierDuplicateObjectNamesWarning(destDuplicateName, false);
                foreach (GameObject duplicateObject in _NamedDestinationGameObjects[destDuplicateName])
                {
                    newDuplicateWarning.AddDuplicate(duplicateObject);
                }
                WarningHandler.GetInstance().AddWarning(newDuplicateWarning);
            }

            // Source Duplicates Warnings
            foreach (string sourceDuplicateName in _SourceDuplicateNames)
            {
                Warnings.AvatarCopierDuplicateObjectNamesWarning newDuplicateWarning = new Warnings.AvatarCopierDuplicateObjectNamesWarning(sourceDuplicateName, true);
                foreach (GameObject duplicateObject in _NamedSourceGameObjects[sourceDuplicateName])
                {
                    newDuplicateWarning.AddDuplicate(duplicateObject);
                }
                WarningHandler.GetInstance().AddWarning(newDuplicateWarning);
            }
        }

        public void LinkNonAttachablesInSource()
        {
            LinkNonAttachablesInSource_R(CopierSettingsHandler.GetInstance().Source);
        }

        public void LinkNonAttachablesInSource_R(GameObject currentSourceObj)
        {
            // Ignore if this Source Obj is an Attachable
            if (CopierSettingsHandler.GetInstance().bCantSearchForAttachables == false)
            {
                if (currentSourceObj != CopierSettingsHandler.GetInstance().Source && _SkeletonBoneNameSet.Contains(currentSourceObj.name) == false)
                {
                    return;
                }
            }

            LinkSourceGameObjectToVirtual(currentSourceObj);

            // Loop through childern
            for (int i = 0; i < currentSourceObj.transform.childCount; i++)
            {
                LinkNonAttachablesInSource_R(currentSourceObj.transform.GetChild(i).gameObject);
            }
        }

        // Attempts to Link this the input SourceGameObject with a free Virtual Object of the same name.
        public VirtualGameObject LinkSourceGameObjectToVirtual(GameObject sourceGameObject)
        {
            // Link Virtual with Source Obj if there appears one to exist
            if (DoesVirtualObjectWithNameExist(sourceGameObject.name))
            {
                foreach (VirtualGameObject virtualObj in GetVirtualGameObjectListFromName(sourceGameObject.name))
                {
                    if (virtualObj.LinkedSource == null)
                    {
                        virtualObj.LinkedSource = sourceGameObject;
                        // Can't forget the dictionary!!
                        _SourceGameObjToVirtualObjDictionary.Add(sourceGameObject, virtualObj);
                        return virtualObj;
                    }
                }
            }

            return null;
        }

        public bool DoesVirtualObjectWithNameExist(string name)
        {
            // The SourceAvatar should always be true
            if (CopierSettingsHandler.GetInstance().bCantSearchForAttachables == false && CopierSettingsHandler.GetInstance().bMatchInputsTogether == true && name == CopierSettingsHandler.GetInstance().Source.name)
            {
                return true;
            }

            return _NameToVirtualObjListDictionary.ContainsKey(name);
        }

        public List<VirtualGameObject> GetVirtualGameObjectListFromName(string name)
        {
            // The SourceAvatar should always exist in the list as well as use the DestinationAvatar's name
            if (CopierSettingsHandler.GetInstance().bCantSearchForAttachables == false && CopierSettingsHandler.GetInstance().bMatchInputsTogether == true && name == CopierSettingsHandler.GetInstance().Source.name)
            {
                return _NameToVirtualObjListDictionary[CopierSettingsHandler.GetInstance().Destination.name];
            }

            return _NameToVirtualObjListDictionary[name];
        }

        //protected void LinkNonAttachableInSource()

        public void LinkVirtualObj(VirtualGameObject virtualObj)
        {
            // If its attachable, then link on its Source Object
            if (virtualObj.bIsAttachable)
            {
                _SourceGameObjToVirtualObjDictionary.Add(virtualObj.LinkedSource, virtualObj);
            }
            // If not Attachable, then its a Destination Object that needs linked
            else
            {
                _DestinationGameObjToVirtualObjDictionary.Add(virtualObj.LinkedDestination, virtualObj);
            }

            if (virtualObj.LinkedDestination != null)
            {
                if (_DestinationGameObjToVirtualObjDictionary.ContainsKey(virtualObj.LinkedDestination) == false)
                {
                    _DestinationGameObjToVirtualObjDictionary.Add(virtualObj.LinkedDestination, virtualObj);
                }
            }

            if (virtualObj.LinkedSource != null)
            {
                if (_SourceGameObjToVirtualObjDictionary.ContainsKey(virtualObj.LinkedSource) == false)
                {
                    _SourceGameObjToVirtualObjDictionary.Add(virtualObj.LinkedSource, virtualObj);
                }
            }

            // Add to Name to Virtual Obj Dict
            if (DoesVirtualObjectWithNameExist(virtualObj.Name))
            {
                GetVirtualGameObjectListFromName(virtualObj.Name).Add(virtualObj);
            }
            else
            {
                List<VirtualGameObject> l = new List<VirtualGameObject>();
                l.Add(virtualObj);
                _NameToVirtualObjListDictionary.Add(virtualObj.Name, l);
            }
        }

        public VirtualGameObject GetVirtualGameObjectFromDestinationObject(GameObject destinationGameObj)
        {
            if (destinationGameObj == null)
            {
                return null;
            }

            if (_DestinationGameObjToVirtualObjDictionary.ContainsKey(destinationGameObj))
            {
                return _DestinationGameObjToVirtualObjDictionary[destinationGameObj];
            }

            return null;
        }

        public VirtualGameObject GetVirtualGameObjectFromSourceObject(GameObject sourceGameObj)
        {
            if (sourceGameObj == null)
            {
                return null;
            }

            if (_SourceGameObjToVirtualObjDictionary.ContainsKey(sourceGameObj))
            {
                return _SourceGameObjToVirtualObjDictionary[sourceGameObj];
            }

            return null;
        }

        public VirtualGameObject GetVirtualGameObjectFromObject(GameObject gameObj)
        {
            VirtualGameObject virtualObj = GetVirtualGameObjectFromDestinationObject(gameObj);

            if (virtualObj == null)
            {
                virtualObj = GetVirtualGameObjectFromSourceObject(gameObj);
            }

            return virtualObj;
        }

        public GameObject GetTopLevelAttachableGameObject(int index)
        {
            return _TopLevelAttachableGameObjects[index];
        }

        public GameObject GetDestinationGameObject(int index)
        {
            return _DestinationGameObjects[index];
        }

        public GameObject GetSourceGameObject(int index)
        {
            return _SourceGameObjects[index];
        }

        // Simply adds all the names of the bones in the imported .fbx to the SourceBoneInAvatarSet HashSet
        public void GetHashSetFromSourceHumanoidData()
        {
            if (CopierSettingsHandler.GetInstance().Source == null)
            {
                return;
            }

            Animator SourceAnimator = CopierSettingsHandler.GetInstance().Source.GetComponent<Animator>();

            if (SourceAnimator == null)
            {
                return;
            }

            Avatar SourceAvatarAvatar = SourceAnimator.avatar;

            if (SourceAvatarAvatar == null)
            {
                return;
            }

            // Add each skeleton bone to the map. Start at 1 to avoid checking against the Avatar's root object.
            for (int i = 1; i < SourceAvatarAvatar.humanDescription.skeleton.Length; i++)
            {
                string name = SourceAvatarAvatar.humanDescription.skeleton[i].name;

                if (_SkeletonBoneNameSet.Contains(name))
                {
                    Debug.LogError("While Getting Hash Set from Source Avatar's HumanoidData, multiple bones of the same exact name were found. This should be impossible");
                    return;
                }

                _SkeletonBoneNameSet.Add(name);
            }
        }
    }
}

#endif