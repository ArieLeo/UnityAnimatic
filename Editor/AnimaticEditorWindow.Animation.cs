using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Voxell.Animatic
{
  using Inspector;

  public partial class AnimaticEditorWindow : EditorWindow
  {
    /// <summary>Initialize animation list view.</summary>
    private void InitializeAnimation(in VisualElement leftPanel, in VisualElement root)
    {
      _inspectorTitle = root.Q<Label>("animation-data-title");

      // animation inspector
      _inspectorView = root.Q<InspectorView>("selected-animation-inspector");

      // animations
      _animationClipListView = new ListView
      {
        showAddRemoveFooter = true,
        showBoundCollectionSize = false,
        showFoldoutHeader = true,
        headerTitle = "Animation Clips",
        fixedItemHeight = 22.0f,
        reorderable = true,
        selectionType = SelectionType.Multiple,
        showBorder = true,
        reorderMode = ListViewReorderMode.Simple
      };
      _animationClipListView.BindProperty(_assetSerializedObject.FindProperty("animationClips"));
      _animationClipListView.onSelectionChange += OnClipSelectionChange;
      leftPanel.Add(_animationClipListView);

      _animationClipListView.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
      _animationClipListView.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    /// <summary>Action taken when a new animation clip is being selected.</summary>
    private void OnClipSelectionChange(IEnumerable<object> selections)
    {
      if (selections == null) return;
      List<AnimationClip> animationClips = new List<AnimationClip>();
      foreach (object s in selections)
      {
        SerializedProperty prop = s as SerializedProperty;
        animationClips.Add(prop.objectReferenceValue as AnimationClip);
      }

      if (animationClips.Count == 1)
      {
        selectedAnimationClip = animationClips[0];
        PreviewSelectedClip();
      } else if (animationClips.Count > 1)
      {
        _inspectorTitle.text = $"{animationClips.Count} Animation Clips";
        if (animationClips.Any(_ => _ == null))
        {
          _inspectorView.ClearEditor();
          return;
        }
        _inspectorView.InitializeInspector(animationClips.ToArray());
        _multiSelect = true;
      }
    }

    /// <summary>Preview selected animation clip.</summary>
    private void PreviewSelectedClip()
    {
      if (selectedAnimationClip == null)
      {
        _inspectorView.ClearEditor();
        _inspectorTitle.text = "No Animation Selected";
        return;
      }
      _inspectorView.InitializeInspector(this, typeof(AnimaticClipEditor));
      _inspectorTitle.text = selectedAnimationClip.name;
      _multiSelect = false;
    }

    private void OnDragUpdate(DragUpdatedEvent evt)
    {
      DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

      string[] dragDropPaths = DragAndDrop.paths;

      // if all of the objects that is being dragged contains at least an animation
      if (dragDropPaths.Length > 0 && dragDropPaths.All(path =>
      {
        AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (animationClip != null) return true;
        return false;
      })) DragAndDrop.visualMode = DragAndDropVisualMode.Link;
    }

    private void OnDragPerform(DragPerformEvent evt)
    {
      string[] dragDropPaths = DragAndDrop.paths;
      List<AnimationClip> newClips = new List<AnimationClip>();

      for (int p=0; p < dragDropPaths.Length; p++)
      {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(dragDropPaths[p]);
        for (int a=0; a < assets.Length; a++)
        {
          Object clip = assets[a];
          if (clip is AnimationClip && !clip.name.StartsWith("__preview__"))
            newClips.Add(clip as AnimationClip);
        }
      }

      Undo.RecordObject(animaticAsset, "Add Animation Clips");
      AnimationClip[] prevClips = animaticAsset.animationClips;
      animaticAsset.animationClips = new AnimationClip[prevClips.Length + newClips.Count];

      // copy initial clips to the asset
      for (int p=0; p < prevClips.Length; p++) animaticAsset.animationClips[p] = prevClips[p];
      // copy new clips to the asset
      for (int n=0; n < newClips.Count; n++) animaticAsset.animationClips[prevClips.Length + n] = newClips[n];
    }
  }
}