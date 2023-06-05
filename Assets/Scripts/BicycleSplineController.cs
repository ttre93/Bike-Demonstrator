using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
public class BicycleSplineController : MonoBehaviour {


    public Transform target; //waypoint target
    public GameObject road; 
    public Animator riderAnimator; //pedaling animation
    public Animator wheelsAnimator; //animation of wheels
    private BicyclePowerSim bicycleSim;
    public float power = 0;

    void Start() {
        bicycleSim = GetComponent<BicyclePowerSim>();
    }
   
    void Update() {
        //Raycast to get the slope normal and the road Y position to snap the bike on the road
        RaycastHit hit = new RaycastHit();
        float snapYPosition = this.transform.position.y;
        Vector3 normal = Vector3.up; 
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -Vector3.up, out hit, Mathf.Infinity)) {
            snapYPosition = hit.point.y;
            normal = hit.normal;
        }

        //get the inclination angle
        Vector3 temp = Vector3.Cross(normal, transform.forward);
        Vector3 d = Vector3.Cross(temp, normal);
        float angle = Mathf.Sign(d.y) * Mathf.Acos(Mathf.Clamp(normal.y, -1f, 1f));

        //set the slope grade
        bicycleSim.slopeGrade = Mathf.Lerp(bicycleSim.slopeGrade, Mathf.Tan(angle) * 100, Time.deltaTime * 5);
        GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().SetTrainerSlope(bicycleSim.slopeGradeInt);

        //power input
        power = GameObject.Find("BikeTrainer").GetComponent<BikeTrainer>().instantaneousPower;
        
        bicycleSim.SetPower(power);

        //move the gameObject
        Vector3 direction = (target.transform.position - this.transform.position).normalized;
        Vector3 newPos = transform.position + direction * (bicycleSim.speed / 3.6f) * Time.deltaTime;
        newPos.y = snapYPosition;
        transform.position = newPos;

        //rotate the gameObject
        Vector3 viewingVector = target.transform.position - transform.position;
        viewingVector.y = 0;
        if (viewingVector != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(viewingVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);
        }

        Quaternion slopeRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, 5f * Time.deltaTime);

        //animation
        riderAnimator.speed = Mathf.Min(power / 50f, 2);
        wheelsAnimator.speed = Mathf.Min(bicycleSim.speed / 3.6f, 1);
        
    }
}
