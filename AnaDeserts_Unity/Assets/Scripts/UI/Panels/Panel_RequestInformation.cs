using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Panel_RequestInformation : PanelBhv
{
    private static Panel_RequestInformation Instance;

    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnEdit;

    [SerializeField] private TextMeshProUGUI txtCostumer;
    [SerializeField] private TextMeshProUGUI txtDate;
    [SerializeField] private TextMeshProUGUI txtTime;
    [SerializeField] private TextMeshProUGUI txtTotalAmount;
    [SerializeField] private TextMeshProUGUI txtDeliveryCost;
    [SerializeField] private TextMeshProUGUI txtTotalCost;

    [SerializeField] private TasteRequested tasteRequestedPrefab;
    [SerializeField] private RectTransform tasteContainer;
    [SerializeField] private RectTransform content;
    private float originalHeight;

    private RequestData requestData;
    protected override void Start()
    {
        Instance = this;
        base.Start();
        originalHeight = content.sizeDelta.y;

        btnEdit.onClick.AddListener(OnButtonEdit_Click);

        btnBack.onClick.AddListener(() => { Close(); });
    }

    public static void Show(string costumer, RequestData requestData)
    {
        Instance.Init(costumer, requestData);
        Instance.ShowPanel();
    }

    public static void Reload(string costumer, RequestData requestData)
    {
        if (!Instance.canvas.enabled)
            return;

        Instance.Init(costumer, requestData, false);
    }

    private void Init(string costumer, RequestData requestData, bool returnIfSame = true)
    {
        if (returnIfSame)
            if (txtCostumer.text == costumer)
                return;

        content.SetAnchorPosY(0);

        txtCostumer.text = costumer;

        this.requestData = requestData;

        txtDate.text = requestData.date;

        txtTime.text = requestData.time;

        txtTotalAmount.text = requestData.totalAmount.ToString();

        txtDeliveryCost.text = "S/." + requestData.deliveryCost.ToString("0.00");
        txtTotalCost.text = "S/." + requestData.totalCost.ToString("0.00");

        if (tasteContainer.childCount > 0)
        {
            for (int i = 0; i < tasteContainer.childCount; i++)
                Destroy(tasteContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < requestData.tasteSettingsInfoList.Count; i++)
        {
            Instantiate(tasteRequestedPrefab, tasteContainer).Init(requestData.tasteSettingsInfoList[i]);
        }

        float heightToAdd = requestData.tasteSettingsInfoList.Count * tasteRequestedPrefab.rectTransform.sizeDelta.y;

        content.SetAnchorHeight(originalHeight + heightToAdd);

    }

    private void OnButtonEdit_Click()
    {
        Panel_AddRequest.Show(txtCostumer.text, requestData);

    }
}
