#if (UNITY_EDITOR)

namespace SablesTools.AvatarCopier.Operations
{
    public class Operation
    {
        protected bool _bUserSetEnabled = true;
        public bool bUserSetEnabled { get { return _bUserSetEnabled; } set { _SetUserSetEnabled(value); } }

        protected virtual void _SetUserSetEnabled(bool value)
        {
            _bUserSetEnabled = value;
        }

        //public virtual bool IsEnabledInUI()
        //{
        //    return bUserSetEnabledInUI;
        //}
        
        public virtual bool IsFullyEnabled()
        {
            return bUserSetEnabled;
        }
    }
}

#endif