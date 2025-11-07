using UnityEngine;
using UnityEngine.Playables;

// PlayableBehaviour는 Timeline 클립의 동작을 정의합니다.
public class TimeScaleBehaviour : PlayableBehaviour
{
    // Inspector에서 설정할 새로운 Time Scale 값
    public float newTimeScale = 1.0f;

    private float defaultTimeScale = 1.0f;

    // 클립이 재생되기 시작할 때 호출됩니다.
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        // 현재 Time Scale 값을 저장해 둡니다.
        defaultTimeScale = Time.timeScale;
        // 새로운 Time Scale 값을 적용합니다.
        Time.timeScale = newTimeScale;
    }

    // 클립 재생이 멈출 때 호출됩니다. (일시정지 또는 종료)
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        // 클립이 끝나면 원래 Time Scale 값으로 복원합니다.
        // 유효한 플레이어 상태일 때만 복원하도록 하여, 에디터가 멈추는 등의 상황을 방지합니다.
        if (info.effectivePlayState == PlayState.Paused)
        {
            Time.timeScale = defaultTimeScale;
        }
    }
}
