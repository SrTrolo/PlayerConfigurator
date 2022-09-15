using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float rotationSpeed;
    private void OnMouseDrag()
    {
        transform.Rotate(Vector3.down * Time.deltaTime * Input.GetAxis("Mouse X")* rotationSpeed);
    }

}
