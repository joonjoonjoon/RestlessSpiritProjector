using UnityEngine;
using System.Collections;

public class AudioQueuer : MonoSingleton<AudioQueuer>
{
    public static void PlayAudioOnNextBar(AudioSource toPlay, float barLength, float defaultBarLength)
    {
        AudioQueuer.instance.StartWaitForNextBar(toPlay, barLength, defaultBarLength);
    }

    private void StartWaitForNextBar(AudioSource toPlay, float barLength, float defaultBarLength)
    {
        StartCoroutine(WaitForNextBar(toPlay, barLength, defaultBarLength));
    }

    private IEnumerator WaitForNextBar(AudioSource toPlay, float barLength, float defaultBarLength)
    {
        if (barLength == 0)
        {
            barLength = defaultBarLength;
        }
        float timeTil2Secs = barLength - Time.time % barLength;
        yield return new WaitForSeconds(timeTil2Secs);
        toPlay.Play();
    }
}
