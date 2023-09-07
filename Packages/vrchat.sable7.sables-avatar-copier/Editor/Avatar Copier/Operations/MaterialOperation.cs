#if (UNITY_EDITOR)
using UnityEngine;
using System.Collections.Generic;
#if (VRC_SDK_VRCSDK3)
using VRC;
#endif

namespace SablesTools.AvatarCopier.Operations
{
    public class MaterialOperation : Operation
    {
        protected class LinkedSmartMaterialList
        {
            public LinkedSmartMaterialListNode RootNode { get; set; } = null;

            protected Material[] _CopyToMaterialList = null;
            protected Material[] _CopyFromMaterialList = null;

            //protected int _MaxToIndex = 0;

            public LinkedSmartMaterialList(Material[] copyToMaterialList, Material[] copyFromMaterialList)
            {
                _CopyToMaterialList = copyToMaterialList;
                //_MaxToIndex = copyToMaterialList.Length;
                _CopyFromMaterialList = copyFromMaterialList;



                for (int toMatIndex = 0; toMatIndex < _CopyToMaterialList.Length; toMatIndex++)
                {
                    Material copyToMat = _CopyToMaterialList[toMatIndex];

                    if (copyToMat == null)
                    {
                        continue;
                    }

                    for (int fromMatIndex = 0; fromMatIndex < _CopyFromMaterialList.Length; fromMatIndex++)
                    {
                        Material copyFromMat = _CopyFromMaterialList[fromMatIndex];

                        if (copyFromMat == null)
                        {
                            continue;
                        }

                        CreateNewNode(LevenshteinDistance(_CopyToMaterialList[toMatIndex].name, _CopyFromMaterialList[fromMatIndex].name), toMatIndex, fromMatIndex);
                    }
                }
            }

            public int GetLength()
            {
                int count = 0;
                LinkedSmartMaterialListNode currentNode = RootNode;
                while (true)
                {
                    if (currentNode == null)
                    {
                        break;
                    }

                    count++;
                    currentNode = currentNode.NextNode;
                }

                return count;
            }

            protected LinkedSmartMaterialListNode CreateNewNode(int score, int copyToIndex, int copyFromIndex)
            {
                LinkedSmartMaterialListNode newNode = new LinkedSmartMaterialListNode(score, copyToIndex, copyFromIndex);

                if (RootNode == null)
                {
                    RootNode = newNode;
                }
                else
                {
                    LinkedSmartMaterialListNode currentNode = RootNode;
                    int count = 0;
                    while (true)
                    {
                        if (score < currentNode.Score)
                        {
                            if (currentNode.PreviousNode != null)
                            {
                                currentNode.PreviousNode.NextNode = newNode;
                                newNode.PreviousNode = currentNode.PreviousNode;
                            }
                            else
                            {
                                RootNode = newNode;
                            }

                            newNode.NextNode = currentNode;
                            currentNode.PreviousNode = newNode;

                            break;
                        }

                        // If reached end of linked-list, add the new node to the end
                        if (currentNode.NextNode == null)
                        {
                            currentNode.NextNode = newNode;
                            newNode.PreviousNode = currentNode;
                            break;
                        }

                        // next node
                        currentNode = currentNode.NextNode;

                        count++;
                        if (count > 500)
                        {
                            break;
                        }
                    }
                }

                return newNode;
            }

            public Material[] CreateSmartMaterialList()
            {
                Material[] newMaterialArray = _CopyToMaterialList.Clone() as Material[];

                List<int> foundCopyToIndexArray = new List<int>();
                List<int> foundCopyFromIndexArray = new List<int>();


                LinkedSmartMaterialListNode currentNode = RootNode;
                while (true)
                {
                    if (currentNode == null)
                    {
                        break;
                    }
                    currentNode = currentNode.NextNode;
                }

                currentNode = RootNode;
                int matCount = 0;
                while (true)
                {
                    if (currentNode == null)
                    {
                        break;
                    }

                    bool bFound = false;
                    for (int i = 0; i < foundCopyToIndexArray.Count; i++)
                    {
                        if (foundCopyToIndexArray[i] == currentNode.CopyToIndex)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (bFound == false)
                    {
                        for (int i = 0; i < foundCopyFromIndexArray.Count; i++)
                        {
                            if (foundCopyFromIndexArray[i] == currentNode.CopyFromIndex)
                            {
                                bFound = true;
                                break;
                            }
                        }
                    }


                    if (bFound == false)
                    {
                        newMaterialArray[currentNode.CopyToIndex] = _CopyFromMaterialList[currentNode.CopyFromIndex];
                        foundCopyToIndexArray.Add(currentNode.CopyToIndex);
                        foundCopyFromIndexArray.Add(currentNode.CopyFromIndex);
                        matCount++;
                    }

                    currentNode = currentNode.NextNode;

                    if (matCount >= _CopyToMaterialList.Length || matCount >= _CopyFromMaterialList.Length)
                    {
                        break;
                    }
                }

                return newMaterialArray;
            }

            // Returns the amount of changes in the string that are required to match the in target string
            protected int LevenshteinDistance(string source, string target)
            {
                // from https://stackoverflow.com/questions/2344320/comparing-strings-with-tolerance

                // degenerate cases
                if (source == target) return 0;
                if (source.Length == 0) return target.Length;
                if (target.Length == 0) return source.Length;

                // create two work vectors of integer distances
                int[] v0 = new int[target.Length + 1];
                int[] v1 = new int[target.Length + 1];

                // initialize v0 (the previous row of distances)
                // this row is A[0][i]: edit distance for an empty s
                // the distance is just the number of characters to delete from t
                for (int i = 0; i < v0.Length; i++)
                    v0[i] = i;

                for (int i = 0; i < source.Length; i++)
                {
                    // calculate v1 (current row distances) from the previous row v0

                    // first element of v1 is A[i+1][0]
                    //   edit distance is delete (i+1) chars from s to match empty t
                    v1[0] = i + 1;

                    // use formula to fill in the rest of the row
                    for (int j = 0; j < target.Length; j++)
                    {
                        var cost = (source[i] == target[j]) ? 0 : 1;
                        v1[j + 1] = System.Math.Min(v1[j] + 1, System.Math.Min(v0[j + 1] + 1, v0[j] + cost));
                    }

                    // copy v1 (current row) to v0 (previous row) for next iteration
                    for (int j = 0; j < v0.Length; j++)
                        v0[j] = v1[j];
                }

                return v1[target.Length];
            }
        }

        protected class LinkedSmartMaterialListNode
        {
            public LinkedSmartMaterialListNode NextNode { get; set; } = null;
            public LinkedSmartMaterialListNode PreviousNode { get; set; } = null;

            public int CopyToIndex { get; set; } = 0;
            public int CopyFromIndex { get; set; } = 0;

            //public bool bUsed { get; set; } = false;

            public int Score { get; set; } = 0;

            public LinkedSmartMaterialListNode(int score, int copyToIndex, int copyFromIndex)
            {
                Score = score;
                CopyToIndex = copyToIndex;
                CopyFromIndex = copyFromIndex;
            }
        }

        //protected SablesTools.AvatarCopier.Data.VirtualGameObject _VirtualObject = null;
        //public SablesTools.AvatarCopier.Data.VirtualGameObject VirtualObject { get { return _VirtualObject; } }

        protected SablesTools.AvatarCopier.Operations.OverridingComponentOperation _OverridingCompOp = null;
        public SablesTools.AvatarCopier.Operations.OverridingComponentOperation OverridingCompOp { get { return _OverridingCompOp; } }

        protected Material[] _PreExistingMaterials;
        protected Material[] _OverridingMaterials;

        public bool bSmartCopy = false;

        public MaterialOperation(OverridingComponentOperation overridingCompOp)
        {
            _OverridingCompOp = overridingCompOp;

            bUserSetEnabled = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseMaterialOperations");
            bSmartCopy = Handlers.CopierSettingsHandler.GetInstance().GetBoolDataValue("bDefaultUseSmartMaterialCopy");

            Renderer preExistingRenderer = overridingCompOp.ReplacingPreExistingCompOp.OriginComponent as Renderer;
            Renderer overridingRenderer = overridingCompOp.OriginComponent as Renderer;

            _PreExistingMaterials = preExistingRenderer.sharedMaterials.Clone() as Material[];
            _OverridingMaterials = overridingRenderer.sharedMaterials.Clone() as Material[];
        }

        public override bool IsFullyEnabled()
        {
            if (OverridingCompOp.IsFullyEnabled() == false)
            {
                return false;
            }

            return base.IsFullyEnabled();
        }

        public void ApplyMaterialOperation()
        {
            Renderer runtimeRenderer = OverridingCompOp.RunTimeComponent as Renderer;

            if (bSmartCopy)
            {
                LinkedSmartMaterialList linkedMatList = new LinkedSmartMaterialList(_PreExistingMaterials, _OverridingMaterials);

                runtimeRenderer.sharedMaterials = linkedMatList.CreateSmartMaterialList();
            }
            // if not smartly, just copy over based on indexes
            else
            {
                Material[] newMaterials = new Material[_PreExistingMaterials.Length];

                for (int i = 0; i < _OverridingMaterials.Length && i < _PreExistingMaterials.Length; i++)
                {
                    Debug.Log("Dasdfas: " + _PreExistingMaterials[i] + "~~" + _OverridingMaterials[i]);
                    newMaterials[i] = _OverridingMaterials[i];
                }

                runtimeRenderer.sharedMaterials = newMaterials;


                // Should already be copying off indexes??
            }
        }
    }
}
#endif