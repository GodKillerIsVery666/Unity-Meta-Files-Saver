/*************************************************
	作者：彩冰
    创建日期：2024-02-02 20:55:59
    功能: 
        1.打印所有cs.meta文件路径
        2.保存所有cs.meta文件为zip文件
        3.解压替换所有cs.meta文件
*************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.IO.Compression;

public class MetaFilesSaver
{
    [MenuItem("Meta文件/打印CSMeta文件", priority = 1)]
    static void DebugMetaFiles()
    {
        Director(Application.dataPath, (str) =>
         {
             if (str.Contains(".cs.meta"))
             {
                 Debug.Log(str);
             }
         });
    }
    [MenuItem("Meta文件/保存所有CSMeta文件", priority = 2)]
    static void SaveMetaFiles()
    {
        CopySpecificFilesAndCreateZip(Application.dataPath, Application.dataPath + "/CSMetas", Application.dataPath + "/CSMetas.zip", "*.cs.meta");
    }
    [MenuItem("Meta文件/提出并替换所有CSMeta文件", priority = 2)]
    static void ExtractMetaFiles()
    {
        ExtractZipToCurrentDirectory(Application.dataPath + "/CSMetas.zip");
    }

    public static void Director(string dir, Action<string> action)
    {

        if (Directory.Exists(dir))
        {
            DirectoryInfo d = new DirectoryInfo(dir);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    Director(fsinfo.FullName, action);
                }
                else
                {
                    action?.Invoke(fsinfo.FullName);
                }
            }
        }
        else if (File.Exists(dir))
        {
            action?.Invoke(dir);
        }
    }

    public static void CopySpecificFilesAndCreateZip(string sourceDirectory, string destinationDirectory, string zipFilePath, string fileSearchPattern)
    {
        // 确保目标目录存在
        Directory.CreateDirectory(destinationDirectory);

        foreach (string dirPath in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourceDirectory, destinationDirectory));
        }

        foreach (string filePath in Directory.GetFiles(sourceDirectory, fileSearchPattern, SearchOption.AllDirectories))
        {
            string destFilePath = filePath.Replace(sourceDirectory, destinationDirectory);
            string destDirPath = Path.GetDirectoryName(destFilePath);

            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            File.Copy(filePath, destFilePath, true);
        }

        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        ZipFile.CreateFromDirectory(destinationDirectory, zipFilePath);
        Debug.Log("****已成功保存为CSMetas.Zip文件****");

        Debug.Log(destinationDirectory);
        Directory.Delete(destinationDirectory, true);
        //Debug.Log("****已删除CSMetas文件夹****");

        AssetDatabase.Refresh();
    }

    public static void ExtractZipToCurrentDirectory(string zipFilePath)
    {
        try
        {
            // 获取zip文件所在的目录
            string destinationDirectory = Path.GetDirectoryName(zipFilePath);

            // 确保目标解压目录存在
            if (destinationDirectory == null || !Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException("目标目录不存在。");
            }

            ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory, true);
            Debug.Log("CSMetas提取完成。");

            if (Directory.Exists(destinationDirectory + "/CSMetas"))
            {
                Directory.Delete(destinationDirectory + "/CSMetas", true);
                //Debug.Log("****已删除CSMetas文件夹****");
            }

            AssetDatabase.Refresh();
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"找不到指定的文件。 {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogError($"指定的路径无效。{ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"写入权限不足，拒绝访问。 {ex.Message}");
        }
        catch (InvalidDataException ex)
        {
            Debug.LogError($"该文件不是有效的zip存档。{ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"发生I/O错误。{ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"发生其他错误: {ex.Message}");
        }
    }
}
