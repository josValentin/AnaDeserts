using UnityEngine;
using System;

public class Popup_RequestOptions : PopupBhv
{
    private static Popup_RequestOptions Instance;

    [SerializeField] private UnityEngine.UI.Button btnFinishRequest;
    float prefabHeight;
    private TMPro.TextMeshProUGUI txtButton;

    private Action onClickAction;

    protected override void Start()
    {
        base.Start();

        Instance = this;

        prefabHeight = btnFinishRequest.GetComponent<RectTransform>().sizeDelta.y;
        txtButton = btnFinishRequest.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        btnFinishRequest.onClick.AddListener(OnFinishRequestButton_Click);
    }

    public static void Show(string buttonText, Action OnClick)
    {
        Instance.Init(buttonText, OnClick);
        Instance.checkGraphicsEveryShow = false;
        Instance.ShowPanel();
    }


    public static void Show(params OptionButtonSetting[] buttonSettings)
    {
        Instance.Init(buttonSettings);
        Instance.checkGraphicsEveryShow = true;

        Instance.ShowPanel();
    }

    private void Init(params OptionButtonSetting[] buttonSettings)
    {

        if (window.childCount > 1)
        {
            for (int i = 1; i < window.childCount; i++)
            {
                Destroy(window.GetChild(i).gameObject);
            }
        }

        txtButton.text = buttonSettings[0].buttonName;
        onClickAction = buttonSettings[0].OnClick;

        if (buttonSettings.Length > 1)
        {
            for (int i = 1; i < buttonSettings.Length; i++)
            {
                UnityEngine.UI.Button btn = Instantiate(btnFinishRequest, window);

                btn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = buttonSettings[i].buttonName;
                Debug.Log(i);
                int index = i;
                btn.onClick.AddListener(() => {

                    buttonSettings[index].OnClick?.Invoke();
                    Helpers.CoroutinesHelper.ActionAfterTime(this, 0.1f, () => {
                        Close();
                    });
                });
            }

        }

        window.SetAnchorHeight(prefabHeight * buttonSettings.Length);

    }

    private void Init(string buttonText, Action OnClick)
    {
        txtButton.text = buttonText;
        onClickAction = OnClick;

        window.SetAnchorHeight(prefabHeight);

        if(window.childCount > 1)
        {
            for (int i = 1; i < window.childCount; i++)
            {
                Destroy(window.GetChild(i).gameObject);
            }
        }
    }

    private void OnFinishRequestButton_Click()
    {
        onClickAction?.Invoke();

        Helpers.CoroutinesHelper.ActionAfterTime(this, 0.1f, () => {
            Close();
        });
    }

    public class OptionButtonSetting
    {
        public string buttonName;

        public Action OnClick;

        public static OptionButtonSetting Create(string buttonName, Action OnClick)
        {
            OptionButtonSetting button = new OptionButtonSetting();

            button.buttonName = buttonName;

            button.OnClick = OnClick;

            return button;
        }
    }
}
