using UnityEngine;

namespace CauldronCodebase
{
    public class ScriptableObjectIdAttribute : PropertyAttribute { }

    public class ScriptableObjectWithId : ScriptableObject
    {
        [ScriptableObjectId] public string Id;
    }

}