using System;
using UnityEngine;

namespace Client.Common.AnimatorTools
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true , Inherited = true)]
    public class AnimatorCallbackDispatcherAttribute: PropertyAttribute
    {
    }
}