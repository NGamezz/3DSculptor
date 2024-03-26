using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AutoSave autoSave;

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ToolHandler toolHandler;

    [SerializeField] private MeshCreator meshCreator;

    private event System.Action OnUpdate;
    private event System.Action OnFixedUpdate;
    private event System.Action OnDestroyEvent;
    private event System.Action OnDisableEvent;

    void Start ()
    {
        inputHandler.Initialize(ref OnUpdate, ref OnDestroyEvent);
        inputHandler.UponBindingActivation += toolHandler.ActivateTool;

        toolHandler.InitializeTools(ref inputHandler, ref OnUpdate, ref OnDisableEvent, ref meshCreator);
        
        meshCreator.OnStart();
        
        autoSave.StartAutoSave();
    }

    void Update ()
    {
        OnUpdate?.Invoke();
    }

    private void FixedUpdate ()
    {
        OnFixedUpdate?.Invoke();
    }

    private void OnDestroy ()
    {
        OnDestroyEvent?.Invoke();
    }
}