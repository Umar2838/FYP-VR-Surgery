using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class SkullCut : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bowl") && PatientManager._instance.state == SurgeryState.SkullRemoving)
        {
            gameObject.SetActive(false);

            PatientManager._instance.skull2.gameObject.transform.position = other.transform.position; 
            PatientManager._instance.skullBowlReseter= new TransformReseter(PatientManager._instance.skull2.gameObject);
            PatientManager._instance.skull2.gameObject.SetActive(true);
            PatientManager._instance.StartPushBrainTut();
        }
        else if (other.CompareTag("Finish") && PatientManager._instance.state == SurgeryState.SkullFixing)
        {
            PatientManager._instance.skullSkinReseter.ResetTransform();
            PatientManager._instance.skull.gameObject.SetActive(true);
            gameObject.SetActive(false);
            PatientManager._instance.StartSkinFixTut();
            //var eventListener = GetComponent<InteractableUnityEventWrapper>();
            //eventListener.WhenSelect.RemoveAllListeners();
            //eventListener.WhenUnselect.RemoveAllListeners();
            //Debug.LogError(eventListener.WhenSelect.GetPersistentEventCount());
            //GetComponent<HandGrabInteractable>().enabled = false;
        }
    }
}
