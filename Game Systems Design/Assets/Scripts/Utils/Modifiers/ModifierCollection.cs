using System.Collections.Generic;

namespace MMStdLib.Utils
{
    public interface IModifierCollection<T>
    {
        void AddModifier(IModifier<T> modifier);
        void RemoveModifier(IModifier<T> modifier);
        void RemoveModifier(string modifierName);
        void ClearModifiers();
        T Process(T value);
    }

    public class ModifierCollection<T>
    {
        private readonly List<IModifier<T>> _modifiers = new List<IModifier<T>>();

        public void AddModifier(IModifier<T> modifier)
        {
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(IModifier<T> modifier)
        {
            _modifiers.Remove(modifier);
        }

        public void RemoveModifier(string modifierName)
        {
            _modifiers.RemoveAll(m => m.ModifierName == modifierName);
        }

        public T Process(T value)
        {
            T modifiedValue = value;
            foreach (var modifier in _modifiers)
            {
                modifiedValue = modifier.Process(modifiedValue);
            }
            return modifiedValue;
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
        }
    }
}
