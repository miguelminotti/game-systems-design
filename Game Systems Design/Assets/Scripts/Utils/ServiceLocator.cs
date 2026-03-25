using System.Collections.Generic;
using UnityEngine;

namespace MMStdLib.Utils
{
    public static class ServiceLocator
    {
        private static Dictionary<System.Type, object> _services = new Dictionary<System.Type, object>();

        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered. Overwriting.");
            }
            _services[type] = service;
        }

        public static T GetService<T>()
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new System.Exception($"Service of type {type} is not registered.");
        }

        public static void UnregisterService<T>()
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
            else
            {
                Debug.LogWarning($"Service of type {type} is not registered. Cannot unregister.");
            }
        }

        public static bool IsServiceRegistered<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public static void ClearAllServices()
        {
            _services.Clear();
        }
    }
}