using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public static class EditorTextureToMultipleOf4
{
    [MenuItem("Assets/Pad textures to multiple of 4", priority=1200)]
    public static void MakeMultipleOf4()
    {
        foreach (Object o in Selection.objects)
        {
          
            Texture2D texture = o as Texture2D;
            if (texture != null)
            {
                
                //
                int mofW = 4 * Mathf.CeilToInt(texture.width / 4.0f);
                int mofH = 4 * Mathf.CeilToInt(texture.height / 4.0f);
                if (texture.width != mofW || texture.height != mofH)
                {
                    string origTexAssetPath = AssetDatabase.GetAssetPath(texture);
                    string exportPath = origTexAssetPath;

                    TextureImporter timp = (TextureImporter) TextureImporter.GetAtPath(origTexAssetPath);
                    var origIsReadable = timp.isReadable;
                    //timp.isReadable = true;
                    //timp.SaveAndReimport();

                    exportPath = exportPath.Substring(0, exportPath.Length - ".png".Length);
                    exportPath = exportPath + "_mof.png";

                

                    int xDiff = mofW-texture.width;
                    int yDiff = mofH - texture.height;
                    Texture2D nooTex = new Texture2D(mofW, mofH);

                    //make nooTex all transparent
                    nooTex.SetPixels(makeTransparentPixelArr(mofW * mofH));
                    nooTex.Apply();


                    //---------
                    // Create a temporary RenderTexture of the same size as the texture
                    RenderTexture tmp = RenderTexture.GetTemporary(
                                        texture.width,
                                        texture.height,
                                        0,
                                        RenderTextureFormat.Default,
                                        RenderTextureReadWrite.Linear);

                    // Blit the pixels on texture to the RenderTexture
                    Graphics.Blit(texture, tmp);
                    RenderTexture previous = RenderTexture.active;

                    RenderTexture.active = tmp;
                    // Create a new readable Texture2D to copy the pixels to it
                    //Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
                    // Copy the pixels from the RenderTexture to the new Texture
                    nooTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), xDiff/2, yDiff/2);
                    nooTex.Apply();
                    // Reset the active RenderTexture
                    RenderTexture.active = previous;
                    // Release the temporary RenderTexture


                    //---------

                    byte[] pngBytes = nooTex.EncodeToPNG();

                    string exportPathFull = 
                        //System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), exportPath);
                        System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), origTexAssetPath);

                    System.IO.File.WriteAllBytes(exportPathFull, pngBytes);
                    AssetDatabase.Refresh();

                    //TextureImporter nuTimp = (TextureImporter)TextureImporter.GetAtPath(exportPath);
                    //EditorUtility.CopySerialized(timp, nuTimp);
                    //nuTimp.SaveAndReimport();

                    //Texture2D AssetDatabase.LoadAssetAtPath<Texture2D>(exportPathFull);
                    //Debug.Log(AssetDatabase.GetAssetPath(tex));
                    //AssetDatabase.CreateAsset(nooTex, exportPath);

                    RenderTexture.ReleaseTemporary(tmp);
   
                }
            }
        }
    }

    static Color[] makeTransparentPixelArr(int nEls)
    {
        Color[] cont = new Color[nEls];
        for (int i = 0; i < cont.Length; i++)
        {
            cont[i] = new Color(0, 0, 0, 0);
        }
        return cont;
    }
}
#endif