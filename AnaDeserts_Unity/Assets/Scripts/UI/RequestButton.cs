using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RequestButton : MonoBehaviour
{
    [SerializeField] private HoldTimeButton holdButton;

    [SerializeField]
    private Button btnEdit;

    [SerializeField] private TextMeshProUGUI txtCostumer;

    [SerializeField] private TextMeshProUGUI txtTotalCount;

    [SerializeField] private TextMeshProUGUI txtTotalCost;

    private RequestData requestData;

    public void Init(string costumer, RequestData requestData)
    {
        this.txtCostumer.text = costumer;
        this.requestData = requestData;

        txtTotalCount.text = requestData.totalAmount.ToString();
        txtTotalCost.text = requestData.totalCost.ToString("0.00");

        btnEdit.onClick.AddListener(OnEditButton_Click);

        holdButton.OnCompleteHold.AddListener(OnRequestButton_HoldComplete);

        holdButton.onClick.AddListener(OnRequestButton_Click);
    }

    public void UpdateButton(string costumer, RequestData requestData)
    {
        this.txtCostumer.text = costumer;
        this.requestData = requestData;

        txtTotalCount.text = requestData.totalAmount.ToString();
        txtTotalCost.text = requestData.totalCost.ToString("0.00");
    }

    private void OnEditButton_Click()
    {
        Panel_AddRequest.Show(txtCostumer.text, requestData);
    }

    private void OnRequestButton_HoldComplete()
    {
        Popup_RequestOptions.Show("Finalizar Pedido", () => {
            Panel_Main.RemoveButton(txtCostumer.text);
            AppManager.FinishRequest(txtCostumer.text);

            Notification.Show("Pedido finalizado");
        });

    }

    private void OnRequestButton_Click()
    {
        Debug.Log("Clicked");
        Panel_RequestInformation.Show(txtCostumer.text, requestData);
    }

    public string GetCostumerName()
    {
        return txtCostumer.text;
    }
}
