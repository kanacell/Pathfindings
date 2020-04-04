using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System.Reflection;

public static class MonobehaviourExtension
{
	#region Public Methods
	public static void Invoke(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
	{
		MethodInfo methodInfo = FindMethod(_MonoBehaviour, _MethodName, _Parameters);
		methodInfo.Invoke(_MonoBehaviour, _Parameters);
	}

	public static T GetReturnValue<T>(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
	{
		MethodInfo methodInfo = FindMethod(_MonoBehaviour, _MethodName, _Parameters);
		return (T) methodInfo.Invoke(_MonoBehaviour, _Parameters);
	}
	#endregion

	#region Protected Methods
	#endregion

	#region Private Methods
	private static MethodInfo FindMethod(this MonoBehaviour _MonoBehaviour, string _MethodName, params object[] _Parameters)
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
		MethodInfo methodInfo = methodsInfos.Where(Match).FirstOrDefault();
		Assert.IsNotNull(methodInfo, "no method found");
		return methodInfo;
	}
	#endregion
}
