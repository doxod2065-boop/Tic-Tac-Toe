using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour
{
    [Header("Размеры")]
    public int width = 7;
    public int height = 7;

    private CellController[,] cells;
    private string[,] boardState;

    private Vector2Int startX = new Vector2Int(0, 6);
    private Vector2Int startO = new Vector2Int(6, 0);

    private HashSet<Vector2Int> revealedX = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> revealedO = new HashSet<Vector2Int>();

    private static readonly Vector2Int[] neighborOffsets = new Vector2Int[8]
    {
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        new Vector2Int(-1,  0),                      new Vector2Int(1,  0),
        new Vector2Int(-1,  1), new Vector2Int(0,  1), new Vector2Int(1,  1)
    };

    private void Awake()
    {
        cells = new CellController[width, height];
        boardState = new string[width, height];
        FindAndAssignCells();
    }

    private void FindAndAssignCells()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            CellController cell = child.GetComponent<CellController>();
            if (cell != null)
            {
                int x = i % width;
                int y = i / width;
                cell.SetCoordinates(x, y);
                cells[x, y] = cell;
            }
        }
    }

    public void ResetBoard()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                boardState[x, y] = "";

        foreach (var cell in cells) cell?.ResetCell();

        revealedX.Clear();
        revealedO.Clear();

        MarkCell(startX.x, startX.y, "X");
        MarkCell(startO.x, startO.y, "O");
    }

    public bool IsCellEmpty(int x, int y) =>
        x >= 0 && x < width && y >= 0 && y < height && string.IsNullOrEmpty(boardState[x, y]);

    public void MarkCell(int x, int y, string symbol)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        boardState[x, y] = symbol;
        cells[x, y]?.SetSymbol(symbol);
    }

    public bool IsStartCell(int x, int y) =>
        (x == startX.x && y == startX.y) || (x == startO.x && y == startO.y);

    public void RemovePiece(int x, int y)
    {
        if (IsStartCell(x, y)) return;
        boardState[x, y] = "";
        cells[x, y]?.ResetCell();
    }

    public void RevealArea(int cx, int cy, string player)
    {
        var target = (player == "X") ? revealedX : revealedO;
        Vector2Int[] offsets = { new(0, 0), new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        foreach (var off in offsets)
        {
            int nx = cx + off.x, ny = cy + off.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                target.Add(new Vector2Int(nx, ny));
        }
        UpdateVisibilityForPlayer(player);
    }

    public void UpdateVisibilityForPlayer(string player)
    {
        HashSet<Vector2Int> visible = new HashSet<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (boardState[x, y] == player)
                {
                    visible.Add(new Vector2Int(x, y));
                    foreach (var off in neighborOffsets)
                    {
                        int nx = x + off.x, ny = y + off.y;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            visible.Add(new Vector2Int(nx, ny));
                    }
                }
            }
        }

        var revealed = (player == "X") ? revealedX : revealedO;
        visible.UnionWith(revealed);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                cells[x, y]?.SetVisible(visible.Contains(new Vector2Int(x, y)));
    }

    public void HighlightAllCells(bool active)
    {
        Color highlight = new Color(1f, 1f, 0.5f, 0.3f);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (active) cells[x, y]?.SetTemporaryHighlight(highlight);
                else cells[x, y]?.ClearTemporaryHighlight();
    }

    public List<Vector2Int> GetValidMoves(string player)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (IsCellEmpty(x, y) && IsAdjacentToPlayer(x, y, player))
                    moves.Add(new Vector2Int(x, y));
        return moves;
    }

    private bool IsAdjacentToPlayer(int x, int y, string player)
    {
        foreach (var off in neighborOffsets)
        {
            int nx = x + off.x, ny = y + off.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && boardState[nx, ny] == player)
                return true;
        }
        return false;
    }

    public bool HasValidMoves(string player) => GetValidMoves(player).Count > 0;

    public bool CheckWin(string player, int threshold)
    {
        Vector2Int start = (player == "X") ? startX : startO;
        bool[,] visited = new bool[width, height];
        bool found = false;
        FindLongestPath(start, player, visited, 1, threshold, ref found);
        return found;
    }

    private void FindLongestPath(Vector2Int cur, string player, bool[,] visited, int len, int th, ref bool found)
    {
        if (found) return;
        if (len >= th) { found = true; return; }
        visited[cur.x, cur.y] = true;
        foreach (var off in neighborOffsets)
        {
            int nx = cur.x + off.x, ny = cur.y + off.y;
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny] && boardState[nx, ny] == player)
                FindLongestPath(new Vector2Int(nx, ny), player, visited, len + 1, th, ref found);
        }
        visited[cur.x, cur.y] = false;
    }

    public void HighlightValidMoves(string player)
    {
        ClearAllHighlights();
        foreach (var m in GetValidMoves(player))
            cells[m.x, m.y]?.Highlight(true);
    }

    public void ClearAllHighlights()
    {
        foreach (var c in cells) c?.ClearHighlight();
    }

    public void DisableAllCells()
    {
        foreach (var c in cells) c?.SetInteractable(false);
    }

    public void SetAllCellsInteractable(bool interactable)
    {
        foreach (var c in cells) c?.SetInteractable(interactable);
    }

    [ContextMenu("Auto Assign")]
    private void AutoAssign() => FindAndAssignCells();
}
