using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class Root : MonoBehaviour
{
    [SerializeField]
    private CameraController _camera;

    [SerializeField]
    private Vector3 _defaultCameraPosition;
    [SerializeField]
    private Vector3 _defaultCameraTarget;

    private VisualElement _menu;
    private Button _backButton;
    private Label _costMeter;
    private Configurator[] _submenus;
    private Configurator _currentSubmenu;
    private List<VisualElement> _submenuLinks = new List<VisualElement>();
    private Label _summaryLabel;
    private Toggle _summaryToggle;

    private Car[] _cars;
    private Car _currentCar;
    private GroupBox _carInfo;
    private Label _carData;

    private static VisualTreeAsset template;

    public int Cost => _currentCar.BaseCost + _submenus.Select(m => m.Cost).Sum();
    public CarTraits Traits => _currentCar.GetComponentsInChildren<IPart>().Select(p => p.Modifiers).Aggregate(_currentCar.baseTraits, (acc, x) => acc + x);

    void Awake() {
        var uiRoot = GetComponent<UIDocument>().rootVisualElement;
        _menu = uiRoot[0];

        //load the menu template
        template ??= Resources.Load<VisualTreeAsset>("SubmenuTemplate");

        //set up 'back' button. This always exists, but is invisible when at the top level.
        _backButton = uiRoot.Q<Button>("BackButton");
        _backButton.RegisterCallback<ClickEvent>((e) =>
        {
            RootMenu();
        });

        //set up the cost meter. Since all changes derive from user clicks, update this only when we get a click event.
        //this will lead to a few redundant recalculations but it shouldn't be a huge issue. I think.
        //this should fire when the event has bubbled up to the top, i.e., after any ConfigurableParts.
        _costMeter = uiRoot.Q<Label>("TotalCost");
        uiRoot.RegisterCallback<ClickEvent>((e) => {
            UpdateCost();
            UpdateData();
        });

        //load the cars, and set up the current car. This will be managed by the dropdown.
        _cars = GetComponentsInChildren<Car>();
        _currentCar = _cars[0];
        _carInfo = uiRoot.Q<GroupBox>("CarInfo");
        _carData = uiRoot.Q<Label>("CarData");

        BindCarInfo();

        var _carDropdown = uiRoot.Q<DropdownField>("CarDropdown");
        _carDropdown.choices = new List<string>(_cars.Select(c => c.Name));
        _carDropdown.value = _carDropdown.choices[0];
        _carDropdown.RegisterCallback<ChangeEvent<string>>((e) => ChangeCar(_carDropdown.index));

        //load the submenus belonging to the current car
        _submenus = _currentCar.GetComponentsInChildren<Configurator>(true);

        //set up the 'show summary' button
        var _summaryBox = uiRoot.Q<ScrollView>("Summary");
        _summaryLabel = new Label();
        _summaryBox.Add(_summaryLabel);
        _summaryToggle = uiRoot.Q<Toggle>("ShowSummaryToggle");
        _summaryToggle.RegisterCallback<ChangeEvent<bool>>((e) =>
        {
            if (e.newValue) {
                _summaryBox.style.display = DisplayStyle.Flex;
                _summaryLabel.text = Summary();
            } else {
                _summaryBox.style.display = DisplayStyle.None;
            }
        });
    }

    private void UpdateCost() {
        _costMeter.text = $"{Cost} teeth";
    }

    private void UpdateData() {
        _carData.text = Car.ListTraits(Traits);
    }

    private void RootMenu() {
        if (_currentSubmenu != null) {
            _currentSubmenu.enabled = false;
        }
        _currentSubmenu = null;
        MoveCamera(_defaultCameraPosition, _defaultCameraTarget);

        foreach (Configurator sub in _submenus) {
            VisualElement ve = template.Instantiate();
            SerializedObject serializedSubmenu = new SerializedObject(sub);
            ve.Bind(serializedSubmenu);

            ve.RegisterCallback<ClickEvent>((e) => NavigateToSub(sub));
            _menu.Add(ve);
            _submenuLinks.Add(ve);
            _backButton.visible = false;
        }
    }

    private void HideRootMenu() {
        foreach (VisualElement s in _submenuLinks)
        {
            _menu.Remove(s);
        }
        _submenuLinks.Clear();
    }

    private void NavigateToSub(Configurator s)
    {
        HideRootMenu();
        _backButton.visible = true;
        _currentSubmenu = s;
        s.enabled = true;
    }

    void Start() {
        ChangeCar(0);
    }

    public void MoveCamera(Vector3 newPosition, Vector3 newTarget) {
        _camera.TargetPosition = newPosition;
        _camera.NewLookTarget = newTarget;

    }

    public void ChangeCar(int newIndex) {
        //get rid of everything to do with this car
        //if we're in a submenu, kill it
        if (_currentSubmenu != null) {
            _currentSubmenu.enabled = false;
        }
        //if we're at the root, get rid of that
        HideRootMenu();

        //load the new car. Assume the index is in bounds.
        _currentCar = _cars[newIndex];
        _submenus = _currentCar.GetComponentsInChildren<Configurator>(true);
        BindCarInfo();
        UpdateCost();
        UpdateData();

        _defaultCameraPosition = _currentCar.DefaultCameraPosition;
        _defaultCameraTarget = _currentCar.DefaultCameraTarget;

        //draw the new root menu
        RootMenu();
    }

    public void BindCarInfo() {
        _carInfo.Bind(new SerializedObject(_currentCar));
    }

    void OnDisable() {
        HideRootMenu();
    }

    public string Summary() {
        List<string> parts = new List<string>(_currentCar.GetComponentsInChildren<IPart>(false).Select(p => p.Summary()));
        return $@"Car: {_currentCar.Name} - {_currentCar.BaseCost}
Parts:
{string.Join<string>(System.Environment.NewLine, parts)}
Total:
{Cost} teeth";
    }
}
