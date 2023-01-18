using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInputFieldPlugin;
using TMPro;
using UnityEngine.UI;

public class TasteSetting : MonoBehaviour
{
    [SerializeField] private HoldTimeButton holdButton;
    public string tasteName {
        get { return txtName.text; }
        set { txtName.text = value; }
    }

    public DessertType dessertType;

    public DessertKey GetKey()
    {
        return DessertKey.Create(tasteName, dessertType);
    }

    [HideInInspector] public float lastValue;

    public RectTransform rectTransform;
    public AdvancedInputField advInputAmount;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtCost;
    [SerializeField] private Image imgIcon;

    [HideInInspector]public DessertData data;

    [SerializeField] private Sprite truffleSpr;
    [SerializeField] private Sprite chocotejaSpr;

    public void Init(DessertKey key, DessertData data)
    {
        txtName.text = key.name;
        dessertType = key.dessertType;
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
        txtCost.text = data.cost.ToString("0.00");
        imgIcon.color = data.colorInfo.ToUnityColor();

        this.data = data;

        holdButton.OnCompleteHold.AddListener(() => {
            Popup_RequestOptions.Show("Remover de la lista", () => {
                Panel_AddRequest.RemoveTasteSetting(key);
            });
        });
    }
}
