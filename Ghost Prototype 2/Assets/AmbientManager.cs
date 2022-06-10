using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AmbientManager : MonoSingleton<AmbientManager> {

    void Awake()
    {
        instance = this;

    }
    private List<AudioSource> sources;
    private List<AmbientSount> ambientsounds;
    private List<AudioSource> listening;

    void Start()
    {

        listening = GetComponentsInChildren<AudioSource>().ToList();
        listening.AddRange(GetComponents<AudioSource>().ToList());

        ambientsounds = new List<AmbientSount>();
        sources = new List<AudioSource>();
        for (int i = 0; i < SoundManager.instance.orbIdle.Length; i++)
        {
            var clip = SoundManager.instance.orbIdle[i];
            if (clip == null) continue;
            var source = CreateAudioSource(clip.name);
            sources.Add(source);
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
    }

    private AudioSource CreateAudioSource(string v)
    {
        var go = new GameObject(v);
        go.transform.parent = transform;
        var source = go.AddComponent<AudioSource>();
        var ambientsount= go.AddComponent<AmbientSount>();
        ambientsounds.Add(ambientsount);
        return source;
    }

    void Update()
    {
        bool isplaying = false;
        foreach (var source in listening)
        {
            if (source.isPlaying) isplaying = true;
        }


        foreach (var source in ambientsounds)
        {
            if( GhostsManager.instance.ghosts[ambientsounds.IndexOf(source)].state != GhostBehaviour.states.world)
            {
                source.off = true;
            }
            else
            {
                source.off = false;
            }

            if (isplaying )
            {
                source.mute = true;
            }
            else
            {
                source.mute = false;
            }
        }

    }

}
