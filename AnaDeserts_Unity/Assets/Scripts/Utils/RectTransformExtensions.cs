using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public static void SetAnchorHeight(this RectTransform rect, float height)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
    }

    public static void AddAnchorHeight(this RectTransform rect, float heightToAdd)
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + heightToAdd);
    }

    public static void SetAnchorWidth(this RectTransform rect, float width)
    {
        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
    }

    public static void SetAnchorPosX(this RectTransform rect, float posX)
    {
        rect.anchoredPosition = new Vector2(posX, rect.anchoredPosition.y);
    }
    public static void SetAnchorPosY(this RectTransform rect, float posY)
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, posY);
    }
}
