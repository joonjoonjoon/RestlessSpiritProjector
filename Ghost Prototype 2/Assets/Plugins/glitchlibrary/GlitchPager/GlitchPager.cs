using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

//NOTE: Background color update needs to happen before levelMenu does it's thing, for Nightmode to load properly.
[ScriptOrder(-1)]
public class GlitchPager : MonoBehaviour
{
    [InspectorNote("Pager Settings", NoteColor.pink)]
    public bool _settings;

    public bool enableFlicking;
    public bool overrideDisableAllInput;
    // used when there's multiple pagers...
    public GlitchPager overrideUseIndexFrom;
    // used when there's multiple pagers...
    public float bounceLimit;
    public float flickSensitivityMaxTime;
    // in time...
    public float flickSensitivityMinDist;
    // in procent dist of Screen.width...
    private float canvasWidth;
    private float worldScreenWidth;
    private float screenWidth;
    public float snapSpeed;
    public EaseType snapEaseType;
    public UnityEvent onBeforeChangePage;
    public UnityEvent onAfterChangePage;
    public Action onBeforeChangePageAction;
    public Action onAfterChangePageAction;
    public bool autoEnableAllPages;
    public bool useLeftOverflow;
    public bool useRightOverflow;
    public int OpenOnPage;
    public int leftBound;
    // in case 1st page is hidden or inaccessible

    [InspectorNote("Runtime Settings", NoteColor.white)]
    public bool _runtimesettings;

    public bool allowInput;
    private bool allowMovement;
    public bool allowArrows;
    public int currentIndex;
    public int lastIndex;


    [InspectorNote("References", NoteColor.green)]
    public bool _references;

    public Canvas parentCanvas;
    public RectTransform Pages;
    public RectTransform Dots;
    public GameObject DotPrefab;
    public RectTransform ActiveDot;
    public float DotSpacing;
    public float dotLerpSpeed;
    public Camera moveCam;
    public CanvasGroup leftArrow;
    public CanvasGroup rightArrow;
    public Transform[] deadzoneFolders;
    // private stuff

    private float lastTouchDistance;
    private int previousFrameIndex;
    private Vector2 touchDelta;
    private Queue<Vector2> touchDeltaHistory;
    private float touchStartedAtTime;

    private int startedOnPage;
    private Tweener cameraTweener;
    private bool dragging;
    [SerializeField]
    [ReadOnly]
    private bool hasArrived;
    [SerializeField]
    [ReadOnly]
    private GlitchPagerDeadzone[] Deadzones;
    // don't allow starting a press here

    #if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
    private Vector2 lastMousePosition;
    // delta isn't automatically calculated, so we need this
    #else
    private float deltaTimeIgnoreScale;         // actual touches use DeltaTime, which is connected to Timescale, which we won't ever want here. This uses actual time diff to bypass this.
    private float lastRecordedTime;
#endif

    void Awake()
    {
        hasArrived = true;
    }

    void Start()
    {
        var deadzonesList = new List<GlitchPagerDeadzone>();
        foreach (var folder in deadzoneFolders)
        {
            deadzonesList.AddRange(folder.GetComponentsInChildren<GlitchPagerDeadzone>());
        }
        Deadzones = deadzonesList.ToArray();

        parentCanvas = GetComponentInParent<Canvas>();
        parentCanvas.renderMode = RenderMode.WorldSpace;

        Reset();

        
    }

    public void Reset()
    {
        touchDeltaHistory = new Queue<Vector2>(4);
        PlacePages();
        PlaceDots();

        Show(OpenOnPage);

        UpdateArrows();
    }

    private void PlacePages()
    {
        
        canvasWidth = (Pages.parent as RectTransform).sizeDelta.x;
        screenWidth = Screen.width;

        if (canvasWidth == 0)
        {
            canvasWidth = (parentCanvas.transform as RectTransform).sizeDelta.x;
        }

        // set overflow backgrounds
        if (useLeftOverflow)
        {
            var page = Pages.GetChild(0);
            page.SetParent(Pages.parent, true);
            page.transform.localPosition = new Vector3(canvasWidth * -1, 0, 0);
        }
        if (useRightOverflow && Pages.childCount > 1)
        {
            var page = Pages.GetChild(Pages.childCount - 1);
            page.SetParent(Pages.parent, true);
            page.transform.localPosition = new Vector3(canvasWidth * Pages.childCount, 0, 0);
        }

        // space pages
        for (int i = 0; i < Pages.childCount; i++)
        {
            Transform page = Pages.GetChild(i);
            page.transform.localPosition = new Vector3(canvasWidth * i, 0, 0);
            if (autoEnableAllPages)
                page.gameObject.SetActive(true);
        }

        // calc screen width
        worldScreenWidth = Mathf.Abs(moveCam.ScreenToWorldPoint(Vector3.zero.withZ(moveCam.transform.position.z)).x - moveCam.ScreenToWorldPoint(new Vector3(Screen.width, 0, moveCam.transform.position.z)).x);
        if (worldScreenWidth == 0)
        {
            Debug.Log("Warning, I recorded a screen width of 0. This should never happen. Is the Pager camera set to perspective?");
        }
        else if (!moveCam.orthographic)
        {
            Debug.Log("Warning, Pager camera set to perspective. This might have unexpected consequences.");
        }

        allowMovement = Pages.childCount > 1;

    }

    private void PlaceDots()
    {
        if (Pages.childCount <= 1)
        {
            Dots.gameObject.SetActive(false);
            ActiveDot.gameObject.SetActive(false);
            DotPrefab.gameObject.SetActive(false);
        }
        else
        {
            // enable prefab etc...
            DotPrefab.gameObject.SetActive(true);
            Dots.gameObject.SetActive(true);
            ActiveDot.gameObject.SetActive(true);

            // destroy existing
            Dots.Clear();

            // copy prefab
            float dotWidth = (DotPrefab.transform as RectTransform).sizeDelta.x;
            float dotBarWidth = Pages.childCount * dotWidth + (Pages.childCount - 1) * DotSpacing;
            float x = -dotBarWidth / 2 + dotWidth / 2;
            for (int i = 0; i < Pages.childCount; i++)
            {
                GameObject dot = Instantiate(DotPrefab) as GameObject;
                dot.transform.SetParent(Dots, false);
                dot.transform.localPosition = Vector3.right * x;
                x += dotWidth + DotSpacing;
                if (i == 0)
                    ActiveDot.transform.localPosition = dot.transform.localPosition;
            }

            // disable prefab
            DotPrefab.gameObject.SetActive(false);
        }
    }

    public void MoveDotToCurrent()
    {
        HOTween.To(ActiveDot, dotLerpSpeed, new TweenParms()
            .Prop("localPosition", Dots.GetChild(currentIndex).localPosition)
            .UpdateType(UpdateType.TimeScaleIndependentUpdate)
            .OnComplete(() =>
                {
                    onAfterChangePage.Invoke();
                    if (onAfterChangePageAction != null)
                        onAfterChangePageAction.Invoke();
                })
        );
    }

    void Update()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
#else
        deltaTimeIgnoreScale = Time.realtimeSinceStartup - lastRecordedTime;
#endif
        if (overrideDisableAllInput)
            return;

        var detectIndexChanged = currentIndex;

        if (allowMovement && allowInput)
        {
            // do swipe logic
            if (TouchOrMousePressed())
            {
                EnqueueTouchDelta();
                if (touchDeltaHistory.Count >= 1000)
                {
                    touchDeltaHistory.Dequeue();
                }

                if (TouchOrMouseBegan())
                {
                    UpdateTouchBegan();
                }
                if (TouchOrMouseMoved())
                {
                    UpdateTouchMoved();
                }
                if (TouchOrMouseEnded())
                {
                    UpdateTouchEnded();
                }
            }

            if (currentIndex != previousFrameIndex)
            {
                onBeforeChangePage.Invoke();
                if (onBeforeChangePageAction != null)
                    onBeforeChangePageAction.Invoke();
                MoveDotToCurrent();
            }
            previousFrameIndex = currentIndex;
        }
        
        if (currentIndex != detectIndexChanged)
        {
            lastIndex = detectIndexChanged;
        }

#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
#else
        lastRecordedTime = Time.realtimeSinceStartup;
#endif
    }

    public void UpdateTouchBegan()
    {        
        dragging = true;
        hasArrived = false;
        startedOnPage = CalculateCurrentPage();
        touchStartedAtTime = Time.time;
        touchDeltaHistory = new Queue<Vector2>();
        lastTouchDistance = 0;
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        lastMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        touchDelta = Vector2.zero;
#endif
    }

    public void UpdateTouchMoved()
    {
        if (!dragging)
            return;
        float min = 0;
        float max = (Pages.childCount - 1) * worldScreenWidth;

        float increment = -touchDelta.x * worldScreenWidth / screenWidth * (moveCam.transform.localPosition.x < min || moveCam.transform.localPosition.x > max ? 0.3f : 1);

        var newX = Mathf.Clamp(moveCam.transform.localPosition.x + increment, min - (worldScreenWidth * bounceLimit), max + (worldScreenWidth * bounceLimit));

        if (allowInput)
            moveCam.transform.localPosition = moveCam.transform.localPosition.withX(newX);
        lastTouchDistance += touchDelta.magnitude;
    }

    public void UpdateTouchEnded()
    {
        if (!dragging)
            return;
        dragging = false;

        if (enableFlicking && Time.time - touchStartedAtTime < flickSensitivityMaxTime)
        {
            // a flick happened!
            float total = 0;
            foreach (var v in touchDeltaHistory)
            {
                total += v.x;
            }
            if (Mathf.Abs(total) > flickSensitivityMinDist * screenWidth)
            {
                if (total < 0)
                {
                    currentIndex++;
                    if (currentIndex >= Pages.childCount)
                        currentIndex = Pages.childCount - 1;
                }
                else
                {

                    currentIndex--;
                    if (currentIndex < 0)
                        currentIndex = 0;
                }
                currentIndex = Mathf.Clamp(currentIndex, startedOnPage - 1, startedOnPage + 1);
            }
        }
        else
        {
            // just snap cause there wasn't a flick
            currentIndex = (int)Mathf.Clamp((moveCam.transform.localPosition.x + worldScreenWidth / 2f) / worldScreenWidth, 0, Pages.childCount - 1);
        }
        TweenToCurrentIndex();

    }


    bool TouchOrMousePressed()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            return true;
#else
        if (Input.touchCount > 0) return true;
#endif
        return false;
    }

    bool TouchOrMouseBegan()
    {
        Vector3 touchPosition = Vector3.zero;
        bool touchBegan = false;
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        if (Input.GetMouseButtonDown(0))
        {
            touchBegan = true;
            touchPosition = moveCam.ScreenToWorldPoint(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
        {
            touchBegan = true;
            touchPosition = moveCam.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y,0));
        }
#endif

        if (touchBegan)
        {
            touchBegan = !IsInDeadzone(touchPosition);
        }

        return touchBegan;
    }

    bool TouchOrMouseMoved()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (Input.GetMouseButton(0) && (currentMousePosition - lastMousePosition) == Vector2.zero)
            return true;
#else
        if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Began))
            return true;
#endif
        return false;
    }

    bool TouchOrMouseEnded()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        if (Input.GetMouseButtonUp(0))
            return true;
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) return true;
#endif
        return false;
    }

    void EnqueueTouchDelta()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER)
        if (true)
        {
            var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            touchDelta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).deltaTime != 0)
            {
                touchDelta = Input.GetTouch(0).deltaPosition * (deltaTimeIgnoreScale / Input.GetTouch(0).deltaTime);
            }
        }
#endif
        touchDeltaHistory.Enqueue(touchDelta);
    }

    public void Show(int index)
    {
        if (overrideDisableAllInput)
            return;
        if (index >= Pages.childCount)
            index = 0;
        currentIndex = index;
        TweenToCurrentIndex(false);
    }

    public Transform GetPage(int index)
    {
        return Pages.GetChild(index);
    }

    public int GetPageCount()
    {
        return Pages.childCount;
    }

    public Transform GetCurrentPage()
    {
        //if (overrideUseIndexFrom != null) return Pages.GetChild(overrideUseIndexFrom.currentIndex);   <-- WHY WAS THIS EVER?
        return Pages.GetChild(currentIndex);
    }

    public float GetCurrentPosition()
    {
        return (moveCam.transform.localPosition.x + worldScreenWidth / 2f) / worldScreenWidth - 0.5f;
    }

    public void Lock()
    {
        allowInput = false;
    }

    public void Unlock()
    {
        allowInput = true;
    }

    public bool isMoving()
    {
        return !hasArrived;
    }

    public void GoToNextPage(bool force = true)
    {
        if (!allowInput && !force)
            return;
        UpdateArrows();
        if (currentIndex < Pages.childCount - 1)
        {
            dragging = false;
            currentIndex++;
            TweenToCurrentIndex();
        }

    }

    public void GoToPreviousPage(bool force = true)
    {
        if (!allowInput && !force)
            return;
        UpdateArrows();
        if (currentIndex > 0)
        {
            dragging = false;
            currentIndex--;
            TweenToCurrentIndex();
        }
    }

    public void GoToPage(int id, bool animate = true)
    {
        currentIndex = id;
        TweenToCurrentIndex(animate);
    }

    public void TweenToCurrentIndex(bool animate = true)
    {
        var speedMultiplier = 1;
        if (!animate)
            speedMultiplier = 0;

        var dist = Mathf.Abs((moveCam.transform.localPosition.x - currentIndex * worldScreenWidth) / worldScreenWidth);
        if (cameraTweener != null)
            HOTween.Kill(cameraTweener);
        cameraTweener = HOTween.To(moveCam.transform, snapSpeed * dist * speedMultiplier, new TweenParms()
            .Prop("localPosition", new Holoville.HOTween.Plugins.PlugVector3X(currentIndex * worldScreenWidth))
            .Ease(snapEaseType)
            .OnComplete(() =>
                {
                    UpdateArrows();
                    hasArrived = true;
                })
        );
    }

    public void UpdateArrows()
    {
        if (leftArrow == null || rightArrow == null)
            return;

        if (allowArrows)
        {
            FadeAssistantHT.FadeIn(leftArrow).Play();
            FadeAssistantHT.FadeIn(rightArrow).Play();
        }
        else
        {
            FadeAssistantHT.FadeOut(leftArrow).Play();
            FadeAssistantHT.FadeOut(rightArrow).Play();
            return;
        }

        if (currentIndex <= leftBound)
        {
            FadeAssistantHT.FadeOut(leftArrow).Play();
        }
        else if (currentIndex >= Pages.childCount - 1)
        {
            FadeAssistantHT.FadeOut(rightArrow).Play();
        }

    }


    int CalculateCurrentPage()
    {
        float pos = 0;
        pos = moveCam.transform.localPosition.x / worldScreenWidth;
        return (int)Mathf.Clamp(Mathf.Round(pos), 0, Pages.childCount - 1);
    }

    public void ShowArrows()
    {
        allowArrows = true;
        UpdateArrows();
    }

    public void ShowDots()
    {
        FadeAssistantHT.FadeIn(Dots).Play();
        FadeAssistantHT.FadeIn(ActiveDot).Play();
    }

    public void HideArrows()
    {
        allowArrows = false;
        UpdateArrows();
    }

    public void HideDots()
    {
        FadeAssistantHT.FadeOut(Dots).Play();
        FadeAssistantHT.FadeOut(ActiveDot).Play();
    }

    public void LockAndHide()
    {
        Lock();
        HideArrows();
        HideDots();
    }

    public bool IsInDeadzone(Vector3 touchPosition)
    {
        for (int i = 0; i < Deadzones.Length; i++)
        {
            if (!Deadzones[i].gameObject.activeSelf)
                continue;

            var tp = touchPosition;
            if (Deadzones[i].stationary)
            {
                tp = touchPosition - moveCam.transform.position;
            }

            var rect = Deadzones[i].GetComponent<Image>().rectTransform.rect;
            rect.size = rect.size * parentCanvas.transform.localScale.x;
            rect.position = rect.position * parentCanvas.transform.localScale.x;
            rect.x += Deadzones[i].transform.position.x;
            rect.y += Deadzones[i].transform.position.y;

            if (rect.Contains(tp))
            {
                return true;
            }
        }
        return false;
    }
}
 