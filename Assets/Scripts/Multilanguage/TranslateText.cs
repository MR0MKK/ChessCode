using UnityEngine;

// Các bản dịch từ các ngôn ngữ
// Đọc thông qua lớp MultiText => quản lý bản dịch
[CreateAssetMenu]
public class TranslateText : ScriptableObject
{
    [TextArea(5, 10)] [SerializeField] string english = null;
    [TextArea(5, 10)] [SerializeField] string vietnamese = null;
    [TextArea(5, 10)] [SerializeField] string catalan = null;
    [TextArea(5, 10)] [SerializeField] string italian = null;

    
    // Chọn ngôn ngữ của bạn
    public string GetText(Options.Language language)
    {
        switch (language)
        {
            case Options.Language.EN:
                return english;
            case Options.Language.VI:
                return vietnamese;
            case Options.Language.CA:
                return catalan;
            case Options.Language.IT:
                return italian;
            default:
                return english;
        }
    }
}