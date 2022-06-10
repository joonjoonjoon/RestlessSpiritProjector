using Holoville.HOTween;
using System;

// Used to make Tweens or Sequences that don't actually do shit, where you just wanna get the oncomplete and seq. logic...

/*
    var seq = new Sequence(new SequenceParms());
    seq.Insert(1,HOTween.To(new Dummy(), 0, new TweenParms()
           .Prop("dummy",100)
           .OnComplete(()=>{Debug.Log("FART");})));
    seq.Play();
 */
public class Dummy
{
    public float dummy;

    public static float InsertDummyInSequence(Sequence sequence, Action action, float time)
    { 
        return sequence.Insert(time,
            HOTween.To(new Dummy(), 0, new TweenParms()
                .Prop("dummy", 100)
                .OnComplete(() =>
                    {
                        action();
                    })
            )
        );
    }

    public static float AppendDummyInSequence(Sequence sequence, Action action)
    {
        return sequence.Append(
            HOTween.To(new Dummy(), 0, new TweenParms()
                .Prop("dummy", 100)
                .OnComplete(() =>
                    {
                        action();
                    })
            )
        );
    }

}

