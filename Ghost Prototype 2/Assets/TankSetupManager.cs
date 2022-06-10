using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSetupManager : MonoSingleton<TankSetupManager>
{
    public DragPoint[] dragPointSprites;
    public AutoStoredPosition[] dragPointPositions;
    public GameObject tankBackground;
         
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void OnEnable()
    {
        if(TankMeshGenerator.IsInstantiated()) // can sometimes happen on load
        {
            TankMeshGenerator.instance.LiveUpdate = true;
        }
        foreach (var item in dragPointSprites)
        {
            item.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (TankMeshGenerator.IsInstantiated()) // can sometimes happen on load
        {
            TankMeshGenerator.instance.LiveUpdate = false;
        }
        foreach (var item in dragPointSprites)
        {
            item.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tankBackground.SetActive(!tankBackground.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var item in dragPointPositions)
            {
                item.ResetToDefault();
            }
        }
    }

}
