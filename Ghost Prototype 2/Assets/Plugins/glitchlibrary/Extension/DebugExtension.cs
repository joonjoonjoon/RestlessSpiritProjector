using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System.Diagnostics;
using System.Reflection;
#endif

public class DebugExtension : MonoBehaviour
{
    public enum BlipType
    {
        None,
        Warning,
        Error
    }

    public static void Blip(string text)
    {
        DoBlip(null, Color.green, text, BlipType.None);
    }

    public static void Blip(Object context = null, string text = "",Color? color = null)
    {
        if (color == null) color = Color.green;
        DoBlip(context, (Color)color, text, BlipType.None);
    }

    public static void BlipError(Object context = null, string text = "",Color? color = null)
    {
        if (color == null) color = Color.red;
        DoBlip(context, (Color)color, text, BlipType.Error);
    }

    private static void DoBlip(Object context, Color color, string text, BlipType type)
    {
#if UNITY_EDITOR
        StackTrace st = new StackTrace();
        StackFrame sf = st.GetFrame(2);

        MethodBase currentMethodName = sf.GetMethod();
        string methodname = currentMethodName.Name;
        string file = currentMethodName.DeclaringType.ToString();
#else   
        string file = "";
        string methodname ="";
#endif

        switch (type)
        {
            case BlipType.None:
                UnityEngine.Debug.Log(ColorPad("[blip]", color) + " " + text + " " + file + ":" + methodname, context);
                break;
            case BlipType.Warning:
                UnityEngine.Debug.LogWarning(ColorPad("[blip]", color) + " " + text + " " + file + ":" + methodname, context);
                break;
            case BlipType.Error:
                UnityEngine.Debug.LogError(ColorPad("[blip]", color) + " " + text + " " + file + ":" + methodname, context);
                break;
            default:
                break;
        }
    }

    public static string ColorPad(string value, Color color)
    {
        return "<color=#" + ColorExtension.colorToHex(color) + ">" + value + "</color>";
    }
}
