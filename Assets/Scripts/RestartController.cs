using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class RestartController : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки")]
    [SerializeField] private bool m_debugLogs = true;

    private Button m_button;

    void Awake()
    {
        m_button = GetComponent<Button>();

        if (m_button == null)
        {
            return;
        }

        m_button.onClick.RemoveAllListeners();
        m_button.onClick.AddListener(OnRestart);
    }

    void OnRestart()
    {
        GameManager gm = FindGameManager();

        if (gm != null)
        {
            gm.RestartGame();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    GameManager FindGameManager()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            if (m_debugLogs) Debug.Log("GameManager найден через FindFirstObjectByType");
            return gm;
        }

        if (m_debugLogs) Debug.LogWarning("GameManager не найден!");
        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_debugLogs) Debug.Log("IPointerClickHandler сработал");
        OnRestart();
    }

    public void ForceRestart()
    {
        OnRestart();
    }
}
