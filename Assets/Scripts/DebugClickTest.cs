using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugClickTest : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            CellController cellController = button.GetComponent<CellController>();
            string hasCellController = cellController != null ? "Есть" : "Нет";

            TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
            Text legacyText = button.GetComponentInChildren<Text>();
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            var type = gameManager.GetType();
            var boardField = type.GetField("m_boardController",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var uiField = type.GetField("m_uiManager",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (boardField != null)
            {
                object boardValue = boardField.GetValue(gameManager);
            }

            if (uiField != null)
            {
                object uiValue = uiField.GetValue(gameManager);
            }
        }
    }

    [ContextMenu("Test Click")]
    void TestClick()
    {
        Button button = GameObject.Find("Cell_0")?.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.Invoke();
        }
    }
}
