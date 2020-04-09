using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Reflection;

public static class MonobehaviourExtension
{
    #region Public Methods
    public static void CustomInvoke(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
    {
        MethodInfo methodInfo = FindMethod(_MonoBehaviour, _MethodName, _Parameters);
        methodInfo.Invoke(_MonoBehaviour, _Parameters);
    }

    public static T GetReturnValue<T>(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
    {
        MethodInfo methodInfo = FindMethod(_MonoBehaviour, _MethodName, _Parameters);
        return (T)methodInfo.Invoke(_MonoBehaviour, _Parameters);
    }

    public static void ClearStaticCache(this MonoBehaviour _MonoBehaviour)
    {
        s_CacheRequests.Clear();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private static MethodInfo FindMethod(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
    {
        MethodRequest key = new MethodRequest(_MethodName, _Parameters);
        MethodInfo methodInfo = null;
        if (!s_CacheRequests.TryGetValue(key, out methodInfo))
        {
            BindingFlags flags = BindingFlags.Public;
            flags |= BindingFlags.NonPublic;
            flags |= BindingFlags.FlattenHierarchy;
            flags |= BindingFlags.Static;
            flags |= BindingFlags.Instance;
            flags |= BindingFlags.InvokeMethod;
            MethodInfo[] methodsInfos = _MonoBehaviour.GetType().GetMethods(flags);

            bool Match(MethodInfo _MethodInfo)
            {
                bool valid = _MethodInfo.Name == _MethodName;
                ParameterInfo[] parametersInfo = _MethodInfo.GetParameters();
                valid = valid && _Parameters.Length == parametersInfo.Length;
                for (int i = 0; i < parametersInfo.Length && valid; i++)
                {
                    valid = valid && (_Parameters[i] == null || _Parameters[i] != null && parametersInfo[i].ParameterType == _Parameters[i].GetType());
                }
                return valid;
            }
            methodInfo = methodsInfos.Where(Match).FirstOrDefault();
            s_CacheRequests.Add(key, methodInfo);
        }

        Assert.IsNotNull(methodInfo, "no method found");
        return methodInfo;
    }
    #endregion

    #region Private Attributes
    private static Dictionary<MethodRequest, MethodInfo> s_CacheRequests = new Dictionary<MethodRequest, MethodInfo>();
    #endregion

    #region Structs
    public struct MethodRequest
    {
        #region Public Methods
        public MethodRequest(string _MethodName, params object[] _Parameters)
        {
            MethodName = _MethodName;
            ParametersTypes = new System.Type[_Parameters.Length];
            for (int i = 0; i < _Parameters.Length; i++)
            {
                ParametersTypes[i] = _Parameters[i].GetType();
            }
        }

        public static bool operator ==(MethodRequest _A, MethodRequest _B)
        {
            bool areEquals = _A.MethodName == _B.MethodName;
            areEquals = areEquals && _A.ParametersTypes.Length == _B.ParametersTypes.Length;
            for (int i = 0; i < _A.ParametersTypes.Length && areEquals; i++)
            {
                areEquals = areEquals && _A.ParametersTypes[i] == _B.ParametersTypes[i];
            }
            return areEquals;
        }

        public static bool operator !=(MethodRequest _A, MethodRequest _B)
        {
            return !(_A == _B);
        }

        public override bool Equals(object obj)
        {
            return obj is MethodRequest request && this == request;
        }

        /**/
        public override int GetHashCode()
        {
            int hashcode = MethodName.GetHashCode();
            for (int i = 0; i < ParametersTypes.Length; i++)
            {
                hashcode += ParametersTypes[i].GetHashCode();
            }
            return hashcode;
        } 
        /**/
        #endregion

        #region Public Attributes
        public string MethodName;
        public System.Type[] ParametersTypes;
        #endregion
    }
    #endregion
}
