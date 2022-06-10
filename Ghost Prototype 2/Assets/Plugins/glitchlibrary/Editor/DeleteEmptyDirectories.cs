using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;

public class DeleteEmptyDirectories : MonoBehaviour
{

    [MenuItem("GlitchLibrary/Delete Empty Directories")]
    public static void DeleteEmptyDirectoriesDo()
    {
        System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(Application.dataPath);
        recursiveDirCrawl(downloadedMessageInfo);
    }

    static void recursiveDirCrawl(DirectoryInfo dir)
    {
        foreach (DirectoryInfo child in dir.GetDirectories())
        {
            //Debug.Log(child.ToString());
            recursiveDirCrawl(child);

            if (child.GetDirectories().Length == 0)
            {
                if (child.GetFiles().Length == 0)
                {
                    // Delete folder if no files/subdirs
                    child.Delete(true);
                    Debug.Log("DELETED " + child.ToString());
                }
                else if (child.GetFiles().Length > 0 &&
                         child.GetFiles().All(item => item.Name.Split('.').Length > 1 && item.Name.Split('.')[1] == "meta"))
                {
                    // Delete folder if only .meta file is present
                    child.Delete(true);
                    Debug.Log("DELETED " + child.ToString());
                }
            }
        }
    }
}
