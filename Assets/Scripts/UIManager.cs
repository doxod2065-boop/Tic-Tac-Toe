using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_statusText;

    public void UpdateStatus(bool isXTurn)
    {
        if (m_statusText != null)
            m_statusText.text = isXTurn ? "Ход: Крестики (X)" : "Ход: Нолики (O)";
    }

    public void ShowGameResult(string message)
    {
        if (m_statusText != null)
            m_statusText.text = message;
    }
}
