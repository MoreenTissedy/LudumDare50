using CauldronCodebase;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Editor
{
    [CustomPropertyDrawer(typeof(Encounter.PotionResult))]
    public class PotionResultPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var potionRect = new Rect(position.x, position.y, 150, position.height/2);
            var influenceCoef = new Rect(position.x + 170, position.y, position.width - 170, position.height/2.5f);
            var bonusEvent = new Rect(position.x, position.y + position.height/2f, position.width/2.2f, position.height/2.5f);
            var bonusCard = new Rect(position.x + position.width/2f, position.y + position.height/2, position.width/2.2f, position.height/2.5f);

            
            EditorGUILayout.BeginHorizontal();
            float coefValue = property.FindPropertyRelative("influenceCoef").floatValue;
            if (coefValue > 0)
            {
                GUI.color = new Color32(0x79, 0xCD, 0x71, 0xFF);
            }
            else if (coefValue < 0)
            {
                GUI.color = new Color32(0xCD, 0x4F, 0x43, 0xFF);
            }
            
            EditorGUI.PropertyField(potionRect, property.FindPropertyRelative("potion"), GUIContent.none);
            GUI.color = Color.white;
            EditorGUI.PropertyField(influenceCoef, property.FindPropertyRelative("influenceCoef"), GUIContent.none);
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUI.PropertyField(bonusEvent, property.FindPropertyRelative("bonusEvent"), GUIContent.none);
            EditorGUI.PropertyField(bonusCard, property.FindPropertyRelative("bonusCard"), GUIContent.none);
            EditorGUILayout.EndHorizontal();

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label)*3f;
        }
    }
}