using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GlitchDebugUIController : MonoBehaviour {

	#region control references

    public Material backgroundMaterial;
	public Canvas innerCanvas;
	public Image debugBackground;
	public Text debugText;
	public GameObject togglePrefab;
	public GlitchDebug glitchDebug;
	public Toggle showToggle;
	public Toggle pauseToggle;
    public EventSystem eventSystem;
    public HudFPS hudFps;
    public Transform toggleField;
    public List<Toggle> generatedToggles;
    public List<string> generatedLevels;
	#endregion


	#region menu controls

    private void Init()
    {
        generatedToggles = new List<Toggle>();
        generatedLevels = new List<string>();
    }

    public void UpdateEventSystem()
    {
        //GlitchDebug.Log("Updating Event System.");
        if (GameObject.FindObjectsOfType<EventSystem>().Length == 0)
        {
            eventSystem.gameObject.SetActive(true);
        }
    }


    public void Clear() {
        if (generatedToggles == null) Init();

		glitchDebug.ClearMessages();

		debugText.text = "";
		generatedLevels.Clear();
		foreach (var item in generatedToggles) {
			Destroy(item.gameObject);
		}
		generatedToggles.Clear();
	}

	public bool CheckToggleIsOn(string level)
    {
        if (generatedToggles == null) Init();

		var index = generatedLevels.IndexOf(level);
		if(index >= 0)
		{
			return generatedToggles[index].isOn;
		}
	    return false;
	}

	public void AddToggle(string level)
	{
        if (generatedToggles == null) Init(); 

		if(generatedLevels.IndexOf(level) < 0)
		{
			var toggle = Instantiate(togglePrefab) as GameObject;
		    var levelObj = glitchDebug.GetLevelObject(level);
			toggle.GetComponentInChildren<Text>().text = "<color="+levelObj.GetHexColor()+">"+level.Substring(0,Mathf.Clamp(4,0,level.Length)).ToLower()+"</color>";
			toggle.transform.SetParent(toggleField);
            toggle.transform.localScale = togglePrefab.transform.localScale;
			toggle.gameObject.name = level+"Toggle";
			generatedLevels.Add(level);
			generatedToggles.Add(toggle.GetComponent<Toggle>());

			if(levelObj.showDefault)
			{
				toggle.GetComponent<Toggle>().isOn = true;
			}
			toggle.GetComponent<Toggle>().onValueChanged.AddListener(GeneratedToggleValueChanged);
		}
	}

	void GeneratedToggleValueChanged(bool value)
	{
		glitchDebug.RefreshDebugText();
		PauseToggleClick(false);
	}

	public void ShowToggleClick(bool value)
	{
        if(showToggle != null)
        {
            showToggle.isOn = value; // this means we called it from a script instead of the event
            innerCanvas.enabled = value;
            hudFps.show = value;
        }
	}

	public void PauseToggleClick(bool value)
	{
        if (pauseToggle != null)
        {
            pauseToggle.isOn = value; // this means we called it from a script instead of the event
        }
		glitchDebug.paused = value;

		if(!value) //unpaused -> refresh
		{
			glitchDebug.RefreshDebugText();
		}
	}

    public void ToggleBlackWhite()
    {
        if (backgroundMaterial.color.r == 1)
        {
            backgroundMaterial.color = new Color(0,0,0,0.3f);
        }
        else
        {
            backgroundMaterial.color = new Color(1, 1, 1, 0.3f);
        }
    }

	public void DestroyEverything()
	{
		Destroy(transform.parent.gameObject);
	}

	public void ToggleRaycast()
	{
		debugText.raycastTarget = ! debugText.raycastTarget;
	}
	#endregion


}
