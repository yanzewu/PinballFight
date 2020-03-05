using UnityEngine;
using UnityEngine.EventSystems;

public class EventTest :MonoBehaviour {

    private void Start() {
        var et = new EventTrigger.Entry();
        et.eventID = EventTriggerType.PointerDown;
        et.callback.AddListener((data) => {on_pointdown((PointerEventData)data);});
        GetComponent<EventTrigger>().triggers.Add(
            et
        );

        var et2 = new EventTrigger.Entry();
        et2.eventID = EventTriggerType.Drag;
        et2.callback.AddListener((data) => {on_pointclick((PointerEventData)data);});
        GetComponent<EventTrigger>().triggers.Add(et2);
    }

    public void on_pointdown(PointerEventData data){
        Debug.Log("pointdown: " + data.pointerPressRaycast.worldPosition.ToString());
        
    }

    public void on_pointclick(PointerEventData data){
        Debug.Log("pointup: " + data.position.ToString());
        //transform.position = data.pointerPressRaycast.worldPosition;
    }

}