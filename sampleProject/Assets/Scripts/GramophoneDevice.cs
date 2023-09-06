using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using Microsoft.Win32;
using System;

public class GramophoneDevice : MonoBehaviour {

    SerialPort stream;
    public string Port;
    public InputField velocityInput;
    public float VelocityScale;
    private static GramophoneDevice instance;
    //gramophoneStart
    float velocity;
	float inputVal;
	float systemTime;
	float inputVal2;
    //gramophoneEnd
    //treadmillStart
    float treadmillSystemTime;
    float treadmillVelocity;
    float treadmillRecording;
    float treadmillLick;
    float treadmillPortA;
    float treadmillPortB;
    float treadmillPortC;
    float treadmillPorts;
    //treadmillEnd
    static string[] stringSeparators = new string[] { "\r\n" };
    public bool reverseDirection;
    [HideInInspector] public bool OpenedA;
    [HideInInspector] public bool OpenedB;
    bool isGramophone;
    bool isTreadmill;

    public static GramophoneDevice Instance()
    {
        return instance;
    }

    public string AutodetectArduinoPort()
    {
        List<string> comports = new List<string>();
        string temp;
        RegistryKey rk1 = Registry.LocalMachine;
        RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
        foreach (string s in rk2.GetSubKeyNames())
        {
            RegistryKey rk3 = rk2.OpenSubKey(s);
            foreach (string s1 in rk3.GetSubKeyNames())
            {
                if (s1.Contains("VID_2341") && s1.Contains("PID_804"))
                {
                    Gramophone();
                    RegistryKey rk4 = rk3.OpenSubKey(s1);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null)
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
                if (s1.Contains("VID_2341") && s1.Contains("PID_0058"))
                {

                    Treadmill();
                    RegistryKey rk4 = rk3.OpenSubKey(s1);
                    foreach (string s2 in rk4.GetSubKeyNames())
                    {
                        RegistryKey rk5 = rk4.OpenSubKey(s2);
                        if ((temp = (string)rk5.GetValue("FriendlyName")) != null)
                        {
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                            {
                                comports.Add(temp);
                            }
                        }
                    }
                }
            }
        }

        if (comports.Count > 0)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                if (comports.Contains(s))
                    return s;
            }
        }

        return "COM100";
    }

    void Gramophone()
    {
        isGramophone = true;
        isTreadmill = false;
    }
    void Treadmill()
    {
        isGramophone = false;
        isTreadmill = true;
    }

    /*/public void inputVelocityValue (string stringVelocity)
	{
		VelocityScale = float.Parse(velocityInput.text);
	}*/

    public int Reverse()
    {
        if (reverseDirection == true)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public float GetVelocity()
    {
        return -velocity * VelocityScale * Reverse();
    }

    public void ResetTimer()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("R");
        }

    }

    //controlls a servo if you send numbers between 0 and 5
    public void ControlServo(int value)
    {
        if(value < 0 || value > 5) return;
        if(stream.IsOpen)
        {
            stream.WriteLine(value.ToString());
        }

    }

    public void ClosePort() 
	{
        stream.WriteLine("a");
        stream.WriteLine("b");
        stream.WriteLine("c");
        stream.Close();
        OpenedA = false;
        OpenedB = false;
    }

    public void OpenA()
    {
        OpenedA = true;

        if(stream.IsOpen)
        {
            stream.WriteLine("A");
        }

    }

    public void CloseA()
    {
        OpenedA = false;

        if(stream.IsOpen)
        {
            stream.WriteLine("a");
        }
    }

    public void OpenB()
    {
        OpenedB = true;

        if(stream.IsOpen)
        {
            stream.WriteLine("B");
        }
    }

    public void CloseB()
    {
        OpenedB = false;

         if(stream.IsOpen)
        {
            stream.WriteLine("b");
        }       
    }

    public void OpenC()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("C");
        }
    }

    public void CloseC()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("c");
        }        
    }

	public float GetInputVal()
	{
		return inputVal;  //recording
	}
	
	public float GetInputVal2()
	{
		return inputVal2;  //lick
	}
	
	public float GetSystime()
	{    
		return systemTime;
	}

    public float GetTreadmillPorts()
    {
        return treadmillPorts;
    }

    void Awake()
    {
        instance = this;
    }

    /* void Start()
     {
         Port = AutodetectArduinoPort();
         velocity = 0;
         stream = new SerialPort(Port, 115200);
         stream.Open();
         velocityInput.onEndEdit.AddListener(inputVelocityValue);
         //if (isTreadmill) VelocityScale = 0.06f;
         Reverse();
     }*/

    void Start()
    {
        Port = AutodetectArduinoPort();
        velocity = 0;
        stream = new SerialPort(Port, 115200);

        try
        {
            stream.Open();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open the serial port: " + e.Message);
            // Handle the error or provide a default behavior
            return;
        }

        //velocityInput.onEndEdit.AddListener(inputVelocityValue);
        Reverse();
    }


    void Update ()
    {
        if (stream.IsOpen)
        {
            string value = stream.ReadExisting();
            if(value == "") return;
            string[] lines;
            lines = value.Split(stringSeparators, System.StringSplitOptions.None);
            var line = lines[lines.Length - 2];
            string[] words;
            words = line.Split(' ');
			if (words.Length == 4)
            { 
			    systemTime = float.Parse (words [0]);
				velocity = float.Parse (words [1]);
				inputVal = float.Parse (words [2]);
				inputVal2 = float.Parse (words [3]);
				
			}
            if (words.Length > 4)
            {
                systemTime = float.Parse(words[0]);
                inputVal = float.Parse(words[1]);
                velocity = float.Parse(words[2]);
                inputVal2 = float.Parse(words[6]);
                treadmillPorts = float.Parse(words[8]);
            }

        }
		
	}
}
