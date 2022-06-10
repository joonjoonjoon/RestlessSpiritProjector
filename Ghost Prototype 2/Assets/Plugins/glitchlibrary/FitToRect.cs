
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu ("Layout/FitTo RectTransform Fitter", 142), ExecuteInEditMode, RequireComponent (typeof(RectTransform))]
public class FitToRect : UIBehaviour, ILayoutController, ILayoutSelfController 
{
	private RectTransform m_Rect;

	[SerializeField]
	protected RectTransform m_FitTo;

	[SerializeField]
	protected bool Horizontal = true;

	[SerializeField]
	protected bool Vertical = true;

	[SerializeField]
	protected bool UseActualSize = false;
	
	private RectTransform rectTransform
	{
		get
		{
			if (this.m_Rect == null)
			{
				this.m_Rect = base.GetComponent<RectTransform> ();
			}
			return this.m_Rect;
		}
	}
	
	
	
	//
	// Constructors
	//
	protected FitToRect ()
	{
	}
	
	//
	// Methods
	//
	protected override void OnDisable ()
	{
		//this.m_Tracker.Clear ();
		LayoutRebuilder.MarkLayoutForRebuild (this.rectTransform);
		base.OnDisable ();
	}
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		this.SetDirty ();
	}
	
	protected override void OnRectTransformDimensionsChange ()
	{
		this.SetDirty ();
	}
	
//		protected override void OnValidate ()
//		{
//			this.SetDirty ();
//		}
	
	protected void SetDirty ()
	{
		if (!this.IsActive ())
		{
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild (this.rectTransform);
	}
	
	public virtual void SetLayoutHorizontal ()
	{
		if(Horizontal)
		{
			if(UseActualSize)
			{
				rectTransform.sizeDelta = new Vector2(m_FitTo.rect.width, rectTransform.sizeDelta.y);
			}
			else
			{
				rectTransform.sizeDelta = new Vector2(LayoutUtility.GetPreferredWidth(m_FitTo), rectTransform.sizeDelta.y);
			}
		}
		
	}
		
	public virtual void SetLayoutVertical ()
	{
		if(Vertical)
		{
			if(UseActualSize)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, m_FitTo.rect.height);
			}
			else	
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, LayoutUtility.GetPreferredHeight(m_FitTo));
			}
		}
	}
}
