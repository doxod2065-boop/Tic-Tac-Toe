using UnityEngine;
using UnityEngine.UI;
using Abilities;

public class GameManager : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private BoardController board;
    [SerializeField] private UIManager ui;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private Button restartButton;

    private bool isXTurn = true;
    private bool isGameOver = false;
    private bool isExtraMove = false;

    private AbilityType pendingAbility = AbilityType.None;
    private string pendingOwner = "";

    private void Start()
    {
        SetupRestartButton();
        if (abilityManager == null) abilityManager = FindFirstObjectByType<AbilityManager>();
        StartNewGame();
    }

    private void SetupRestartButton()
    {
        if (restartButton == null)
        {
            GameObject obj = GameObject.Find("RestartButton");
            if (obj != null) restartButton = obj.GetComponent<Button>();
        }
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
            restartButton.interactable = true;
        }
    }

    public void RestartGame() => StartNewGame();

    private void StartNewGame()
    {
        if (board == null) { Debug.LogError("BoardController не назначен!"); return; }

        isXTurn = true;
        isGameOver = false;
        isExtraMove = false;
        pendingAbility = AbilityType.None;
        pendingOwner = "";

        board.ResetBoard();
        abilityManager?.ResetAll();
        UpdateForPlayer("X");
    }

    private void UpdateForPlayer(string player)
    {
        board.UpdateVisibilityForPlayer(player);
        board.HighlightValidMoves(player);
        abilityManager?.StartTurn(player);
        ui?.UpdateTurn(player == "X");
    }

    public void OnCellClicked(int x, int y)
    {
        if (isGameOver) return;

        string currentPlayer = isXTurn ? "X" : "O";

        if (pendingAbility != AbilityType.None && pendingOwner == currentPlayer)
        {
            ApplyPendingAbility(x, y);
            return;
        }

        ExecuteNormalMove(x, y, currentPlayer);
    }

    private void ExecuteNormalMove(int x, int y, string player)
    {
        if (!board.IsCellEmpty(x, y))
        {
            Debug.Log("Клетка уже занята!");
            return;
        }

        var valid = board.GetValidMoves(player);
        if (!valid.Contains(new Vector2Int(x, y)))
        {
            Debug.Log("Недопустимый ход!");
            return;
        }

        board.MarkCell(x, y, player);
        board.UpdateVisibilityForPlayer(player);
        board.HighlightValidMoves(player);

        if (CheckVictory())
        {
            EndGame();
            return;
        }

        AbilityType usedBonus = abilityManager?.ApplyBonus(player) ?? AbilityType.None;

        if (usedBonus == AbilityType.DoubleMove && !isExtraMove)
        {
            isExtraMove = true;
            ui?.UpdateTurn(isXTurn);
            return;
        }
        else if (usedBonus == AbilityType.Recon || usedBonus == AbilityType.Shoot)
        {
            pendingAbility = usedBonus;
            pendingOwner = player;
            board.SetAllCellsInteractable(true);
            board.HighlightAllCells(true);
            ui?.UpdateTurn(isXTurn);
            return;
        }

        if (isExtraMove)
        {
            isExtraMove = false;
            SwitchTurn();
            return;
        }

        SwitchTurn();
    }

    private void ApplyPendingAbility(int x, int y)
    {
        string player = pendingOwner;

        if (pendingAbility == AbilityType.Recon)
        {
            board.RevealArea(x, y, player);
            ui?.ShowMessage($"Разведка в ({x},{y})");
        }
        else if (pendingAbility == AbilityType.Shoot)
        {
            if (board.IsStartCell(x, y))
                Debug.Log("Нельзя стрелять в стартовую клетку!");
            else if (board.IsCellEmpty(x, y))
                Debug.Log("Пустая клетка — выстрел без результата.");
            else
                board.RemovePiece(x, y);
        }

        board.HighlightAllCells(false);
        pendingAbility = AbilityType.None;
        pendingOwner = "";

        board.UpdateVisibilityForPlayer(player);

        if (CheckVictory())
            EndGame();
        else
            SwitchTurn();
    }

    private void SwitchTurn()
    {
        isXTurn = !isXTurn;
        string nextPlayer = isXTurn ? "X" : "O";

        if (!board.HasValidMoves(nextPlayer))
        {
            isGameOver = true;
            ui?.ShowMessage("Ничья! Нет доступных ходов.");
            board.DisableAllCells();
            board.ClearAllHighlights();
            return;
        }

        UpdateForPlayer(nextPlayer);
    }

    private bool CheckVictory()
    {
        if (board.CheckWin("X", 10))
        {
            isGameOver = true;
            ui?.ShowMessage("Победили X!");
            return true;
        }
        if (board.CheckWin("O", 10))
        {
            isGameOver = true;
            ui?.ShowMessage("Победили O!");
            return true;
        }
        return false;
    }

    private void EndGame()
    {
        board.DisableAllCells();
        board.ClearAllHighlights();
    }
}