using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
public class JoyStickControlPanel : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IDragHandler
{
    public Joystick joystick;
    public Image image;
    public void OnPointerDown(PointerEventData eventData)
    {
        recordPos = eventData.position;
        joystick.resetStartPos(recordPos);
        image.transform.position = recordPos;
    }

    Vector2 hidePos = new Vector2(-500, -500);
    public void OnPointerUp(PointerEventData eventData)
    {
        joystick.stopMoving(hidePos);
        image.transform.position = hidePos;
    }

    Vector2 recordPos;
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 nowPos = eventData.position;
        Vector2 diff =nowPos - recordPos;
        //如果超過了
        float limit = joystick.MovementRange;
        float nowLength = diff.magnitude;
        if (nowLength > limit)
        {
            recordPos = recordPos + diff.normalized*(nowLength-limit);
            joystick.resetStartPos(recordPos);
        }
        joystick.OnDrag(eventData);
        image.transform.position = recordPos;

    }
}
