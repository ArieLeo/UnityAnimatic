using UnityEngine;
using Unity.Mathematics;

namespace Voxell.Animatic
{
  using Inspector;

  [CreateAssetMenu(menuName = "Animatic/Asset", fileName = "NewAnimaticAsset")]
  public class AnimaticAsset : ScriptableObject
  {
    public GameObject targetPrefab;
    public AnimationClip[] animationClips;
    [Range(0.01f, 0.2f)] public float segmentInterval = 0.1f;
    public AnimationSegment[] animationSegments;

    [Button]
    public void SegmentData()
    {
      GameObject target = Instantiate<GameObject>(targetPrefab);

      int segmentCount = 0;

      for (int c=0; c < animationClips.Length; c++)
      {
        AnimationClip clip = animationClips[c];
        float clipLength = clip.length;
        int partitionCount = (int)(clipLength / segmentInterval);
        segmentCount += partitionCount;
      }

      Debug.Log(segmentCount);

      DestroyImmediate(target);
    }
  }

  public struct AnimationSegment
  {
    public float3 position;
    public float3 prevVelocity;
    public float3 currVelocity;
    public float3 predVelocity;
    public float segmentTime;
  }

  public struct AnimationSettings
  {
    public AnimationClip clip;
    public bool canBeLooped;
  }
}