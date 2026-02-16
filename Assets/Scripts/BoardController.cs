using UnityEngine;
using System.Collections.Generic;

public enum GameResult
{
    None,
    Win,
    Draw
}

public class BoardController : MonoBehaviour
{
    [Header("Размеры поля")]
    public int m_width = 7;
    public int m_height = 7;

    private CellController[,] m_cells;
    private string[,] m_boardState;

    private Vector2Int m_startX = new Vector2Int(0, 6);
    private Vector2Int m_startO = new Vector2Int(6, 0);

    private HashSet<Vector2Int> m_revealedCellsX = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> m_revealedCellsO = new HashSet<Vector2Int>();

    void Awake()
    {
        InitializeArrays();
    }

    void Start()
    {
        if (m_cells[0, 0] == null)
            FindAndAssignCells();
    }

    void InitializeArrays()
    {
        m_cells = new CellController[m_width, m_height];
        m_boardState = new string[m_width, m_height];
    }

    void FindAndAssignCells()
    {
        if (m_cells == null || m_boardState == null)
            InitializeArrays();

        int childCount = transform.childCount;
        if (childCount != m_width * m_height)
        {
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            CellController cell = child.GetComponent<CellController>();
            if (cell != null)
            {
                int x = i % m_width;
                int y = i / m_width;
                cell.SetCoordinates(x, y);
                m_cells[x, y] = cell;
            }
        }
    }

    public void ResetBoard()
    {
        if (m_cells[0, 0] == null)
            FindAndAssignCells();

        if (m_boardState == null)
            m_boardState = new string[m_width, m_height];

        for (int x = 0; x < m_width; x++)
            for (int y = 0; y < m_height; y++)
                m_boardState[x, y] = "";

        for (int x = 0; x < m_width; x++)
            for (int y = 0; y < m_height; y++)
                m_cells[x, y]?.ResetCell();

        m_revealedCellsX.Clear();
        m_revealedCellsO.Clear();

        MarkCell(m_startX.x, m_startX.y, "X");
        MarkCell(m_startO.x, m_startO.y, "O");
    }

    public bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || x >= m_width || y < 0 || y >= m_height) return false;
        return string.IsNullOrEmpty(m_boardState[x, y]);
    }

    public void MarkCell(int x, int y, string symbol)
    {
        if (x < 0 || x >= m_width || y < 0 || y >= m_height) return;
        m_boardState[x, y] = symbol;
        m_cells[x, y]?.SetSymbol(symbol);
    }

    public bool IsStartCell(int x, int y)
    {
        return (x == m_startX.x && y == m_startX.y) || (x == m_startO.x && y == m_startO.y);
    }

    public void RemovePiece(int x, int y)
    {
        if (x < 0 || x >= m_width || y < 0 || y >= m_height) return;
        if (IsStartCell(x, y)) return;

        m_boardState[x, y] = "";
        m_cells[x, y]?.ResetCell();
    }

    public void RevealArea(int cx, int cy, string player)
    {
        HashSet<Vector2Int> targetSet = (player == "X") ? m_revealedCellsX : m_revealedCellsO;

        Vector2Int[] offsets = {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        foreach (var offset in offsets)
        {
            int nx = cx + offset.x;
            int ny = cy + offset.y;
            if (nx >= 0 && nx < m_width && ny >= 0 && ny < m_height)
            {
                targetSet.Add(new Vector2Int(nx, ny));
            }
        }
    }

    public void UpdateVisibilityForPlayer(string player)
    {
        HashSet<Vector2Int> visibleCells = new HashSet<Vector2Int>();

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                if (m_boardState[x, y] == player)
                {
                    visibleCells.Add(new Vector2Int(x, y));
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int nx = x + dx, ny = y + dy;
                            if (nx >= 0 && nx < m_width && ny >= 0 && ny < m_height)
                                visibleCells.Add(new Vector2Int(nx, ny));
                        }
                    }
                }
            }
        }

        HashSet<Vector2Int> revealed = (player == "X") ? m_revealedCellsX : m_revealedCellsO;
        visibleCells.UnionWith(revealed);

        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                bool isVisible = visibleCells.Contains(new Vector2Int(x, y));
                m_cells[x, y]?.SetVisible(isVisible);
            }
        }
    }

    public List<Vector2Int> GetValidMoves(string player)
    {
        List<Vector2Int> valid = new List<Vector2Int>();
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                if (IsCellEmpty(x, y) && IsAdjacentToPlayer(x, y, player))
                    valid.Add(new Vector2Int(x, y));
            }
        }
        return valid;
    }

    bool IsAdjacentToPlayer(int x, int y, string player)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < m_width && ny >= 0 && ny < m_height)
                    if (m_boardState[nx, ny] == player)
                        return true;
            }
        }
        return false;
    }

    public void SetAllCellsInteractable(bool interactable)
    {
        for (int x = 0; x < m_width; x++)
        {
            for (int y = 0; y < m_height; y++)
            {
                if (m_cells[x, y] != null)
                {
                    m_cells[x, y].SetInteractable(interactable);
                }
            }
        }
    }
    public bool HasValidMoves(string player)
    {
        return GetValidMoves(player).Count > 0;
    }

    public bool CheckWin(string player)
    {
        Vector2Int start = (player == "X") ? m_startX : m_startO;
        bool[,] visited = new bool[m_width, m_height];
        int longestPath = FindLongestPath(start, player, visited);

        return longestPath >= 10;
    }

    private int FindLongestPath(Vector2Int current, string player, bool[,] visited)
    {
        visited[current.x, current.y] = true;
        int maxLength = 1;

        foreach (Vector2Int neighbor in GetNeighbors(current))
        {
            if (neighbor.x >= 0 && neighbor.x < m_width && neighbor.y >= 0 && neighbor.y < m_height &&
                !visited[neighbor.x, neighbor.y] &&
                m_boardState[neighbor.x, neighbor.y] == player)
            {
                int length = 1 + FindLongestPath(neighbor, player, visited);
                if (length > maxLength) maxLength = length;
            }
        }

        visited[current.x, current.y] = false;
        return maxLength;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                neighbors.Add(new Vector2Int(cell.x + dx, cell.y + dy));
            }
        }
        return neighbors;
    }

    public void HighlightValidMoves(string player)
    {
        ClearAllHighlights();
        foreach (var move in GetValidMoves(player))
            m_cells[move.x, move.y]?.Highlight(true);
    }

    public void ClearAllHighlights()
    {
        foreach (var cell in m_cells)
            cell?.ClearHighlight();
    }

    public void DisableAllCells()
    {
        foreach (var cell in m_cells)
            if (cell != null && cell.TryGetComponent(out UnityEngine.UI.Button btn))
                btn.interactable = false;
    }

    [ContextMenu("Auto Assign Coordinates")]
    void EditorAutoAssign()
    {
        InitializeArrays();
        FindAndAssignCells();
    }
}