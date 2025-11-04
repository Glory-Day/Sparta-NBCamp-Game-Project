using System;
using UnityEngine;
using Backend.Util.Extension;

namespace Backend.Object.Character.Player
{
    public class CameraDistanceRaycaster : MonoBehaviour
    {
        #region SERIALIZABLE PROPERTIES API

        [field: Header("Target Reference")]
        [field: Tooltip("The target transform reference that the camera will look at.\n\n" +
                        "카메라의 트랜스폼 참조.")]
        [field: SerializeField]
        public Transform CameraTransform { get; private set; }

        [field: Tooltip("The target transform reference that the camera mounted rig.\n\n" +
                        "카메라가 부착된 리그의 트랜스폼 참조.")]
        [field: SerializeField]
        public Transform CameraRigTransform { get; private set; }

        [field: Header("Controller Settings")]
        [field: Tooltip("This value controls how smoothly the old camera distance will be interpolated toward the new distance. " +
                        "Setting this value to '50f' (or above) will result in no (visible) smoothing at all. " +
                        "Setting this value to '1f' (or below) will result in very noticeable smoothing. " +
                        "In most cases, a value of '25f' is recommended.\n\n" +
                        "해당 값은 기존 카메라 거리에서 새 거리로의 보간이 얼마나 부드럽게 이루어지는지를 제어한다. " +
                        "이 값을 '50f'(또는 그 이상)로 설정하면 (눈에 띄는) 보간 효과가 전혀 발생하지 않는다. " +
                        "이 값을 '1f'(또는 그 이하)로 설정하면 매우 뚜렷한 보간 효과가 발생한다. " +
                        "대부분의 경우 '25f' 값을 권장한다.")]
        [field: SerializeField] public float SmoothingFactor { get; private set; } = 25f;

        [field: Header("Layer Overrides")]
        [field: Tooltip("Layer mask used for ray casting." +
                        "레이 캐스팅에 사용되는 레이어 마스크.\n\n")]
        [field: SerializeField] public LayerMask IncludeLayers { get; private set; } = ~0;

        [field: Tooltip("List of colliders to exclude when ray casting.\n\n" +
                        "레이 캐스팅 시 제외할 콜라이더 목록.")]
        [field: SerializeField] public Collider[] ExcludeColliders { get; private set; }

        [field: Header("Cast Settings")]
        [field: Tooltip("Obstacle scanning cast method mode.\n\n" +
                        "장애물을 스캔할 캐스트 기능 모드.")]
        [field: SerializeField] public CastMode CastMode { get; private set; }

        [field: Tooltip(
            "Additional distance which is added to the raycast's length to prevent the camera from clipping into level geometry. " +
            "For most situations, the default value of '0.1f' is sufficient. " +
            "You can try increasing this distance a bit if you notice a lot of clipping. " +
            "This value is only used when 'Raycast' is selected.\n\n" +
            "레이케스트 길이에 추가되는 거리로, 카메라가 레벨 지오메트리에 클리핑되는 것을 방지한다. " +
            "대부분의 경우 기본값인 '0.1f'로 충분하다. " +
            "클리핑 현상이 많이 발생한다면 이 거리를 조금 늘려볼 수 있다. " +
            "이 값은 'Raycast'가 선택된 경우에만 사용된다.")]
        public float MinimumCastingDistance { get; set; } = 0.1f;

        [field: Tooltip("Radius of spherical cast, used only when 'SphereCast' is selected.\n\n" +
                        "구형 탐지 반경, 'SphereCast'가 선택된 경우에만 사용된다.")]
        public float Radius { get; set; } = 0.2f;

        #endregion

        private RaycastExcluder _excluder;

        private float _distance;

        private void Awake()
        {
            IncludeLayers = IncludeLayers.Remove(RaycastExcluder.ExcludeLayer);

            _excluder = new RaycastExcluder(ExcludeColliders);

            if (CameraTransform == null)
            {
                Debug.LogWarning("No camera transform has been assigned.", this);
            }

            if (CameraRigTransform == null)
            {
                Debug.LogWarning("No camera target transform has been assigned.", this);
            }

            // If the necessary transform references not have been assigned, disable this script.
            if (CameraTransform == null || CameraRigTransform == null)
            {
                enabled = false;

                return;
            }

            // Set initial starting distance.
            _distance = (CameraRigTransform.position - transform.position).magnitude;
        }

        private void LateUpdate()
        {
            _excluder.Apply();

            // Calculate current distance by casting a ray cast.
            var distance = GetCameraDistance();

            _excluder.Restore();

            // Linear interpolation is applied to the movement distance for smoother transitions.
            _distance = Mathf.Lerp(_distance, distance, SmoothingFactor * Time.deltaTime);

            // Set new position of camera transform reference.
            CameraTransform.position = transform.position + ((CameraRigTransform.position - transform.position).normalized * _distance);
        }

        /// <returns>
        /// Maximum distance by casting a ray (or sphere) from this transform to the camera rig transform.
        /// </returns>
        private float GetCameraDistance()
        {
            return CastMode switch
            {
                CastMode.Raycast => GetDistanceByRayCast(),
                CastMode.SphereCast => GetDistanceBySphereCast(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetDistanceByRayCast()
        {
            var direction = CameraRigTransform.position - transform.position;

            var ray = new Ray(transform.position, direction);
            var distance = direction.magnitude + MinimumCastingDistance;

            if (Physics.Raycast(ray, out var hit, distance, IncludeLayers, QueryTriggerInteraction.Ignore) == false)
            {
                // If no obstacle was hit, return full distance.
                return direction.magnitude;
            }

            // Check if minimum distance from obstacles can be subtracted from hit distance, then return distance.
            if (hit.distance - MinimumCastingDistance < 0f)
            {
                return hit.distance;
            }

            return hit.distance - MinimumCastingDistance;
        }

        private float GetDistanceBySphereCast()
        {
            var direction = CameraRigTransform.position - transform.position;

            var ray = new Ray(transform.position, direction);
            var distance = direction.magnitude;

            var isHit = Physics.SphereCast(ray, Radius, out var hit, distance, IncludeLayers, QueryTriggerInteraction.Ignore);

            // If no obstacle was hit, return full distance.
            return isHit ? hit.distance : direction.magnitude;
        }
    }
}
