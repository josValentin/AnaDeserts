using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using Helpers;

public class PopupBhv : Panel_Base
{
    private enum PopUpType
    {
        PopUp,
        Fade
    }

    [SerializeField]
    private Image imgPanel;

    [SerializeField]
    protected RectTransform window;
    private Image imgWindow;



    [SerializeField]
    private float timeTransition = 0.2f;
    [SerializeField]
    private float bgShownFadeValue = 0.85f;

    [SerializeField] private PopUpType popUpType;

    [SerializeField] private Graphic[] graphs;

    [SerializeField] private bool ModifyWindowAlpha = true;

    [SerializeField] protected bool checkGraphicsEveryShow;

    [SerializeField] protected bool UseGraphicSelected = false;

    protected virtual void Start()
    {

        if (imgPanel.GetComponent<Button>() != null)
            imgPanel.GetComponent<Button>().onClick.AddListener(() => Close());

        imgPanel.SetAlpha(0);

        switch (popUpType)
        {
            case PopUpType.PopUp:
                window.localScale = Vector2.zero;

                break;
            case PopUpType.Fade:
                imgWindow = window.GetComponent<Image>();
                if (!checkGraphicsEveryShow && !UseGraphicSelected)
                    graphs = imgWindow.GetComponentsInChildren<Graphic>();

                if (ModifyWindowAlpha)
                    imgWindow.SetAlpha(0);
                break;
            default:
                break;
        }

        canvas.enabled = false;

        //
    }

    protected void ShowPanel()
    {
        if (canvas.enabled)
            return;


        this.ActionAfterTime(0.1f, () =>
        {
            canvas.enabled = true;

            switch (popUpType)
            {
                case PopUpType.PopUp:
                    window.DOScale(Vector2.one, timeTransition);

                    break;
                case PopUpType.Fade:
                    if (ModifyWindowAlpha)
                        imgWindow.DOFade(1, timeTransition);

                    if (checkGraphicsEveryShow && !UseGraphicSelected)
                        graphs = GetComponentsInChildren<Graphic>();

                    for (int i = 0; i < graphs.Length; i++)
                    {
                        graphs[i].DOFade(1, timeTransition);
                    }

                    break;
                default:
                    break;
            }
            imgPanel.DOFade(bgShownFadeValue, timeTransition);


        });

    }

    public override void Close(Action onClose = null)
    {
        switch (popUpType)
        {
            case PopUpType.PopUp:
                window.DOScale(Vector2.zero, timeTransition);
                break;
            case PopUpType.Fade:
                if (ModifyWindowAlpha)
                    imgWindow.DOFade(0, timeTransition);

                for (int i = 0; i < graphs.Length; i++)
                {
                    graphs[i].DOFade(0, timeTransition);
                }
                break;
            default:
                break;
        }


        imgPanel.DOFade(0f, timeTransition).OnComplete(() =>
        {
            onClose?.Invoke();
            canvas.enabled = false;
        });
    }
}
