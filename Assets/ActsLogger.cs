using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActsLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text consoleText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int maxLines = 300;

    private void OnEnable()
    {
        //Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDisable()
    {
        //Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        AddLine(logString);
    }

    public void AddLine(string line)
    {
        consoleText.text += line + "\n";

        // limit text length
        var lines = consoleText.text.Split('\n');
        if (lines.Length > maxLines)
        {
            consoleText.text = string.Join("\n", lines, lines.Length - maxLines, maxLines);
        }

        // auto-scroll to bottom
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
