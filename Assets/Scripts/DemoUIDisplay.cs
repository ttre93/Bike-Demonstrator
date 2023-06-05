using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DemoUIDisplay : MonoBehaviour {

    public BicyclePowerSim bicycleSim;
    public Text envText;
    public Text speedText;
    public Text uiText;


    void Update() {


        float totalF = Mathf.Abs(bicycleSim.Fgravity) + Mathf.Abs(bicycleSim.FrollingResitance )+ Mathf.Abs(bicycleSim.FaeroDrag);
        float fpercent = 0;
        float rpercent = 0;
        float apercent = 0;
        if (totalF != 0) {
            fpercent = Mathf.Abs(bicycleSim.Fgravity) / totalF * 100;
            rpercent = Mathf.Abs(bicycleSim.FrollingResitance) / totalF * 100;
            apercent = Mathf.Abs(bicycleSim.FaeroDrag) / totalF * 100;
        }

        envText.text = "Slope: " + bicycleSim.slopeGrade.ToString("F1") + " %" + "\n"

                       + "Gravity Force: " + bicycleSim.Fgravity.ToString("F1") + " n" + " (" + fpercent.ToString("F0") + "%)" + "\n"
                       + "Roll Resist Force: " + bicycleSim.FrollingResitance.ToString("F1") + " n" + " (" + rpercent.ToString("F0") + "%)" + "\n"
                       + "Aero Drag Force: " + bicycleSim.FaeroDrag.ToString("F1") + " n" + " (" + apercent.ToString("F0") + "%)" + "\n"
                       + "Resisting Force: " + bicycleSim.resistingForce.ToString() + " n" + "\n";

        speedText.text = "Speed: " + bicycleSim.speed.ToString("F1") + " km/h" + "\n"
                     + "Power: " + bicycleSim.power.ToString("F0") + " w";

        uiText.text = "";
        uiText.text += "Fitness E: " + GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().connected + "\n";
        uiText.text += "power = " + GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().instantaneousPower + "\n";
        uiText.text += "speed= " + GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().speed + "\n";
        uiText.text += "elapsedTime= " + GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().elapsedTime + "\n";
        uiText.text += "distanceTraveled= " + GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().distanceTraveled + "\n";

    }
}
