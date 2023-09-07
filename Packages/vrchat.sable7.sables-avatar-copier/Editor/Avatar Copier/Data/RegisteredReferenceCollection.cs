#if (UNITY_EDITOR)
using System.Collections.Generic;
#if (VRC_SDK_VRCSDK3)
#endif

namespace SablesTools.AvatarCopier.Data
{
    public class RegisteredReferenceCollection
    {
        public List<RegisteredReference> RegisteredReferences = new List<RegisteredReference>();
        public bool bOpenInUI = false;

        public void AddRegisteredData(RegisteredReference newRefData)
        {
            RegisteredReferences.Add(newRefData);
        }

        public RegisteredReference GetRegisteredDataOfName(string inName)
        {
            foreach (RegisteredReference RefData in RegisteredReferences)
            {
                if (RefData.PropFieldName == inName)
                {
                    return RefData;
                }
            }

            return null;
        }

        public int GetRegisteredDataCountOfName(string inName)
        {
            RegisteredReference RefData = GetRegisteredDataOfName(inName);

            if (RefData != null)
            {
                return RefData.GetReferenceCount();
            }

            return 0;
        }

        public void Reset()
        {
            RegisteredReferences = new List<RegisteredReference>();
            bOpenInUI = false;
        }

        public bool HasWarning()
        {
            foreach (RegisteredReference regRef in RegisteredReferences)
            {
                for (int i = 0; i < regRef.GetReferenceCount(); i++)
                {
                    if (regRef.GetReferenceElement(i).RefWarning != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasUnSurpressedWarning()
        {
            foreach (RegisteredReference regRef in RegisteredReferences)
            {
                for (int i = 0; i < regRef.GetReferenceCount(); i++)
                {
                    if (regRef.GetReferenceElement(i).RefWarning != null && regRef.GetReferenceElement(i).RefWarning.bSurpressed == false)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

#endif