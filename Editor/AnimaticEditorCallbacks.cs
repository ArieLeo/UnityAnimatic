using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Voxell.Animatic
{
  public class AnimaticEditorCallbacks : MonoBehaviour
  {
    internal static bool ShowAnimaticEditorWindow(string path)
    {
      if (!(Selection.activeObject is AnimaticAsset)) return false;
      AnimaticAsset animaticAsset = Selection.activeObject as AnimaticAsset;
      string guid = AssetDatabase.AssetPathToGUID(path);

      // if window is already opened, focus on it
      AnimaticEditorWindow[] animaticWindows = Resources.FindObjectsOfTypeAll<AnimaticEditorWindow>();
      for (int w=0; w < animaticWindows.Length; w++)
      {
        if (animaticWindows[w].selectedGuid == guid)
        {
          animaticWindows[w].Focus();
          return true;
        }
      }

      // open window and dock beside other animatic editor windows
      AnimaticEditorWindow window = EditorWindow.CreateWindow<AnimaticEditorWindow>(
        animaticAsset.name, typeof(AnimaticEditorWindow)
      );
      window.Focus();
      window.Initialize(ref animaticAsset, guid);
      return true;
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
      string path = AssetDatabase.GetAssetPath(instanceID);
      return ShowAnimaticEditorWindow(path);
    }
  }
}