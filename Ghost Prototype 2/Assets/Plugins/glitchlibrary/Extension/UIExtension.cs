using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class UIExtension
{
    public static void GlitchUIAnimate(this TMPro.TextMeshProUGUI parent, GlitchUIElementAnimation.AnimationType type)
    {
        var animation = parent.GetComponent<GlitchUIElementAnimation>();
        if(animation == null)
        {
            animation = parent.gameObject.AddComponent<GlitchUIElementAnimation>();
        }
        animation.Animate(type);
    }
}


