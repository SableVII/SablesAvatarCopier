#if (UNITY_EDITOR)
using System.Collections.Generic;
using SablesTools.AvatarCopier.Operations;
using UnityEngine;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Handlers
{  
    public class MaterialOperationHandler : OperationHandler
    {
        private static MaterialOperationHandler _Instance = null;

        public static MaterialOperationHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new MaterialOperationHandler();
            }

            return _Instance;
        }

        private MaterialOperationHandler()
        {

        }

        protected List<MaterialOperation> MaterialOperations = new List<MaterialOperation>();
        protected List<MaterialOperation> SavedMaterialOperations = new List<MaterialOperation>();

        public void Reset()
        {
            MaterialOperations = new List<MaterialOperation>();
            SavedMaterialOperations = new List<MaterialOperation>();
        }

        public void SavedReset()
        {
            SavedMaterialOperations = new List<MaterialOperation>(MaterialOperations);
            MaterialOperations = new List<MaterialOperation>();
        }

        public void ApplySavedData()
        {
            foreach (MaterialOperation SavedMaterialOp in SavedMaterialOperations)
            {
                foreach (MaterialOperation MaterialOp in MaterialOperations)
                {
                    if (SavedMaterialOp.OverridingCompOp.OriginComponent == MaterialOp.OverridingCompOp.OriginComponent)
                    {
                        MaterialOp.bUserSetEnabled = SavedMaterialOp.bUserSetEnabled;
                        MaterialOp.bSmartCopy = SavedMaterialOp.bSmartCopy;
                        break;
                    }
                }
            }
        }

        // Preform after ComponentOperations are created
        public void CreateMaterialOperations()
        {
            for (int i = 0; i < ComponentOperationHandler.GetInstance().OverridingCompOperations.Count; i++)
            {
                OverridingComponentOperation overridingCompOp = ComponentOperationHandler.GetInstance().OverridingCompOperations[i];

                if (overridingCompOp.IsReplacing && typeof(Renderer).IsAssignableFrom(overridingCompOp.ComponentType))
                {
                    MaterialOperation newMaterialOp = new MaterialOperation(overridingCompOp);

                    MaterialOperations.Add(newMaterialOp);
                }
            }
        }

        public int GetMaterialOperationCount()
        {
            return MaterialOperations.Count;
        }

        public int GetEnabledMaterialOperationCount()
        {
            int count = 0;
            foreach (MaterialOperation MaterialOp in MaterialOperations)
            {
                if (MaterialOp.bUserSetEnabled)
                {
                    count++;
                }
            }

            return count;
        }

        public MaterialOperation GetMaterialOperation(int index)
        {
            if (index >= 0 && index < MaterialOperations.Count)
            {
                return MaterialOperations[index];
            }

            return null;
        }
    }
}

#endif