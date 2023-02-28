

using UnityEditor;
using UnityEngine;

namespace Framework
{
    // [CanEditMultipleObjects, CustomEditor(typeof(CameraAction))]
    public class CameraActionItemInspector : Editor
    {
        // // 序列化对象
        // private SerializedObject obj;
        // void OnEnable()
        // {
        //     // 获取当前的序列化对象（target：当前检视面板中显示的对象）
        //     obj = new SerializedObject(target);
        // }
        // public override void OnInspectorGUI()
        // {
        //    
        //     // 查找floatArray属性
        //     // var elements = this.serializedObject.FindProperty("actionList");
        //     var elements = this.serializedObject.FindProperty("actionList");
        //     // 属性元素可见，控件展开状态
        //     if (EditorGUILayout.PropertyField(elements))
        //     {
        //         // 缩进一级
        //         EditorGUI.indentLevel++;
        //         // 设置元素个数
        //         elements.arraySize = EditorGUILayout.DelayedIntField("Size", elements.arraySize);
        //         // 绘制元素
        //         for (int i = 0, size = elements.arraySize; i < size; i++)
        //         {
        //             // 检索属性数组元素
        //             var element = elements.GetArrayElementAtIndex(i);
        //             EditorGUILayout.DropdownButton(GUIContent.none, FocusType.Passive);
        //             EditorGUILayout.PropertyField(element);
        //         }
        //         // 重置缩进
        //         EditorGUI.indentLevel--;
        //     }
        //
        //     // 空格
        //     EditorGUILayout.Space();
        //     
        //     if (GUI.changed) 
        //     {
        //         EditorUtility.SetDirty(target);
        //     }
        //     // 保存序列化数据，否则会出现设置数据丢失情况
        //     serializedObject.ApplyModifiedProperties ();
        // }
    }
}