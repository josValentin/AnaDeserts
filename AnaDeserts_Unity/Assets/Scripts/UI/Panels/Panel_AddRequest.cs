using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdvancedInputFieldPlugin;
using Helpers;
using Unity.Notifications.Android;

public class Panel_AddRequest : PanelBhv
{
    private static Panel_AddRequest Instance;

    [SerializeField] private Button btnBack;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtLastButton;

    [SerializeField] private AdvancedInputField advInputCostumerName;

    [SerializeField] private TasteSetting tasteSettingPrefab;
    [SerializeField] private RectTransform tasteContainer;

    [SerializeField]
    private RectTransform content;

    private float originalContentHeight;

    [SerializeField] private TextMeshProUGUI txtButtonDate;
    [SerializeField] private TextMeshProUGUI txtButtonTime;

    //
    [SerializeField]
    private TextMeshProUGUI txtSelectedDate;

    [SerializeField]
    private TextMeshProUGUI txtSelectedTime;

    //
    [SerializeField]
    private Button btnSelectTaste;


    [SerializeField]
    private RectTransform lowerSideRect;

    [SerializeField] private AdvancedInputField advInputDeliveryCost;
    private float lastValueDeliveryCost;

    [SerializeField]
    private AdvancedInputField advInputFinalCost;

    private List<TasteSetting> tasteSettingsList = new List<TasteSetting>();

    [HideInInspector]
    public float currentRequestCost = 0;

    [SerializeField] private Button btnAddRequest;

    public void SetDateSelected(string date)
    {
        txtSelectedDate.text = date;
        txtButtonDate.text = "Cambiar";
    }

    public void SetTimeSelected(string time)
    {
        txtSelectedTime.text = time;
        txtButtonTime.text = "Cambiar";
    }

    private float currentHeight;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Instance = this;

        btnBack.onClick.AddListener(() => { Close(); });

        btnSelectTaste.onClick.AddListener(OnSelectTaste_ButtonClick);

        advInputFinalCost.OnEndEdit.AddListener(OnAdvInputFinalCost_EndEdit);

        btnAddRequest.onClick.AddListener(OnAddRequest_ButtonClick);

        originalContentHeight = content.sizeDelta.y;
        currentHeight = originalContentHeight;

        relativeHeight = Screen.height / canvas.scaleFactor;

        advInputDeliveryCost.OnEndEdit.AddListener((string currentText, EndEditReason reason) =>
        {

            if (string.IsNullOrEmpty(currentText))
            {
                advInputDeliveryCost.Text = "0.00";
                Instance.currentRequestCost -= lastValueDeliveryCost;
                lastValueDeliveryCost = 0;
            }
            else
            {
                float cost = float.Parse(currentText);

                if (cost <= 0)
                {
                    advInputDeliveryCost.Text = "0.00";
                    Instance.currentRequestCost -= lastValueDeliveryCost;
                    lastValueDeliveryCost = 0;
                }
                else
                {

                    Instance.currentRequestCost -= lastValueDeliveryCost;
                    Instance.currentRequestCost += cost;
                    lastValueDeliveryCost = cost;
                }

            }

            Instance.advInputFinalCost.Text = Instance.currentRequestCost.ToString("0.00");

        });

    }
    float relativeHeight;
    float lastPosY;
    bool wasSelected;
    bool anyFieldWasSelected;
    private void LateUpdate()
    {
        if (advInputFinalCost.Selected)
        {
            Vector2 position = lowerSideRect.anchoredPosition;
            position.y = AppManager.KeyBoardHeight;
            lowerSideRect.anchoredPosition = position;
            if (!wasSelected)
                wasSelected = true;
        }
        else
        {
            if (wasSelected)
            {
                Vector2 position = lowerSideRect.anchoredPosition;
                position.y = AppManager.KeyBoardHeight;
                lowerSideRect.anchoredPosition = position;
                if (AppManager.KeyBoardHeight == 0)
                {
                    wasSelected = false;
                }
            }
        }

        bool oneIsSelected = false;
        for (int i = 0; i < tasteSettingsList.Count; i++)
        {
            TasteSetting setting = tasteSettingsList[i];
            if (setting.advInputAmount.Selected)
            {
                float target = currentHeight + AppManager.KeyBoardHeight;
                content.SetAnchorHeight(target);

                float rectSettingHeight = setting.rectTransform.sizeDelta.y;

                content.SetAnchorPosY(((target - relativeHeight) - (tasteSettingsList.Count - i) * rectSettingHeight) + rectSettingHeight);

                if (!anyFieldWasSelected)
                    anyFieldWasSelected = true;

                oneIsSelected = true;
                break;
            }
        }

        if (!oneIsSelected)
        {
            if (anyFieldWasSelected)
            {
                content.SetAnchorPosY(lastPosY);
                content.SetAnchorHeight(currentHeight);
                anyFieldWasSelected = false;
            }
        }

    }
    private static Vector2 WorldToCanvas(RectTransform canvas, Camera camera, Vector3 position)
    {
        Vector2 temp = camera.WorldToViewportPoint(position);
        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }

    public static void AddTasteSetting(DessertKey key, DessertData truffleData, int startAmount = 1)
    {
        for (int i = 0; i < Instance.tasteSettingsList.Count; i++)
        {
            if (Instance.tasteSettingsList[i].GetKey().Equals(key))
                return;
        }

        TasteSetting tasteSetting = Instantiate(Instance.tasteSettingPrefab, Instance.tasteContainer);

        tasteSetting.Init(key, truffleData);

        Instance.content.AddAnchorHeight(tasteSetting.rectTransform.sizeDelta.y);

        tasteSetting.lastValue = startAmount * truffleData.cost;

        Instance.currentRequestCost += tasteSetting.lastValue;

        tasteSetting.advInputAmount.OnBeginEdit.AddListener((BeginEditReason reason) => {

            Instance.lastPosY = Instance.content.anchoredPosition.y;
        });
        tasteSetting.advInputAmount.OnEndEdit.AddListener((string currentText, EndEditReason reason) =>
        {

            if (string.IsNullOrEmpty(currentText))
            {
                tasteSetting.advInputAmount.Text = "0";
                Instance.currentRequestCost -= tasteSetting.lastValue;
                tasteSetting.lastValue = 0;
            }
            else
            {
                int amount = int.Parse(currentText);

                if (amount < 0)
                {
                    tasteSetting.advInputAmount.Text = "0";
                    Instance.currentRequestCost -= tasteSetting.lastValue;
                    tasteSetting.lastValue = 0;
                }
                else
                {

                    float newCost = amount * tasteSetting.data.cost;
                    Instance.currentRequestCost -= tasteSetting.lastValue;
                    Instance.currentRequestCost += newCost;
                    tasteSetting.lastValue = newCost;
                }

            }

            Instance.advInputFinalCost.Text = Instance.currentRequestCost.ToString("0.00");

        });

        tasteSetting.advInputAmount.Text = startAmount.ToString();

        Instance.tasteSettingsList.Add(tasteSetting);

        Instance.advInputFinalCost.Text = Instance.currentRequestCost.ToString("0.00");

        Instance.currentHeight = Instance.content.sizeDelta.y;
    }

    public static void RemoveTasteSetting(DessertKey key)
    {
        for (int i = 0; i < Instance.tasteSettingsList.Count; i++)
        {
            if (Instance.tasteSettingsList[i].GetKey().Equals(key))
            {
                TasteSetting tasteSetting = Instance.tasteSettingsList[i];

                Instance.currentRequestCost -= tasteSetting.lastValue;

                Instance.content.AddAnchorHeight(-tasteSetting.rectTransform.sizeDelta.y);

                //
                Destroy(Instance.tasteSettingsList[i].gameObject);

                Instance.tasteSettingsList.RemoveAt(i);

                Instance.advInputFinalCost.Text = Instance.currentRequestCost.ToString("0.00");

                Instance.currentHeight = Instance.content.sizeDelta.y;

                return;
            }
        }



    }

    private void OnAdvInputFinalCost_EndEdit(string arg0, EndEditReason arg1)
    {
        Vector2 pos = lowerSideRect.anchoredPosition;
        pos.y = 0;
        lowerSideRect.anchoredPosition = pos;
    }

    private bool Editting = false;
    public static void Show()
    {
        if (Instance.Editting)
        {
            Instance.Editting = false;

            Instance.Restore();
        }
        Instance.content.SetAnchorPosY(0);

        Instance.ShowPanel();
    }

    private void Restore()
    {
        advInputCostumerName.Clear();

        advInputFinalCost.Clear();

        advInputDeliveryCost.Text = "0.00";

        advInputCostumerName.interactable = true;

        txtSelectedDate.text = "--/--/--";
        txtSelectedTime.text = "--:--";

        txtButtonDate.text = "Establecer";
        txtButtonTime.text = "Establecer";


        Instance.txtTitle.text = "Crear Pedido";
        Instance.txtLastButton.text = "Crear Pedido";

        for (int i = 0; i < tasteSettingsList.Count; i++)
        {
            Destroy(tasteSettingsList[i].gameObject);
        }

        tasteSettingsList.Clear();

        content.SetAnchorHeight(originalContentHeight);

        currentRequestCost = 0;
    }

    public static void Show(string costumer, RequestData requestData)
    {
        Instance.InitEditting(costumer, requestData);
        Instance.content.SetAnchorPosY(0);
        Instance.ShowPanel();
    }

    private void InitEditting(string costumer, RequestData requestData)
    {
        advInputCostumerName.Text = costumer;

        advInputCostumerName.interactable = false;

        Editting = true;

        txtSelectedDate.text = requestData.date;
        txtButtonDate.text = "Cambiar";
        txtSelectedTime.text = requestData.time;
        txtButtonTime.text = "Cambiar";

        advInputDeliveryCost.Text = requestData.deliveryCost.ToString("0.00");

        if (tasteSettingsList.Count > 0)
        {
            for (int i = 0; i < tasteSettingsList.Count; i++)
            {
                Destroy(tasteSettingsList[i].gameObject);
            }

            tasteSettingsList.Clear();

            content.SetAnchorHeight(originalContentHeight);
        }
        currentRequestCost = 0;
        for (int i = 0; i < requestData.tasteSettingsInfoList.Count; i++)
        {
            AddTasteSetting(requestData.tasteSettingsInfoList[i].key, requestData.tasteSettingsInfoList[i].dessertData, requestData.tasteSettingsInfoList[i].amount);
        }

        advInputFinalCost.Text = requestData.totalCost.ToString("0.00");

        txtTitle.text = "Editar Pedido";
        txtLastButton.text = "Finalizar";
    }

    private void OnSelectTaste_ButtonClick()
    {
        EventCenter.Broadcast(EventDefine.SelectDessert_ShowPanel);
    }

    private void OnAddRequest_ButtonClick()
    {
        string costumerName = advInputCostumerName.Text;



        if (string.IsNullOrEmpty(costumerName))
        {
            Notification.Show("Escribe el nombre del comprador");
            return;
        }

        if (!Editting)
            if (AppManager.RequestsDic.ContainsKey(costumerName))
            {
                Notification.Show("Esta persona ya tiene un pedido en curso");
                return;
            }

#if !UNITY_EDITOR && UNITY_ANDROID
        if (txtSelectedDate.text == "--/--/--")
        {
            Notification.Show("Establece una fecha de entrega");
            return;
        }

        if (txtSelectedTime.text == "--:--")
        {
            Notification.Show("Establece una hora de entrega");
            return;
        }
#endif

        if (tasteSettingsList.Count == 0)
        {
            Notification.Show("Agrega sabores al pedido");
            return;
        }

        List<TasteSettingInfo> info = new List<TasteSettingInfo>();

        int totalCount = 0;

        foreach (TasteSetting setting in tasteSettingsList)
        {
            int amount = int.Parse(setting.advInputAmount.Text);
            if (amount == 0)
                continue;

            totalCount += int.Parse(setting.advInputAmount.Text);
            info.Add(new TasteSettingInfo(setting.GetKey(), setting.data, amount));
        }

        // Remove notification
        if(Editting)
            AndroidNotificationCenter.CancelScheduledNotification(AppManager.RequestsDic[costumerName].notificationID);

        RequestData requestData = new RequestData(txtSelectedDate.text, txtSelectedTime.text, totalCount, float.Parse(advInputDeliveryCost.Text) , float.Parse(advInputFinalCost.Text), info, ProgramNotificationAndReturnID());

        if (Editting)
        {
            Editting = false;
            AppManager.OverwriteRequest(costumerName, requestData);
            Panel_Main.ChangeButtonValue(costumerName, requestData);
            Panel_RequestInformation.Reload(costumerName, requestData);

            Notification.Show("Guardado");
        }
        else
        {
            Panel_Main.AddRequestButton(costumerName, requestData);

            AppManager.SaveRequest(costumerName, requestData);

            Notification.Show("Pedido agregado");
        }


        Close(() =>
        {
            Restore();
        });
    }

    private int ProgramNotificationAndReturnID()
    {
        //
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidNotificationChannel channel = new AndroidNotificationChannel();
        channel.Id = advInputCostumerName.Text + "_channel";
        channel.Name = "Default Channel";
        channel.Description = "For Generic Notifications";
        channel.Importance = Importance.Default;

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        AndroidNotification notification = new AndroidNotification();




        notification.Title = $"(Recordatorio) Pedido para {advInputCostumerName.Text}";
        notification.SmallIcon = "icon_app_small";

        System.DateTime selectedTime = AppManager.ConvertStrToDateTime(txtSelectedDate.text, txtSelectedTime.text);

        if (selectedTime == System.DateTime.MinValue)
            Debug.Log("Is Min Value");

        if (System.DateTime.Now.Day == selectedTime.Day)
        {
            notification.FireTime = System.DateTime.Now.AddSeconds(1);
            notification.Text = $"Este pedido se entrega hoy a las {txtSelectedTime.text}";

        }
        else
        {
            notification.FireTime = selectedTime.AddDays(-1);
            notification.Text = $"Este pedido se entrega mañana a las {txtSelectedTime.text}";

        }

        int selectedID = AppManager.RequestsDic.Count + 1;


        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", selectedID);

        return selectedID;
#else
        return 0;
#endif
    }


}
