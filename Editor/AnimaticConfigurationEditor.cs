using UnityEngine;
using UnityEditor;

namespace Voxell.Animatic
{
  using Inspector;

  public class AnimaticConfigurationEditor : AbstractVXScriptableEditor
  {
    private SerializedProperty _targetPrefab;
    private SerializedProperty _segmentInterval;

    public override void FindProperties()
    {
      base.FindProperties();
      _targetPrefab = serializedScriptableObject.FindProperty("targetPrefab");
      _segmentInterval = serializedScriptableObject.FindProperty("segmentInterval");
    }

    public override void OnRender()
    {
      EditorGUILayout.PropertyField(_targetPrefab);
      EditorGUILayout.PropertyField(_segmentInterval);
    }
  }
}