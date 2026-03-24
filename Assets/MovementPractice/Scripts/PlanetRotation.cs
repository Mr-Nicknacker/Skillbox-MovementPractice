using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField]private float _galaxyRotationSpeed=10f;
    [SerializeField]private float _planetRotationSpeed=100f;
    private Quaternion _rotationAroundSun;
    private Quaternion _rotationAroundSelf;
    private void FixedUpdate()
    {
        _rotationAroundSun = Quaternion.Euler(0f, _galaxyRotationSpeed*Time.deltaTime, 0f);
        _rotationAroundSelf = Quaternion.Euler(0f, _planetRotationSpeed * Time.deltaTime, 0f);
        transform.position = _rotationAroundSun * transform.position;
        transform.rotation *= _rotationAroundSelf;
    }
}
