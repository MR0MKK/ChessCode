using UnityEngine;

// Thêm hai hộp để điều chỉnh độ phân giải

public static class LetterBoxer
{
    
    public static void AddLetterBoxing()
    {
        // Một camera thấy thế được tạo => màn hình sẽ thay đổi độ phân giải
        // Nó được đặt sau main camera

        Camera letterBoxerCamera = new GameObject().AddComponent<Camera>();
        letterBoxerCamera.backgroundColor = Color.black;
        letterBoxerCamera.cullingMask = 0;
        letterBoxerCamera.depth = -100;
        letterBoxerCamera.farClipPlane = 1;
        letterBoxerCamera.useOcclusionCulling = false;
        letterBoxerCamera.allowHDR = false;
        letterBoxerCamera.allowMSAA = false;
        letterBoxerCamera.clearFlags = CameraClearFlags.Color;
        letterBoxerCamera.name = "Letter Boxer Camera";

        // Camera chính sẽ cho ra độ phân giải 16:9

        PerformSizing();
    }

    // Điều chỉnh kích thước của camera chính với độ phân giải được chỉ định.

    static void PerformSizing()
    {
        Camera mainCamera = Camera.main;

        float targetRatio = 16.0f / 9.0f;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        float scaleheight = windowaspect / targetRatio;

        // Nếu độ phân giải của máy ảnh nhỏ hơn 16:9, kích thước máy ảnh sẽ được chỉnh
        if (scaleheight < 1.0f)
        {
            Rect rect = mainCamera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            mainCamera.rect = rect;
        }

        // Nếu độ phân giải của máy ảnh nhỏ hơn 16:9, camera chính sẽ được thay đổi

        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = mainCamera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            mainCamera.rect = rect;
        }
    }
}