using UnityEngine;
using System;
using System.Collections;

public class Timer : MonoSingleton<Timer>
{
    // EXAMPLE: Timer.ExecuteNextFrame(() => { folder.rotation = Quaternion.Euler(0,0,45);});

    [ReadOnly]
    public int count;

    private string gameObjectName;

    void Update()
    {
        if (String.IsNullOrEmpty(gameObjectName))
        {
            gameObjectName = name;
        }

        name = gameObjectName + " : " + count;
    }
    public static void KillTimerIfNotNull(Coroutine victim)
    {
        if (victim == null)
            return;
        instance.DoKillTimer(victim);
    }
    public static void KillTimer(Coroutine victim)
    {
        if (victim == null)
            return;
        instance.DoKillTimer(victim);
    }

    public void DoKillTimer(Coroutine victim)
    {
        StopCoroutine(victim);
        if (count > 0)
        {
            count--;
        }
    }

    public static Coroutine WaitForCondition(Action function, Func<bool> condition)
    {
        return instance.DoWaitForConditionCoroutine(function, condition);
    }

    public Coroutine DoWaitForConditionCoroutine(Action function, Func<bool> condition)
    {
        return StartCoroutine(WaitForConditionCoroutine(function, condition));
    }

    public IEnumerator WaitForConditionCoroutine(Action function, Func<bool> condition)
    {
        count++;
        while (true)
        {
            if (condition())
            {
                function();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        count--;
        yield return new WaitForEndOfFrame();
    }

    public static Coroutine DelayedExecuteRepeating(Action function, float time)
    {
        return instance.DoDelayedExecuteCoroutine(function, time, repeating: true);
    }

    public static Coroutine DelayedExecute(Action function, float time, bool realTime = false)
    {
        if (realTime)
        {
            return instance.DoDelayedRealTimeExecuteCoroutine(function, time);
        }
        else
        {
            return instance.DoDelayedExecuteCoroutine(function, time);
        }
    }

    public static Coroutine ExecuteNextFrame(Action function)
    {
        return instance.DoExecuteNextFrameCoroutine(function);
    }

    public Coroutine DoDelayedExecuteCoroutine(Action function, float time, bool repeating = false)
    {
        if (repeating)
        {
            return StartCoroutine(DelayedExecuteRepeatingCoroutine(function, time));
        }

        return StartCoroutine(DelayedExecuteCoroutine(function, time));    
    }

    public IEnumerator DelayedExecuteCoroutine(Action function, float time)
    {
        count++;
        yield return new WaitForSeconds(time);
        function();
        count--;
    }

    public IEnumerator DelayedExecuteRepeatingCoroutine(Action function, float time)
    {
        count++;
        while (true)
        {
            yield return new WaitForSeconds(time);
            function();    
        }
    }

    public Coroutine DoDelayedRealTimeExecuteCoroutine(Action function, float time)
    {
        return StartCoroutine(DelayedRealTimeExecuteCoroutine(function, time));
    }

    public IEnumerator DelayedRealTimeExecuteCoroutine(Action function, float time)
    {
        count++;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(time));
        function();
        count--;
    }


    public Coroutine DoExecuteNextFrameCoroutine(Action function)
    {
        return StartCoroutine(ExecuteNextFrameCoroutine(function));
    }

    public IEnumerator ExecuteNextFrameCoroutine(Action function)
    {
        count++;
        yield return new WaitForEndOfFrame();
        function();
        count--;
    }

    public static void KillAll()
    {
        instance.StopAllCoroutines();
    }

}
