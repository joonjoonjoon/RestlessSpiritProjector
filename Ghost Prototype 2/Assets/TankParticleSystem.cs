using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TankParticleSystem : MonoSingleton<TankParticleSystem> {
    private ParticleSystem ps;
    private Tweener tweener;
    public float startsize;
    public Color color;
    void Awake()
    {
        instance = this;
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var main = ps.main;
        main.startSize = new ParticleSystem.MinMaxCurve(startsize);
        main.startColor = new ParticleSystem.MinMaxGradient(Color.Lerp(main.startColor.color,color,0.1f));
    }

    public void StartMismatch()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; } , 1f, 2f);
        color = Color.Lerp(Color.white, Color.red,0.5f);
    }

    public void StopMismatch()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; }, 0f, 0.2f);
    }

    public void StartPair()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; }, 4f, 4f);
        color = Color.Lerp(Color.white, Color.magenta, 0.5f);
    }

    public void StopPair()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; }, 0f, 0.2f);
    }

    public void StartRegular()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; }, 0.5f, 2f);
        color = Color.white;
    }
    public void StopRegular()
    {
        if (tweener != null) tweener.Kill();
        DOTween.To(() => startsize, x => { startsize = x; }, 0f, 0.2f);
    }

}
