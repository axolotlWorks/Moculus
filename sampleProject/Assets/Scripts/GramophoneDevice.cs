using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

//Moculus Gramophone device input controller

public class GramophoneDevice : MonoBehaviour {

    public string Port = "COM3";

    public float VelocityScale = 1.0f;

    SerialPort stream;

    float velocity;

    private static GramophoneDevice instance;

    void Awake(){
        instance = this;
    }
    public static GramophoneDevice Instance(){
        return instance;
    }

    public float GetVelocity()
    {
        return - velocity * VelocityScale;
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


    public void OpenA()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("A");
        }
    }

    public void CloseA()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("a");
        }
    }

    public void OpenB()
    {
        if(stream.IsOpen)
        {
            stream.WriteLine("B");
        }
    }

    public void CloseB()
    {
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

	void Start () {
        velocity = 0;
        stream = new SerialPort(Port,115200);
        stream.Open();
		
	}

    static string[] stringSeparators = new string[] { "\r\n" };

	void Update () {

        if (stream.IsOpen)
        {
            string value = stream.ReadExisting();
            if(value == "") return;
            string[] lines;
            lines = value.Split(stringSeparators, System.StringSplitOptions.None);

            var line = lines[lines.Length - 2];

            string[] words;
            words = line.Split(' ');

            if(words.Length == 3) velocity = float.Parse(words[1]);

        }
		
	}
}
