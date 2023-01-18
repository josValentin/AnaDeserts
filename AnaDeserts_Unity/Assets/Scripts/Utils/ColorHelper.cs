using UnityEngine;

namespace Helpers
{
    public static class ColorHelper
    {
        public static Color GetAlphaChanged(this Color color, float value)
        {
            return new Color(color.r, color.g, color.b, value);
        }

        public static void SetAlpha(this UnityEngine.UI.Image image, float value)
        {
            Color color = image.color;
            color.a = value;
            image.color = color;
        }

        public static void SetAlpha(this TMPro.TextMeshProUGUI textPro, float value)
        {
            Color color = textPro.color;
            color.a = value;
            textPro.color = color;
        }
    }
}

