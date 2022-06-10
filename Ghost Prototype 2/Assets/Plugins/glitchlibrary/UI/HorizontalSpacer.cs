using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("Layout/Horizontal Spacer", 150)]
public class HorizontalSpacer : HorizontalOrVerticalLayoutGroup
{
    [SerializeField]
    protected bool m_ExpandToEdges;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, false);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalcAlongAxis(1, false);
    }

    public override void SetLayoutHorizontal()
    {
        SetChildrenAlongAxis(0, false);
    }

    public override void SetLayoutVertical()
    {
        SetChildrenAlongAxis(1, false);
    }

    protected new void SetChildrenAlongAxis(int axis, bool isVertical)
    {
        float size = rectTransform.rect.size[axis];

        bool alongOtherAxis = (isVertical ^ (axis == 1));
        if (alongOtherAxis)
        {
            float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float min = LayoutUtility.GetMinSize(child, axis);
                float preferred = LayoutUtility.GetPreferredSize(child, axis);
                float flexible = LayoutUtility.GetFlexibleSize(child, axis);
                if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                    flexible = Mathf.Max(flexible, 1);

                float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                float startOffset = GetStartOffset(axis, requiredSpace);
                SetChildAlongAxis(child, axis, startOffset, requiredSpace);
            }
        }
        else
        {
            float pos = (axis == 0 ? padding.left : padding.top);
            if (GetTotalFlexibleSize(axis) == 0 && GetTotalPreferredSize(axis) < size)
                pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));

            float minMaxLerp = 0;
            if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
                minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

            float itemFlexibleMultiplier = 0;
            if (size > GetTotalPreferredSize(axis))
            {
                if (GetTotalFlexibleSize(axis) > 0)
                    itemFlexibleMultiplier = (size - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
            }

            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                float min = LayoutUtility.GetMinSize(child, axis);
                float preferred = LayoutUtility.GetPreferredSize(child, axis);
                float flexible = LayoutUtility.GetFlexibleSize(child, axis);

                float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                childSize += flexible * itemFlexibleMultiplier;

                if (m_ExpandToEdges)
                {
                    var theSplit = itemFlexibleMultiplier / (rectChildren.Count * 1f);
                    if (i == 0)
                    {
                        SetChildAlongAxis(child, axis, pos, childSize);
                        pos += childSize + spacing + (flexible > 0 ? 0 : itemFlexibleMultiplier / 2f) + theSplit;    
                    }
                    else
                    {
                        SetChildAlongAxis(child, axis, pos + (flexible > 0 ? 0 : itemFlexibleMultiplier / 2f) + theSplit, childSize);
                        pos += childSize + spacing + (flexible > 0 ? 0 : itemFlexibleMultiplier) + theSplit;
                    }
                }
                else // TODO: make other alignments
                {
                    SetChildAlongAxis(child, axis, pos + (flexible > 0 ? 0 : itemFlexibleMultiplier / 2f), childSize);
                    pos += childSize + spacing + (flexible > 0 ? 0 : itemFlexibleMultiplier);
                }
            }
        }
    }
}
