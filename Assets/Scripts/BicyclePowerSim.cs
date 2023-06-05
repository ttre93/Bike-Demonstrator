using UnityEngine;
using System.Collections;

public class BicyclePowerSim : MonoBehaviour {

    //Environmental  parameters 
    public float slopeGrade = 0; //the slope %
    public float airDensity = 1.226f;


    //Bike and rider parameters
    public float totalMass = 85; //mass of the rider and bike in kg
    public float frontalArea = 0.5f;  //front surface of the rider and bike in m2
    public float drivetrainEffectiveness = 95;
    public float coefficientOfRollingResistance = 0.004f; 
    public float dragCoef = 0.63f; 
    public float power { get; private set; }
    public float speed {
        get {
            return currentVelocity * 3.6f;
        }
    }

    private float currentVelocity = 0; //the speed in m/s
    public float Fgravity { get; private set; }
    public float FrollingResitance { get; private set; }
    public float FaeroDrag { get; private set; }
    public float resistingForce;
    public int slopeGradeInt = 0;   //value of slope to be send to the trainer

    //calculate current velocity based on power input and resisting forces
    public float SetPower(float powerInput, bool instant = true) {
        power = powerInput;
        resistingForce = CalculateResistingForces(currentVelocity);
        float wheelPower = drivetrainEffectiveness * powerInput / 100f;
        float totalForce = wheelPower - (resistingForce);

        slopeGradeInt = Mathf.RoundToInt(slopeGrade);

        float acceleration = totalForce / totalMass;
        if (instant)
            currentVelocity += acceleration;
        else

            currentVelocity += (acceleration * Time.deltaTime);

        return currentVelocity * 3.6f;
    }

    //calculate resisting forces based on current velocity, slope and resistance parameters
    private float CalculateResistingForces(float velocity) {
        //Fgravity = 9.8067f * 1 * Mathf.Sin(Mathf.Atan(slopeGrade / 100f)); //bike goes backwards when velocity < 0
        Fgravity = velocity * 9.8067f * 1 * Mathf.Sin(Mathf.Atan(slopeGrade / 100f)); //bike doesnt go backwards because velocity >= 0
        FrollingResitance = velocity * 9.8067f * 1 * Mathf.Cos(Mathf.Atan(slopeGrade / 100f)) * totalMass * coefficientOfRollingResistance;
        FaeroDrag = velocity * 0.5f * dragCoef * frontalArea * airDensity * (velocity * velocity);

        return Fgravity + FrollingResitance + FaeroDrag;

    }

}
