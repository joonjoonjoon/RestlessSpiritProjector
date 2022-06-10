using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoSingleton<SoundManager> {

    public AudioClip[] mismatch;
    public AudioClip[] bottling;
    public AudioClip[] thankyoumusic;
    public AudioClip[] trapmusic;
    public AudioClip[] voiceIsReady;
    public AudioClip[] voicePairCounter;
    public AudioClip[] voicePairRandomOther;
    public AudioClip[] voiceThankYou;
    public AudioClip[] voiceSpiritCaptured;
    public AudioClip[] voicePoweringDown;
    public AudioClip[] voiceWillRestart;
    public AudioClip[] voiceMissmatch;
    public AudioClip[] voiceOrbReleased;
    public AudioClip[] voiceMatchedOrbsTranscended;
    public AudioClip[] voiceMismatchedOrbsReleased;

    public AudioClip[] orbIdle;
    public AudioClip[] orbVoice;
    public AudioClip[] orbDespair;

    public AudioClip[] orbPairResolve;

    public AudioSource source;
    public AudioSource sourceOrbVoice;
    public AudioSource sourceCalmVoice;
    public AudioSource sourceBottle;

    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        sourceOrbVoice = CreateAudioSource("OrbVoice");
        sourceCalmVoice = CreateAudioSource("CalmVoice");
        sourceBottle = CreateAudioSource("bottle");
    }

    private AudioSource CreateAudioSource(string v)
    {
        var go = new GameObject(v);
        go.transform.parent = transform;
        var source = go.AddComponent<AudioSource>();
        return source;
    }

    public void PlayRandom(AudioClip[] list, AudioSource useSource = null)
    {
        var ID = UnityEngine.Random.Range(0, list.Length);
        PlayByID(list, ID, useSource);
    }

    public void PlayByID(AudioClip[] list, int ID, AudioSource useSource = null)
    {
        if (useSource == null) useSource = source;
        var sound = list[ID];
        if(sound!=null)
            useSource.PlayOneShot(sound);
    }

    public void Play(AudioClip clip, AudioSource useSource = null)
    {
        if (useSource == null) useSource = source;
        useSource.PlayOneShot(clip);
    }


    private Coroutine currentSequence;
    public void StopOrbSounds()
    {
        sourceCalmVoice.Stop();
        sourceOrbVoice.Stop();
        source.Stop();
        if (currentSequence != null)
            StopCoroutine(currentSequence);
    }

    public void PlayCatchSequence(int ID)
    {
        StopOrbSounds();
        currentSequence = StartCoroutine(CatchSequence(ID));
    }

    IEnumerator CatchSequence(int ID)
    {
        PlayRandom(voiceSpiritCaptured, sourceCalmVoice);
        while(sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        PlayByID(orbVoice, ID, sourceOrbVoice);
        while (sourceOrbVoice.isPlaying)
        {
            yield return null;
        }
        GhostsManager.instance.TriggerReleaseGhostAfterAudio(ID);
        yield return new WaitForSeconds(4);
        PlayByID(orbDespair, ID, sourceOrbVoice);
        while (sourceOrbVoice.isPlaying)
        {
            yield return null;
        }
        PlayRandom(voiceOrbReleased, sourceCalmVoice);
    }

    public void PlayMismatchSequence(int ID)
    {
        StopOrbSounds();
        currentSequence = StartCoroutine(MismatchSequence(ID));
    }

    IEnumerator MismatchSequence(int ID)
    {
        PlayRandom(mismatch, source);
        PlayRandom(voiceMissmatch, sourceCalmVoice);
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        PlayByID(orbDespair, ID, sourceOrbVoice);
        while (sourceOrbVoice.isPlaying)
        {
            yield return null;
        }
        PlayRandom(voiceMismatchedOrbsReleased, sourceCalmVoice);
    }

    public void PlayResolveSequence(int ID, int counter)
    {
        StopOrbSounds();
        currentSequence = StartCoroutine(ResolveSequence(ID, counter));
    }

    IEnumerator ResolveSequence(int ID,int counter)
    {
        //PlayRandom(mismatch, source);
        /*PlayRandom(voicePairRandomOther, sourceCalmVoice);
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }*/
        PlayByID(orbPairResolve, ID, sourceOrbVoice);
        while (sourceOrbVoice.isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        PlayRandom(voiceMatchedOrbsTranscended, sourceCalmVoice);
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        PlayByID(voicePairCounter, counter, sourceCalmVoice);
    }


    public void PlayEndgameSequence()
    {
        //StopOrbSounds();
        currentSequence = StartCoroutine(EndgameSequence());
    }

    IEnumerator EndgameSequence()
    {
        while (sourceOrbVoice.isPlaying)
        {
            yield return null;
        }
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        PlayRandom(thankyoumusic, source);
        yield return new WaitForSeconds(2);
        PlayByID(voicePairCounter,voicePairCounter.Length-1 , sourceCalmVoice);
        yield return new WaitForSeconds(5);
        PlayRandom(voiceThankYou, sourceCalmVoice);
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        while (source.isPlaying)
        {
            yield return null;
        }
        PlayRandom(voiceWillRestart, sourceCalmVoice);
        while (sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        GhostsManager.instance.OnEndGameSoundSequenceDone();
    }


    Tweener bottletween;
    public void PlayBottleSound()
    {
        if (bottletween != null) bottletween.Kill();
        bottletween = DOTween.To(() => sourceBottle.volume, (x) => sourceBottle.volume = x, 1, 0.1f);
        PlayRandom(bottling, sourceBottle);
        DebugExtension.Blip();
    }

    public void StopBottleSound()
    {
        if (bottletween != null) bottletween.Kill();
        bottletween = DOTween.To(()=>sourceBottle.volume, (x) => sourceBottle.volume = x, 0, 0.5f);
        DebugExtension.Blip();
    }

}
