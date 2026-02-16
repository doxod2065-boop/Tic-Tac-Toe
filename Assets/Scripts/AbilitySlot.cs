using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum AbilityType
{
    None,
    DoubleMove,
    Recon,
    Shoot
}

public enum AbilitySlotState
{
    Inactive,
    Charging,
    Active
}

public class AbilitySlot : MonoBehaviour
{
    [Header("Принадлежность")]
    public string m_owner = "X";
    public string m_position;

    [Header("Тип бонуса")]
    public AbilityType m_abilityType = AbilityType.None;

    [Header("Компоненты")]
    [SerializeField] private Button m_button;
    [SerializeField] private TextMeshProUGUI m_label;
    [SerializeField] private Image m_background;

    public AbilitySlotState m_state = AbilitySlotState.Inactive;

    private Color defaultColor;
    private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    private Color chargingColor = new Color(1f, 0.92f, 0.016f, 0.7f);
    private Color activeColor = new Color(0.2f, 1f, 0.2f, 0.8f);

    void Start()
    {
        if (m_button == null) m_button = GetComponent<Button>();
        if (m_label == null) m_label = GetComponentInChildren<TextMeshProUGUI>();
        if (m_background == null) m_background = GetComponent<Image>();
        if (m_background != null) defaultColor = m_background.color;

        m_button.onClick.AddListener(OnSlotClick);
        UpdateVisual();
    }

    public void Setup(string ownerPlayer, string pos, AbilityType type)
    {
        m_owner = ownerPlayer;
        m_position = pos;
        m_abilityType = type;
        gameObject.name = $"{m_owner}_Slot_{m_position}";
        if (m_label != null) m_label.text = GetAbilityDisplayName();
    }

    private string GetAbilityDisplayName()
    {
        if (m_position == "Center") return "Заряд";
        switch (m_abilityType)
        {
            case AbilityType.DoubleMove: return "+1 ход";
            case AbilityType.Recon: return "Поиск";
            case AbilityType.Shoot: return "Выстрел";
            default: return "?";
        }
    }

    void OnSlotClick()
    {
        AbilityManager.m_instance?.OnAbilitySlotClicked(m_owner, this);
    }

    public void SetState(AbilitySlotState newState)
    {
        m_state = newState;
        UpdateVisual();
    }

    public void SetInteractable(bool value)
    {
        if (m_button != null) m_button.interactable = value;
    }

    private void UpdateVisual()
    {
        if (m_background == null) return;

        switch (m_state)
        {
            case AbilitySlotState.Inactive:
                m_background.color = inactiveColor;
                break;
            case AbilitySlotState.Charging:
                m_background.color = chargingColor;
                break;
            case AbilitySlotState.Active:
                m_background.color = activeColor;
                break;
        }
    }

    public void ResetSlot()
    {
        m_state = AbilitySlotState.Inactive;
        UpdateVisual();
        SetInteractable(false);
        if (m_label != null && m_position != "Center")
            m_label.text = GetAbilityDisplayName();
    }
}