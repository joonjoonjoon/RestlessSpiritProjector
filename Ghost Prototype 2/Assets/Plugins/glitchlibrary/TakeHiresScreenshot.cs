//using UnityEngine;
//using System.Collections;
//#if UNITY_EDITOR
//using System.IO;
//#endif
//
//public class TakeHiresScreenshot : MonoBehaviour {
//    public int resWidth = 2550;
//    public int resHeight = 3300;
//
//    private bool takeHiResShot = false;
//
//    public static string ScreenShotName(int width, int height)
//    {
//        return string.Format("{0}/screen_{1}x{2}_{3}.png",
//                             Application.dataPath,
//                             width, height,
//                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
//    }
//
//    public void TakeHiResShot()
//    {
//        takeHiResShot = true;
//    }
//
//    void LateUpdate()
//    {
//#if UNITY_EDITOR
//        takeHiResShot |= Input.GetKeyDown("k");
//        if (takeHiResShot)
//        {
//            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
//            GetComponent<Camera>().targetTexture = rt;
//            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
//            GetComponent<Camera>().Render();
//            RenderTexture.active = rt;
//            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
//            GetComponent<Camera>().targetTexture = null;
//            RenderTexture.active = null;
//            Destroy(rt);
//            byte[] bytes = screenShot.EncodeToPNG();
//            string filename = ScreenShotName(resWidth, resHeight);
//
//            File.WriteAllBytes(filename, bytes);
//            Debug.Log(string.Format("Took screenshot to: {0}", filename));
//            Application.OpenURL(filename);
//            takeHiResShot = false;
//        }
//#endif
//    }
//}
