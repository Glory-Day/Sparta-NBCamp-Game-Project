using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Backend.Object.UI
{
    public class ItemSlotView : MonoBehaviour
    {
        [Header("Image References")]
        [Tooltip("아이템 아이콘 이미지")]
        [SerializeField] private Image iconImage;

        [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
        [SerializeField] private Image highlightImage;

        [Tooltip("장착 여부 텍스트")]
        [SerializeField] private Image equippedMarkImage;

        [Header("Text References")]
        [Tooltip("아이템 개수 텍스트")]
        [SerializeField] private TextMeshProUGUI countText;

        [Header("Layout Settings")]
        [Tooltip("슬롯 내에서 아이콘과 슬롯 사이의 여백")]
        [SerializeField] private float padding = 1f;

        [Header("View Settings")]
        [Tooltip("하이라이트 이미지 알파 값")]
        [SerializeField] private float alpha = 0.5f;

        [Tooltip("하이라이트 소요 시간")]
        [SerializeField] private float duration = 0.2f;

        private GameObject _iconImageObject;
        private GameObject _highlightImageObject;
        private GameObject _equippedMarkImageObject;
        private GameObject _countTextObject;

        // 현재 하이라이트 알파값
        private float _alpha;

        private void Awake()
        {
            IconImageRectTransform = iconImage.rectTransform;
            HighlightImageRectTransform = highlightImage.rectTransform;

            _iconImageObject = IconImageRectTransform.gameObject;
            _countTextObject = countText.gameObject;
            _highlightImageObject = highlightImage.gameObject;
            _equippedMarkImageObject = equippedMarkImage.gameObject;

            // 피벗을 중앙으로 설정한다.
            IconImageRectTransform.pivot = new Vector2(0.5f, 0.5f);

            // 앵커를 좌상단으로 설정한다.
            IconImageRectTransform.anchorMin = Vector2.zero;
            IconImageRectTransform.anchorMax = Vector2.one;

            // 아이콘 이미지의 패딩을 설정한 값으로 조절한다.
            IconImageRectTransform.offsetMin = Vector2.one * padding;
            IconImageRectTransform.offsetMax = Vector2.one * -padding;

            // 아이콘 이미지와 하이라이트 이미지 크기가 동일하도록 설정한다.
            HighlightImageRectTransform.pivot = IconImageRectTransform.pivot;
            HighlightImageRectTransform.anchorMin = IconImageRectTransform.anchorMin;
            HighlightImageRectTransform.anchorMax = IconImageRectTransform.anchorMax;
            HighlightImageRectTransform.offsetMin = IconImageRectTransform.offsetMin;
            HighlightImageRectTransform.offsetMax = IconImageRectTransform.offsetMax;

            iconImage.raycastTarget = false;
            highlightImage.raycastTarget = false;

            // 아이콘 이미지들을 비활성화 한다.
            _iconImageObject.SetActive(false);
            _equippedMarkImageObject.SetActive(false);
            _highlightImageObject.SetActive(false);
        }

        /// <summary>
        /// 아이템 아이콘 이미지를 다른 슬롯으로 이동한다.
        /// 단, 슬롯에 이미 아이템 아이콘 이미지가 할당되어 있는 경우 치환한다.
        /// </summary>
        public void Move(ItemSlotView other)
        {
            if (other == null || other == this)
            {
                return;
            }

            var sprite = iconImage.sprite;

            // 대상에 아이템이 있으면 교환하고 없으면 이동한다.
            if (other.HasItem)
            {
                IconImage = other.iconImage.sprite;
            }
            else
            {
                Remove();
            }

            other.IconImage = sprite;
        }

        /// <summary>
        /// 슬롯에서 아이템을 제거한다.
        /// </summary>
        public void Remove()
        {
            iconImage.sprite = null;

            _iconImageObject.SetActive(false);
            _countTextObject.SetActive(false);
            _equippedMarkImageObject.SetActive(false);
        }

        /// <summary>
        /// 하이라이트 이미지 색상의 알파 값을 서서히 증가시킨다.
        /// </summary>
        private IEnumerator FadingIn()
        {
            StopCoroutine(nameof(FadingIn));

            _highlightImageObject.SetActive(true);

            while (_alpha <= alpha)
            {
                var color = highlightImage.color;
                highlightImage.color = new Color(color.r, color.g, color.b, _alpha);

                _alpha += alpha / duration * Time.deltaTime;

                yield return null;
            }
        }

        /// <summary>
        /// 하이라이트 이미지의 알파 값을 0%까지 서서히 감소시킨다.
        /// </summary>
        private IEnumerator FadingOut()
        {
            StopCoroutine(nameof(FadingOut));

            while (_alpha >= 0f)
            {
                var color = highlightImage.color;
                highlightImage.color = new Color(color.r, color.g, color.b, _alpha);

                _alpha -= alpha / duration * Time.deltaTime;

                yield return null;
            }

            _highlightImageObject.SetActive(false);
        }

        /// <summary>
        /// 슬롯의 인덱스 번호.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 아이템 개수에 대한 텍스트. (단, 개수가 1 이하일 경우 텍스트 미표시)
        /// </summary>
        public int Count
        {
            set
            {
                //if (!this.IsAccessible) return;

                if (HasItem && value > 1)
                {
                    _countTextObject.SetActive(true);
                }
                else
                {
                    _countTextObject.SetActive(false);
                }

                countText.text = value.ToString();
            }
        }

        /// <summary>
        /// 슬롯에 할당된 아이템 이미지.
        /// </summary>
        public Sprite IconImage
        {
            set
            {
                if (value != null)
                {
                    iconImage.sprite = value;

                    _iconImageObject.SetActive(true);
                }
                else
                {
                    Remove();
                }
            }
        }

        /// <summary>
        /// 슬롯이 아이템을 보유하고 있는지 여부.
        /// </summary>
        public bool HasItem => iconImage.sprite != null;

        //TODO: 해당 코드는 테스트 용도로 작성되어 있다.
        public bool IsEquipped
        {
            set
            {
                _equippedMarkImageObject.SetActive(value);
            }
        }

        /// <summary>
        /// 슬롯에 하이라이트 표시 여부.
        /// </summary>
        public bool IsHighlighted
        {
            set
            {
                StartCoroutine(value ? nameof(FadingIn) : nameof(FadingOut));
            }
        }

        public RectTransform IconImageRectTransform { get; private set; }

        public RectTransform HighlightImageRectTransform { get; private set; }
    }
}
