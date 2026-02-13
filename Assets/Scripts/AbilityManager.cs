using System.Collections.Generic;
using UnityEngine;
using Abilities;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance { get; private set; }

    [SerializeField] private AbilitySlot[] xSlots;
    [SerializeField] private AbilitySlot[] oSlots;

    private Dictionary<string, AbilityType> activeBonus = new();
    private Dictionary<string, AbilityType> chargedBonus = new();
    private Dictionary<string, AbilitySlot> centerSlot = new();

    private void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);
    }

    private void Start()
    {
        AssignSlotTypes("X");
        AssignSlotTypes("O");
    }

    private void AssignSlotTypes(string player)
    {
        var slots = (player == "X") ? xSlots : oSlots;
        foreach (var slot in slots)
        {
            if (slot == null) continue;

            if (slot.Position == "Center")
            {
                slot.Ability = AbilityType.None;
                centerSlot[player] = slot;
                slot.SetState(AbilitySlotState.Active, true);
            }
            else
            {
                slot.Ability = slot.Position switch
                {
                    "Up" => AbilityType.DoubleMove,
                    "Down" => AbilityType.Recon,
                    "Left" => AbilityType.Shoot,
                    "Right" => AbilityType.None,
                    _ => AbilityType.None
                };
                slot.SetState(AbilitySlotState.Inactive, false);
            }
            slot.Setup(player, slot.Position, slot.Ability);
        }
    }

    public void StartTurn(string player)
    {
        if (chargedBonus.TryGetValue(player, out var bonus) && bonus != AbilityType.None)
        {
            activeBonus[player] = bonus;
            chargedBonus[player] = AbilityType.None;
            SetSlotState(player, bonus, AbilitySlotState.Active, false);
        }
        else activeBonus[player] = AbilityType.None;

        if (centerSlot.TryGetValue(player, out var center))
            center.SetState(AbilitySlotState.Active, true);

        var slots = (player == "X") ? xSlots : oSlots;
        foreach (var slot in slots)
        {
            if (slot.Position == "Center") continue;
            if (slot.Ability == activeBonus[player])
                slot.SetState(AbilitySlotState.Active, false);
            else if (slot.Ability != AbilityType.None)
                slot.SetState(AbilitySlotState.Inactive, true);
            else
                slot.SetState(AbilitySlotState.Inactive, false);
        }
    }

    public void OnAbilitySlotClicked(string player, AbilitySlot slot)
    {
        if (!centerSlot.TryGetValue(player, out var center) || center.State != AbilitySlotState.Active) return;
        if (slot.Ability == activeBonus[player] || slot.Ability == AbilityType.None) return;

        chargedBonus[player] = slot.Ability;
        slot.SetState(AbilitySlotState.Charging, false);
        center.SetState(AbilitySlotState.Inactive, false);

        var slots = (player == "X") ? xSlots : oSlots;
        foreach (var s in slots)
            if (s != slot && s.Position != "Center")
                s.SetInteractable(false);
    }

    public AbilityType ApplyBonus(string player)
    {
        if (activeBonus.TryGetValue(player, out var bonus) && bonus != AbilityType.None)
        {
            SetSlotState(player, bonus, AbilitySlotState.Inactive, false);
            activeBonus[player] = AbilityType.None;
            return bonus;
        }
        return AbilityType.None;
    }

    private void SetSlotState(string player, AbilityType ability, AbilitySlotState state, bool interactable)
    {
        var slots = (player == "X") ? xSlots : oSlots;
        foreach (var s in slots)
            if (s.Ability == ability) { s.SetState(state, interactable); break; }
    }

    public void ResetAll()
    {
        foreach (var s in xSlots) s.ResetSlot();
        foreach (var s in oSlots) s.ResetSlot();
        activeBonus.Clear();
        chargedBonus.Clear();
        if (centerSlot.ContainsKey("X")) centerSlot["X"].SetState(AbilitySlotState.Active, true);
        if (centerSlot.ContainsKey("O")) centerSlot["O"].SetState(AbilitySlotState.Active, true);
    }
}