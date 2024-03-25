using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct TextPopup
{
    public float Duration;
    public string Text;

    public TextPopup ( float duration, string text )
    {
        Duration = duration;
        Text = text;
    }
}

public class TextPopUpManager : MonoBehaviour
{
    [SerializeField] private TMP_Text popupTextObject;

    private Queue<TextPopup> textPopupQueue = new();

    private Coroutine currentPopupRoutine = null;

    public void QueuePopup ( TextPopup popup )
    {
        textPopupQueue.Enqueue(popup);
    }

    private void Dequeue ()
    {
        currentPopupRoutine ??= StartCoroutine(PlayPopup(textPopupQueue.Dequeue()));
    }

    private void OnEnable ()
    {
        EventManager<TextPopup>.AddListener(EventType.OnQueuePopup, QueuePopup);
    }

    private void OnDisable ()
    {
        EventManager<TextPopup>.RemoveListener(EventType.OnQueuePopup, QueuePopup);
    }

    private void Start ()
    {
        if ( DataHolder.TextPopupManager != null )
        {
            Destroy(DataHolder.TextPopupManager);
        }
        DataHolder.TextPopupManager = this;
    }

    private void OnDestroy ()
    {
        if ( DataHolder.TextPopupManager != null && DataHolder.TextPopupManager == this )
        {
            DataHolder.TextPopupManager = null;
        }
    }

    private IEnumerator PlayPopup ( TextPopup popup )
    {
        popupTextObject.gameObject.SetActive(true);
        popupTextObject.text = popup.Text;

        yield return new WaitForSeconds(popup.Duration);

        popupTextObject.gameObject.SetActive(false);
        currentPopupRoutine = null;
    }

    private void FixedUpdate ()
    {
        if ( textPopupQueue.Count > 0 )
            Dequeue();
    }
}
