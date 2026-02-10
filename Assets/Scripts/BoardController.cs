using UnityEngine;
using UnityEngine.UI;

public enum GameResult
{
    None,
    Win,
    Draw
}

public class BoardController : MonoBehaviour
{
    [Header("Клетки")]
    [SerializeField] private CellController[] cells;

    private string[] boardState = new string[9];

    void Start()
    {
        if (cells == null || cells.Length == 0)
        {
            FindCellsAutomatically();
        }
    }

    void FindCellsAutomatically()
    {
        cells = GetComponentsInChildren<CellController>();

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] != null)
            {
                cells[i].SetIndex(i);
            }
        }
    }

    public void ResetBoard()
    {
        for (int i = 0; i < boardState.Length; i++)
        {
            boardState[i] = "";
        }

        foreach (CellController cell in cells)
        {
            if (cell != null)
            {
                cell.ResetCell();
            }
        }
    }

    public void MarkCell(int index, string symbol)
    {
        if (index >= 0 && index < boardState.Length)
        {
            boardState[index] = symbol;

            if (index < cells.Length && cells[index] != null)
            {
                cells[index].SetSymbol(symbol);
            }
        }
    }

    public bool IsCellEmpty(int index)
    {
        if (index >= 0 && index < boardState.Length)
        {
            return string.IsNullOrEmpty(boardState[index]);
        }
        return false;
    }

    public GameResult CheckGameResult()
    {
        int[,] winPatterns = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Горизонтали
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Вертикали
            {0, 4, 8}, {2, 4, 6}             // Диагонали
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            int a = winPatterns[i, 0];
            int b = winPatterns[i, 1];
            int c = winPatterns[i, 2];

            if (!string.IsNullOrEmpty(boardState[a]) &&
                boardState[a] == boardState[b] &&
                boardState[b] == boardState[c])
            {
                return GameResult.Win;
            }
        }

        bool allCellsFilled = true;
        int filledCellsCount = 0;

        for (int i = 0; i < boardState.Length; i++)
        {
            if (string.IsNullOrEmpty(boardState[i]))
            {
                allCellsFilled = false;
                break;
            }
            else
            {
                filledCellsCount++;
            }
        }

        if (allCellsFilled)
        {
            return GameResult.Draw;
        }

        return GameResult.None;
    }

    public void DisableAllCells()
    {
        foreach (CellController cell in cells)
        {
            if (cell != null)
            {
                Button button = cell.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = false;
                }
            }
        }
    }

    public bool IsBoardFull()
    {
        foreach (string cell in boardState)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return false;
            }
        }
        return true;
    }

    void FindAllCells()
    {
        FindCellsAutomatically();
    }
}