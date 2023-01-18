using UnityEngine;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using Helpers;

public class Popup_AddTaste : PopupBhv
{
    private static Popup_AddTaste Instance;

    [SerializeField]
    private AdvancedInputField advInputName;

    [SerializeField]
    private AdvancedInputField advInputCost;

    [SerializeField]
    private Button btnAddTaste;
    private TMPro.TextMeshProUGUI txtLastButton;

    [SerializeField]
    private Image imgIcon;
    private Sprite truffleSprite;
    [SerializeField] private Sprite ChocotejaSprite;

    [SerializeField]
    private FlexibleColorPicker colorPicker;

    private bool editing;

    private DessertType dessertType;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        truffleSprite = imgIcon.sprite;
        Instance = this;
        btnAddTaste.onClick.AddListener(OnAddTaste_Buttonclick);
        txtLastButton = btnAddTaste.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        imgIcon.color = colorPicker.color;
    }

    private void OnAddTaste_Buttonclick()
    {

        if (string.IsNullOrEmpty(advInputName.Text))
        {
            Notification.Show("Escribe el nombre del sabor");
            return;
        }

        if (!editing)
            if (AppManager.TrufflesTasteDic.ContainsKey(DessertKey.Create(advInputName.Text, dessertType)))
            {
                Notification.Show("Este sabor ya existe");
                return;
            }


        if (string.IsNullOrEmpty(advInputCost.Text))
        {
            Notification.Show("Añade un costo al sabor");
            return;
        }

        this.ActionAfterReturnedNull(() =>
        {

            DessertData truffleData = new DessertData(float.Parse(advInputCost.Text), colorPicker.color);

            if (editing)
            {
                editing = false;

                AppManager.OverwriteTaste(DessertKey.Create(advInputName.Text, dessertType), truffleData);
                Panel_SelectTastes.ChangeOptionValue(DessertKey.Create(advInputName.Text, dessertType), truffleData);

                Notification.Show("Guardado");
            }
            else
            {
                AppManager.SaveTaste(DessertKey.Create(advInputName.Text, dessertType), truffleData);
                Panel_SelectTastes.AddOption(DessertKey.Create(advInputName.Text, dessertType), truffleData);


                Notification.Show("Sabor agregado");
            }

        });


        Close(Restore);

    }

    public static void Show(DessertType dessertType)
    {
        Instance.dessertType = dessertType;

        switch (dessertType)
        {
            case DessertType.Truffle:
                Instance.imgIcon.sprite = Instance.truffleSprite;
                break;
            case DessertType.Chocoteja:
                Instance.imgIcon.sprite = Instance.ChocotejaSprite;

                break;
            default:
                break;
        }

        if (Instance.editing)
        {
            Instance.editing = false;

            Instance.Restore();

        }
        Instance.ShowPanel();
    }

    public static void Show(DessertKey key, DessertData truffleData)
    {
        Instance.InitEdit(key, truffleData);
        Instance.ShowPanel();
    }

    private void InitEdit(DessertKey key, DessertData truffleData)
    {
        editing = true;

        this.dessertType = key.dessertType;

        switch (dessertType)
        {
            case DessertType.Truffle:
                Instance.imgIcon.sprite = Instance.truffleSprite;
                break;
            case DessertType.Chocoteja:
                Instance.imgIcon.sprite = Instance.ChocotejaSprite;

                break;
            default:
                break;
        }

        advInputName.interactable = false;
        advInputName.Text = key.name;
        advInputCost.Text = truffleData.cost.ToString("0.00");

        Instance.txtLastButton.text = "Finalizar";

        colorPicker.color = truffleData.colorInfo.ToUnityColor();
    }

    private void OnClose_ButtonClick()
    {
        Close();
    }

    private void Restore()
    {
        Instance.advInputName.interactable = true;
        advInputName.Text = "";
        advInputCost.Text = "";
        Instance.txtLastButton.text = "Agregar Sabor";

        colorPicker.color = Color.white;
    }
}
