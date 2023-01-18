using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TasteRequested : MonoBehaviour
{
    public RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtAmount;
    [SerializeField] private TextMeshProUGUI txtCost;
    [SerializeField] private Image imgIcon;

    [SerializeField] private Sprite chocotejaSpr;
    [SerializeField] private Sprite truffleSpr;

    public void Init(TasteSettingInfo info)
    {
        txtName.text = info.key.name;
        txtAmount.text = info.amount.ToString();
        txtCost.text = info.dessertData.cost.ToString("0.00");

        switch (info.key.dessertType)
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

        imgIcon.color = info.dessertData.colorInfo.ToUnityColor();
    }
}
