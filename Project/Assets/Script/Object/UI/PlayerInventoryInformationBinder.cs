using Backend.Object.Character.Player;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Script.Object.UI;
using Script.Object.UI.View;
using UnityEngine;

namespace Backend.Object.UI
{
    public class PlayerInventoryInformationBinder : MonoBehaviour
    {
        [Header("View Reference")]
        [SerializeField] private ItemInventoryView inventoryView;
        [SerializeField] private PointDifferenceTextView[] pointDifferenceTextView;
        [SerializeField] private TextView[] textView;
        [SerializeField] private ImageView imageView;
        [SerializeField] private ActiveView[] activeView;
        [SerializeField] private TextView[] statusTextView;

        private PointDifferenceTextPresenter[] _pointDifferenceTextPresenter;
        private BaseInventoryPresenter _inventoryPresenter;
        private PhysicalPointDifferenceTextPresenter _physicalPointDifferenceTextPresenter;
        private PhysicalDefenseDifferenceTextPresenter _physicalDefenceDifferenceTextPresenter;
        private MagicalPointDifferenceTextPresenter _magicalPointDifferenceTextPresenter;
        private MagicalDefenseDifferenceTextPresenter _magicalDefenceDifferenceTextPresenter;
        private ItemCountDifferenceTextPresenter _itemCountDifferenceTextPresenter;
        private LayoutGroupPresenter[] _layoutGroupPresenters;
        private StatusTextPresenter[] _statusTextPresenter;

        private ImagePresenter _imagePresenter;
        private TextPresenter[] _textPresenter;

        private Dispatcher _dispatcher = new();

        private void Init()
        {
            _pointDifferenceTextPresenter = new PointDifferenceTextPresenter[pointDifferenceTextView.Length];
            _layoutGroupPresenters = new LayoutGroupPresenter[activeView.Length];
            _textPresenter = new TextPresenter[textView.Length];
            _statusTextPresenter = new StatusTextPresenter[statusTextView.Length];
        }

        public void Bind(Inventory invModel, PlayerStatus statModel)
        {
            Init();

            _pointDifferenceTextPresenter[0] = PresenterFactory.Create<LifePointDifferenceTextPresenter>(pointDifferenceTextView[0], statModel, _dispatcher);
            _pointDifferenceTextPresenter[1] = PresenterFactory.Create<EndurancePointDifferenceTextPresenter>(pointDifferenceTextView[1], statModel, _dispatcher);

            _physicalPointDifferenceTextPresenter = PresenterFactory.Create<PhysicalPointDifferenceTextPresenter>(pointDifferenceTextView[2], statModel, _dispatcher);
            _magicalPointDifferenceTextPresenter = PresenterFactory.Create<MagicalPointDifferenceTextPresenter>(pointDifferenceTextView[3], statModel, _dispatcher);
            _physicalDefenceDifferenceTextPresenter = PresenterFactory.Create<PhysicalDefenseDifferenceTextPresenter>(pointDifferenceTextView[4], statModel, _dispatcher);
            _magicalDefenceDifferenceTextPresenter = PresenterFactory.Create<MagicalDefenseDifferenceTextPresenter>(pointDifferenceTextView[5], statModel, _dispatcher);
            _itemCountDifferenceTextPresenter = PresenterFactory.Create<ItemCountDifferenceTextPresenter>(pointDifferenceTextView[6], invModel, _dispatcher);

            _layoutGroupPresenters[0] = PresenterFactory.Create<LayoutGroupPresenter>(activeView[0], invModel, _dispatcher);
            _layoutGroupPresenters[1] = PresenterFactory.Create<LayoutGroupPresenter>(activeView[1], invModel, _dispatcher);

            _textPresenter[0] = PresenterFactory.Create<ItemNamePresenter>(textView[0], invModel, _dispatcher);
            _textPresenter[1] = PresenterFactory.Create<ItemDescriptPresenter>(textView[1], invModel, _dispatcher);

            //StatusPanel
            for (int i = 0; i < statusTextView.Length; i++)
            {
                _statusTextPresenter[i] = PresenterFactory.Create<StatusTextPresenter>(statusTextView[i], statModel, _dispatcher);
            }

            _imagePresenter = PresenterFactory.Create<ImagePresenter>(imageView, invModel, _dispatcher);
            _inventoryPresenter = PresenterFactory.Create<ItemInventoryPresenter>(inventoryView, invModel, _dispatcher);
        }

        public void OnDestroy()
        {
            for (int i = 0; i < _pointDifferenceTextPresenter.Length; i++)
            {
                _pointDifferenceTextPresenter[i]?.Clear();
            }
            for (int i = 0; i < _layoutGroupPresenters.Length; i++)
            {
                _layoutGroupPresenters[i].Clear();
            }
            for (int i = 0; i < _textPresenter.Length; i++)
            {
                _textPresenter[i].Clear();
            }

            _inventoryPresenter.Clear();
            _physicalPointDifferenceTextPresenter.Clear();
            _physicalDefenceDifferenceTextPresenter.Clear();
            _magicalPointDifferenceTextPresenter.Clear();
            _magicalDefenceDifferenceTextPresenter.Clear();
            _itemCountDifferenceTextPresenter.Clear();
            _imagePresenter.Clear();
        }
    }
}
