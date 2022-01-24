using UnityEngine;
using UnityEditor;

namespace Voxell.Animatic
{
  using Inspector;

  public class AnimaticClipEditor : AbstractVXEditor
  {
    private AnimaticEditorWindow _animaticEditorWindow;
    private float _animationClipLength;
    private float _animationPreview;

    public override void OnEnable()
    {
      base.OnEnable();
      _animaticEditorWindow = target as AnimaticEditorWindow;
      _animationPreview = 0.0f;
    }

    public override void OnRender()
    {
      _animationClipLength = _animaticEditorWindow.selectedAnimationClip.length;
      Animator animator = _animaticEditorWindow.targetAnimator;
      if (animator == null)
      {
        EditorGUILayout.HelpBox("A target Animator must be assigned to preview the animation", MessageType.Info);
        return;
      }

      _animationPreview = EditorGUILayout.Slider(
        "Preview Time", _animationPreview,
        0.0f, _animationClipLength
      );

      _animaticEditorWindow.selectedAnimationClip.SampleAnimation(
        animator.gameObject, _animationPreview
      );
    }
  }
}