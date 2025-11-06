using UnityEngine;
using UnityEngine.Playables;

// PlayableAsset은 Timeline에 추가할 수 있는 클립 애셋을 정의합니다.
public class TimeScaleAsset : PlayableAsset
{
    // Inspector에서 슬로우 모션 강도를 조절할 수 있도록 값을 노출합니다.
    [Range(0.01f, 4.0f)] // 0.01배속 ~ 4배속까지 조절 가능
    public float timeScale = 0.5f; // 기본값은 0.5배속

    // Timeline이 이 애셋을 사용하여 Playable을 생성할 때 호출됩니다.
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        // ScriptPlayable을 생성합니다.
        var playable = ScriptPlayable<TimeScaleBehaviour>.Create(graph);

        // Behaviour에 접근하여 Inspector에서 설정한 timeScale 값을 전달합니다.
        var timeScaleBehaviour = playable.GetBehaviour();
        timeScaleBehaviour.newTimeScale = this.timeScale;

        return playable;
    }
}
