using UnityEngine;

namespace EasyLoc
{
    public class SceneLocalizer : BaseLocalizer
    {
        protected override MonoBehaviour[] GetLocalizationObjects()
        {
            return FindObjectsOfType<MonoBehaviour>(true);
        }
    }
}