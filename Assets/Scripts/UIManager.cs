using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    public void UpdateTurn(bool isXTurn)
    {
        if (statusText != null)
            statusText.text = isXTurn ? "Ход: Крестики (X)" : "Ход: Нолики (O)";
    }

    public void ShowMessage(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}