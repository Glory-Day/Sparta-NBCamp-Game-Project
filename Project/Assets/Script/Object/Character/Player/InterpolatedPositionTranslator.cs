using UnityEngine;

namespace Backend.Object.Character.Player
{
    public class InterpolatedPositionTranslator : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Target Reference")]
        [field: Tooltip("The target transform, whose position values will be copied and smoothed.\n\n" +
                        "이동 값이 복사되고 부드럽게 조정될 트렌스폼 레퍼런스.")]
        [field: SerializeField] public Transform Target { get; private set; }

        [field: Header("Method Settings")]
        [field: SerializeField] public UpdateMode UpdateMode { get; private set; }

        [field: Tooltip("If the delay occurring during the process of estimating the rotation value and smoothing it is corrected, then true. otherwise, false.\n\n" +
                        "이동 값이 추정되어 부드럽게 처리하는 과정에서 발생하는 지연을 보정할 것인지 여부.")]
        [field: SerializeField] public bool IsExtrapolated { get; private set; }

        [field: Header("Translation Settings")]
        [field: SerializeField] public InterpolationMode InterpolationMode { get; private set; }

        [field: Tooltip("Speed that controls how fast the current position will be smoothed toward the target position when 'Lerp' is selected.\n\n" +
                        "'Lerp'가 선택되었을 때 현재 위치가 목표 위치로 얼마나 빠르게 부드럽게 이동될지를 제어하는 속도.")]
        public float Speed { get; set; } = 20f;

        [field: Tooltip("Time that controls how fast the current position will be smoothed toward the target position when 'SmoothDamp' is selected.\n\n" +
                        "'SmoothDamp'가 선택되었을 때 현재 위치가 목표 위치로 얼마나 빠르게 부드럽게 이동될지를 제어하는 시간.")]
        public float Time { get; set; } = 0.02f;

        #endregion

        private Vector3 _position;
        private Vector3 _localPosition;

        private Vector3 _velocity;

        private void Awake()
        {
            // If no target has been selected, choose this transform's parent as the target.
            if (Target == null)
            {
                Target = transform.parent;
            }

            _position = transform.position;
            _localPosition = transform.localPosition;
        }

        private void OnEnable()
        {
            // Convert local position offset to world coordinates system.
            Vector3 offset = transform.localToWorldMatrix * _localPosition;

            // Add position offset and set current position.
            _position = Target.position + offset;
        }

        private void Update()
        {
            if (UpdateMode == UpdateMode.Update)
            {
                return;
            }

            Interpolate();
        }

        private void LateUpdate()
        {
            if (UpdateMode == UpdateMode.LateUpdate)
            {
                return;
            }

            Interpolate();
        }

        private void Interpolate()
        {
            var a = _position;
            var b = Target.position;

            // Convert local position offset to world coordinates system.
            Vector3 offset = transform.localToWorldMatrix * _localPosition;

            // If it chose to extrapolate, calculate a new target rotation.
            if (IsExtrapolated)
            {
                var difference = b - (a - offset);
                b += difference;
            }

            // Add local position offset to target.
            b += offset;

            // Smooth (based on chosen smooth mode) and return position.
            _position = InterpolationMode switch
            {
                InterpolationMode.Lerp => Vector3.Lerp(a, b, Speed * UnityEngine.Time.deltaTime),
                InterpolationMode.SmoothDamp => Vector3.SmoothDamp(a, b, ref _velocity, Time),
                _ => Vector3.zero
            };

            // Set position in transform.
            transform.position = _position;
        }
    }
}
