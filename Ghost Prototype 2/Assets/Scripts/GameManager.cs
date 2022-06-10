using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoSingleton<GameManager> {

    [AutoStoredValueManager.StoredValueAttribute]
    public float lightThresholdUp;
    [AutoStoredValueManager.StoredValueAttribute]
    public float pingSpeed;
    [AutoStoredValueManager.StoredValueAttribute]
    public float timeOutAfterLoop;
    [AutoStoredValueManager.StoredValueAttribute]
    public float signalDelay;
    public float initialDelay=0.3f;
    public float finaldelay= 0.3f;

    public Transform tankCenter;

    private Coroutine blinkLoopCoroutine;
    private bool isRunningDetectLoop;
    public bool graphPaused;

    public float GhostDebugLineYOn;
    public float GhostDebugLineYOff;
    public float GhostDebugLineY2On;
    public float GhostDebugLineY2Off;

    public bool freezeAllDetection;

    void Awake()
    {
        instance = this;
    }

	void Start () {
        
        StartCoroutine(DebugGraphLoop());
        StartCoroutine(DetectLoop());
    }

    void Update()
    {
        if (overThreshold())
        {
            StartBlinkLoop();
        }

        if (Input.GetKey(KeyCode.A))
        {
            lightThresholdUp -= 0.01f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            lightThresholdUp += 0.01f;
        }
    }

    public void StartBlinkLoop()
    {
        if (!isRunningDetectLoop && !freezeAllDetection)
        {
            blinkLoopCoroutine = StartCoroutine(BlinkLoop());
        }
    }

    private bool overThreshold()
    {
        return ArduinoManager.lightSensorValue > lightThresholdUp;
    }

    private bool underThreshold()
    {
        return ArduinoManager.lightSensorValue < lightThresholdUp;
    }

    public Dictionary<int, int> caughtGhostDictionary;
    IEnumerator DetectLoop()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();

            if (isRunningDetectLoop)
            {
                if (underThreshold())
                {
                    var detectedGhostID = GhostsManager.instance.GetCurrentActiveGhostWithDelay();
                    if (!caughtGhostDictionary.ContainsKey(detectedGhostID))
                        caughtGhostDictionary.Add(detectedGhostID, 0);
                    caughtGhostDictionary[detectedGhostID] += 1;
                }
            }
        }
    }

    void CancelBlinkLoop()
    {
        ResetAllGhosts();
        GhostsManager.instance.SetIndicator(null);
        StopCoroutine(blinkLoopCoroutine);
        isRunningDetectLoop = false;
    }

    IEnumerator BlinkLoop()
    {
        SoundManager.instance.PlayBottleSound();
        Shake.instance.shakeAmount = 0.1f;
        isRunningDetectLoop = true;
        ResetAllGhosts();
        Debug.Log("#loop: starting detect loop");
        caughtGhostDictionary = new Dictionary<int, int>();
        yield return new WaitForSeconds(initialDelay);
        for (int i = 0; i < GhostsManager.instance.ghosts.Count; i++)
        {
            
            var ghost = GhostsManager.instance.ghosts[i];
            if (ghost.state == GhostBehaviour.states.world)
            {
                ghost.Hide();
                yield return new WaitForSeconds(pingSpeed);
                ghost.Show();
            }
        }

        yield return new WaitForSeconds(initialDelay);

        // DO CATCH LOGIC
        int highestVal = 0;
        int highestKey = -1;
        string debuginfo = "Detection info: ";
        if (caughtGhostDictionary.Keys.Count <= 3)
        {
            foreach (var key in caughtGhostDictionary.Keys)
            {
                if (key == -1) continue;
                debuginfo += "[Ghost " + GhostsManager.instance.ghosts[key].currentID + "]: " + caughtGhostDictionary[key] + ",";
                if (caughtGhostDictionary[key] > highestVal)
                {
                    highestKey = key;
                    highestVal = caughtGhostDictionary[key];
                }

            }
        }

        // actually catch        
        var caughtGhost = highestKey;
        if (caughtGhost >= 0)
        {
            Debug.Log("#loop: caught ghost! " + caughtGhost + "   (" + debuginfo + ")");
            TankParticleSystem.instance.StartRegular();
            var ghost = GhostsManager.instance.ghosts[caughtGhost];
            ghost.Disappear();
            DOTween.To(() => Shake.instance.shakeAmount, x => Shake.instance.shakeAmount = x, 3, 1).OnComplete(() => { Shake.instance.shakeAmount = 0f; });
            yield return new WaitForSeconds(1);
            GhostsManager.instance.CatchGhost(ghost);
            yield return new WaitForSeconds(1);
            TankParticleSystem.instance.StopRegular();
        }

        yield return new WaitForSeconds(timeOutAfterLoop);
        Debug.Log("#loop: ending detect loop");

        SoundManager.instance.StopBottleSound();
        CancelBlinkLoop();
    }

    private void CatchGhost(int i)
    {
        GhostsManager.instance.CatchGhost(GhostsManager.instance.ghosts[i]);
    }

    private void EnableAllGhosts()
    {
        foreach (var item in GhostsManager.instance.ghosts)
        {
            item.Show();
        }
    }

    private void DisableAllGhosts()
    {
        foreach (var item in GhostsManager.instance.ghosts)
        {
            item.Hide();
        }
    }

    private void ResetAllGhosts()
    {
        foreach (var item in GhostsManager.instance.ghosts)
        {
            item.Reset();
        }
    }


    IEnumerator DebugGraphLoop()
    {
        var frequency = 60f; // 60 fps;
        while (true)
        {
            var i = 0;

            var color1 = Color.white.SetA(0.5f);
            //var color2 = Color.yellow.SetA(0.5f);
            var show2 = ArduinoManager.lightSensorValue > lightThresholdUp;
            GlitchDebugGraph.Add("lightsignal", ArduinoManager.lightSensorValue, 100,  color1, 0.25f, false, false);
            GlitchDebugGraph.Add("lightsignal2", (show2?ArduinoManager.lightSensorValue: 0), 100, color1, 0.25f, false, false);

            foreach (var ghost in GhostsManager.instance.ghosts)
            {
                if (ghost.state != GhostBehaviour.states.disabled)
                {
                    GlitchDebugGraph.Add("ghostdelay" + i, (ghost.isShowingWithDelay ? GhostDebugLineY2On : GhostDebugLineY2Off), 100, ghost.color.SetA(1f), 1, false, true);
                }
                i++;
            }
            
            foreach (var ghost in GhostsManager.instance.ghosts)
            {
                if (ghost.state != GhostBehaviour.states.disabled)
                {
                    GlitchDebugGraph.Add("ghost" + i, (ghost.isShowing ? GhostDebugLineYOn : GhostDebugLineYOff), 100, ghost.color.SetA(0.5f), 1, false, true);
                }
                i++;
            }

            while(graphPaused)
            {
                yield return new WaitForSeconds(1 / frequency);
            }

            yield return new WaitForSeconds(1 / frequency);
        }
    }
}
