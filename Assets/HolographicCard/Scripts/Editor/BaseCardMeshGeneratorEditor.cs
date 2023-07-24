using System;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(BaseCardMeshGenerator))]
    public class BaseCardMeshGeneratorEditor : UnityEditor.Editor {
        SerializedProperty cornerRadius, width, height;
        BaseCardMeshGenerator generator;
        void OnEnable() {
            generator = (BaseCardMeshGenerator) target;
            cornerRadius = serializedObject.FindProperty("cornerRadius");
            width = serializedObject.FindProperty("width");
            height = serializedObject.FindProperty("height");
        }
        public override void OnInspectorGUI() {
            serializedObject.Update();
            float newWidth = EditorGUILayout.Slider("Card Width", width.floatValue, 1, 100);
            if (Mathf.Abs(newWidth - width.floatValue) > 0.01f) {
                width.floatValue = newWidth;
                generator.SetWidth(newWidth);
            }
            //
            float newHeight = EditorGUILayout.Slider("Card Height", height.floatValue, 1, 100);
            if (Mathf.Abs(newHeight - height.floatValue) > 0.01f) {
                height.floatValue = newHeight;
                generator.SetHeight(newHeight);
            }
            //
            float newCornerRadius = EditorGUILayout.Slider("Corner Radius", cornerRadius.floatValue, 0.001f, 50);
            if (Mathf.Abs(newCornerRadius - cornerRadius.floatValue) > 0.01f) {
                cornerRadius.floatValue = newCornerRadius;
                generator.SetCurveRadius(newCornerRadius);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}