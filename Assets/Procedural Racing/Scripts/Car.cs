using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Rigidbody rb;
    public Transform[] wheelMeshes;
    public WheelCollider[] wheelColliders;

    public int rotateSpeed;
    public int rotationAngle;
    public int wheelRatateSpeed;
    private int targetRotation;

    public Transform[] grassEffects;
    public float grassEffectOffset;
    public Transform[] skidMarkPivots;

    public Transform back;
    public float constantBackForce;
    public GameObject skidMark;
    public float skidMarkSize;
    public float skidMarkDelay;
    public float minRotationDifference;
    
    public WorldGenerator worldGenerator;
    public GameObject ragdoll;
    private float lastRotation;
    private bool skidMarkRoutine;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SkidMark());
    }
    private void FixedUpdate() {
        // 更新车轮痕迹与粒子特效
        UpdateEffects();
    }
    void LateUpdate()
    {
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            Quaternion quat;
            Vector3 pos;
            wheelColliders[i].GetWorldPose(out pos, out quat);

            wheelMeshes[i].position = pos;
            wheelMeshes[i].Rotate(Vector3.right * wheelRatateSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetMouseButton(0)){
            // 有方向输入
            UpdateTargetRotation();
        } else if(targetRotation != 0){
            // 没有方向输入
            targetRotation = 0;
        }

        Vector3 rotation = new Vector3(transform.localEulerAngles.x, targetRotation, transform.localEulerAngles.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation), rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 更新车的角度
    /// </summary>
    void UpdateTargetRotation(){
        if(Input.GetAxis("Horizontal") == 0){
            if(Input.mousePosition.x < Screen.width / 2){
                // Left
                targetRotation = -rotationAngle;
            } else {
                // Right
                targetRotation = rotationAngle;
            }
        } else {
            // Keyboard input
            targetRotation = (int)Input.GetAxis("Horizontal") * rotationAngle;
        }
    }
    void UpdateEffects(){
        bool addForce = true;
        bool rotated = Mathf.Abs(transform.localEulerAngles.y - lastRotation) > minRotationDifference;
        for (int i = 0; i < grassEffects.Length; i++){
            Transform wheelMesh = wheelMeshes[i + 2];

            RaycastHit hit;
            if(Physics.Raycast(wheelMesh.position, Vector3.down, out hit, grassEffectOffset * 1.5f)){
                if(!grassEffects[i].gameObject.activeSelf){
                    grassEffects[i].gameObject.SetActive(true);
                }

                grassEffects[i].position = hit.point;
                skidMarkPivots[i].position = hit.point;
                grassEffects[i].rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                
                addForce = false;
            } else if (grassEffects[i].gameObject.activeSelf){
                grassEffects[i].gameObject.SetActive(false);
            }
        }

        if(addForce){
            rb.AddForceAtPosition(Vector3.down * constantBackForce, back.position);
            skidMarkRoutine = false;
        } else {
            if (targetRotation != 0){
                if (rotated && !skidMarkRoutine){
                    skidMarkRoutine = true;
                } else if (!rotated && skidMarkRoutine){
                    skidMarkRoutine = false;
                }
            } else { // 直行
                skidMarkRoutine = false;
            }
        }
        lastRotation = transform.localEulerAngles.y;
    }
    public void FallApart(){
        GameObject tmp = Instantiate(ragdoll, transform.position + new Vector3(0,0.5f,0), transform.rotation);
        // 施加向上的力
        tmp.GetComponent<Rigidbody>().AddForce(Vector3.up * 10000, ForceMode.Impulse);
        gameObject.SetActive(false);
    }
    IEnumerator SkidMark(){
        while (true){
            yield return new WaitForSeconds(skidMarkDelay);

            if (!skidMarkRoutine){
                continue;
            }

            for (int i = 0; i < skidMarkPivots.Length; i++){
                GameObject newSkidMark = Instantiate(skidMark, skidMarkPivots[i].position, skidMarkPivots[i].rotation);
                newSkidMark.transform.SetParent(worldGenerator.GetWorldPiece());
                newSkidMark.transform.localScale = new Vector3(1, 1, 4) * skidMarkSize;
            }
        }
    }
}
