using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    
    public bool IsPressed = false;
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public void Test()
    {
        IsPressed = true;
        background.anchoredPosition = ScreenPointToAnchoredPosition(Vector2.one);
        background.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}