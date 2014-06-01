using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FolderMap
{
    public int FolderId;
    public string FolderName;

    [SerializeField]
    private List<FileMap> files = new List<FileMap>();

    public FileMap this[int fileId]
    {
        get
        {
            if (fileId > files.Count)
                throw new IndexOutOfRangeException(string.Format("File Id {0} fell outside of bounds {1}", fileId, files.Count));
            return files[fileId];
        }
        set
        {
            int newSprites = fileId - files.Count + 1;
            for (int i = 0; i < newSprites; i++)
            {
                files.Add(null);
            }
            files[fileId] = value;
        }
    }
}
