using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class T11Joystick : Joystick
{
    public GameObject DefaultJoystick { get { return defaultJoystick; } set { defaultJoystick = value; } }
    [SerializeField] private GameObject defaultJoystick;

    protected override void Start()
    {
        base.Start();
        SimulateTap();
        background.gameObject.SetActive(false);
        defaultJoystick = DefaultJoystick;
        defaultJoystick.gameObject.SetActive(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        defaultJoystick.SetActive(false);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        defaultJoystick.SetActive(true);
        base.OnPointerUp(eventData);
    }

    private void SimulateTap()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);

        OnPointerDown(eventData);
        OnPointerUp(eventData);
    }
}