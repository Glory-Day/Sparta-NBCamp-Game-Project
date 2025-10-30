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
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private PointDifferenceTextView[] pointDifferenceTextView;
        [SerializeField] private TextView[] textView;
        [SerializeField] private ImageView imageView;
        [SerializeField] private ActiveView[] activeView;

        private PointDifferenceTextPresenter[] _pointDifferenceTextPresenter;
        private InventoryPresenter _inventoryPresenter;
        private PhysicalPointDifferenceTextPresenter _physicalPointDifferenceTextPresenter;
        private PhysicalDefenceDifferenceTextPresenter _physicalDefenceDifferenceTextPresenter;
        private MagicalPointDifferenceTextPresenter _magicalPointDifferenceTextPresenter;
        private MagicalDefenceDifferenceTextPresenter _magicalDefenceDifferenceTextPresenter;
        private ItemCountDifferenceTextPresenter _itemCountDifferenceTextPresenter;
        private LayoutGroupPresenter[] _layoutGroupPresenters;

        private ImagePresenter _imagePresenter;
        private TextPresenter[] _textPresenter;

        private Dispatcher _dispatcher = new();

        public void Bind(Inventory invModel, PlayerStatus statModel)
        {
            _pointDifferenceTextPresenter = new PointDifferenceTextPresenter[pointDifferenceTextView.Length];
            _layoutGroupPresenters = new LayoutGroupPresenter[activeView.Length];
            _textPresenter = new TextPresenter[textView.Length];

            _pointDifferenceTextPresenter[0] = PresenterFactory.Create<LifePointDifferenceTextPresenter>(pointDifferenceTextView[0], statModel, 0, _dispatcher);
            _pointDifferenceTextPresenter[1] = PresenterFactory.Create<EndurancePointDifferenceTextPresenter>(pointDifferenceTextView[1], statModel, 1, _dispatcher);

            _physicalPointDifferenceTextPresenter = PresenterFactory.Create<PhysicalPointDifferenceTextPresenter>(pointDifferenceTextView[2], statModel, _dispatcher);
            _magicalPointDifferenceTextPresenter = PresenterFactory.Create<MagicalPointDifferenceTextPresenter>(pointDifferenceTextView[3], statModel, _dispatcher);
            _physicalDefenceDifferenceTextPresenter = PresenterFactory.Create<PhysicalDefenceDifferenceTextPresenter>(pointDifferenceTextView[4], statModel, _dispatcher);
            _magicalDefenceDifferenceTextPresenter = PresenterFactory.Create<MagicalDefenceDifferenceTextPresenter>(pointDifferenceTextView[5], statModel, _dispatcher);
            _itemCountDifferenceTextPresenter = PresenterFactory.Create<ItemCountDifferenceTextPresenter>(pointDifferenceTextView[6], invModel, _dispatcher);

            _layoutGroupPresenters[0] = PresenterFactory.Create<ConsumeLayoutGroupPresenter>(activeView[0], invModel, _dispatcher);
            _layoutGroupPresenters[1] = PresenterFactory.Create<EquipmentLayoutGroupPresenter>(activeView[1], invModel, _dispatcher);

            _textPresenter[0] = PresenterFactory.Create<ItemNamePresenter>(textView[0], invModel, _dispatcher);
            _textPresenter[1] = PresenterFactory.Create<ItemDescriptPresenter>(textView[1], invModel, _dispatcher);

            _imagePresenter = PresenterFactory.Create<ImagePresenter>(imageView, invModel, _dispatcher);
            _inventoryPresenter = PresenterFactory.Create<InventoryPresenter>(inventoryView, invModel, _dispatcher);
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
