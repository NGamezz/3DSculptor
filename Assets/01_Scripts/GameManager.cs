using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AutoSave autoSave;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ToolHandler toolHandler;

    [SerializeField] private MeshCreator meshCreator;

    [SerializeField] private UnityEvent<bool> onPause;

    [SerializeField] private UndoSettings undoSettings;

    [SerializeField] private InputAction pauseInputAction;

    private event System.Action OnUpdate;
    private event System.Action OnFixedUpdate;
    private event System.Action OnDestroyEvent;
    private event System.Action OnDisableEvent;

    private bool applicationPaused = false;

    public void OnResume ()
    {
        onPause?.Invoke(false);
        OnPause(false);
    }

    public void SetAutoSave (bool autoSave)
    {
        this.autoSave.EnableAutoSave = autoSave;
    }

    void Start ()
    {
        string maxAmountOfundo = PlayerPrefs.GetString("MaxAmountOfUndos", undoSettings.maxAmountOfStoredUndos.ToString());

        undoSettings.maxAmountOfStoredUndos = System.Int32.Parse(maxAmountOfundo);

        inputHandler.Initialize(ref OnUpdate, ref OnDestroyEvent);
        inputHandler.UponBindingActivation += toolHandler.ActivateTool;

        toolHandler.InitializeTools(ref inputHandler, ref OnUpdate, ref OnDisableEvent, ref meshCreator);

        autoSave.StartAutoSave();

        pauseInputAction.performed += ExternalPause;
        pauseInputAction.Enable();

        meshCreator.OnStart();
    }

    private void OnEnable ()
    {
        EventManagerGeneric<bool>.AddListener(EventType.OnPause, OnPause);
    }

    private void ExternalPause ( InputAction.CallbackContext ctx )
    {
        onPause?.Invoke(!applicationPaused);
        applicationPaused = !applicationPaused;
    }

    private void OnPause ( bool pause )
    {
        if ( pause )
            pauseInputAction.Disable();
        else if ( pauseInputAction.enabled == false )
            pauseInputAction.Enable();

        applicationPaused = pause;
    }

    void Update ()
    {
        if ( !applicationPaused )
            OnUpdate?.Invoke();
    }

    private void FixedUpdate ()
    {
        if ( !applicationPaused )
            OnFixedUpdate?.Invoke();
    }

    private void OnDisable ()
    {
        applicationPaused = true;

        EventManagerGeneric<bool>.RemoveListener(EventType.OnPause, OnPause);

        meshCreator.OnDisable();

        OnDisableEvent?.Invoke();

        OnDisableEvent = null;
        OnUpdate = null;
        OnFixedUpdate = null;
        OnDestroyEvent = null;
    }

    private void OnDestroy ()
    {
        meshCreator.OnDestroy();
        OnDestroyEvent?.Invoke();
    }
}