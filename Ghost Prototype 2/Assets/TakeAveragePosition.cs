using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeAveragePosition : MonoBehaviour {

    public Transform[] positions;
	
	void Start () {
        
	}

    void Update()
    {
        var total = Vector3.zero;
        foreach (var item in positions)
        {
            total += item.position;
        }
        transform.position = total / positions.Length;
    }
}
