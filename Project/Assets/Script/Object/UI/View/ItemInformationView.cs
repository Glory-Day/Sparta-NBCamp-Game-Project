using Backend.Util.Presentation;
using UnityEngine;
using UnityEngine.UI;
using Test.Data.Base;

namespace Backend.Object.UI
{
    public class ItemInformationView : MonoBehaviour, IView
    {
        #region SERIALIZABLE FIELD API

        [Header("Information References")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;

        #endregion

        private CanvasScaler _canvasScaler;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _canvasScaler = GetComponentInParent<CanvasScaler>();

            TryGetComponent(out _rectTransform);
            _rectTransform.pivot = Anchor.LeftTop;

            DisableChildrenRaycastTarget(transform);

            Hide();
        }

        /// <summary>
        /// 모든 하위 오브젝트의 UI에 대해 레이캐스트의 지정을 해제한다.
        /// </summary>
        private void DisableChildrenRaycastTarget(Transform childTransform)
        {
            // 본인이 Graphic를 상속하면 레이캐스트 지정을 해제한다.
            childTransform.TryGetComponent(out Graphic graphic);
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }

            // 하위 오브젝트가 존재하지 않으면 종료한다.
            var count = childTransform.childCount;
            if (count == 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                DisableChildrenRaycastTarget(childTransform.GetChild(i));
            }
        }

        /// <summary>
        /// 아이템 정보 창의 위치를 설정한다.
        /// </summary>
        public void SetRectPosition(RectTransform rectTransform)
        {
            // 캔버스 스케일러에 따른 해상도 대응을 적용한다.
            var resolution = _canvasScaler.referenceResolution;
            var width = Screen.width / resolution.x;
            var height = Screen.height / resolution.y;
            var scale = _canvasScaler.matchWidthOrHeight;
            var ratio = (width * (1f - scale)) + (height * scale);

            resolution = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            resolution *= ratio;

            // 설명 창의 초기 위치(슬롯에서 우하단 방향) 설정한다.
            _rectTransform.position = rectTransform.position + new Vector3(resolution.x, -resolution.y);
            var position = _rectTransform.position;

            // 설정 창의 너비와 높이 값.
            width = _rectTransform.rect.width * ratio;
            height = _rectTransform.rect.height * ratio;

            // 우측, 하단이 잘렸는지 여부를 판단한다.
            var isRightClipped = position.x + width > Screen.width;
            var isBottomClipped = position.y - height < 0f;

            _rectTransform.position = isRightClipped switch
            {
                // 오른쪽만 잘린 경우에는 슬롯의 좌하단 방향으로 표시한다.
                true when isBottomClipped == false => new Vector2(position.x - width - resolution.x, position.y),

                // 아래쪽만 잘린 경우에는 슬롯의 우상향 방향으로 표시한다.
                false when isBottomClipped => new Vector2(position.x, position.y + height + resolution.y),

                // 모두 잘린 경우에는 슬롯의 좌상단 방향으로 표시한다.
                true => new Vector2(position.x - width - resolution.x, position.y + height + resolution.y),

                // 잘리지 않은 경우에는 슬롯의 우하단 방향으로 표시한다.
                // 즉, 어떠한 방향 설정도 하지 않으면 된다.
                _ => _rectTransform.position
            };
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 아이템 설명 창에 포함될 아이템의 이름과 설명에 관한 정보.
        /// </summary>
        public ItemData Data
        {
            set
            {
                nameText.text = value.Name;
                descriptionText.text = value.Description;
            }
        }
    }
}
