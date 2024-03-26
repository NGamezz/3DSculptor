using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ToolHandler
{
    [SerializeField] private float delayBetweenToolSelection = 0.5f;

    [SerializeField] private BrushData brushData;

    [SerializeField] private float sizeToolIncrement;

    [SerializeField] private UndoSettings undoSettings;

    [SerializeField] private KeyBindSettings sphereToolKeybinds;
    [SerializeField] private KeyBindSettings undoToolKeyBinds;
    [SerializeField] private KeyBindSettings exportToolKeyBinds;
    [SerializeField] private KeyBindSettings saveToolKeyBinds;
    [SerializeField] private KeyBindSettings createNewToolKeyBinds;
    [SerializeField] private KeyBindSettings loadToolKeyBinds;
    [SerializeField] private KeyBindSettings SizePlusToolKeyBinds;
    [SerializeField] private KeyBindSettings SizeMinusToolKeyBinds;

    [SerializeField] private List<Tool> tools = new();
    private Tool currentTool;

    private bool canSwapTool = true;

    public void InitializeTools ( ref InputHandler inputHandler, ref Action OnUpdate, ref Action OnDisableEvent, ref MeshCreator meshCreator )
    {
        SphereTool sphereTool = new();
        OnUpdate += sphereTool.OnUpdate;
        OnDisableEvent += sphereTool.OnDisable;

        sphereTool.SetBrushData(brushData);
        sphereTool.OnAwake();

        sphereTool.KeyBind = sphereToolKeybinds.KeyCodes;

        currentTool = sphereTool;
        sphereTool.Activate(null);

        SaveTool saveTool = new()
        {
            OnRequestRenderTexture = meshCreator.GetRenderTexture,
            KeyBind = saveToolKeyBinds.KeyCodes
        };
        OnDisableEvent += saveTool.OnDisable;
        saveTool.OnStart();

        var chunksHolder = UnityEngine.Object.FindAnyObjectByType<ChunksHolder>();

        ExportTool exportTool = new()
        {
            KeyBind = exportToolKeyBinds.KeyCodes
        };
        exportTool.SetChunksHolder(chunksHolder);

        CreateNew createNew = new()
        {
            KeyBind = createNewToolKeyBinds.KeyCodes
        };
        createNew.OnStart();

        LoadTool loadTool = new()
        {
            KeyBind = loadToolKeyBinds.KeyCodes
        };

        UndoTool undoTool = new()
        {
            KeyBind = undoToolKeyBinds.KeyCodes
        };
        undoTool.SetUndoSettings(undoSettings);
        undoTool.OnEnable();
        OnDisableEvent += undoTool.OnDisable;

        ToolSizeTool sizeToolPlus = new()
        {
            increment = sizeToolIncrement,
            KeyBind = SizePlusToolKeyBinds.KeyCodes
        };
        sizeToolPlus.OnStart();

        ToolSizeTool sizeToolMinus = new()
        {
            increment = -sizeToolIncrement,
            KeyBind = SizeMinusToolKeyBinds.KeyCodes
        };
        sizeToolMinus.OnStart();

        inputHandler.BindInputToCommand(sphereTool, null, sphereTool.KeyBind);
        inputHandler.BindInputToCommand(saveTool, null, saveTool.KeyBind);
        inputHandler.BindInputToCommand(exportTool, null, exportTool.KeyBind);
        inputHandler.BindInputToCommand(createNew, null, createNew.KeyBind);
        inputHandler.BindInputToCommand(loadTool, null, loadTool.KeyBind);
        inputHandler.BindInputToCommand(undoTool, null, undoTool.KeyBind);
        inputHandler.BindInputToCommand(sizeToolPlus, null, sizeToolPlus.KeyBind);
        inputHandler.BindInputToCommand(sizeToolMinus, null, sizeToolMinus.KeyBind);

        AddTool(sphereTool, sizeToolMinus, sizeToolPlus, saveTool, exportTool, loadTool, undoTool, createNew);
    }

    public void AddTool ( params Tool[] tool )
    {
        if ( tool.Length < 1 )
            return;

        for ( int i = 0; i < tool.Length; i++ )
        {
            if ( tools.Contains(tool[i]) )
                return;

            tools.Add(tool[i]);
        }
    }

    public void ActivateTool ( Tool tool )
    {
        if(tool.IgnoreCooldown)
        {
            tool.Activate(currentTool.Brush ? (Brush)currentTool : null);
            return;
        }

        if ( !canSwapTool )
            return;

        currentTool.Deactivate();
        var previousTool = currentTool;
        currentTool = tool;
        currentTool.Activate(tool.Brush ? null : (Brush)previousTool);
        ResetToolSelection();
    }

    private async void ResetToolSelection ()
    {
        canSwapTool = false;
        Debug.Log("Resetting Swap.");
        await Awaitable.WaitForSecondsAsync(delayBetweenToolSelection);
        canSwapTool = true;
    }
}