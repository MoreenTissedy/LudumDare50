using UnityEngine;

namespace EasyLoc
{
    public class HierarchyLocalizer: BaseLocalizer
    {
        protected override MonoBehaviour[] GetLocalizationObjects()
        {
            return GetComponentsInChildren<MonoBehaviour>();
        }
    }
}