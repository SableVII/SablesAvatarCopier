#if (UNITY_EDITOR)
using System.Collections.Generic;
using SablesTools.AvatarCopier.Warnings;
using SablesTools.AvatarCopier.Errors;

namespace SablesTools.AvatarCopier.Handlers
{
    // Handles Warnings!
    public class WarningHandler
    {
        private static WarningHandler _Instance = null;

        public static WarningHandler GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new WarningHandler();
            }

            return _Instance;
        }

        protected List<AvatarCopierWarning> Warnings = new List<AvatarCopierWarning>();
        protected List<AvatarCopierError> Errors = new List<AvatarCopierError>();

        private WarningHandler()
        {

        }

        public void Reset()
        {
            ResetWarings();
            ResetErrors();
        }

        public void ResetWarings()
        {
            Warnings = new List<AvatarCopierWarning>();
        }

        public void ResetErrors()
        {
            Errors = new List<AvatarCopierError>();
        }

        public void AddWarning(AvatarCopierWarning NewWarning)
        {
            /*int InsertIndex = Warnings.Count; 
            for (int i = 0; i < Warnings.Count; i++)
            {
                if (Warnings[i].Order <= NewWarning.Order)
                {
                    break;
                }
            }

            Warnings.Insert(InsertIndex, NewWarning);*/
            Warnings.Add(NewWarning);
        }

        public AvatarCopierWarning GetWarning(int index)
        {
            return Warnings[index];
        }

        public int GetWarningsCount()
        {
            return Warnings.Count;
        }

        public int GetUnSurpressedWarningsCount()
        {
            int count = 0;
            foreach (AvatarCopierWarning warning in Warnings)
            {
                if (!warning.bSurpressed)
                {
                    count++;
                }
            }

            return count;
        }

        public bool HasUnSurpressedWarnings()
        {
            foreach (AvatarCopierWarning warning in Warnings)
            {
                if (!warning.bSurpressed)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddError(AvatarCopierError inError)
        {
            Errors.Add(inError);
        }

        public AvatarCopierError GetError(int index)
        {
            return Errors[index];
        }

        public int GetErrorCount()
        {
            return Errors.Count;
        }

        public bool HasHaultingErrors()
        {
            foreach (AvatarCopierError copierError in Errors)
            {
                if (copierError.bHaultCopy)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasErrors()
        {
            return Errors.Count > 0;
        }

        public void RemoveWarning(AvatarCopierWarning inWarning)
        {
            for (int i = 0; i < Warnings.Count; i++)
            {
                if (inWarning == Warnings[i])
                {
                    Warnings.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
#endif