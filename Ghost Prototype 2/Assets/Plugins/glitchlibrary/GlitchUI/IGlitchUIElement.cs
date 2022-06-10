using System;

public enum GlitchUITransitionTypes
{
    Fade,
    SlideLeft,
    SlideRight,
    SlideUp,
    SlideDown,
    ZoomZero,
    Zoom10x,
}

[Serializable]
public struct GlitchUIElementState
{
    [ReadOnly]
    public bool isShowing;
    [ReadOnly]
    public bool isTransitioning;
}

public interface IGlitchUIElement
{
    GlitchUIElementState glitchUIElementState { get; set; }

    void OnShow(GlitchUITransitionTypes transition);

    void OnHide(GlitchUITransitionTypes transition);

    void OnToggle(GlitchUITransitionTypes transition);
}
