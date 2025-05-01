using UnityEngine;
using System.Collections;
using System.IO;

public class LogToFile : MonoBehaviour
{
    private string logFilePath = "";

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Start()
    {
        logFilePath = Application.dataPath + "logdata.text";
    }

    public void Log(string logString, string stackTrace, LogType type){
        TextWriter tw = new StreamWriter(logFilePath, true);

        tw.WriteLine("[" + System.DateTime.Now + "]" + logString);
        
        tw.Close();
    }

}