using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSetup : MonoSingleton<GameSetup>
{
    public Slider GhostSpeed;
    public Slider GhostsInTankSize;
    public Slider PairsAmount;
    public Slider BottomBoundTweak;
    public Camera tankCamera;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GhostSpeed.value = GhostsManager.instance.speed;
       BottomBoundTweak.value = GhostsManager.instance.screenbottomBoundTweak;
        PairsAmount.value = GhostsManager.instance.maxPairsAllowed;
        GhostsInTankSize.value = AutoSetCameraSettings.instance.ghostsInTankSizeVal;
    }

    void Update()
    {

    }

    public void OnGhostSpeedChanged()
    {
        GhostsManager.instance.speed = GhostSpeed.value;
    }

    public void OnPairsAmountChanged()
    {
        GhostsManager.instance.maxPairsAllowed = (int)PairsAmount.value;

    }

    public void OnGhostInTankSizeChanged()
    {
        AutoSetCameraSettings.instance.ghostsInTankSizeVal = GhostsInTankSize.value;
    }

    public void BottomBoundTweakChanged()
    {
        GhostsManager.instance.screenbottomBoundTweak = BottomBoundTweak.value;
    }


    public void ResetPressed()
    {
        AutoStoredValueManager.ForceStore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
