using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class RestartController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool debugLogs = true;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnRestart);
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
            return gm;
        }

        gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            return gm;
        }

        GameObject gmObj = GameObject.FindGameObjectWithTag("GameManager");
        if (gmObj != null)
        {
            gm = gmObj.GetComponent<GameManager>();
            if (gm != null)
            {
                return gm;
            }
        }

        gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
        {
            gm = gmObj.GetComponent<GameManager>();
            if (gm != null)
            {
                return gm;
            }
        }

        return null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnRestart();
    }

    public void ForceRestart()
    {
        OnRestart();
    }
}
