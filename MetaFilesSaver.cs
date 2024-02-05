/*************************************************
	���ߣ��ʱ�
    �������ڣ�2024-02-02 20:55:59
    ����: 
        1.��ӡ����cs.meta�ļ�·��
        2.��������cs.meta�ļ�Ϊzip�ļ�
        3.��ѹ�滻����cs.meta�ļ�
*************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.IO.Compression;

public class MetaFilesSaver
{
    [MenuItem("Meta�ļ�/��ӡCSMeta�ļ�", priority = 1)]
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
    [MenuItem("Meta�ļ�/��������CSMeta�ļ�", priority = 2)]
    static void SaveMetaFiles()
    {
        CopySpecificFilesAndCreateZip(Application.dataPath, Application.dataPath + "/CSMetas", Application.dataPath + "/CSMetas.zip", "*.cs.meta");
    }
    [MenuItem("Meta�ļ�/������滻����CSMeta�ļ�", priority = 2)]
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
        // ȷ��Ŀ��Ŀ¼����
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
        Debug.Log("****�ѳɹ�����ΪCSMetas.Zip�ļ�****");

        Debug.Log(destinationDirectory);
        Directory.Delete(destinationDirectory, true);
        //Debug.Log("****��ɾ��CSMetas�ļ���****");

        AssetDatabase.Refresh();
    }

    public static void ExtractZipToCurrentDirectory(string zipFilePath)
    {
        try
        {
            // ��ȡzip�ļ����ڵ�Ŀ¼
            string destinationDirectory = Path.GetDirectoryName(zipFilePath);

            // ȷ��Ŀ���ѹĿ¼����
            if (destinationDirectory == null || !Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException("Ŀ��Ŀ¼�����ڡ�");
            }

            ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory, true);
            Debug.Log("CSMetas��ȡ��ɡ�");

            if (Directory.Exists(destinationDirectory + "/CSMetas"))
            {
                Directory.Delete(destinationDirectory + "/CSMetas", true);
                //Debug.Log("****��ɾ��CSMetas�ļ���****");
            }

            AssetDatabase.Refresh();
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"�Ҳ���ָ�����ļ��� {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogError($"ָ����·����Ч��{ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.LogError($"д��Ȩ�޲��㣬�ܾ����ʡ� {ex.Message}");
        }
        catch (InvalidDataException ex)
        {
            Debug.LogError($"���ļ�������Ч��zip�浵��{ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"����I/O����{ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"������������: {ex.Message}");
        }
    }
}
