using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


namespace Code
{
    [CustomEditor(typeof(Star)), CanEditMultipleObjects]
    public sealed class StarEditor : Editor
    {
        private SerializedProperty _center;
        private SerializedProperty _points;
        private SerializedProperty _frequency;

        private Vector3 _pointSnap = new Vector3(0.05f, 0.05f, 0.05f);

        private bool _showList;

        private void OnEnable()
        {
            _center = serializedObject.FindProperty("_center");
            _points = serializedObject.FindProperty("_points");
            _frequency = serializedObject.FindProperty("_frequency");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_center);
            _showList = EditorGUILayout.Foldout(_showList, _points.displayName);

            if (_showList)
            {
                for (int i = 0; i < _points.arraySize; i++)
                {
                    var point = _points.GetArrayElementAtIndex(i);
                    var horizontal = EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(point);
                    if (GUILayout.Button("↓", GUILayout.Width(22)))
                    {
                        MoveElementDown(i);
                    }
                    if (GUILayout.Button("+", GUILayout.Width(22)))
                    {
                        DuplicateItem(i);
                    }
                    if (GUILayout.Button("-", GUILayout.Width(22)))
                    {
                        RemoveItem(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.IntSlider(_frequency, 1, 20);

            var totalPoints = _frequency.intValue * _points.arraySize;

            if (totalPoints < 3)
            {
                EditorGUILayout.HelpBox("At least three points are needed.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(totalPoints + " points in total.", MessageType.Info);
            }
            
            if (!serializedObject.ApplyModifiedProperties() &&
                (Event.current.type != EventType.ExecuteCommand ||
                 Event.current.commandName != "UndoRedoPerformed"))
            {
                return;
            }
            
            foreach (var obj in targets)
            {
                if (obj is Star star)
                {
                    star.UpdateMesh();
                }
            }
        }

        private void MoveElementDown(int index)
        {
            if (_points.arraySize > index + 1)
                _points.MoveArrayElement(index, index + 1);
        }

        private void DuplicateItem(int index)
        {
            _points.InsertArrayElementAtIndex(index);
        }

        private void RemoveItem(int index)
        {
            _points.DeleteArrayElementAtIndex(index);
        }
        
        private void OnSceneGUI()
        {
            if (!(target is Star star))
            {
                return;
            }
            var starTransform = star.transform;
            var angle = -360f / (star.Frequency * star.Points.Length);
            for (var i = 0; i < star.Points.Length; i++)
            {
                var rotation = Quaternion.Euler(0f, 0f, angle * i);
                var oldPoint = starTransform.TransformPoint(rotation *
                                                            star.Points[i].Position);
                var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity,
                    0.02f, _pointSnap, Handles.DotHandleCap);
                if (oldPoint == newPoint)
                {
                    continue;
                }
                star.Points[i].Position = Quaternion.Inverse(rotation) *
                                          starTransform.InverseTransformPoint(newPoint);
                star.UpdateMesh();
            }

        }
    }
}