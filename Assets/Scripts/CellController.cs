using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellController : MonoBehaviour
{
    [Header("Настройки")]
    public int cellIndex = -1;

    [Header("Ссылки")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI symbolText;

    private GameManager gameManager;

    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (symbolText == null)
        {
            symbolText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnCellClick);
        }
    }

    void OnCellClick()
    {

        if (gameManager != null)
        {
            gameManager.MakeMove(cellIndex);
        }
    }

    public void SetSymbol(string symbol)
    {

        if (symbolText != null)
        {
            symbolText.text = symbol;
        }

        if (button != null)
        {
            button.interactable = false;
        }
    }

    public void ResetCell()
    {

        if (symbolText != null)
        {
            symbolText.text = "";
        }

        if (button != null)
        {
            button.interactable = true;
        }
    }

    public void SetIndex(int index)
    {
        cellIndex = index;
    }
}