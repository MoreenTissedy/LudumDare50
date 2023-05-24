using UnityEditor;
using UnityEngine;

namespace Client.Common.AnimatorTools.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorCallbackDispatcherAttribute))]
    public class AnimatorDispatcherEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MonoBehaviour component = property.serializedObject.targetObject as MonoBehaviour;
            if (component is not IAnimatorCallbackReceiver)
            {
                EditorGUILayout.HelpBox(new GUIContent("Please implement IAnimatorCallbackReceiver"));
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property, new GUIContent("Animator"));
            Animator target = property.objectReferenceValue as Animator;
            if (IsInNeedOfMediator(target, component))
            {
                if (GUILayout.Button("Create callback receiver", GUILayout.Width(150)))
                {
                    CreateMediators(target, component);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool IsInNeedOfMediator(Animator target, MonoBehaviour component)
        {
            return target != null
                   && target.gameObject != (component).gameObject
                   && !component.TryGetComponent(out AnimatorCallbackReceiverMediator _);
        }

        private void CreateMediators(Animator target, MonoBehaviour component)
        {
            AnimatorCallbackReceiverMediator receiver = component.gameObject.AddComponent<AnimatorCallbackReceiverMediator>();
            receiver.AddDispatcher(target);
            EditorUtility.SetDirty(receiver);
            EditorUtility.SetDirty(target);
        }
    }
}