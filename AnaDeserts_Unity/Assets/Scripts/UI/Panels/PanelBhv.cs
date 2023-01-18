using DG.Tweening;
using Helpers;
using UnityEngine;

public class PanelBhv : Panel_Base
{
    [SerializeField] private RectTransform panelRect;

    [SerializeField]
    private float timeTransition = 0.22f;

    float targetOutScreen;

    private enum Orientation
    {
        Horizontal,
        Vertical
    }

    [SerializeField]private bool negative;

    [SerializeField] private Orientation orientaion;

    protected virtual void Start()
    {
        SetStartPos();
    }

    protected void ShowPanel()
    {
        if (canvas.enabled)
            return;

        canvas.enabled = true;

        this.ActionAfterTime(0.1f, () => {

            switch (orientaion)
            {
                case Orientation.Horizontal:
                    panelRect.DOAnchorPosX(0, timeTransition);

                    break;
                case Orientation.Vertical:
                    panelRect.DOAnchorPosY(0, timeTransition);

                    break;
            }


        });
    }

    public override void Close(System.Action onClose = null)
    {
        switch (orientaion)
        {
            case Orientation.Horizontal:
                panelRect.DOAnchorPosX(targetOutScreen, timeTransition).OnComplete(() =>
                {
                    onClose?.Invoke();
                    canvas.enabled = false;
                });
                break;
            case Orientation.Vertical:
                panelRect.DOAnchorPosY(targetOutScreen, timeTransition).OnComplete(() =>
                {
                    onClose?.Invoke();
                    canvas.enabled = false;
                });
                break;
            default:
                break;
        }

    }

    private void SetStartPos()
    {
        switch (orientaion)
        {
            case Orientation.Horizontal:
                targetOutScreen = Screen.width / canvas.scaleFactor;

                if (negative)
                    targetOutScreen *= -1;

                panelRect.anchoredPosition = new Vector2(targetOutScreen, 0);

                break;
            case Orientation.Vertical:
                targetOutScreen = Screen.height / canvas.scaleFactor;

                if (negative)
                    targetOutScreen *= -1;

                panelRect.anchoredPosition = new Vector2(0, targetOutScreen);

                break;
            default:
                break;
        }

        this.ActionAfterReturnedNull(() => {
            canvas.enabled = false;
        });

    }
}
