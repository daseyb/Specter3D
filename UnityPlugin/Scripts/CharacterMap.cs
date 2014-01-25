using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterMap : MonoBehaviour
{
    [SerializeField]
    public List<FolderMap> Folders = new List<FolderMap>();

    public Sprite GetSprite(int folderId, int fileId)
    {
        if (folderId > Folders.Count)
        {
            throw new IndexOutOfRangeException(string.Format("Folder Id {0} fell outside of bounds {1}", folderId, Folders.Count));
        }
        var folder = Folders[folderId];
        var file = folder[fileId];
        return file.Sprite;
    }

    public void SetFile(int folderId, int fileId, FileMap fileMap)
    {
        //Debug.Log(string.Format("Setting Folder:{0} File:{1} to FileMap:'{2}'", folderId, fileId, fileMap));
        int newFolders = folderId - Folders.Count + 1;
        if (newFolders > 0) Debug.Log("Adding " + newFolders.ToString() + " folders");
        for (int i = 0; i < newFolders; i++)
        {
            Folders.Add(new FolderMap());
        }

        Folders[folderId][fileId] = fileMap;
    }

    public void ChangeSprite(string packedData)
    {
		//Debug.Log ("Called ChangeSprite(" + packedData + ")");
        var unpacked = packedData.Split(';');
        if (unpacked.Length != 3)
            throw new Exception("Invalid parameter supplied to ChangeSprite --   " + packedData);

        string relativePath = unpacked[0];
        int folderId, fileId;
        if (!int.TryParse(unpacked[1], out folderId))
            throw new Exception("Invalid suppled folderID --   " + unpacked[1]);
        if (!int.TryParse(unpacked[2], out fileId))
            throw new Exception("Invalid suppled fileId --   " + unpacked[2]);

        var target = transform.Find(relativePath);
        if (target == null)
            Debug.Log("Unable to find relative child --   " + relativePath);
        else
        {
            var spriteRenderer = target.GetComponent<SpriteRenderer>();
			var sprite = GetSprite(folderId, fileId);
			if(sprite == null) Debug.Log ("Sprite Not Found!");
			spriteRenderer.sprite = sprite;
        }
    }
}

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

[Serializable]
public class FileMap
{
    public string FilePath;
    public Sprite Sprite;
}