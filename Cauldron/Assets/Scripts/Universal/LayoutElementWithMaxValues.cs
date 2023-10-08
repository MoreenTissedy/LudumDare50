using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace Client.Client.Scripts.Common.Utils
{
    [RequireComponent(typeof(RectTransform))]
    [System.Serializable]
    public class LayoutElementWithMaxValues : LayoutElement
    {
        public float _maxHeight;
        public float _maxWidth;

        public bool _useMaxWidth;
        public bool _useMaxHeight;

        private bool _ignoreOnGettingPrefferedSize;

        public override int layoutPriority
        {
            get => _ignoreOnGettingPrefferedSize ? -1 : base.layoutPriority;
            set => base.layoutPriority = value;
        }

        public override float preferredHeight
        {
            get
            {
                if (_useMaxHeight)
                {
                    bool defaultIgnoreValue = _ignoreOnGettingPrefferedSize;
                    _ignoreOnGettingPrefferedSize = true;

                    float baseValue = LayoutUtility.GetPreferredHeight(transform as RectTransform);

                    _ignoreOnGettingPrefferedSize = defaultIgnoreValue;

                    return baseValue > _maxHeight ? _maxHeight : baseValue;
                }
                else
                    return base.preferredHeight;
            }
            set => base.preferredHeight = value;
        }

        public override float preferredWidth
        {
            get
            {
                if (_useMaxWidth)
                {
                    bool defaultIgnoreValue = _ignoreOnGettingPrefferedSize;
                    _ignoreOnGettingPrefferedSize = true;

                    var baseValue = LayoutUtility.GetPreferredWidth(transform as RectTransform);

                    _ignoreOnGettingPrefferedSize = defaultIgnoreValue;

                    return baseValue > _maxWidth ? _maxWidth : baseValue;
                }
                else
                {
                    return base.preferredWidth;
                }
            }
            set => base.preferredWidth = value;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LayoutElementWithMaxValues), true)]
    [CanEditMultipleObjects]
    public class LayoutMaxSizeEditor : LayoutElementEditor
    {
        private LayoutElementWithMaxValues _layoutMax;

        private SerializedProperty _maxHeightProperty;
        private SerializedProperty _maxWidthProperty;

        private SerializedProperty _useMaxHeightProperty;
        private SerializedProperty _useMaxWidthProperty;

        private RectTransform _rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();

            _layoutMax = target as LayoutElementWithMaxValues;
            _rectTransform = _layoutMax.transform as RectTransform;

            _maxHeightProperty = serializedObject.FindProperty(nameof(_layoutMax._maxHeight));
            _maxWidthProperty = serializedObject.FindProperty(nameof(_layoutMax._maxWidth));

            _useMaxHeightProperty = serializedObject.FindProperty(nameof(_layoutMax._useMaxHeight));
            _useMaxWidthProperty = serializedObject.FindProperty(nameof(_layoutMax._useMaxWidth));
        }

        public override void OnInspectorGUI()
        {
            Draw(_maxWidthProperty, _useMaxWidthProperty);
            Draw(_maxHeightProperty, _useMaxHeightProperty);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }

        private void Draw(SerializedProperty property, SerializedProperty useProperty)
        {
            Rect position = EditorGUILayout.GetControlRect();

            GUIContent label = EditorGUI.BeginProperty(position, null, property);

            Rect fieldPosition = EditorGUI.PrefixLabel(position, label);

            Rect toggleRect = fieldPosition;
            toggleRect.width = 16;

            Rect floatFieldRect = fieldPosition;
            floatFieldRect.xMin += 16;


            bool use = EditorGUI.Toggle(toggleRect, useProperty.boolValue);
            useProperty.boolValue = use;

            if (use)
            {
                EditorGUIUtility.labelWidth = 4;
                property.floatValue = EditorGUI.FloatField(floatFieldRect, new GUIContent(" "), property.floatValue);
                EditorGUIUtility.labelWidth = 0;
            }


            EditorGUI.EndProperty();
        }
    }

#endif
}