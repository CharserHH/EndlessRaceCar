using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform camTarget;
    public float height = 5f;
    public float rotationDamping = 1f;
    public float heightDamping = 1f;
    public float distance = 6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate() {
        if(camTarget == null){
            return;
        }

        float wantedRotationAngle = camTarget.eulerAngles.y;
        float wantedHeight = camTarget.position.y + height;
        float curRotationAngle = transform.eulerAngles.y;
        float curHeight = transform.position.y;

        curRotationAngle = Mathf.LerpAngle(curRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        curHeight = Mathf.Lerp(curHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, curRotationAngle, 0);

        transform.position = camTarget.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position = new Vector3(transform.position.x, curHeight, transform.position.z);

        transform .LookAt(camTarget);
    }
}
