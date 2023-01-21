using UnityEngine;
using UnityEngine.UI;

public class Translator : MonoBehaviour
{
    [SerializeField] private string ruVersion;
    [SerializeField] private string enVersion;

    private void Start()
    {
        Text toTranslate = GetComponent<Text>();

        if(Application.systemLanguage == SystemLanguage.Russian)
        {
            toTranslate.text = ruVersion.ToString();
        }
        else
        {
            toTranslate.text = enVersion.ToString();
        }
    }
}
