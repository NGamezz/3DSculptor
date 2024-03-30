using UnityEngine;

public class InvokeNonGenericEvent : MonoBehaviour
{
    [SerializeField] private EventType eventType;

    public void Activate ()
    {
        EventManager.InvokeEvent(eventType);
        EventManagerGeneric<TextPopup>.InvokeEvent(new(2, "Creating New."), EventType.OnQueuePopup);
    }
}