﻿using System;
using System.Linq;

namespace RuntimeUnitTestToolkit
{
    [Preserve(AllMembers = true)]
    public partial class UnitTestLoader
    {
#if !(UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        public static void Register()
        {
            try
            {
#if NETFX_CORE
			    foreach (var type in typeof(UnitTestLoader).GetTypeInfo().GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).OrderBy(x=>x.Name))
#else
                foreach (var type in typeof(UnitTestLoader).GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).OrderBy(x => x.Name))
#endif
                {
                    if (type.Name.StartsWith("<")) continue;

                    var test = Activator.CreateInstance(type);


#if NETFX_CORE
                    foreach (var method in type.GetTypeInfo().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(x=>x.Name))
#else
                    foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(x => x.Name))
#endif
                    {
                        if (method.Name == "Equals" || method.Name == "GetHashCode" || method.Name == "ToString" || method.Name == "GetType") continue;

                        var m = method;
                        UnitTestRoot.AddTest(type.Name, m.Name, () => m.Invoke(test, Type.EmptyTypes));
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }
    }
}