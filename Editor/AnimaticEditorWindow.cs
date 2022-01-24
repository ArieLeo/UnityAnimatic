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
    private SerializedObject _serializedObject;
    private SerializedObject _assetSerializedObject;

    public string selectedGuid;
    public AnimaticAsset animaticAsset;
    public Animator targetAnimator;

    public AnimationClip selectedAnimationClip;
    private bool _multiSelect;

    private ListView _animationClipListView;
    private Label _inspectorTitle;
    private InspectorView _inspectorView;

    public void CreateGUI()
    {
      _serializedObject = new SerializedObject(this);
      if (animaticAsset != null) Initialize(ref animaticAsset, selectedGuid);
    }

    public void Initialize(ref AnimaticAsset animaticAsset, string guid)
    {
      this.animaticAsset = animaticAsset;
      this.selectedGuid = guid;
      _assetSerializedObject = new SerializedObject(animaticAsset);

      // root visual element
      VisualElement root = rootVisualElement;

      // import UXML
      VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
        "Packages/voxell.animatic/Editor/AnimaticEditorWindow.uxml"
      );
      visualTree.CloneTree(root);

      // toolbar
      Toolbar toolbar = root.Q<Toolbar>("toolbar");
      Button viewInProjButton = root.Q<Button>("view-in-project");
      viewInProjButton.clicked += () => { EditorGUIUtility.PingObject(this.animaticAsset); };

      // target object
      ObjectField targetObjectField = new ObjectField("Target Animator")
      {
        objectType = typeof(Animator),
        allowSceneObjects = true
      };
      targetObjectField.BindProperty(_serializedObject.FindProperty("targetAnimator"));
      toolbar.Add(targetObjectField);

      // save asset button
      Button saveButton = root.Q<Button>("save-asset");
      saveButton.Add(new Image
      {
        image = EditorGUIUtility.IconContent("d_SaveAs").image,
        scaleMode = ScaleMode.ScaleToFit
      });
      saveButton.clicked += SaveAsset;

      // panels
      VisualElement leftPanel = root.Q<VisualElement>("left-panel");
      VisualElement rightPanel = root.Q<VisualElement>("right-panel");
      InitializeAnimation(in leftPanel, in root);

      // configure asset button
      Button configureButton = leftPanel.Q<Button>("configure-asset");
      configureButton.clicked += ConfigureAsset;

      InitializeInspectorView();
    }

    private void ConfigureAsset()
    {
      _inspectorTitle.text = "Asset Configuration";
      _inspectorView.InitializeInspector(animaticAsset, typeof(AnimaticConfigurationEditor));
      _animationClipListView.ClearSelection();
      selectedAnimationClip = null;
    }

    private void InitializeInspectorView()
    {
      // check if there is any previously selected animation clip
      if (selectedAnimationClip != null)
      {
        List<AnimationClip> animationClips = animaticAsset.animationClips.ToList();
        // check if the scriptable object still contatins this clip
        if (animationClips.Contains(selectedAnimationClip))
        {
          _animationClipListView.AddToSelection(animationClips.IndexOf(selectedAnimationClip));
          PreviewSelectedClip();
          return;
        }
      }

      // if there are no previously selected animation clip, asset configuration will be the default view
      ConfigureAsset();
    }

    private void OnGUI()
    {
      Repaint();
    }

    private void SaveAsset()
    {
      EditorUtility.SetDirty(animaticAsset);
      AssetDatabase.SaveAssets();
    }

    private void OnDrawGizmos()
    {
      _inspectorView.ClearEditor();
    }
  }
}