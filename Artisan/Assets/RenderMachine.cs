using UnityEngine;
using System.IO;

public class RenderMachine : MonoBehaviour
{
    [SerializeField] RenderTexture output;
    [SerializeField] Camera cam;
    [SerializeField] string outputFilePath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam.Render();
        Texture2D texture = new Texture2D(output.width, output.height);
        RenderTexture.active = output;
        texture.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);
        texture.Apply();
        File.WriteAllBytes(outputFilePath, texture.EncodeToPNG());
    }

}
