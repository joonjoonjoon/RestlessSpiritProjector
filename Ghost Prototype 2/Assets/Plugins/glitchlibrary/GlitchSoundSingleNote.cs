using UnityEngine;
using System.Collections;

[System.Serializable]
public class GlitchSoundSingleNote
{
    public GlitchSoundNotes note;
    public int octave;
    public float GetPitchValue()
    {
        //Debug.Log("Note: " + note + " Octave: " + octave);
        return Mathf.Pow(1.05946f, (int)note + (octave - 1) * 12);
    }
}
