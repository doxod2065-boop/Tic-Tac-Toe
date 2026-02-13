using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Abilities;

public enum AbilitySlotState { Inactive, Charging, Active }

public class AbilitySlot : MonoBehaviour
{
    [field: SerializeField] public string Owner { get; private set; }
    [field: SerializeField] public string Position { get; private set; }
    [field: SerializeField] public AbilityType Ability { get; set; }

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;

    public AbilitySlotState State { get; private set; }

    private Color defaultColor;
    private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    private Color chargingColor = new Color(1f, 0.92f, 0.016f, 0.7f);
    private Color activeColor = new Color(0.2f, 1f, 0.2f, 0.8f);

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (label == null) label = GetComponentInChildren<TextMeshProUGUI>();
        if (background == null) background = GetComponent<Image>();
        if (background != null) defaultColor = background.color;
        button?.onClick.AddListener(OnClick);
    }

    public void Setup(string owner, string pos, AbilityType ability)
    {
        Owner = owner;
        Position = pos;
        Ability = ability;
        gameObject.name = $"{owner}_{pos}";
        if (label != null) label.text = GetDisplayName();
    }

    private string GetDisplayName()
    {
        if (Position == "Center") return "Энергия";
        return Ability switch
        {
            AbilityType.DoubleMove => "+1",
            AbilityType.Recon => "Поиск",
            AbilityType.Shoot => "Выстрел",
            _ => "?"
        };
    }

    private void OnClick() => AbilityManager.Instance?.OnAbilitySlotClicked(Owner, this);

    public void SetState(AbilitySlotState state, bool interactable)
    {
        State = state;
        UpdateVisual();
        SetInteractable(interactable);
    }

    private void UpdateVisual()
    {
        if (background == null) return;
        background.color = State switch
        {
            AbilitySlotState.Inactive => inactiveColor,
            AbilitySlotState.Charging => chargingColor,
            AbilitySlotState.Active => activeColor,
            _ => defaultColor
        };
    }

    public void SetInteractable(bool value)
    {
        if (button != null) button.interactable = value;
    }

    public void ResetSlot()
    {
        SetState(AbilitySlotState.Inactive, false);
        if (label != null && Position != "Center") label.text = GetDisplayName();
    }
}