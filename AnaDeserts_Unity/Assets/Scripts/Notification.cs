using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Helpers;
public class Notification : MonoBehaviour
{
    private static Notification Instance;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image bg;
    [SerializeField] private TMPro.TextMeshProUGUI txtMessage;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        canvas.enabled = false;

        bg.SetAlpha(0);
        txtMessage.SetAlpha(0);
    }

    public static void Show(string message)
    {
        if (Instance.canvas.enabled)
        {
            Instance.txtMessage.text = message;
        }
        else
        {
            Instance.canvas.enabled = true;
            Instance.txtMessage.text = message;

            Instance.bg.DOFade(1, 0.6f).OnComplete(CloseAfterShown);

            Instance.txtMessage.DOFade(1, 0.6f);
        }

    }

    private static void CloseAfterShown()
    {
        Instance.ActionAfterTime(0.6f, () =>
        {
            Instance.txtMessage.DOFade(0, 0.6f);

            Instance.bg.DOFade(0, 0.6f).OnComplete(() => {
                Instance.canvas.enabled = false;
            });
        });
    }
}
