using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TasteOption : MonoBehaviour
{
    [HideInInspector] public DessertData data;

    [SerializeField] private HoldTimeButton holdButton;

    public string tasteName
    {
        get { return txtName.text; }
        set
        {
            txtName.text = value;
        }
    }

    public DessertType dessertType;

    public DessertKey GetKey()
    {
        return DessertKey.Create(tasteName, dessertType);
    }

    public string costStr
    {
        get { return txtCost.text; }
        set
        {
            txtCost.text = value;
        }
    }

    public bool isSelected
    {
        get { return toggle.isOn; }
        set
        {
            toggle.isOn = value;
        }
    }
    public Color iconColor
    {
        get { return imgIcon.color; }
        set
        {
            imgIcon.color = value;
        }
    }

    [SerializeField]
    private TextMeshProUGUI txtName;

    [SerializeField]
    private TextMeshProUGUI txtCost;

    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private Image imgIcon;
    [SerializeField] private Sprite truffleSpr;
    [SerializeField] private Sprite chocotejaSpr;
    public void Init(DessertKey key, DessertData data)
    {
        this.data = data;

        this.tasteName = key.name;

        this.dessertType = key.dessertType;
        switch (dessertType)
        {
            case DessertType.Truffle:
                imgIcon.sprite = truffleSpr;
                break;
            case DessertType.Chocoteja:
                imgIcon.sprite = chocotejaSpr;

                break;
            default:
                break;
        }

    }

    private void Start()
    {
        holdButton.onClick.AddListener(() => {

            toggle.isOn = !toggle.isOn;
        });
        holdButton.OnCompleteHold.AddListener(() =>
        {

            Popup_RequestOptions.Show(

                Popup_RequestOptions.OptionButtonSetting.Create("Editar", () =>
                {
                    Popup_AddTaste.Show(GetKey(), data);
                }),
                Popup_RequestOptions.OptionButtonSetting.Create("Eliminar", () =>
                {

                    AppManager.DeleteTaste(DessertKey.Create(tasteName, dessertType));
                    Panel_SelectTastes.DeleteOption(GetKey());
                    Notification.Show("Sabor Eliminado");
                })

                );

        });
    }

}
