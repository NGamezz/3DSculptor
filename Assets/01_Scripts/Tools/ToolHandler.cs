using System;
using System.Collections;
using UnityEngine;

public class ToolHandler : MonoBehaviour
{
    [SerializeField] private float delayBetweenToolSelection = 0.5f;

    private Tool[] tools;
    private Tool currentTool;

    private bool canSwapTool = true;

    private void Start()
    {
        tools = FindObjectsByType<Tool>(FindObjectsSortMode.None);

        foreach(var tool in tools)
        {
            tool.Deactivate();
        }

        currentTool = FindAnyObjectByType<SphereTool>();
        if ( currentTool == null && tools.Length > 0)
            currentTool = tools[0];

        if ( currentTool == null )
            throw new Exception("No Tools Found.");

        currentTool.Activate();
    }

    private void FixedUpdate ()
    {
        CheckToolSwitch(SwitchCallback);
    }

    private void CheckToolSwitch(Action<Tool> callBack)
    {
        if ( !canSwapTool )
            return;

        foreach(var tool in tools)
        {
            if(tool.keyBind.IsKeyBindActivated())
            {
                callBack?.Invoke(tool);
                StartCoroutine(ResetToolSelection());
                return;
            }
        }
    }

    private IEnumerator ResetToolSelection()
    {
        canSwapTool = false;
        Debug.Log("Resetting Swap.");
        yield return new WaitForSeconds(delayBetweenToolSelection);
        canSwapTool = true;
    }

    private void SwitchCallback(Tool tool)
    {
        if(tool == currentTool)
            return;

        if(tool.Utility == true)
        {
            tool.Activate();
            return;
        }

        currentTool.Deactivate();
        currentTool = tool;
        currentTool.Activate();
    }
}