using UnityEngine;
using UnityEngine.UI;


// Chuyển đổi ngôn ngữ theo ngôn ngữ đã chọn trong cài đặt.

[RequireComponent(typeof(Text))]
public class MultiText : MonoBehaviour
{
    
    // TEXT cần dịch
    Text text = null;

    // Tệp chứa các bản dịch khác nhau 
    [SerializeField] TranslateText textAsset = null;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        UpdateText(Options.ActiveLanguage);
    }

    
    // Cập nhật ngôn ngữ mới
    
    void UpdateText(Options.Language language)
    {
        text.text = textAsset.GetText(language);
    }
}