using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// TrackAsset은 Timeline에 표시될 트랙을 정의합니다.
// TrackClipType 속성을 사용하여 이 트랙이 어떤 타입의 클립을 받을지 지정합니다.
[TrackClipType(typeof(TimeScaleAsset))]
public class TimeScaleTrack : TrackAsset
{
}
