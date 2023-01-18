using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBlock : PopupBhv
{
    private static LoadingBlock Instance;

    [SerializeField] private UnityEngine.UI.Text label;

    protected override void Start()
    {
        base.Start();

        Instance = this;
    }
    public static void ShowLoading(string label)
    {
        Instance.label.text = label;

        if (!Instance.canvas.enabled)
            Instance.ShowPanel();
    }

    public static void CloseLoading(System.Action OnClose = null)
    {
        Instance.Close(OnClose);
    }
}
