using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;

    private WorldGenerator generator;
    private Car car;
    Transform carTransform;
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.FindObjectOfType<Car>();
        generator = GameObject.FindObjectOfType<WorldGenerator>();

        if (car != null){
            carTransform = car.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        if (car != null){
            CheckRotation();
        }
    }

    void CheckRotation(){
        Vector3 dir = Vector3.forward;

        float carRotation = carTransform.localEulerAngles.y;

        if (carRotation > car.rotationAngle * 2f){
            carRotation = (360 - carRotation) * -1f;
        }

        transform.Rotate(dir * -rotationSpeed * (carRotation / car.rotationAngle) * (36f / generator.dimensions.x) * Time.deltaTime);
    }
}
