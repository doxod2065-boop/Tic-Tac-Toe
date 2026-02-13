using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellController : MonoBehaviour
{
    public Vector2Int Coordinates { get; private set; }

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI symbolText;
    [SerializeField] private Image background;

    private GameManager gameManager;
    private Color defaultColor;
    private Color hiddenColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private string currentSymbol = "";
    private bool isHighlighted = false;
    private bool isVisible = true;
    private Color? temporaryColor = null;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (symbolText == null) symbolText = GetComponentInChildren<TextMeshProUGUI>();
        if (background == null) background = GetComponent<Image>();
        if (background != null) defaultColor = background.color;

        button?.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnClick() => gameManager?.OnCellClicked(Coordinates.x, Coordinates.y);

    public void SetCoordinates(int x, int y) => Coordinates = new Vector2Int(x, y);

    public void SetSymbol(string symbol)
    {
        currentSymbol = symbol;
        if (symbolText != null)
        {
            symbolText.text = symbol;
            symbolText.color = symbol == "X" ? Color.red : Color.blue;
        }
        SetInteractable(false);
    }

    public void ResetCell()
    {
        currentSymbol = "";
        if (symbolText != null) symbolText.text = "";
        SetInteractable(true);
        isHighlighted = false;
        temporaryColor = null;
        isVisible = true;
        if (background != null) background.color = defaultColor;
    }

    public void Highlight(bool active)
    {
        isHighlighted = active;
        UpdateBackground();
    }

    public void ClearHighlight()
    {
        isHighlighted = false;
        UpdateBackground();
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        UpdateBackground();
    }

    public void SetTemporaryHighlight(Color color)
    {
        temporaryColor = color;
        if (background != null) background.color = color;
    }

    public void ClearTemporaryHighlight()
    {
        temporaryColor = null;
        UpdateBackground();
    }

    private void UpdateBackground()
    {
        if (background == null) return;
        if (temporaryColor.HasValue)
        {
            background.color = temporaryColor.Value;
            return;
        }
        if (isHighlighted)
        {
            background.color = new Color(0.3f, 1f, 0.3f, 0.5f);
            return;
        }
        background.color = isVisible ? defaultColor : hiddenColor;
    }

    public void SetInteractable(bool value)
    {
        if (button != null) button.interactable = value;
    }
}