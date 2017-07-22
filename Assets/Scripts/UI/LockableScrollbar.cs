using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Locks the scrollbar to the bottom by  default, and stays there when dragged
[RequireComponent(typeof(Scrollbar))]
public class LockableScrollbar : MonoBehaviour, IDragHandler {

    private bool locked = true;
    private Scrollbar sb;

    private const float LOCK_THRESHOLD = 0.05f;

    void Start() {
        sb = GetComponent<Scrollbar>();
    }

    void Update() {
        if(locked) {
            sb.value = 0;
        }
    }

    public void OnDrag(PointerEventData data) {
        // If the drag bar is below a certain level, set locked
        locked = sb.value <= LOCK_THRESHOLD;
    }
}
