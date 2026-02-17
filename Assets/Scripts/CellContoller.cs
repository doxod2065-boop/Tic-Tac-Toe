using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellController : MonoBehaviour
{
    public Vector2Int coordinates;

    [Header("Компоненты")]
    [SerializeField] private Button m_button;
    [SerializeField] private TextMeshProUGUI m_symbolText;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Outline m_outline;

    private GameManager m_gameManager;
    private Color defaultColor;
    private Color hiddenColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private string m_symbol = "";
    private bool isHighlighted = false;
    private bool isVisible = true;

    void Start()
    {
        if (m_button == null) m_button = GetComponent<Button>();
        if (m_symbolText == null) m_symbolText = GetComponentInChildren<TextMeshProUGUI>();
        if (m_backgroundImage == null) m_backgroundImage = GetComponent<Image>();
        if (m_outline == null) m_outline = GetComponent<Outline>();

        if (m_backgroundImage != null)
            defaultColor = m_backgroundImage.color;

        if (m_button != null)
        {
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(OnCellClick);
        }

        m_gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnCellClick()
    {
        if (m_gameManager != null)
            m_gameManager.ProcessCellClick(coordinates.x, coordinates.y);
    }

    public void SetInteractable(bool value)
    {
        if (m_button != null)
            m_button.interactable = value;
    }

    public void SetSymbol(string symbol)
    {
        m_symbol = symbol;
        if (m_symbolText != null)
        {
            m_symbolText.text = symbol;
            m_symbolText.color = symbol == "X" ? Color.red : Color.blue;
        }
        if (m_button != null)
            m_button.interactable = false;
    }

    public void ResetCell()
    {
        m_symbol = "";
        if (m_symbolText != null)
            m_symbolText.text = "";
        if (m_button != null)
            m_button.interactable = true;
        isHighlighted = false;
        isVisible = true;
        if (m_backgroundImage != null)
            m_backgroundImage.color = defaultColor;
    }

    public void SetCoordinates(int x, int y)
    {
        coordinates = new Vector2Int(x, y);
    }

    public void Highlight(bool active)
    {
        isHighlighted = active;
        UpdateBackgroundColor();
    }

    public void ClearHighlight()
    {
        isHighlighted = false;
        UpdateBackgroundColor();
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        UpdateBackgroundColor();

        if (m_symbolText != null)
            m_symbolText.text = visible ? m_symbol : "";

        // Рамка всегда включена для визуальной структуры
        // if (m_outline != null) m_outline.enabled = visible;

        if (m_button != null && visible && string.IsNullOrEmpty(m_symbol))
            m_button.interactable = true;
        else if (m_button != null)
            m_button.interactable = false;
    }

    private void UpdateBackgroundColor()
    {
        if (m_backgroundImage == null) return;

        if (isHighlighted)
            m_backgroundImage.color = new Color(0.3f, 1f, 0.3f, 0.5f); // зелёная подсветка
        else
            m_backgroundImage.color = isVisible ? defaultColor : hiddenColor;
    }

    public void SetTemporaryHighlight(Color color)
    {
        if (m_backgroundImage != null)
            m_backgroundImage.color = color;
    }

    public void ClearTemporaryHighlight()
    {
        UpdateBackgroundColor();
    }
}