using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InspectionManager : MonoBehaviour
{
    public Button button;
    bool clipped,puntured;
    public GameObject secondClip;
    public GameObject bloodFX;
    public TextMeshProUGUI upperText, lowerText;
    public Image lowerImg, upperImg;
    private void Awake()
    {
        button.onClick.AddListener(() => { SceneManager.LoadScene(0); });
    }
    IEnumerator Start()
    {
        float time = 0;
        Image img1 = upperImg;
        Image img2 = lowerImg;
        int i = 0;
        while (true)
        {
            if (!clipped)
            {
                upperText.text = $"{120 + Random.Range(-5, 5)}/{80 + Random.Range(-5, 5)}";
                lowerText.text = $"{Random.Range(20, 61)}";

            }
            else if (clipped &&!puntured)
            {
                upperText.text = $"{96 + Random.Range(-5, 5)}/{64 + Random.Range(-5, 5)}";
                lowerText.text = $"{Random.Range(16, 48)}";

            }
            else if (clipped && puntured && i<4)
            {
                upperText.text = $"{96 + ((120 - 96) / 5 * (i + 1)) + Random.Range(-5, 5)}/{64 + ((80 - 64) / 5 * (i + 1)) + Random.Range(-5, 5)}";
                lowerText.text = $"{Random.Range(16, 48)}";

                i++;
            }
            else
            {
                upperText.text = $"{120 + Random.Range(-5, 5)}/{80 + Random.Range(-5, 5)}";

                lowerText.text = $"{Random.Range(20, 61)}";
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn") &&!clipped)
        {
            clipped = true;
            other.gameObject.SetActive(false);
            secondClip.SetActive(true);
        }
        else if (other.CompareTag("Finish") && clipped && !puntured)
        {
            LeanTween.scale(gameObject, transform.localScale / 2, 2);
            puntured = true;
            bloodFX.SetActive(true);
        }
    }
}
