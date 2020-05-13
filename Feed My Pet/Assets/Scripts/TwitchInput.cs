using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TwitchInput : MonoBehaviour
{
    Process bot;

    // Start is called before the first frame update
    void Start()
    {
        string indexJs = Application.dataPath + "/Resources/TwitchBot/index.js";

        #if UNITY_EDITOR
            indexJs = "D:/git/Feed-My-Pet/Feed My Pet/Resources/TwitchBot/index.js";
        #endif

        Debug("Starting Twitch bot at " + indexJs);

        bot = new Process();
        bot.StartInfo.FileName = "node";
        bot.StartInfo.Arguments = "\"" + indexJs + "\"";
        bot.EnableRaisingEvents = true;

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


    void DataReceived( object sender, DataReceivedEventArgs eventArgs )
    {
        Debug("<color=lightblue>BOT:</color><size=14><color=white>" + eventArgs.Data + "</color></size>\n");
    }
 
 
    void ErrorReceived( object sender, DataReceivedEventArgs eventArgs )
    {
        Debug("<color=red>BOT:</color><size=14><color=white>" + eventArgs.Data + "</color></size>\n");
    }
 


    void Debug(string txt) {
        UnityEngine.Debug.Log(txt);
    }

    void OnDisable() {
        bot.CloseMainWindow();
    }
}
