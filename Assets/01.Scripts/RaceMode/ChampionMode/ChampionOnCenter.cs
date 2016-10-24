﻿using UnityEngine;

public class ChampionOnCenter : MonoBehaviour
{
	/// <summary>
	/// The strength of the spring.
	/// </summary>
	
	public float springStrength = 8f;
	private bool isResetPos = false;

	public SpringPanel.OnFinished onFinished;
	
	UIDraggablePanel mDrag;
	GameObject mCenteredObject;
	

	public GameObject centeredObject { get { return mCenteredObject; } }

	
	void OnEnable () { 
//		isResetPos = false;
		Recenter(); 
		//RecenterTest();
	}

	void Start(){
	//	isResetPos = false;
	//	Recenter(); 
	}
	void OnDragFinished () {}//if (enabled) Recenter(); }
	
	/// <summary>
	/// Recenter the draggable list on the center-most child.
	/// </summary>
	



	public void Recenter ()
	{
		//return;
		//Utility.LogWarning("recenter");
	//	if(isResetPos) return;
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIDraggablePanel>(gameObject);
			
			if (mDrag == null)
			{
				//Utility.LogWarning(GetType() + " requires " + typeof(UIDraggablePanel) + " on a parent object in order to work", this);
				enabled = false;
				return;
			}
			else
			{
				mDrag.onDragFinished = OnDragFinished;
				
				if (mDrag.horizontalScrollBar != null)
					mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;
				
				if (mDrag.verticalScrollBar != null)
					mDrag.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mDrag.panel == null) return;
		
		// Calculate the panel's center in world coordinates
		Vector4 clip = mDrag.panel.clipRange;
		Transform dt = mDrag.panel.cachedTransform;
		Vector3 center = dt.localPosition;
		center.x += clip.x;
		center.y += clip.y;
		center = dt.parent.TransformPoint(center);
		
		// Offset this value by the momentum
		Vector3 offsetCenter = center - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		
		float min = float.MaxValue;
		Transform closest = null;
		Transform trans = transform;
		
		// Determine the closest child
		if(isResetPos){
			for (int i = 0, imax = trans.childCount; i < imax; ++i)
			{
				Transform t = trans.GetChild(i);
				float sqrDist = Vector3.SqrMagnitude(t.position - offsetCenter);
				if (sqrDist < min)
				{
					min = sqrDist;
					closest = t;
				}
			}
		}else{
			//int cnt = GV.TeamSeason-1;
			int cnt = 1;
			Transform t2 = trans.GetChild(cnt);
			closest = t2;
			float sqrDist = Vector3.SqrMagnitude(t2.position - offsetCenter);
			if (sqrDist < min)
			{
		//		min = sqrDist;
		//		closest = t2;
			}
		}

		if (closest != null)
		{
			mCenteredObject = closest.gameObject;
			
			// Figure out the difference between the chosen child and the panel's center in local coordinates
			Vector3 cp = dt.InverseTransformPoint(closest.position);
			Vector3 cc = dt.InverseTransformPoint(center);
			Vector3 offset = cp - cc;
			
			// Offset shouldn't occur if blocked by a zeroed-out scale
			if (mDrag.scale.x == 0f) offset.x = 0f;
			if (mDrag.scale.y == 0f) offset.y = 0f;
			if (mDrag.scale.z == 0f) offset.z = 0f;
		//	Utility.LogWarning(closest.name);
			// Spring the panel to this calculated position
			SpringPanel.Begin(mDrag.gameObject, dt.localPosition - offset, springStrength).onFinished = onFinished;
		}
		else mCenteredObject = null;
	}
	
	
}