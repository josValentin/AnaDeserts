using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_SelectTastes : PopupBhv
{
    private static Panel_SelectTastes Instance;

    [SerializeField]
    private Button btnAddNewTaste;

    [SerializeField]
    private Button btnConfirmSelection;

    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private TasteOption tasteOptionPrefab;
    float heightPrefab;

    private List<TasteOption> trufflesOptionsList = new List<TasteOption>();

    private DessertType dessertType;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Instance = this;

        btnAddNewTaste.onClick.AddListener(OnAddNewTasteButtonclick);
        btnConfirmSelection.onClick.AddListener(OnConfirmSelection_ButtonClick);

        heightPrefab = tasteOptionPrefab.GetComponent<RectTransform>().sizeDelta.y;



    }

    private void OnAddNewTasteButtonclick()
    {
        Popup_AddTaste.Show(dessertType);
    }

    public static void AddOption(DessertKey key, DessertData data)
    {
        TasteOption option = Instantiate(Instance.tasteOptionPrefab, Instance.content);

        option.costStr = "S/." + data.cost.ToString("0.00");
        option.iconColor = data.colorInfo.ToUnityColor();
        option.Init(key, data);

        Instance.trufflesOptionsList.Add(option);

        Instance.content.AddAnchorHeight(Instance.heightPrefab);
    }

    public static void ChangeOptionValue(DessertKey key, DessertData data)
    {
        for (int i = 0; i < Instance.trufflesOptionsList.Count; i++)
        {
            TasteOption option = Instance.trufflesOptionsList[i];
            if (option.GetKey().Equals(key))
            {
                option.costStr = "S/." + data.cost.ToString("0.00");
                option.iconColor = data.colorInfo.ToUnityColor();
                option.Init(key, data);
                return;
            }
        }
    }

    public static void DeleteOption(DessertKey key)
    {
        for (int i = 0; i < Instance.trufflesOptionsList.Count; i++)
        {
            TasteOption option = Instance.trufflesOptionsList[i];
            if (option.GetKey().Equals(key))
            {
                Destroy(option.gameObject);
                Instance.trufflesOptionsList.RemoveAt(i);
                Instance.content.AddAnchorHeight(-Instance.heightPrefab);
                return;
            }
        }
    }

    private void OnConfirmSelection_ButtonClick()
    {
        for (int i = 0; i < trufflesOptionsList.Count; i++)
        {
            if (trufflesOptionsList[i].isSelected)
                Panel_AddRequest.AddTasteSetting(trufflesOptionsList[i].GetKey(), trufflesOptionsList[i].data);
        }

        Helpers.CoroutinesHelper.ActionAfterTime(this, 0.1f, () =>
        {
            Close();
        });


    }
    public static void Show(DessertType dessertType)
    {
        if (Instance.dessertType == dessertType)
        {
            Instance.ShowPanel();
            return;
        }


        Instance.dessertType = dessertType;

        if (Instance.content.childCount > 0)
        {
            // Clear...
            for (int i = 0; i < Instance.content.childCount; i++)
                Destroy(Instance.content.GetChild(i).gameObject);

            Instance.trufflesOptionsList.Clear();
            Instance.content.SetAnchorHeight(0);
        }


        foreach (var item in AppManager.TrufflesTasteDic)
        {
            if (item.Key.dessertType == dessertType)
                AddOption(item.Key, item.Value);
        }
        Instance.ShowPanel();
    }
}
