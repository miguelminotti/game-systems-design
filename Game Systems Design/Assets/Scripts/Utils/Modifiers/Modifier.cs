using System;
using UnityEngine;

namespace MMStdLib.Utils
{
    public interface IModifier<T>
    {
        string ModifierName { get; }
        T Process(T value);
    }
    
    public class Modifier<T> : IModifier<T>
    {
        public string ModifierName => _modifierName;

        private readonly string _modifierName;
        private readonly Func<T, T> _processingAction;

        public Modifier(string name, Func<T, T> processingAction)
        {
            _modifierName = name;
            _processingAction = processingAction;
        }

        public T Process(T value)
        {
            if (_processingAction == null)
                return value;
            return _processingAction(value);
        }
    }
}