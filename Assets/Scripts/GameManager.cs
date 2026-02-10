using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Основные компоненты")]
    [SerializeField] private BoardController m_boardController;
    [SerializeField] private UIManager m_uiManager;

    [Header("Кнопка рестарта (опционально)")]
    [SerializeField] private Button m_restartButton;

    private bool m_isXTurn = true;
    private bool m_gameOver = false;

    void Start()
    {
        SetupRestartButton();

        StartNewGame();
    }

    void SetupRestartButton()
    {
        if (m_restartButton == null)
        {
            GameObject restartObj = GameObject.Find("RestartButton");
            if (restartObj != null)
            {
                m_restartButton = restartObj.GetComponent<Button>();
            }
            else
            {
                Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
                foreach (Button btn in allButtons)
                {
                    if (btn.gameObject.name.ToLower().Contains("restart"))
                    {
                        m_restartButton = btn;
                        break;
                    }
                }
            }
        }

        if (m_restartButton != null)
        {
            m_restartButton.onClick.RemoveAllListeners();
            m_restartButton.onClick.AddListener(RestartGame);
            m_restartButton.interactable = true;
            m_restartButton.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        StartNewGame();

        if (m_restartButton != null)
        {
            m_restartButton.interactable = true;
        }
    }

    public void StartNewGame()
    {
        m_isXTurn = true;
        m_gameOver = false;

        if (m_boardController != null)
        {
            m_boardController.ResetBoard();
        }

        if (m_uiManager != null)
        {
            m_uiManager.UpdateStatus(m_isXTurn);
        }

        if (m_restartButton != null && !m_restartButton.interactable)
        {
            m_restartButton.interactable = true;
        }
    }

    public void MakeMove(int cellIndex)
    {
        if (m_gameOver || !m_boardController.IsCellEmpty(cellIndex))
            return;

        string currentSymbol = m_isXTurn ? "X" : "O";
        m_boardController.MarkCell(cellIndex, currentSymbol);

        GameResult result = m_boardController.CheckGameResult();

        if (result != GameResult.None)
        {
            m_gameOver = true;

            string resultMessage = result == GameResult.Win ?
                $"Победили {currentSymbol}!" : "Ничья!";

            m_uiManager.ShowGameResult(resultMessage);
            m_boardController.DisableAllCells();

            if (m_restartButton != null)
            {
                m_restartButton.interactable = true;
            }
        }
        else
        {
            m_isXTurn = !m_isXTurn;
            m_uiManager.UpdateStatus(m_isXTurn);
        }
    }

    public void TestForceRestart()
    {
        RestartGame();
    }
}