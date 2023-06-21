using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsoleLogType { Messgage, Warning, Error };

public static class ConsoleManager
{
    [SerializeField] static Color messageColour;
    [SerializeField] static Color warningColour;
    [SerializeField] static Color errorColour;

     

    public static void LogMessage(string msg, ConsoleLogType logType)
    {
        Color textColour;
        switch (logType)
        {
            case ConsoleLogType.Messgage:
                Debug.Log("<color=white>" + msg + "</color>");
                textColour = messageColour;
                break;
            case ConsoleLogType.Warning:
                Debug.Log("<color=yellow>" + msg + "</color>");
                textColour = warningColour;
                break;
            case ConsoleLogType.Error:
                Debug.Log("<color=red>" + msg + "</color>");
                textColour = errorColour;
                break;
        }
    }
}
