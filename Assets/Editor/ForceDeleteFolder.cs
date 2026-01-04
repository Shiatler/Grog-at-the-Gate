using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ForceDeleteFolder : EditorWindow
{
    [MenuItem("Tools/Force Delete SidekickCharacters Folder")]
    public static void DeleteSidekickCharactersFolder()
    {
        string folderPath = "Assets/Synty/SidekickCharacters";
        
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            // Try alternative paths
            folderPath = "Assets/Imports/SidekickCharacters";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogWarning($"SidekickCharacters folder not found at expected paths. Please specify the correct path.");
                return;
            }
        }

        if (EditorUtility.DisplayDialog("Force Delete Folder",
            $"Are you sure you want to delete '{folderPath}'?\n\n" +
            "This will:\n" +
            "1. Refresh the asset database\n" +
            "2. Attempt to delete the folder\n\n" +
            "If it still fails, close Unity and delete manually from Windows Explorer.",
            "Delete", "Cancel"))
        {
            try
            {
                // Refresh asset database to release any locks
                AssetDatabase.Refresh();
                
                // Wait a moment for locks to release
                System.Threading.Thread.Sleep(500);
                
                // Try to delete through AssetDatabase first
                if (AssetDatabase.DeleteAsset(folderPath))
                {
                    Debug.Log($"Successfully deleted: {folderPath}");
                    AssetDatabase.Refresh();
                }
                else
                {
                    // If AssetDatabase fails, try direct file system deletion
                    string fullPath = Path.Combine(Application.dataPath, folderPath.Replace("Assets/", ""));
                    
                    if (Directory.Exists(fullPath))
                    {
                        Debug.LogWarning($"AssetDatabase deletion failed. Attempting direct file system deletion...");
                        Debug.LogWarning($"If this fails, please:\n" +
                            "1. Close Unity completely\n" +
                            "2. Delete the folder manually from: " + fullPath + "\n" +
                            "3. Reopen Unity");
                        
                        // Try to delete directory
                        try
                        {
                            Directory.Delete(fullPath, true);
                            File.Delete(fullPath + ".meta");
                            Debug.Log($"Successfully deleted folder directly: {fullPath}");
                            AssetDatabase.Refresh();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to delete folder: {e.Message}\n\n" +
                                "SOLUTION: Close Unity completely, then delete the folder manually from Windows Explorer:\n" +
                                fullPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting folder: {e.Message}\n\n" +
                    "SOLUTION: Close Unity completely, then delete the folder manually from Windows Explorer.");
            }
        }
    }
}

