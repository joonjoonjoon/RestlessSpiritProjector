using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GlitchUIElementAnimation : MonoBehaviour {

    public float autoLoopDelay = -1; // -1 = disabled
    public AnimationType[] autoLoopTypes;


    public enum AnimationType{
        ScaleUpOnce,
        Jiggle,
        JiggleSmall,
        SqueezeJuice,
        Stop
    }

    private Sequence currentTweener;
    private FakeTransform startVals;
    private bool inited;


    void OnEnable()
    {
        
        if(autoLoopDelay >= 0)
        {
            StartCoroutine(autoLoop());
        }
    }



    IEnumerator autoLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(autoLoopDelay);
            if (gameObject.activeSelf)
            {
                foreach (var item in autoLoopTypes)
                {
                    Animate(item);
                }
            }
        }
    }

    void Init()
    {
        if (!inited)
        {
            startVals = transform.ToFakeTransform(true);
        }
        inited = true;
    }

    public void Animate(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.ScaleUpOnce:
                ScaleUpOnce();
                break;
            case AnimationType.Stop:
                Stop();
                break;
            case AnimationType.Jiggle:
                Jiggle();
                break;
            case AnimationType.JiggleSmall:
                Jiggle();
                break;
            case AnimationType.SqueezeJuice:
                SqueezeJuice();
                break;
            default:
                break;
        }
    }

    public void Stop()
    {
        currentTweener.Complete();
    }

    public void ScaleUpOnce()
    {
        Init();
        var seq = DOTween.Sequence();
        seq.Append(ScaleUp(0.15f,1.3f));
        seq.Append(ScaleDefault(0.3f));
        seq.Play();
        currentTweener = seq;
    }

    public void Jiggle()
    {
        Init();
        var seq = DOTween.Sequence();
        var t = 0.15f;
        var a = 15;
        seq.Append(RotateTo(t / 2f, -a));
        seq.Append(RotateTo(t, a));
        seq.Append(RotateTo(t, -a));
        seq.Append(RotateTo(t, a));
        seq.Append(RotateTo(t, 0));
        seq.Play();
        currentTweener = seq;
    }

    public void JiggleSmall()
    {
        Init();
        var seq = DOTween.Sequence();
        var t = 0.15f;
        var a = 7.5f;
        seq.Append(RotateTo(t / 2f, -a));
        seq.Append(RotateTo(t, a));
        seq.Append(RotateTo(t, -a));
        seq.Append(RotateTo(t, a));
        seq.Append(RotateTo(t, 0));
        seq.Play();
        currentTweener = seq;
    }


    public void SqueezeJuice()
    {
        Init();
        var seq = DOTween.Sequence();
        var t = 0.1f;
        seq.Append(ScaleNonUniform(t/2f, new Vector3(0.9f,1.25f,1)));
        seq.Append(ScaleNonUniform(t / 2f, new Vector3(1.25f, 0.9f, 1))); 
        seq.Append(ScaleDefault(t));
        seq.Play();
        currentTweener = seq;
    }

    private Tweener RotateTo(float time, float angle)
    {
        return transform.DORotate(transform.localEulerAngles.withZ(angle), time);
    }

    private Tweener ScaleNonUniform(float time, Vector3 multiplier)
    {
        return DOTween.To(() => transform.localScale,
            (x) => transform.localScale = x,
            Vector3.Scale(startVals.scale,multiplier),
            time);
    }

    private Tweener ScaleUp(float time, float multiplier)
    {
        return DOTween.To(() => transform.localScale,
            (x) => transform.localScale = x,
            startVals.scale * multiplier,
            time);
    }

    private Tweener ScaleDefault(float time)
    {
        return DOTween.To(() => transform.localScale,
            (x) => transform.localScale = x,
            startVals.scale,
            time);
    }
}
