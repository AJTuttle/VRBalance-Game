

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if !UNITY_ANDROID
using System.IO.Ports;
#endif
public enum PressState
{
    LONGPRESSED,
    UNPRESSED,
    SINGLEPRESSED,
    RELEASED
}
public class Stick : MonoBehaviour
{
    public ArduinoGamepad gamePad;
    public bool polynomialFit;
    public double[] ploynomialCoefficients = { 18.7562e-12, -172.8266e-9, 900.6556e-3 }; //obtained from 2nd degree polynomial fit
    public long stick_value;
    //Linear regression values
    public float slope, intercept;

    public double weight_on_stick; 
    // public long heart_rate;

    public long maxValue = 1000000;
    public long minValue = -1000000;
    public PressState button_state;
    public long press_threshold;
    public long press_release_threshold;
    float press_time;
    public float long_press_threshold = 1.0f;


    byte pressStateByte = 0;  //0 == unpress, 1 = down , 2 = pressed 3 = up

    // Start is called before the first frame update
    void Start()
    {

        button_state = PressState.UNPRESSED;
        press_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        long newValue = gamePad.lastValue;
        

        if (newValue < maxValue && newValue > minValue && newValue != -1)
        {
            stick_value = newValue;
            if (polynomialFit)
            {
                weight_on_stick = ploynomialCoefficients[0] * stick_value * stick_value + ploynomialCoefficients[1] * stick_value + ploynomialCoefficients[2];
            }
            else
            {
                weight_on_stick = (stick_value - intercept) / slope;
            }
        }


        if (stick_value < press_threshold) {
            if (pressStateByte < 2) pressStateByte++;
            else if (pressStateByte == 3) pressStateByte = 1;
        }
        else if (stick_value > press_threshold && (pressStateByte == 1 || pressStateByte == 2)) {
            pressStateByte = 3;
        }
        else {
            pressStateByte = 0;
        }


        //handle input from the smart stick
        if (stick_value < press_threshold && (button_state == PressState.UNPRESSED || button_state == PressState.RELEASED))
        {
            //single pressed | rising edge
            press_time = Time.time;
            button_state = PressState.SINGLEPRESSED;
            //Debug.Log("single pressed " + button_state);
        }
        else if (stick_value < press_threshold && (button_state == PressState.SINGLEPRESSED || button_state == PressState.LONGPRESSED) && Time.time - press_time >= long_press_threshold)
        {
            // button is long pressed
            button_state = PressState.LONGPRESSED;
            //Debug.Log("long pressed " + button_state);
        }
        else if (stick_value >= press_release_threshold && (button_state == PressState.SINGLEPRESSED || button_state == PressState.LONGPRESSED))
        {
            // button released: falling edge : 
            button_state = PressState.RELEASED;
            //Debug.Log("released " + button_state);
        }
        else if (stick_value >= press_release_threshold)
        {
            button_state = PressState.UNPRESSED;
            //Debug.Log("unpressed: " + button_state);
        }

    }

    public bool get() {
        return pressStateByte > 0;
    }
    public bool getUp() {
        return pressStateByte == 3;
    }
    public bool getDown() {
        return pressStateByte == 1;
    }

}
