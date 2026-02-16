using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private BoardController m_boardController;
    [SerializeField] private UIManager m_uiManager;
    [SerializeField] private Button m_restartButton;

    [Header("Способности")]
    [SerializeField] private AbilityManager m_abilityManager;

    private bool m_isXTurn = true;
    private bool m_gameOver = false;
    private bool m_isExtraMove = false;

    private AbilityType m_pendingAbility = AbilityType.None;
    private string m_pendingOwner = "";

    void Start()
    {
        SetupRestartButton();

        if (m_abilityManager == null)
            m_abilityManager = FindFirstObjectByType<AbilityManager>();

        StartNewGame();
    }

    void SetupRestartButton()
    {
        if (m_restartButton == null)
        {
            GameObject obj = GameObject.Find("RestartButton");
            if (obj != null) m_restartButton = obj.GetComponent<Button>();
        }
        if (m_restartButton != null)
        {
            m_restartButton.onClick.RemoveAllListeners();
            m_restartButton.onClick.AddListener(RestartGame);
            m_restartButton.interactable = true;
        }
    }

    public void RestartGame()
    {
        StartNewGame();
    }

    void StartNewGame()
    {
        if (m_boardController == null)
        {
            return;
        }

        m_isXTurn = true;
        m_gameOver = false;
        m_isExtraMove = false;
        m_pendingAbility = AbilityType.None;
        m_pendingOwner = "";

        m_boardController.ResetBoard();
        m_boardController.UpdateVisibilityForPlayer("X");
        m_boardController.HighlightValidMoves("X");

        if (m_abilityManager != null)
        {
            m_abilityManager.ResetAbilities();
            m_abilityManager.StartTurn("X");
        }

        m_uiManager?.UpdateStatus(true);
    }

    public void ProcessCellClick(int x, int y)
    {
        if (m_gameOver) return;

        string currentPlayer = m_isXTurn ? "X" : "O";

        if (m_pendingAbility != AbilityType.None && m_pendingOwner == currentPlayer)
        {
            ApplyPendingAbility(x, y);
            return;
        }

        MakeMove(x, y);
    }

    void MakeMove(int x, int y)
    {
        string currentPlayer = m_isXTurn ? "X" : "O";

        if (!m_boardController.IsCellEmpty(x, y))
        {
            return;
        }

        var valid = m_boardController.GetValidMoves(currentPlayer);
        if (!valid.Contains(new Vector2Int(x, y)))
        {
            return;
        }

        m_boardController.MarkCell(x, y, currentPlayer);

        m_boardController.UpdateVisibilityForPlayer(currentPlayer);
        m_boardController.HighlightValidMoves(currentPlayer);

        if (CheckWinCondition())
        {
            m_boardController.DisableAllCells();
            m_boardController.ClearAllHighlights();
            return;
        }

        AbilityType usedBonus = m_abilityManager != null
            ? m_abilityManager.ApplyBonus(currentPlayer)
            : AbilityType.None;

        if (usedBonus == AbilityType.DoubleMove && !m_isExtraMove)
        {
            m_isExtraMove = true;
            m_uiManager?.UpdateStatus(m_isXTurn);
            return;
        }
        else if (usedBonus == AbilityType.Recon || usedBonus == AbilityType.Shoot)
        {
            m_pendingAbility = usedBonus;
            m_pendingOwner = currentPlayer;

            m_boardController.SetAllCellsInteractable(true);

            m_uiManager?.UpdateStatus(m_isXTurn);
            return;
        }

        if (m_isExtraMove)
        {
            m_isExtraMove = false;
            SwitchTurn();
            return;
        }

        SwitchTurn();
    }

    void ApplyPendingAbility(int x, int y)
    {
        string currentPlayer = m_pendingOwner;

        if (m_pendingAbility == AbilityType.Recon)
        {
            m_boardController.RevealArea(x, y, currentPlayer);
            m_boardController.UpdateVisibilityForPlayer(currentPlayer);
            m_uiManager?.ShowGameResult($"Разведка в ({x},{y})");
        }
        else if (m_pendingAbility == AbilityType.Shoot)
        {
            if (!m_boardController.IsStartCell(x, y))
            {
                m_boardController.RemovePiece(x, y);
                m_uiManager?.ShowGameResult($"Выстрел в ({x},{y})");
            }

            m_boardController.UpdateVisibilityForPlayer(currentPlayer);
        }

        m_pendingAbility = AbilityType.None;
        m_pendingOwner = "";

        SwitchTurn();
    }

    void SwitchTurn()
    {
        m_isXTurn = !m_isXTurn;
        string nextPlayer = m_isXTurn ? "X" : "O";

        if (!m_boardController.HasValidMoves(nextPlayer))
        {
            m_gameOver = true;
            m_uiManager?.ShowGameResult("Ничья! Нет доступных ходов.");
            m_boardController.DisableAllCells();
            m_boardController.ClearAllHighlights();
            return;
        }

        m_boardController.UpdateVisibilityForPlayer(nextPlayer);
        m_boardController.HighlightValidMoves(nextPlayer);

        m_abilityManager?.StartTurn(nextPlayer);
        m_uiManager?.UpdateStatus(m_isXTurn);
    }

    bool CheckWinCondition()
    {
        bool xWin = m_boardController.CheckWin("X");
        bool oWin = m_boardController.CheckWin("O");

        if (xWin)
        {
            m_gameOver = true;
            m_uiManager?.ShowGameResult("Победили X!");
            return true;
        }
        if (oWin)
        {
            m_gameOver = true;
            m_uiManager?.ShowGameResult("Победили O!");
            return true;
        }
        return false;
    }

    public BoardController GetBoardController() => m_boardController;
    public UIManager GetUIManager() => m_uiManager;
    public string GetGameState() => $"Ход: {(m_isXTurn ? "X" : "O")}, Игра окончена: {m_gameOver}";
}