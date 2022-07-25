using UnityEngine;
using UnityEngine.UI;

class PhotoHandler
{
    private static string _path;
    private static NativeCamera.Permission _permission;

    public static string Path {
        get
        {
            string ret = _path;
            _path = null;
            return ret;
        }
    }
    public static NativeCamera.Permission Permission { get => _permission; }

    public static void TakePicture(int maxSize, RawImage img = null, Texture texture = null)
    {
        _permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log($"Photo Handler - Image path: {path}");
            if (path != null)
            {
                _path = path;
                // Create a Texture2D from the captured image
                Texture2D photoTexture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (photoTexture == null)
                {
                    Debug.Log($"Photo Handler - Couldn't load texture from {path}");
                    return;
                }
                NativeCamera.ImageProperties properties = NativeCamera.GetImageProperties(path);
                if (img != null)
                    ApplyToImage(photoTexture, properties, img);

                if (texture != null)
                    ApplyToTexture(photoTexture, texture);
                return;
            }
            else
            {
                _path = null;
                Debug.Log($"Photo Handler - No path!");
                return;
            }
        }, maxSize);

        Debug.Log($"Photo Handler - Permission result: {_permission}");
    }

    private static void ApplyToImage(Texture2D photoTexture, NativeCamera.ImageProperties properties, RawImage img) 
    {
        Vector2 sd = img.rectTransform.sizeDelta;
        float max = Mathf.Max(sd.x, sd.y);
        float r = (float)properties.width / (float)properties.height;
        switch (properties.orientation)
        {
            case NativeCamera.ImageOrientation.Rotate270:
            case NativeCamera.ImageOrientation.Rotate90:
                {
                    Debug.Log($"Photo Handler - Portrait");
                    sd.Set(max*r, max);
                    break;
                }
            default:
                {
                    Debug.Log($"Photo Handler - Landscape");
                    sd.Set(max, max/r);
                    break;
                }
        }

        img.rectTransform.sizeDelta = sd;
        img.texture = photoTexture;

    }

    private static void ApplyToTexture(Texture2D photoTexture, Texture texture)
    {
        texture = photoTexture;
    }


}

