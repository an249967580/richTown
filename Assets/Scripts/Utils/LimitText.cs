using UnityEngine;
using UnityEngine.UI;

public class LimitText {

    static string suffix = "...";

	public static void LimitAndSet(string text, Text tv, int maxWidth)
    {
        int suffixWidth = CalculateLengthOfText(suffix, tv);

        int length = CalculateLengthOfText(text, tv);
        if(length > maxWidth)
        {
            text = StripLength(text, tv, maxWidth - suffixWidth) + suffix;
        }
        tv.text = text;
    }

    public static void LimitAndSet(string text, Text tv, string suffix, int maxWidth)
    {
        int suffixWidth = CalculateLengthOfText(suffix, tv);

        int length = CalculateLengthOfText(text + suffix, tv);
        if(length > maxWidth)
        {
            text = StripLength(text, tv, maxWidth - suffixWidth) + suffix;
        }
        tv.text = text + suffix;
    }

    public static string Limit(string text, Text tv, int maxWidth)
    {
        int suffixWidth = CalculateLengthOfText(suffix, tv);

        int length = CalculateLengthOfText(text, tv);
        if (length > maxWidth)
        {
            return StripLength(text, tv, maxWidth - suffixWidth) + suffix;
        }
        return text;
    }

    static string StripLength(string text,Text tv, int maxWidth)
    {
        int totalLength = 0;
        Font font = tv.font;
        font.RequestCharactersInTexture(text, tv.fontSize, tv.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = text.ToCharArray();
        int i = 0;
        foreach(char c in arr)
        {
            font.GetCharacterInfo(c, out characterInfo, tv.fontSize);
            int newLength = totalLength + characterInfo.advance;
            if(newLength > maxWidth)
            {
                if (Mathf.Abs(newLength - maxWidth) > Mathf.Abs(maxWidth - totalLength))
                {
                    break;
                }
                else
                {
                    totalLength = newLength;
                    i++;
                    break;
                }
            }
            totalLength += characterInfo.advance;
            i++;
        }
        return text.Substring(0, i);
    }

    static int CalculateLengthOfText(string message, Text text)
    {
        int totalLength = 0;
        Font font = text.font;  //chatText is my Text component
        font.RequestCharactersInTexture(message, text.fontSize, text.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();

        char[] arr = message.ToCharArray();

        foreach (char c in arr)
        {
            font.GetCharacterInfo(c, out characterInfo, text.fontSize);

            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}
