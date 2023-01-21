using UnityEngine;

public class TranslatorTextMesh : MonoBehaviour
{
    [SerializeField] private string ruVersion;
    [SerializeField] private string enVersion;

    private void Start()
    {
        TextMesh toTranslate = GetComponent<TextMesh>();

        if (Application.systemLanguage == SystemLanguage.Russian)
        {
            toTranslate.text = ruVersion.ToString();
        }
        else
        {
            toTranslate.text = enVersion.ToString();
        }
    }
}
