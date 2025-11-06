using System.Collections;
using System.Collections.Generic;
using Backend.Object.Character.Player;
using Backend.Object.UI;
using Backend.Object.UI.Presenter;
using Backend.Object.UI.View;
using Backend.Util.Presentation;
using Script.Object.UI;
using Script.Object.UI.View;
using UnityEngine;

public class PlayerEquipmentInformationBinder : MonoBehaviour
{
    #region ViewReference
    [Header("Main Panel Reference")]
    [SerializeField] private TextView mainTextView;
    [SerializeField] private EquipmentView mainPlayerInventoryView;
    [SerializeField] private EquipmentView[] mainEquipInventoryView;

    [Header("Description Panel Reference")]
    [SerializeField] private ImageView descriptImageView;
    [SerializeField] private TextView descriptConsumeTextView;
    [SerializeField] private PointDifferenceTextView[] descriptConsumeDifferenceTextView;
    [SerializeField] private PointDifferenceTextView[] descriptEquipDifferenceTextView;

    [Header("Status Panel Reference")]
    [SerializeField] private StatusTextView[] statusTextView;

    [Header("Layout Panel Reference")]
    [SerializeField] private ActiveView[] layoutView;

    [Header("Popup Panel Reference")]
    [SerializeField] private ButtonView buttonView;

    private TextPresenter _mainTextPresenter;
    private EquipmentInventoryPresenter[] _mainEquipInventoryPresenter;
    private PlayerInventoryPresenter _mainPlayerInventoryPresenter;

    private ImagePresenter _descriptImagePresenter;
    private TextPresenter _descriptConsumeTextPresenter;
    private PointDifferenceTextPresenter[] _descriptConsumeDifferenceTextPresenter;
    private PointDifferenceTextPresenter[] _descriptEquipDifferenceTextPresenter;

    private StatusTextPresenter[] _statusTextPresenter;

    private LayoutGroupPresenter[] _layoutPresenter;

    private ButtonPresenter _buttonPresenter;
    #endregion

    private Dispatcher _dispatcher = new();
    private Inventory[] _inventory; //각 장비창, 사용아이템들의 인벤토리 리스트
    private void Init()
    {
        _mainEquipInventoryPresenter = new EquipmentInventoryPresenter[mainEquipInventoryView.Length];
        _descriptConsumeDifferenceTextPresenter = new PointDifferenceTextPresenter[descriptConsumeDifferenceTextView.Length];
        _descriptEquipDifferenceTextPresenter = new PointDifferenceTextPresenter[descriptEquipDifferenceTextView.Length];
        _statusTextPresenter = new StatusTextPresenter[statusTextView.Length];
        _layoutPresenter = new LayoutGroupPresenter[layoutView.Length];
        _inventory = new Inventory[mainEquipInventoryView.Length];
    }

    public void Bind(Inventory[] model, PlayerStatus status) //전체 총괄 인벤토리
    {
        Init();

        //인벤토리 할당
        for (var i = 0; i < mainEquipInventoryView.Length; i++)
        {
            //var slotCapacity = mainEquipInventoryView[i]._horizontalSlotCount * mainEquipInventoryView[i]._verticalSlotCount;
            //_inventory[i] = new Inventory(slotCapacity, slotCapacity);
            _mainEquipInventoryPresenter[i] = PresenterFactory.Create<EquipmentInventoryPresenter>(mainEquipInventoryView[i], model[i + 1], _dispatcher);
        }
        _mainPlayerInventoryPresenter = PresenterFactory.Create<PlayerInventoryPresenter>(mainPlayerInventoryView, model[0], _dispatcher);

        //레이아웃 할당
        for (int i = 0; i < layoutView.Length; i++)
        {
            _layoutPresenter[i] = PresenterFactory.Create<LayoutGroupPresenter>(layoutView[i], model[0], _dispatcher);
        }

        //MainPanels
        _mainTextPresenter = PresenterFactory.Create<ItemNamePresenter>(mainTextView, model[0], _dispatcher);

        //DescriptPanel
        _descriptImagePresenter = PresenterFactory.Create<ImagePresenter>(descriptImageView, model[0], _dispatcher);

        //DescriptPanel Consume
        _descriptConsumeDifferenceTextPresenter[0] = PresenterFactory.Create<ItemCountDifferenceTextPresenter>(descriptConsumeDifferenceTextView[0], model[0], _dispatcher);

        //DescriptPanel Equip
        _descriptEquipDifferenceTextPresenter[0] = PresenterFactory.Create<PhysicalPointDifferenceTextPresenter>(descriptEquipDifferenceTextView[0], status, _dispatcher);
        _descriptEquipDifferenceTextPresenter[1] = PresenterFactory.Create<MagicalPointDifferenceTextPresenter>(descriptEquipDifferenceTextView[1], status, _dispatcher);
        _descriptEquipDifferenceTextPresenter[2] = PresenterFactory.Create<PhysicalDefenseDifferenceTextPresenter>(descriptEquipDifferenceTextView[2], status, _dispatcher);
        _descriptEquipDifferenceTextPresenter[3] = PresenterFactory.Create<MagicalDefenseDifferenceTextPresenter>(descriptEquipDifferenceTextView[3], status, _dispatcher);
        _descriptConsumeTextPresenter = PresenterFactory.Create<ItemDescriptPresenter>(descriptConsumeTextView, model[0], _dispatcher);

        //StatusPanel
        for (int i = 0; i < statusTextView.Length; i++)
        {
            _statusTextPresenter[i] = PresenterFactory.Create<StatusTextPresenter>(statusTextView[i], status, _dispatcher);
        }

        //PopupPanel
        _buttonPresenter = PresenterFactory.Create<ButtonPresenter>(buttonView, status, _dispatcher);


        //UIManager를 키고 끄고, Get으로 불러오기
    }

    private void OnDestroy()
    {

    }
}
