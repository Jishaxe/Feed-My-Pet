using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TwitchInput : MonoBehaviour
{
    Process bot;

    FoodSpawner _spawner;

    Queue<string> _commandsToProcess = new Queue<string>();

    // Start is called before the first frame update

    void StartBot() {
        string indexJs = Application.streamingAssetsPath + "/YoutubeBot/index.js";

        Debug("Starting Twitch bot at " + indexJs);

        bot = new Process();
        bot.StartInfo.FileName = "node";
        bot.StartInfo.Arguments = "\"" + indexJs + "\"";
        bot.EnableRaisingEvents = true;

        bot.StartInfo.CreateNoWindow = true;
        bot.StartInfo.UseShellExecute = false;
        bot.StartInfo.RedirectStandardError = true;
        bot.StartInfo.RedirectStandardInput = true;
        bot.StartInfo.RedirectStandardOutput = true;
        bot.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        bot.OutputDataReceived += new DataReceivedEventHandler( DataReceived );
        bot.ErrorDataReceived += new DataReceivedEventHandler( ErrorReceived );
        
        bot.Start();

        bot.BeginOutputReadLine();
        bot.BeginErrorReadLine();
    }

    void Awake() {
        //QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        _spawner = GameObject.Find("FoodSpawner").GetComponent<FoodSpawner>();
        StartBot();
    }   

    void Update() {
        while (_commandsToProcess.Count > 0) {
            string cmd = _commandsToProcess.Dequeue();

            if (cmd.StartsWith("feed")) {
                _spawner.QueueSpawn(int.Parse(cmd.Split(' ')[1]), cmd.Split(' ')[2]);
                continue;
            }
        }
    }


    void DataReceived( object sender, DataReceivedEventArgs eventArgs )
    {
        if (eventArgs.Data == "" || eventArgs.Data == null) return;

        Debug("<color=lightblue>BOT:</color><size=14><color=white>" + eventArgs.Data + "</color></size>\n");
        if (eventArgs.Data.StartsWith("CMD")) {
            _commandsToProcess.Enqueue(eventArgs.Data.Split(new string[]{"CMD "}, StringSplitOptions.None)[1]);
        }
    }
 
 
    void ErrorReceived( object sender, DataReceivedEventArgs eventArgs )
    {
        if (eventArgs.Data == "" || eventArgs.Data == null) return;

        Err("<color=red>BOT:</color><size=14><color=white>" + eventArgs.Data + "</color></size>\n");
    }
 


    void Debug(string txt) {
        UnityEngine.Debug.Log(txt);
    }

    void Err(string txt) {
        UnityEngine.Debug.LogError(txt);
    }

    void OnDisable() {
        bot.CloseMainWindow();
    }
}
