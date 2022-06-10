using UnityEngine;


public enum NoteColor
{
    white,
    blue,
    green,
    yellow,
    pink
}

public class InspectorNoteAttribute : PropertyAttribute
{
    public readonly string header;
    public readonly string message;
    public readonly Color color;

    public InspectorNoteAttribute(string header, string message = "")
    {
        this.header = header;
        this.message = message;
        this.color = Color.white;
    }

    public InspectorNoteAttribute(string header, NoteColor color)
    {
        this.header = header;
        this.message = "";
        this.color = NoteColorToColor(color);
    }

    public InspectorNoteAttribute(string header, string message, NoteColor color)
    {
        this.header = header;
        this.message = message;
        this.color = NoteColorToColor(color);
    }

    public InspectorNoteAttribute(string header, string message, string hexColor)
    {
        this.header = header;
        this.message = message;
        this.color = ColorExtension.hexToColor(hexColor);
    }

    private Color NoteColorToColor(NoteColor col)
    {
        Color color = Color.white;

        switch (col)
        {
            case NoteColor.blue:
                color = ColorExtension.hexToColor("007ACC");
                break;
            case NoteColor.green:
                color = Color.green;
                break;
            case NoteColor.yellow:
                color = Color.yellow;
                break;
            case NoteColor.pink:
                color = ColorExtension.hexToColor("EA81DC");
                break;
        }

        return color;
    }
}