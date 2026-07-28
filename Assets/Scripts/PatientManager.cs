using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PatientManager : MonoBehaviour
{
    public static PatientManager _instance;
    public Button startButton;
    public Button switchButton;
    public List<GameObject> borders = new();
    public HandGrabInteractable skin, skull, skull2, brain, animatedBrain, clip, lancet, picker;
    public bool ezMode;
    public GameObject stiches;
    public GameObject blood;
    public TextMeshProUGUI upperText, lowerText;
    public Image lowerImg, upperImg;
    public int bpUpper, bpLower;
    [Header("Hint GameObjects")]
    public GameObject lancetHint;
    public GameObject cutTheSkin;
    public GameObject cutTheSkull;
    public GameObject refixSkull;
    public GameObject pushTheBrain;
    public GameObject grabTheClip;
    public GameObject clipTheAneurysm;
    public TextMeshProUGUI animatedBrainText, pickerText;
    public List<GameObject> clippedClips = new();
    public List<GameObject> aneurysms = new();
    public TransformReseter lancetReseter, skullSkinReseter, skullBowlReseter, skinReseter;
    public SurgeryState state;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        stiches.SetActive(false);
        skullSkinReseter = new(skull.gameObject);
        clip.enabled = false;
        lancet.enabled = false;
        startButton.onClick.AddListener(() =>
        {
            startButton.transform.root.gameObject.SetActive(false);
            StartGame();
        });
        foreach (var item in borders)
        {

            item.gameObject.SetActive(false);
        }
        switchButton.onClick.AddListener(() => { SceneManager.LoadScene(1); }) ;
        StartCoroutine(StartBeating());
    }
    IEnumerator StartBeating()
    {
        float time = 0;
        Image img1 = upperImg;
        Image img2 = lowerImg;

        while (true)
        {
            if (state == SurgeryState.Punture || state == SurgeryState.BrainPulling ||
     state == SurgeryState.BrainPushing || state == SurgeryState.Cliping)
            {
                upperText.text = $"{96+ UnityEngine.Random.Range(-5, 5)}/{64 + UnityEngine.Random.Range(-5, 5)}";
                lowerText.text= $"{UnityEngine.Random.Range(20, 61)}";

            }
            else
            {
                upperText.text = $"{120 + UnityEngine.Random.Range(-5, 5)}/{80 + UnityEngine.Random.Range(-5, 5)}";
                lowerText.text = $"{UnityEngine.Random.Range(16, 48)}";
            }
            time = 0;
            img1.fillAmount = 0;
            img2.fillAmount = 0;
            while (time < 2)
            {
                img1.fillAmount = time / 2;
                img2.fillAmount = time / 2;
                time += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
    public void StartGame()
    {
        foreach (var item in borders)
        {

            item.gameObject.SetActive(true);
        }
        lancet.enabled = true;

        skinReseter = new TransformReseter(skin.transform.parent.gameObject);
        //lancetReseter = new(lancet.transform.parent.gameObject);
        //lancet.GetComponentInParent<InteractableUnityEventWrapper>().WhenUnselect.AddListener(() =>
        //{
        //    lancetReseter.Reset();
        //});
        CutSkinTut();
    }
    public void CutSkinTut()
    {
        state = SurgeryState.SkinCutting;
        lancetHint.SetActive(true);
    }
    public void RemoveBorder(GameObject border)
    {
        if (state != SurgeryState.SkinCutting)
        {
            return;
        }
        border.gameObject.SetActive(false);

        if (borders.Find(x => x.activeInHierarchy) == null || ezMode)
        {
            
            state = SurgeryState.SkinPeeling;

            skin.enabled = true;
            cutTheSkin.SetActive(true);
            cutTheSkin.GetComponentInChildren<TextMeshProUGUI>().text = "Peel the Skin";
            //lancet.enabled = false;
            //lancetReseter.Reset();
        }
    }
    public void LancetGrabbed(bool isGrabbed)
    {
        if (isGrabbed)
        {
            lancetHint.SetActive(false);
            if (state == SurgeryState.SkinCutting)
            {
                cutTheSkin.SetActive(true);
            }

        }
        else
        {
            if (state == SurgeryState.SkinCutting)
                lancetHint.SetActive(true);
        }
    }
    public void ListenSkin(GameObject go)
    {
        if (go.transform.eulerAngles.x > 0 && go.transform.eulerAngles.x < 50 && state == SurgeryState.SkinPeeling)
        {

            skin.enabled = false;
            cutTheSkin.SetActive(false);
            cutTheSkull.SetActive(true);
            skull.enabled = true;
            state = SurgeryState.SkullRemoving;

            var eventListener = skull.GetComponent<InteractableUnityEventWrapper>();

            var text = cutTheSkull.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "Put the skull on right Bowl.";
            eventListener.WhenSelect.AddListener(() =>
            {

                cutTheSkull.SetActive(true);
                text.text = "Put the skull on right Bowl.";

            });
            eventListener.WhenUnselect.AddListener(() =>
            {
                text.text = "Grab the skull";
                skullSkinReseter.ResetTransform();

            });
            Debug.Log(go.transform.rotation.eulerAngles);

        }
        else if (state == SurgeryState.SkinFixing)
        {
            skinReseter.ResetTransform();
            state = SurgeryState.None;
            skin.enabled = false;
            stiches.SetActive(true);
            StartCoroutine(DelayRestart());

        }

    }
    IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }
    public void SkullToDefault()
    {
        if (state == SurgeryState.None)
        {
            return;
        }
        if (state == SurgeryState.SkullRemoving)
        {
            return;
        }
        Debug.LogError("reset");
        skullSkinReseter.ResetTransform();
    }
    public void SkullToDefaultBowl()
    {
        if (state == SurgeryState.None)
        {
            return;
        }
        skullBowlReseter.ResetTransform();
    }
    public void StartPushBrainTut()
    {
        if (state == SurgeryState.None)
        {
            return;
        }
        skull.enabled = false;
        skull.gameObject.SetActive(false);
        skullBowlReseter.ResetTransform();
        state = SurgeryState.BrainPushing;
        cutTheSkull.SetActive(false);
        picker.enabled = true;
        pickerText.text = "Grab the Picker";
        pickerText.transform.parent.gameObject.SetActive(true);

        var pickerListener = picker.GetComponentInParent<InteractableUnityEventWrapper>();
        pickerListener.WhenSelect.AddListener(() => { PickerGrab(true); });
        pickerListener.WhenUnselect.AddListener(() => { PickerGrab(false); });
        //brain.enabled = true;
    }
    public void PickerGrab(bool grab)
    {
        if (state == SurgeryState.BrainPushing)
        {
            if (grab)
            {
                pickerText.transform.parent.gameObject.SetActive(false);
                pushTheBrain.SetActive(true);
            }
            else
            {
                pushTheBrain.SetActive(false);
                pickerText.transform.parent.gameObject.SetActive(true);
            }
        }
        else if (state == SurgeryState.BrainPulling)
        {
            if (grab)
            {
                pickerText.transform.parent.gameObject.SetActive(false);
                animatedBrainText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                animatedBrainText.transform.parent.gameObject.SetActive(false);
                pickerText.transform.parent.gameObject.SetActive(true);
            }
        }
        else if (state == SurgeryState.Punture)
        {
            if (grab)
            {
                pickerText.transform.parent.gameObject.SetActive(false);
                animatedBrainText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                animatedBrainText.transform.parent.gameObject.SetActive(false);
                pickerText.transform.parent.gameObject.SetActive(true);
            }
        }
        else
        {
            pickerText.transform.parent.gameObject.SetActive(false);

        }
    }
    public void HoverOnBrain()
    {

        if (state == SurgeryState.None)
        {
            return;
        }
        state = SurgeryState.None;
        brain.enabled = false;
        TransformReseter brainReset = new TransformReseter(brain.transform.parent.gameObject);
        LeanTween.rotate(brain.transform.parent.gameObject, animatedBrain.transform.parent.gameObject.transform.eulerAngles, 1.5f);
        LeanTween.scale(brain.transform.parent.gameObject, animatedBrain.transform.parent.gameObject.transform.lossyScale, 1.5f);
        LeanTween.move(brain.transform.parent.gameObject, animatedBrain.transform.parent.gameObject.transform.position, 1.5f)

            .setOnComplete(() =>
            {
                state = SurgeryState.Cliping;
                clipTheAneurysm.GetComponentInChildren<TextMeshProUGUI>().text = "Clip the Aneurysm";
                brain.enabled = false;
                brain.transform.parent.gameObject.SetActive(false);
                animatedBrain.transform.parent.gameObject.SetActive(true);
                clip.enabled = true;
                grabTheClip.SetActive(true);
                pushTheBrain.SetActive(false);
                brainReset.ResetTransform();


            });


    }
    public void HoverOnAnimatedBrain()
    {
        if (state == SurgeryState.None)
        {
            return;
        }
        if (state != SurgeryState.BrainPulling)
        {
            return;
        }
        animatedBrain.enabled = false;
        state = SurgeryState.None;
        animatedBrain.enabled = false;
        var pickerListener = picker.GetComponentInParent<InteractableUnityEventWrapper>();
        pickerListener.WhenSelect.RemoveAllListeners();
        pickerListener.WhenUnselect.RemoveAllListeners();
        LeanTween.rotate(animatedBrain.transform.parent.gameObject, brain.transform.parent.gameObject.transform.eulerAngles, 1.5f);
        LeanTween.move(animatedBrain.transform.parent.gameObject, brain.transform.parent.gameObject.transform.position, 1.5f)

            .setOnComplete(() =>
            {
                state = SurgeryState.SkullFixing;
                animatedBrainText.transform.parent.gameObject.SetActive(false);
                animatedBrain.enabled = false;
                animatedBrain.transform.parent.gameObject.SetActive(false);
                brain.transform.parent.gameObject.SetActive(true);
                refixSkull.transform.parent.gameObject.SetActive(true);
                skull2.enabled = true;
                var eventListener = skull2.GetComponent<InteractableUnityEventWrapper>();
                var text = refixSkull.GetComponentInChildren<TextMeshProUGUI>();
                refixSkull.gameObject.SetActive(true);
                text.text = "Grab the skull";

                skull.enabled = true;

                eventListener.WhenSelect.AddListener(() =>
                {
                    if (state == SurgeryState.SkullFixing)
                    {
                        text.text = "Refix the skull";
                    }
                });
                eventListener.WhenUnselect.AddListener(() =>
                {
                    if (state == SurgeryState.SkullFixing)
                    {
                        SkullToDefaultBowl();
                        text.text = "Grab the skull";
                    }
                });
            });





    }
    public void ClipGrab(bool grab)
    {

        if (grab)
        {
            if (state == SurgeryState.Cliping)
            {
                clipTheAneurysm.SetActive(true);
                grabTheClip.SetActive(false);
            }


        }
        else
        {
            if (state == SurgeryState.Cliping)
            {
                clipTheAneurysm.gameObject.SetActive(true);
                
            }
            else if (state != SurgeryState.Cliping)
            {
                clipTheAneurysm.SetActive(false);
                grabTheClip.gameObject.SetActive(false);
            }


        }
    }
    public void ClipAneurysm()
    {

        if (state != SurgeryState.Cliping)
        {
            return;
        }

        grabTheClip.SetActive(false);
        clipTheAneurysm.SetActive(false);
        foreach (var item in clippedClips)
        {

            item.SetActive(true);
        }
        clip.gameObject.SetActive(false);
        clip.enabled = false;
        StartCoroutine(DelayAction(delegate
        {

            //animatedBrain.enabled = true;
            state = SurgeryState.Punture;
            pickerText.text = "Grab the Picker";
            pickerText.transform.parent.gameObject.SetActive(true);
            animatedBrainText.text = "Punture with spacer";

        }, 2));

    }
    public void Punture()
    {
        if (state == SurgeryState.None)
        {
            return;
        }
        state = SurgeryState.None;
        blood.SetActive(true);

        foreach (var item in aneurysms)
        {

            Vector3 size = item.gameObject.transform.localScale / 1.8f;
            LeanTween.scale(item, size, 2f).setOnComplete(() =>
            {
                if (state != SurgeryState.BrainPulling)
                {
                    state = SurgeryState.BrainPulling;
                    animatedBrainText.transform.parent.gameObject.SetActive(true);
                    animatedBrainText.text = "Pull Brain to initial position";
                }
            });
        }
    }
    public void SkullFixed()
    {
        skullSkinReseter.ResetTransform();
    }

    internal void StartSkinFixTut()
    {
        state = SurgeryState.SkinFixing;
        skin.enabled = true;
        skull.enabled = false;
        skull2.enabled = false;
        cutTheSkull.SetActive(false);
        refixSkull.SetActive(false);
    }
    IEnumerator DelayAction(UnityAction action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }
}
[Serializable]
public class TransformReseter
{
    public GameObject go;
    public Vector3 initialPos;
    public Quaternion initialRot;

    public TransformReseter(GameObject go)
    {
        this.go = go;
        initialPos = go.transform.position;
        initialRot = go.transform.rotation;
    }
    public void ResetTransform()
    {
        go.transform.position = initialPos;
        go.transform.rotation = initialRot;
    }

}
public enum SurgeryState
{
    None, SkinCutting, SkinPeeling, SkullRemoving, BrainPushing, Cliping, Punture, BrainPulling, SkullFixing,
    SkinFixing
}
