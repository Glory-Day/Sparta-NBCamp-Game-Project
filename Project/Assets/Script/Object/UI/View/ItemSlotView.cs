using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Backend.Object.UI
{
    public class ItemSlotView : MonoBehaviour
    {
        [Header("Component References")]
        [Tooltip("아이템 아이콘 이미지")]
        [SerializeField] private Image iconImage;

        [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
        [SerializeField] private Image highlightImage;

        [Tooltip("아이템 개수 텍스트")]
        [SerializeField] private TextMeshProUGUI amountText;

        [Tooltip("장착 여부 텍스트")]
        [SerializeField] private TextMeshProUGUI equipmentText;

        [Header("Layout Settings")]
        [Tooltip("슬롯 내에서 아이콘과 슬롯 사이의 여백")]
        [SerializeField] private float padding = 1f;

        [Header("Highlight Settings")]
        [Tooltip("하이라이트 이미지 알파 값")]
        [SerializeField] private float alpha = 0.5f;

        [Tooltip("하이라이트 소요 시간")]
        [SerializeField] private float duration = 0.2f;

        // 비활성화된 슬롯의 색상
        private static readonly Color DisabledSlotImageColor = new (0.2f, 0.2f, 0.2f, 0.5f);
        // 비활성화된 아이콘 색상
        private static readonly Color DisabledIconImageColor = new (0.5f, 0.5f, 0.5f, 0.5f);

        private Image _slotImage;

        private GameObject _iconObject;
        private GameObject _amountTextObject;
        private GameObject _highlightImageObject;
        private GameObject _equipmentTextObject;

        // 현재 하이라이트 알파값
        private float _alpha;

        // 슬롯 접근가능 여부
        private bool _isSlotAccessible = true;
        // 아이템 접근가능 여부
        private bool _isItemAccessible = true;

        private void Awake()
        {
            _slotImage = GetComponent<Image>();
            SlotImageRectTransform = GetComponent<RectTransform>();
            IconImageRectTransform = iconImage.rectTransform;
            HighlightImageRectTransform = highlightImage.rectTransform;

            _iconObject = IconImageRectTransform.gameObject;
            _amountTextObject = amountText.gameObject;
            _highlightImageObject = highlightImage.gameObject;
            _equipmentTextObject = equipmentText.gameObject;

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
            _iconObject.SetActive(false);
            _equipmentTextObject.SetActive(false);
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

            if (IsAccessible == false || other.IsAccessible == false)
            {
                return;
            }

            var sprite = iconImage.sprite;

            // 대상에 아이템이 있으면 교환한다. 없으면 이동한다.
            if (other.HasItem)
            {
                ItemImage = other.iconImage.sprite;
            }
            else
            {
                Remove();
            }

            other.ItemImage = sprite;
        }

        /// <summary>
        /// 슬롯에서 아이템을 제거한다.
        /// </summary>
        public void Remove()
        {
            iconImage.sprite = null;

            _iconObject.SetActive(false);
            _amountTextObject.SetActive(false);
            _equipmentTextObject.SetActive(false);
        }

        /// <summary>
        /// 하이라이트 이미지 색상의 알파 값을 서서히 증가시킨다.
        /// </summary>
        private IEnumerator HighlightFadeInRoutine()
        {
            StopCoroutine(nameof(HighlightFadeOutRoutine));

            _highlightImageObject.SetActive(true);

            float unit = alpha / duration;

            while (_alpha <= alpha)
            {
                var color = highlightImage.color;

                highlightImage.color = new Color(color.r, color.g, color.b, _alpha);

                _alpha += unit * Time.deltaTime;

                yield return null;
            }
        }

        /// <summary> 하이라이트 알파값 0%까지 서서히 감소 </summary>
        private IEnumerator HighlightFadeOutRoutine()
        {
            StopCoroutine(nameof(HighlightFadeInRoutine));

            float unit = alpha / duration;

            while (_alpha >= 0f)
            {
                var color = highlightImage.color;

                highlightImage.color = new Color(color.r, color.g, color.b, _alpha);

                _alpha -= unit * Time.deltaTime;

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
        public int Amount
        {
            set
            {
                //if (!this.IsAccessible) return;

                if (HasItem && value > 1)
                {
                    _amountTextObject.SetActive(true);
                }
                else
                {
                    _amountTextObject.SetActive(false);
                }

                amountText.text = value.ToString();
            }
        }

        /// <summary>
        /// 슬롯에 할당된 아이템 이미지.
        /// </summary>
        public Sprite ItemImage
        {
            set
            {
                if (value != null)
                {
                    iconImage.sprite = value;

                    _iconObject.SetActive(true);
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

        /// <summary>
        /// 접근 가능한 슬롯인지 여부.
        /// </summary>
        public bool IsAccessible => _isSlotAccessible && _isItemAccessible;

        /// <summary>
        /// 슬롯 자체의 활성화/비활성화 여부.
        /// </summary>
        public bool IsSlotAccessible
        {
            set
            {
                // 중복 처리는 지양
                if (_isSlotAccessible == value)
                {
                    return;
                }

                if (value)
                {
                    _slotImage.color = Color.black;
                }
                else
                {
                    _slotImage.color = DisabledSlotImageColor;

                    _iconObject.SetActive(false);
                    _amountTextObject.SetActive(false);
                    _equipmentTextObject.SetActive(false);
                }

                _isSlotAccessible = value;
            }
        }

        /// <summary>
        /// 아이템 활성화/비활성화 여부.
        /// </summary>
        public bool IsItemAccessible
        {
            set
            {
                // 중복 처리는 제외한다.
                if (_isItemAccessible == value)
                {
                    return;
                }

                if (value)
                {
                    iconImage.color = Color.white;
                    amountText.color = Color.white;
                }
                else
                {
                    iconImage.color = DisabledIconImageColor;
                    amountText.color = DisabledIconImageColor;
                }

                _isItemAccessible = value;
            }
        }

        //TODO: 해당 코드는 테스트 용도로 작성되어 있다.
        public bool IsEquipped
        {
            set
            {
                print(value);

                _equipmentTextObject.SetActive(value);
            }
        }

        /// <summary>
        /// 슬롯에 하이라이트 표시 여부.
        /// </summary>
        public bool IsHighlighted
        {
            set
            {
                if (IsAccessible == false)
                {
                    return;
                }

                StartCoroutine(value ? nameof(HighlightFadeInRoutine) : nameof(HighlightFadeOutRoutine));
            }
        }

        public RectTransform SlotImageRectTransform { get; private set; }

        public RectTransform IconImageRectTransform { get; private set; }

        public RectTransform HighlightImageRectTransform { get; private set; }
    }
}
