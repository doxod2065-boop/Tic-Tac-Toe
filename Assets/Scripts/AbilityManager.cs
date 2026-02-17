using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager m_instance { get; private set; }

    [Header("Слоты игроков")]
    public AbilitySlot[] m_xSlots;
    public AbilitySlot[] m_oSlots;

    private Dictionary<string, AbilityType> m_activeBonus = new Dictionary<string, AbilityType>();
    private Dictionary<string, AbilityType> m_chargedBonus = new Dictionary<string, AbilityType>();
    private Dictionary<string, AbilitySlot> m_centerSlot = new Dictionary<string, AbilitySlot>();
    private bool m_initialized = false;

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (m_initialized) return;

        AbilitySlot[] allSlots = FindObjectsByType<AbilitySlot>(FindObjectsSortMode.None);
        List<AbilitySlot> xList = new List<AbilitySlot>();
        List<AbilitySlot> oList = new List<AbilitySlot>();

        foreach (var slot in allSlots)
        {
            if (slot.m_owner == "X")
                xList.Add(slot);
            else if (slot.m_owner == "O")
                oList.Add(slot);
        }

        m_xSlots = xList.ToArray();
        m_oSlots = oList.ToArray();

        m_activeBonus["X"] = AbilityType.None;
        m_activeBonus["O"] = AbilityType.None;
        m_chargedBonus["X"] = AbilityType.None;
        m_chargedBonus["O"] = AbilityType.None;

        AssignSlotTypes("X");
        AssignSlotTypes("O");

        m_initialized = true;
    }

    void AssignSlotTypes(string player)
    {
        AbilitySlot[] slots = (player == "X") ? m_xSlots : m_oSlots;
        foreach (var slot in slots)
        {
            if (slot.m_position == "Center")
            {
                slot.m_abilityType = AbilityType.None;
                m_centerSlot[player] = slot;
                slot.SetState(AbilitySlotState.Active);
                slot.SetInteractable(true);
            }
            else
            {
                switch (slot.m_position)
                {
                    case "Up":
                        slot.m_abilityType = AbilityType.DoubleMove;
                        break;
                    case "Down":
                        slot.m_abilityType = AbilityType.Recon;
                        break;
                    case "Left":
                        slot.m_abilityType = AbilityType.Shoot;
                        break;
                    case "Right":
                        slot.m_abilityType = AbilityType.None; // задел на будущее
                        break;
                }
                slot.SetState(AbilitySlotState.Inactive);
                slot.SetInteractable(false);
            }
            slot.Setup(slot.m_owner, slot.m_position, slot.m_abilityType);
        }
    }

    public void StartTurn(string player)
    {
        if (!m_initialized) Initialize();

        if (m_chargedBonus[player] != AbilityType.None)
        {
            m_activeBonus[player] = m_chargedBonus[player];
            m_chargedBonus[player] = AbilityType.None;
            SetSlotStateByAbility(player, m_activeBonus[player], AbilitySlotState.Active, true);
        }
        else
        {
            m_activeBonus[player] = AbilityType.None;
        }

        if (m_centerSlot.ContainsKey(player) && m_centerSlot[player] != null)
        {
            m_centerSlot[player].SetState(AbilitySlotState.Active);
            m_centerSlot[player].SetInteractable(true);
        }

        AbilitySlot[] slots = (player == "X") ? m_xSlots : m_oSlots;
        foreach (var slot in slots)
        {
            if (slot.m_position == "Center") continue;
            if (slot.m_abilityType == m_activeBonus[player])
            {
                slot.SetState(AbilitySlotState.Active);
                slot.SetInteractable(false);
            }
            else if (slot.m_abilityType != AbilityType.None)
            {
                slot.SetState(AbilitySlotState.Inactive);
                slot.SetInteractable(true);
            }
            else
            {
                slot.SetState(AbilitySlotState.Inactive);
                slot.SetInteractable(false);
            }
        }
    }

    public void OnAbilitySlotClicked(string player, AbilitySlot clickedSlot)
    {
        if (!m_centerSlot.ContainsKey(player) || m_centerSlot[player].m_state != AbilitySlotState.Active)
        {
            return;
        }

        if (clickedSlot.m_abilityType == m_activeBonus[player])
        {
            return;
        }

        if (clickedSlot.m_abilityType == AbilityType.None)
        {
            return;
        }

        m_chargedBonus[player] = clickedSlot.m_abilityType;

        clickedSlot.SetState(AbilitySlotState.Charging);
        clickedSlot.SetInteractable(false);

        m_centerSlot[player].SetState(AbilitySlotState.Inactive);
        m_centerSlot[player].SetInteractable(false);

        AbilitySlot[] slots = (player == "X") ? m_xSlots : m_oSlots;
        foreach (var slot in slots)
        {
            if (slot == clickedSlot) continue;
            if (slot.m_position == "Center") continue;
            slot.SetInteractable(false);
        }
    }

    public AbilityType ApplyBonus(string player)
    {
        AbilityType bonus = m_activeBonus[player];
        if (bonus != AbilityType.None)
        {
            SetSlotStateByAbility(player, bonus, AbilitySlotState.Inactive, false);
            m_activeBonus[player] = AbilityType.None;
        }
        return bonus;
    }

    private void SetSlotStateByAbility(string player, AbilityType ability, AbilitySlotState state, bool interactable)
    {
        AbilitySlot[] slots = (player == "X") ? m_xSlots : m_oSlots;
        foreach (var slot in slots)
        {
            if (slot.m_abilityType == ability)
            {
                slot.SetState(state);
                slot.SetInteractable(interactable);
                break;
            }
        }
    }

    public void ResetAbilities()
    {
        foreach (var slot in m_xSlots) { slot.ResetSlot(); }
        foreach (var slot in m_oSlots) { slot.ResetSlot(); }

        m_activeBonus["X"] = AbilityType.None;
        m_activeBonus["O"] = AbilityType.None;
        m_chargedBonus["X"] = AbilityType.None;
        m_chargedBonus["O"] = AbilityType.None;

        if (m_centerSlot.ContainsKey("X"))
        {
            m_centerSlot["X"].SetState(AbilitySlotState.Active);
            m_centerSlot["X"].SetInteractable(true);
        }
        if (m_centerSlot.ContainsKey("O"))
        {
            m_centerSlot["O"].SetState(AbilitySlotState.Active);
            m_centerSlot["O"].SetInteractable(true);
        }
    }
}
