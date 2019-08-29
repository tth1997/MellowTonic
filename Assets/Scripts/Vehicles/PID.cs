using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PID : MonoBehaviour
{
    public float Kp;
    public float Ki;
    public float Kd;

    public float outputMax;
    public float outputMin;

    public float preError;

    public float integral;
    //public float integralMax;
    //public float integralMin;

    public float derivative;

    public float output;

    /*
    public void SetBounds()
    {
        integralMax = Divide(outputMax, Ki);
        integralMin = Divide(outputMin, Ki);
    }

    public float Divide(float a, float b)
    {
        System.Func<float, float> inv = (n) => 1 / (n != 0 ? n : 1);
        var iVec = inv(b);
        return a * iVec;
    }
    public Vector3 MinMax(Vector3 min, Vector3 max, Vector3 val)
    {
        return Vector3.Min(Vector3.Max(min, val), max);
    }
    */

    public float Cycle(float currentSpeed, float targetSpeed, float Dt)
    {
        var error = targetSpeed - currentSpeed;
        integral += error + Dt;
        derivative = (error - preError) / Dt;
        output = error * Kp + integral * Ki + derivative * Kd;

        preError = error;
        return output;
    }
}
