using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CircleSlider : Selectable , IDragHandler , IEndDragHandler , IBeginDragHandler
{
    [SerializeField]
    private RectTransform m_HandleRect;

    public RectTransform handleRect
    {
        get { return m_HandleRect; }
        set { if(SetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } }
    }

    [SerializeField]
    private RectTransform m_FillRect;

    public RectTransform fillrect
    {
        get { return m_FillRect; }
        set { if (SetPropertyUtility.SetClass(ref m_FillRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } }
    }

    [SerializeField]
    private float m_MinValue = 0;
    public float minValue { get { return m_MinValue; } set { if (SetPropertyUtility.SetStruct(ref m_MinValue, value)) { Set(m_Value); UpdateVisuals(); } } }

    [SerializeField]
    private float m_MaxValue = 1;
    public float maxValue { get { return m_MaxValue; } set { if (SetPropertyUtility.SetStruct(ref m_MaxValue, value)) { Set(m_Value); UpdateVisuals(); } } }

    [SerializeField]
    private float m_Value;
    public float value {
        get { 
            if (wholeNumbers)
                return Mathf.Round(m_Value);
            return m_Value;
        }
        set
        {
            Set(value); 
        }
    }

    [SerializeField]
    private Image.Origin360 m_FillOrigin;
    public Image.Origin360 fillOrigin { get { return m_FillOrigin; } set { if (SetPropertyUtility.SetStruct(ref m_FillOrigin, value)) { Set(m_Value); UpdateVisuals(); } } }

    [SerializeField]
    private float m_Radius;
    public float radius { get { return m_Radius; } set { if (SetPropertyUtility.SetStruct(ref m_Radius, value)) { UpdateVisuals(); } } }

    [SerializeField]
    private bool m_ClockWise;

    public bool clockWise {
        get { return m_ClockWise; }
        set {
            if (SetPropertyUtility.SetStruct(ref m_ClockWise, value))
            {
                UpdateCachedReferences();
                UpdateVisuals();
            } }
    }

    [SerializeField]
    private bool m_WholeNumbers = false;
    public bool wholeNumbers { get { return m_WholeNumbers; } set { if (SetPropertyUtility.SetStruct(ref m_WholeNumbers, value)) { Set(m_Value); UpdateVisuals(); } } }

    public float normalizedValue
    {
        get
        {
            if (Mathf.Approximately(m_MinValue, m_MaxValue))
                return 0;
            return Mathf.InverseLerp(m_MinValue, m_MaxValue, value);
        }

        set
        {
            this.value = Mathf.Lerp(m_MinValue, m_MaxValue, value);
        }
    }

    private Action<float> m_OnValueChanged;
    private Action m_OnDragEnd;
    private Action m_OnTweenComplete;
    private Image m_FillImage;
    private bool m_DelayedUpdateVisuals = false;
    private bool m_dragHandler;
    private Camera m_eventCamera;
    private Coroutine m_TweenCor;
    

    protected override void OnEnable()
    {
        UpdateCachedReferences();
        Set(m_Value);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (IsActive())
        {
            UpdateCachedReferences();
            Set(m_Value, false);
            m_DelayedUpdateVisuals = true;
        }
    }
#endif

    void Update()
    {
        if (m_DelayedUpdateVisuals)
        {
            m_DelayedUpdateVisuals = false;
            UpdateVisuals();
            
        }

    }

    void UpdateCachedReferences()
    {
        if (m_FillRect != null)
        {
            m_FillImage = m_FillRect.GetComponent<Image>();
            m_FillImage.fillMethod = Image.FillMethod.Radial360;
            m_FillImage.fillOrigin = (int)m_FillOrigin;
            m_FillImage.fillClockwise = m_ClockWise;
            
        }
    }

    private bool MayDrag(PointerEventData eventData)
    {
        return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
    }

    public void OnDrag(PointerEventData eventData)
    {   
        if (!MayDrag(eventData))
            return;
        if (m_dragHandler)
        {
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_FillRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;
            float angle = CalAngle(localCursor);
            normalizedValue = (angle / 360);
        }
    }

    

    public void OnEndDrag(PointerEventData eventData)
    {
        m_dragHandler = false;
        if (m_OnDragEnd != null)
        {
            m_OnDragEnd();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_HandleRect != null)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position,
                eventData.enterEventCamera))
            {
                m_dragHandler = true;
            }
        }
        else
        {
            m_dragHandler = true;
        }

    }

    

    private float CalAngle(Vector2 pos)
    {
        float angle = 0;
        float rad = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        switch (m_FillOrigin)
        {
            case Image.Origin360.Top:
                angle = m_ClockWise ? 90 - rad : rad + 270;
                break;
            case Image.Origin360.Left:
                angle = m_ClockWise ? 180 - rad : rad + 180;
                break;
            case Image.Origin360.Right:
                angle = m_ClockWise ?  - rad : rad;
                break;
            case Image.Origin360.Bottom:
                angle = m_ClockWise ? 270 -rad : rad + 90;
                break;
        }

        if (angle > 360)
        {
            angle -= 360;
        }

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    private void UpdateVisuals()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            UpdateCachedReferences();
#endif
        if (m_FillImage != null)
        {
            m_FillImage.fillAmount = normalizedValue;
        }
        
        if (m_HandleRect != null)
        {
            m_HandleRect.transform.localPosition = CalPos(normalizedValue * 360);
        }
    }

    private Vector2 CalPos(float angle)
    {
        
        Vector2 pos = Vector2.zero;
        switch (m_FillOrigin)
        {
            case Image.Origin360.Left:
                angle = m_ClockWise ? 180 -angle : angle + 180;
                break;
            case Image.Origin360.Top:
                angle = m_ClockWise ? 90 -angle : angle + 90;
                break;
            case Image.Origin360.Right:
                angle = m_ClockWise ? -angle : angle;
                break;
            case Image.Origin360.Bottom:
                angle = m_ClockWise ? 270 - angle : angle - 90;
                break;
        }
        angle = Mathf.Deg2Rad * angle;
        pos.y = Mathf.Sin(angle) * m_Radius;
        pos.x = Mathf.Cos(angle) * m_Radius;
        return pos;
    }

    

    private void Set(float input , bool sendCallback = true)
    {
        float newValue = ClampValue(input);
        if (m_Value == newValue)
            return;

        m_Value = newValue;
        UpdateVisuals();
        if (sendCallback)
        {
            if (m_OnValueChanged != null)
            {
                m_OnValueChanged.Invoke(newValue);
            }
        }
    }

    float ClampValue(float input)
    {
        float newValue = Mathf.Clamp(input, m_MinValue, m_MaxValue);
        if (wholeNumbers)
            newValue = Mathf.Round(newValue);
        return newValue;
    }

    public void OnValueChange(Action<float> call)
    {
        m_OnValueChanged = call;
    }

    public void OnDragEnd(Action call)
    {
        m_OnDragEnd = call;
    }

    public void TweenValue(float val , float time , Action call = null)
    {
        m_TweenCor = StartCoroutine(_tweenValue(val, time));
        m_OnTweenComplete = call;
    }

    public void StopTween()
    {
        if (m_TweenCor != null)
        {
            StopCoroutine(m_TweenCor);
        }

    }

    IEnumerator _tweenValue(float val , float time)
    {
        float t = 0;
        float oldValue = value;
        float interval = val - oldValue;

        while (t < time)
        {
            t += Time.deltaTime;
            value = oldValue + (t / time) * interval;
            yield return 0;
        }

        if (m_OnTweenComplete != null)
        {
            m_OnTweenComplete();
        }
        value = val;
    }

    
}
