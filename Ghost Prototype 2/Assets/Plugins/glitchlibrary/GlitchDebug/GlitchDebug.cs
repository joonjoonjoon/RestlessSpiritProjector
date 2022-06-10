using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class GlitchDebug : MonoBehaviour {
	public const string WARNING = "warning";
	public const string ERROR = "error";
	public const string INFORMATION = "info";
	public const string IMPORTANT = "important";

	public bool showOnStartup;
	public bool paused;
	public int maxLines = 100;
	public GlitchDebugUIController UIController;
	public List<GlitchDebugLevel> presetLevels;

    private int currentLines;
	private Queue<GlitchDebugMessage> messages; //sdvsdfgsdf
	private static GlitchDebug instance;


	void Awake() {
        if (instance != null)
        {
            
            DestroyImmediate(gameObject);
            GlitchDebug.Log("Double GlitchDebug script detected, destroying everything.");
        }
        else
        {
            instance = this;
            UIController.UpdateEventSystem();
            transform.parent.parent = null;
			DontDestroyOnLoad(transform.parent.gameObject);
            if (messages == null) messages = new Queue<GlitchDebugMessage>();

            if (showOnStartup)
            {
                UIController.ShowToggleClick(true);
            }
            else
            {
                UIController.ShowToggleClick(false);
            }
        }

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

    void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (Application.isPlaying)
        {
            UIController.UpdateEventSystem();
        }
    }


	private void log(string message, string level)
	{
        if (UIController.debugText == null) return;

        level = level.ToLower();

        var levelObj = GetLevelObject(level);

        string newMessage = "\n<color=" + levelObj.GetHexColor() + ">" + message + "</color>";

        // Check if logging
		if(levelObj.log)
		{
			messages.Enqueue(new GlitchDebugMessage(newMessage, level));
			if(messages.Count > maxLines) messages.Dequeue();
		}

        // add button
		UIController.AddToggle(level);

        // check if showing
		if(!paused && UIController.CheckToggleIsOn(level))
		{
			UIController.debugText.text = UIController.debugText.text += newMessage;
		    currentLines++;
		    if (currentLines > maxLines)
		    {
		        var pos = UIController.debugText.text.IndexOf('\n',1);
                UIController.debugText.text = UIController.debugText.text.Remove(0, pos);
		        currentLines--;
		    }
		}

		// check if console logging
		if(levelObj.sendToConsole)
		{
			Debug.Log(newMessage);
		}

	}

    public GlitchDebugLevel GetLevelObject(string level)
    {
        foreach (var levelObject in instance.presetLevels.Where(l => String.Equals(l.level, level, StringComparison.CurrentCultureIgnoreCase)))
        {
            return levelObject;
        }
        return new GlitchDebugLevel();
    }

    public void RefreshDebugText()
    {
        if(UIController.generatedLevels != null)
        {
            var generatedLevels = UIController.generatedLevels.Where((t, i) => !UIController.generatedToggles[i].isOn);

            if(generatedLevels != null)
            {
                IEnumerable<GlitchDebugMessage> messagesArray =
                    generatedLevels.Aggregate<string, IEnumerable<GlitchDebugMessage>>(messages, (current, level) => current
                    .Where(message => !String.Equals(message.level, level, StringComparison.CurrentCultureIgnoreCase)));

                //for (int i = 0; i < UIController.generatedLevels.Count; i++) {
                //    if(!UIController.generatedToggles[i].isOn)
                //    {
                //        var level = UIController.generatedLevels[i];
                //        messagesArray = messagesArray.Where(message => message.level.ToLower() != level.ToLower());
                //    }
                //}

                if (messagesArray != null)
                {
                    currentLines = messagesArray.Count();
                    UIController.debugText.text = ConvertStringArrayToString(messagesArray);
                }
            }
        }
	}

	public void ClearMessages() {
		messages.Clear();
	}

	public static void Log(string message, string level=WARNING)
	{
        if (instance == null) return;
 		instance.log(message, level);
	}

	static string ConvertStringArrayToString(IEnumerable<GlitchDebugMessage> array)
	{
		StringBuilder builder = new StringBuilder();
		foreach (GlitchDebugMessage msg in array)
		{
			builder.Append(msg.message);
		}
		return builder.ToString();
	}

    public void ToggleBlackWhite()
    {
        UIController.ToggleBlackWhite();
    }
}

[System.Serializable]
public class GlitchDebugLevel
{  
	public string level;
	public bool log = true;
	public bool showDefault = true;
	public bool sendToConsole = true;
	public Color color;
	private string colorHex ="";

	public string GetHexColor()
	{
		if(colorHex =="") colorHex = "#" + ColorToHex(color);
		return colorHex;
	}

	string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
}

public class GlitchDebugMessage
{
	public string message;
	public string level;

	public GlitchDebugMessage(string message, string level)
	{
		this.message = message;
		this.level = level;
	}
}
