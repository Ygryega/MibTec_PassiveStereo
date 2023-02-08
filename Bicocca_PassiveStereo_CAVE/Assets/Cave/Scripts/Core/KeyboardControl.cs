using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : NetworkBehaviour
{
    [SerializeField] int speed;
    [SerializeField] int rotationSpeed;

    [Header("Input Axis Name")]
    [SerializeField] private string xMovementAxis;
    [SerializeField] private string yMovementAxis;
    [SerializeField] private string zMovementAxis;

    [SerializeField] private string yRotationAxis;

    [ServerCallback]
    void Update()
    {
        float xTranslation = Input.GetAxis(xMovementAxis);
        float yTranslation = Input.GetAxis(yMovementAxis);
        float zTranslation = Input.GetAxis(zMovementAxis);

        xTranslation *= Time.deltaTime * speed;
        yTranslation *= Time.deltaTime * speed;
        zTranslation *= Time.deltaTime * speed;

        transform.Translate(xTranslation, yTranslation, zTranslation);

        float yRotation = Input.GetAxis(yRotationAxis);
        transform.Rotate(0, yRotation * Time.deltaTime * rotationSpeed, 0);

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            transform.Rotate(0, 10 * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            transform.Rotate(0, -10 * Time.deltaTime, 0);
        }
    }
}
