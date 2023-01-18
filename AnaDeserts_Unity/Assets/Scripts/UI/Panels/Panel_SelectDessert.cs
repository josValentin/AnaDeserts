using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_SelectDessert : PopupBhv
{
    [SerializeField] private Button btnTruffles;
    [SerializeField] private Button btnChocotejas;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        btnTruffles.onClick.AddListener(OnTrufflesButtonClick);
        btnChocotejas.onClick.AddListener(OnChocotejasButtonClick);

        EventCenter.AddListener(EventDefine.SelectDessert_ShowPanel, ShowPanel);
    }

    void OnTrufflesButtonClick()
    {
        Close(() => Panel_SelectTastes.Show(DessertType.Truffle));
    }

    void OnChocotejasButtonClick()
    {
        Close(() => Panel_SelectTastes.Show(DessertType.Chocoteja));
    }
}
