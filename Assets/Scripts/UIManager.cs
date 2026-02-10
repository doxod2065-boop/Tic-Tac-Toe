using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Основные UI элементы")]
    [SerializeField] private TextMeshProUGUI gameStatusText;

    [Header("Альтернативный вариант: раздельные панели")]
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Настройки")]
    [SerializeField] private bool useSingleText = true;

    void Start()
    {
        AutoFindUIElements();

        if (useSingleText && gameStatusText != null)
        {
            gameStatusText.text = "Ход: Крестики (X)";
        }
        else if (!useSingleText)
        {
            if (statusPanel != null) statusPanel.SetActive(true);
            if (resultPanel != null) resultPanel.SetActive(false);
            if (statusText != null) statusText.text = "Ход: Крестики (X)";
        }
    }

    void AutoFindUIElements()
    {
        if (gameStatusText == null)
        {
            TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
            foreach (var text in allTexts)
            {
                string name = text.gameObject.name.ToLower();
                if (name.Contains("status") || name.Contains("info") || name.Contains("game"))
                {
                    gameStatusText = text;
                    break;
                }
            }
        }

        if (!useSingleText)
        {
            if (statusPanel == null)
                statusPanel = GameObject.Find("StatusPanel");
            if (resultPanel == null)
                resultPanel = GameObject.Find("ResultPanel");
            if (statusText == null)
                statusText = GameObject.Find("StatusText")?.GetComponent<TextMeshProUGUI>();
            if (resultText == null)
                resultText = GameObject.Find("ResultText")?.GetComponent<TextMeshProUGUI>();
        }
    }

    public void UpdateStatus(bool isXTurn)
    {
        string player = isXTurn ? "Крестики (X)" : "Нолики (O)";
        string message = $"Ход: {player}";

        if (useSingleText && gameStatusText != null)
        {
            gameStatusText.text = message;
        }
        else if (!useSingleText)
        {
            if (statusText != null) statusText.text = message;
            if (statusPanel != null) statusPanel.SetActive(true);
            if (resultPanel != null) resultPanel.SetActive(false);
        }
    }

    public void ShowGameResult(string resultMessage)
    {

        if (useSingleText && gameStatusText != null)
        {
            string displayMessage = resultMessage;
            if (resultMessage.Contains("Победили X"))
                displayMessage = resultMessage;
            else if (resultMessage.Contains("Победили O"))
                displayMessage = resultMessage;
            else if (resultMessage.Contains("Ничья"))
                displayMessage = resultMessage;

            gameStatusText.text = displayMessage;
        }
        else if (!useSingleText)
        {
            if (resultText != null) resultText.text = resultMessage;
            if (statusPanel != null) statusPanel.SetActive(false);
            if (resultPanel != null) resultPanel.SetActive(true);
        }
    }

    public void ShowGameResult(GameResult result, string winningSymbol)
    {
        string message = "";

        if (result == GameResult.Win)
        {
            message = $"Победили {winningSymbol}!";
        }
        else if (result == GameResult.Draw)
        {
            message = "Ничья!";
        }

        ShowGameResult(message);
    }

    [ContextMenu("Тест: Показать победу X")]
    public void TestShowWinX()
    {
        ShowGameResult("Победили X!");
    }

    [ContextMenu("Тест: Показать ничью")]
    public void TestShowDraw()
    {
        ShowGameResult("Ничья!");
    }
}