using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("border"))
        {
            PatientManager._instance.RemoveBorder(other.gameObject);
        }
    }
}
