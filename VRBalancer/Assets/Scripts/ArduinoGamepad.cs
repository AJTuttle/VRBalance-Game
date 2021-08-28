
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if !UNITY_ANDROID
using System.IO.Ports;
#endif

using System;
using System.Threading;


public class ArduinoGamepad : MonoBehaviour
{
	/*Configuration block*/
	public static int numDigitalIn = 3;
	public static int numAnalogIn = 4;
	public static int numDigitalOut = 1;
	public static int numAnalogOut = 1;
	const int startByte = 0x72;
	public string port = "\\\\.\\COM8";
	public int baudRate = 9600;

    public InputField comPortText;

	public long lastValue;
	public string lastValueString;


	public int maxValue = 1000000;
	public int minValue = -1000000;

    /*Private members*/

#if !UNITY_ANDROID
	SerialPort p;
#endif

    ThreadStart serialThreadStart;
	Thread serialThread;
	bool threadQuit = false;
	public bool[] digitalIns = new bool[numDigitalIn];
	public ushort[] analogIns = new ushort[numAnalogIn];
	public bool[] digitalOuts = new bool[numDigitalOut];
	public byte[] analogOuts = new byte[numAnalogOut];

#if !UNITY_ANDROID
	void Start()
	{
		
	}

    public void connectToArduino() {
        p = new SerialPort(comPortText.text, baudRate, Parity.None, 8, StopBits.One);
        p.ReadTimeout = 5;
        p.WriteTimeout = 5;
        p.Open();
        serialThreadStart = new ThreadStart(serialThreadFunc);
        serialThread = new Thread(serialThreadStart);
        serialThread.Start();

    }


    public int getAnalogInput(int a)
	{
		lock (analogIns)
		{
			return analogIns[a];
		}
	}

	public void setAnalogOutput(int a, int v)
	{
		lock (analogOuts)
		{
			analogOuts[a] = (byte)v;
		}

	}

	public bool getDigitalInput(int d)
	{
		lock (digitalIns)
		{
			return digitalIns[d];
		}
	}

	public void setDigitalOutput(int d, bool v)
	{
		lock (digitalOuts)
		{
			digitalOuts[d] = v;
		}
	}

	void writePacket()
	{
		p.BaseStream.WriteByte(startByte);
		lock (digitalOuts)
		{
			int bitField = 0;
			for (int i = 0; i < digitalOuts.Length; i++)
			{
				int shift = i % 8;
				if (digitalOuts[i])
				{
					bitField = bitField | (1 << shift);
				}
				if (shift == 7 || i == (digitalOuts.Length - 1))
				{
					if (bitField == startByte)
					{
						p.BaseStream.WriteByte(startByte);
					}
					p.BaseStream.WriteByte((byte)bitField);
					bitField = 0;
				}
			}
		}
		lock (analogOuts)
		{
			for (int i = 0; i < analogOuts.Length; i++)
			{
				if (analogOuts[i] == startByte)
				{
					p.BaseStream.WriteByte(startByte);
				}
				p.BaseStream.WriteByte(analogOuts[i]);
			}
			p.BaseStream.Flush();
		}
	}

	void readPacket(byte[] buffer)
	{
		p.BaseStream.Flush();
	}

	void serialThreadFunc()
	{
		byte[] buffer = new byte[numAnalogIn * 2 + (numDigitalIn + 7) / 8];
		int readIndex = 0;
		int lastByte = 0;

		while (!threadQuit)
		{
			int b = -1;
			string line;
			try
			{
				b = 0;// p.ReadByte();
				line = p.ReadLine();
				//Debug.Log(line);
			}
			catch (TimeoutException)
			{
				writePacket();
				continue;
			}

			try {
				//string val2 = line.Split(':')[1];
                long newValue;
                long.TryParse(line, out newValue);
				if (newValue < maxValue && newValue > minValue && newValue != -1) {
					lastValue = newValue;
				}

				lock (lastValueString) {
					lastValueString = line;
				}
			} catch (Exception e) {

			}


			//if (b == startByte && lastByte != startByte)
			//{
			//	lastByte = b;
			//	continue;
			//}
			//else if (b != startByte && lastByte == startByte)
			//{
			//	lastByte = b;
			//	readIndex = 0;
			//}
			//else if (b == startByte && lastByte == startByte)
			//{
			//	lastByte = 0;
			//}
			//else
			//{
			//	lastByte = b;
			//}
			//lock (buffer)
			//{
			//	buffer[readIndex++] = (byte)b;
			//}

			//if (readIndex == buffer.Length)
			//{
			//	readPacket(buffer);

			//	int.TryParse(line, out int newValue);
			//	if (newValue < maxValue && newValue > minValue && newValue != -1)
			//	{
			//		lastValue = newValue;
			//	}

			//	lock (lastValueString)
			//	{
			//		lastValueString = line;
			//	}

			//	writePacket();
			//	readIndex = 0;
			//}
		}
		if (p.IsOpen)
		{
			p.Close();
		}

	}

	void OnApplicationQuit()
	{
		threadQuit = true;
	}
#endif
}
