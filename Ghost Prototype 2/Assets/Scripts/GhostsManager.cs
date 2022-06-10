using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

//[ScriptOrder(-100)]
public class GhostsManager : MonoSingleton<GhostsManager> {

    public List<GhostBehaviour> ghosts;
    public List<ToBeAvoided> avoidNodes;
    public RectTransform indicator;

    public Vector2 screenWorldDimensions;
    public Vector2 tankWorldDimensions;
    public float screenDimensionScaleDownX;
    public float screenDimensionScaleDownY;
    [AutoStoredValueManager.StoredValue]
    public float screenbottomBoundTweak;
    public float tankDimensionScaleDownX;
    public float tankDimensionScaleDownY;


    [AutoStoredValueManager.StoredValue]
    public float speed;
    public float turnSpeed;

    [AutoStoredValueManager.StoredValue]
    public float maxPairsAllowed;
    public float avoidanceTurnSpeed;
    public float ghostMinDistance;
    public Camera tankCamera;

    public string[] ghostNames;
    public int pairCounter;

    [System.Serializable]
    public class ghostPair
    {
        public int ghost1ID;
        public int ghost2ID;
    }
    public ghostPair[] ghostPairs;

    void Awake () {
        instance = this;
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            var ghost = t.GetComponent<GhostBehaviour>();
            if (ghost != null) ghosts.Add(ghost);
        }
        SetIndicator(null);


        ScreenResizeEvent.ResizeEvent += RecalcWorldDimensions;
        //RecalcWorldDimensions();
        avoidNodes = GameObject.FindObjectsOfType<ToBeAvoided>().ToList();

    }

    void OnDestroy()
    {
        ScreenResizeEvent.ResizeEvent -= RecalcWorldDimensions;
    }

    void Start()
    {
        var pairs = ghostPairs.ToList();
        while (pairs.Count > maxPairsAllowed)
        {
            pairs.Remove(pairs[Random.Range(0, pairs.Count)]);
        }

        foreach (var pair in pairs)
        {
            ghosts[pair.ghost1ID].Enable();
            ghosts[pair.ghost2ID].Enable();
        }

        foreach (var item in ghosts)
        {
            if(item.state== GhostBehaviour.states.disabled)
            {
                item.Disable();
            }
        }
    }

    void RecalcWorldDimensions(Vector2 screenDimensions)
    {
        var bl = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var tr = Camera.main.ScreenToWorldPoint(Vector3.zero.withX(screenDimensions.x).withY(screenDimensions.y));

        screenWorldDimensions = new Vector2(Mathf.Abs((bl-tr).x),Mathf.Abs((bl-tr).y));

        bl = tankCamera.ScreenToWorldPoint(Vector3.zero);
        tr = tankCamera.ScreenToWorldPoint(Vector3.zero.withX(tankCamera.pixelWidth).withY(tankCamera.pixelHeight));

        tankWorldDimensions = new Vector2(Mathf.Abs((bl - tr).x), Mathf.Abs((bl - tr).y));
    }


    void Update()
    {
        UpdateGhostCollisionAvoidance();

        int i = 0;
        foreach (var ghost in ghosts)
        {
            if(ghost.state == GhostBehaviour.states.world || ghost.state == GhostBehaviour.states.tank)
            {
                ghost.currentID = i;
                i++;
            }
        }
    }

    void UpdateGhostCollisionAvoidance()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            if (Vector3.Distance(ghosts[i].transform.position, ghosts[i].destination.ToVector3().withZ(ghosts[i].transform.position.z)) < ghostMinDistance)
            {
                //Debug.Log("TOO CLOSE TO DEST");

                ghosts[i].ChangeDestination();
            }

            if (ghosts[i].state != GhostBehaviour.states.world)
                continue;
            for (int j = 0; j < ghosts.Count; j++)
            {
                if (ghosts[j].state != GhostBehaviour.states.world)
                    continue;
                if (i == j) continue;

                
                if (Vector3.Distance(ghosts[i].transform.position, ghosts[j].transform.position) < ghostMinDistance)
                {
                    //Debug.Log("TOO CLOSE");
                    ghosts[i].ChangeDestination();
                    var awayAngle =
                        Vector3Extension.Angle2D(ghosts[i].transform.position, ghosts[j].transform.position) + 180;
                    ghosts[i].angle = Mathf.LerpAngle(ghosts[i].angle, awayAngle, avoidanceTurnSpeed * i/ghosts.Count * Time.deltaTime);
                }
            }

            for (int j = 0; j < avoidNodes.Count; j++)
            {
                if (Vector3.Distance(ghosts[i].transform.position, avoidNodes[j].transform.position) < avoidNodes[j].radius)
                {
                    //Debug.Log("TOO CLOSE TO NODE");
                    ghosts[i].ChangeDestination();
                    var awayAngle =
                        Vector3Extension.Angle2D(ghosts[i].transform.position, avoidNodes[j].transform.position) + 180;
                    ghosts[i].angle = Mathf.LerpAngle(ghosts[i].angle, awayAngle, avoidanceTurnSpeed * Time.deltaTime);
                }
            }


  

            if (IsOutOfBounds(ghosts[i]))
            {
                //Debug.Log("ISOUTOFBOUNDS");

                ghosts[i].ChangeDestination();
                ghosts[i].angle = Vector3Extension.Angle2D(ghosts[i].transform.position,
                    ghosts[i].destination.ToVector3());
            }
        }
    }

    public int GetCurrentActiveGhostWithDelay()
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            var ghost = ghosts[i];
            if(ghost.state== GhostBehaviour.states.world && !ghost.isShowingWithDelay)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetIndicator(GhostBehaviour ghost)
    {
        indicator.gameObject.SetActive(true);
        if (ghost != null)
            indicator.transform.position = indicator.transform.position.withX(ghost.transform.position.x);
        else
            indicator.gameObject.SetActive(false);
    }

    public Vector2 GetRandomPositionWorld()
    {
        var x = Random.value - 0.5f;
        var y = Random.value - 0.5f;

        x = Mathf.Clamp(x * 1.5f, -0.5f, 0.5f) * screenDimensionScaleDownX;
        y = Mathf.Clamp(y * 1.5f, -0.5f, 0.5f) * screenDimensionScaleDownY;

        x = x * screenWorldDimensions.x;
        y = y * screenWorldDimensions.y;

        return new Vector2(x, y);
    }

    public Vector2 GetRandomPositionTank()
    {
        /// TODO: DEFINE
        var x = Random.value - 0.5f;
        var y = Random.value - 0.5f;

        x = Mathf.Clamp(x * 1.5f, -0.5f, 0.5f) * tankDimensionScaleDownX;
        y = Mathf.Clamp(y * 1.5f, -0.5f, 0.5f) * tankDimensionScaleDownY;

        x = x * tankWorldDimensions.x + tankCamera.transform.position.x;
        y = y * tankWorldDimensions.y + tankCamera.transform.position.y;

        return new Vector2(x, y);
    }

    public bool IsOutOfBounds(GhostBehaviour ghost)
    {
        if (ghost.state == GhostBehaviour.states.world)
        {
            var left = ghost.transform.position.x < -screenWorldDimensions.x / 2f;
            var right = ghost.transform.position.x > screenWorldDimensions.x / 2f;
            var up = ghost.transform.position.y < (-screenWorldDimensions.y / 2f) * screenbottomBoundTweak;
            var down = ghost.transform.position.y > screenWorldDimensions.y / 2f;
            return left || right || up || down;
        }
        else if (ghost.state == GhostBehaviour.states.tank)
        {
            var pos = tankCamera.transform.position.withZ(0) - ghost.transform.position.withZ(0);
            var left = pos.x < -tankWorldDimensions.x / 2f;
            var right = pos.x > tankWorldDimensions.x / 2f;
            var up = pos.y < -tankWorldDimensions.y / 2f;
            var down = pos.y > tankWorldDimensions.y / 2f;
            return left || right || up || down;
        }
        return false;
    }

    public void CatchGhost(GhostBehaviour ghost)
    {
        ghost.Catch();
        ghost.transform.position = GetRandomPositionTank().ToVector3().withZ(ghost.transform.position.z);
        ghost.Appear();
        ghost.ChangeDestination(true);

        DoCheckAllGhostsInTank(ghost);
    }


    public void ReleaseGhost(GhostBehaviour ghost)
    {
        ghost.Release();
        ghost.transform.position = GetRandomPositionWorld().ToVector3().withZ(ghost.transform.position.z);
        ghost.Appear();
        ghost.ChangeDestination(true);
    }

    public void TriggerReleaseGhostAfterAudio(int ID)
    {
        ReleaseGhost(ghosts[ID]);
    }

    public void ResolveGhost(GhostBehaviour ghost)
    {
        ghost.Resolve();
    }
    public void DisableGhost(GhostBehaviour ghost)
    {
        ghost.Disable();
    }


    private void DoCheckAllGhostsInTank(GhostBehaviour lastAdded)
    {
        var tanked = new List<GhostBehaviour>();
        foreach (var ghost in ghosts)
        {
            if(ghost.state == GhostBehaviour.states.tank)
            {
                tanked.Add(ghost);
            }
        }

        if (tanked.Count == 1)
        {
            SoundManager.instance.PlayCatchSequence(tanked[0].ID);
            //SoundManager.instance.PlayByID(SoundManager.instance.orbVoice, tanked[0].ID);
        }
        else if (tanked.Count >= 2)
        {
            bool paired = false;
            ghostPair actualPair = null;
            foreach (var pair in ghostPairs)
            {
                int tankedGhostsInPair = 0;
                foreach (var tankedGhost in tanked)
                {
                    var id = ghosts.IndexOf(tankedGhost);
                    if (id == pair.ghost1ID || id == pair.ghost2ID)
                    {
                        tankedGhostsInPair++;
                    }
                }

                if (tankedGhostsInPair >= 2)
                {
                    paired = true;
                    actualPair = pair;
                    break;
                }
            }

            if (paired)
            {
                // RESOLVE!!!!
                StartCoroutine(ResolveSequence(tanked, actualPair));
            }
            else
            {
                // MISMATCH!!!
                StartCoroutine(MismatchSequence(tanked, lastAdded));
            }
        }
    }

    IEnumerator MismatchSequence(List<GhostBehaviour> tanked, GhostBehaviour lastAdded)
    {
        GameManager.instance.freezeAllDetection = true;

        TankParticleSystem.instance.StartMismatch();

        yield return new WaitForSeconds(1.5f);
        
        foreach (var tankedGhost in tanked)
        {
            if (tankedGhost != lastAdded)
                SoundManager.instance.PlayMismatchSequence(tankedGhost.ID);
        }

        while (SoundManager.instance.sourceCalmVoice.isPlaying)
        {
            yield return null;
        }
        while (SoundManager.instance.sourceOrbVoice.isPlaying)
        {
            yield return null;
        }


        foreach (var tankedGhost in tanked)
        {
            tankedGhost.Disappear();
        }
        yield return new WaitForSeconds(2f);

        foreach (var tankedGhost in tanked)
        {
            ReleaseGhost(tankedGhost);
        }
        

        yield return null;
        TankParticleSystem.instance.StopMismatch();
        GameManager.instance.freezeAllDetection = false;
    }


    IEnumerator ResolveSequence(List<GhostBehaviour> tanked, ghostPair actualPair)
    {
        GameManager.instance.freezeAllDetection = true;
        TankParticleSystem.instance.StartPair();


        yield return new WaitForSeconds(1.2f);

        SoundManager.instance.PlayResolveSequence(System.Array.IndexOf(ghostPairs, actualPair), pairCounter);
        pairCounter++;
        yield return new WaitForSeconds(2);
        while (SoundManager.instance.sourceOrbVoice.isPlaying)
        {
            yield return null;
        }

        foreach (var tankedGhost in tanked)
        {
            tankedGhost.Disappear();
        }
        yield return new WaitForSeconds(2);
        foreach (var tankedGhost in tanked)
        {
            ResolveGhost(tankedGhost);
        }

        var ghostsleft = 0;
        foreach (var ghost in ghosts)
        {
            if (ghost.state == GhostBehaviour.states.world)
                ghostsleft++;
        }

        TankParticleSystem.instance.StopPair();

        // DO ENDGAME!!!
        if (ghostsleft == 0)
        {
            GameManager.instance.freezeAllDetection = true;
            SoundManager.instance.PlayEndgameSequence();
            yield return new WaitForSeconds(5);
        }
        else
        {
            GameManager.instance.freezeAllDetection = false;
        }

        yield return null;
    }

    public void OnEndGameSoundSequenceDone()
    {
        WorldParticles.instance.KillmeAndRestart();
    }
}
