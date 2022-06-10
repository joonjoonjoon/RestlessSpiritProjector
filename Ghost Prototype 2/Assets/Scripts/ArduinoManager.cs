using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.IO;

public class ArduinoManager : MonoSingleton<ArduinoManager> {

    public static float lightSensorValue;

    SerialPort stream;

    [AutoStoredValueManager.StoredValue]
    public string port = "COM4";
    private int streamTimeout = 1000;
    private int asyncTimeout = 1000;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Init();
    }

	public void Init () {
        Kill();
        stream = new SerialPort(port, 9600);
        stream.ReadTimeout = streamTimeout;
        try
        {
            stream.Open();
            StartCoroutine
            (
                AsynchronousReadFromArduino
                ((string s) => ReadingReceived(s),        // Callback
                    () => Debug.LogError("Error!"), // Error callback
                    asyncTimeout                             // Timeout (seconds)
                )
            );
        }
        catch  (IOException exception)
        {
            Debug.Log(exception);
        }
    }

    public void Kill()
    {
        StopAllCoroutines();
        if (stream != null)
            stream.Close();
    }


    private void ReadingReceived(string s)
    {
        //Debug.Log(s);
        lightSensorValue = int.Parse(s) / 1024f;
    }

    void OnDestroy()
    {
        Kill();
    }

    void OnApplicationQuit()
    {
        Kill();
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;
        var shortWait = new WaitForSeconds(1/60f);
        var frameWait = new WaitForEndOfFrame();
        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return frameWait;
            }
            else
                yield return shortWait;

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
        {
            Debug.LogError(" FAIL " + diff.Milliseconds + " " + timeout);
            fail();
        }
    }

    public string[] GetSerialPortNames()
    {
        string[] ports = SerialPort.GetPortNames();
        return ports;
    }

    void Update () {
		
	}
}
