using Oculus.Interaction.HandGrab;
using UnityEngine;

public class Clip : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Aneurysm"))
        {
            PatientManager._instance.ClipAneurysm();
            GetComponent<HandGrabInteractable>().enabled = false;
        }
    }
}
