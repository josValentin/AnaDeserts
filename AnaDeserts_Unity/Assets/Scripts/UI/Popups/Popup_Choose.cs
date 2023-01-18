using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Popup_Choose : PopupBhv
{
    private static Popup_Choose Instance;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtBody;

    [SerializeField] private UnityEngine.UI.Button btnContinue;
    [SerializeField] private UnityEngine.UI.Button btnCancel;

    System.Action OnContinue;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Instance = this;

        btnContinue.onClick.AddListener(() => {
            Close(OnContinue);
        });

        btnCancel.onClick.AddListener(() => {
            Close();
        });
    }

    public static void Show(string title, string body, System.Action OnContinue)
    {
        Instance.Init(title, body, OnContinue);
        Instance.ShowPanel();
    }

    private void Init(string title, string body, System.Action OnContinue)
    {
        txtTitle.text = title;
        txtBody.text = body;
        this.OnContinue = OnContinue;
    }
}
