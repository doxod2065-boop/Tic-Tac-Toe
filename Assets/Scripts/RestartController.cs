using UnityEngine;
using UnityEngine.UI;

public class RestartController : MonoBehaviour
{
    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null) gm.RestartGame();
            });
        }
    }
}