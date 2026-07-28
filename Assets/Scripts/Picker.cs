using UnityEngine;

public class Picker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brain") && PatientManager._instance.state == SurgeryState.BrainPulling)
        {
            PatientManager._instance.HoverOnAnimatedBrain();
            }
        else if (other.CompareTag("Brain") && PatientManager._instance.state == SurgeryState.BrainPushing)
        {
            PatientManager._instance.HoverOnBrain();

        }
        else if (other.CompareTag("Aneurysm") && PatientManager._instance.state == SurgeryState.Punture)
        {
            PatientManager._instance.Punture();

        }
    }
}
