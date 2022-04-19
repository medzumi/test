using UnityEngine;

namespace Injecting
{
    public abstract class ScriptableInjectInstructions : ScriptableObject
    {
        public abstract void SetInstructions(Container container);
    }
}