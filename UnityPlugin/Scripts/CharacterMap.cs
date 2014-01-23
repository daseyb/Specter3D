using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterMap : MonoBehaviour
{
    [SerializeField]
    private List<FolderMap> Folders = new List<FolderMap>();

    public Sprite GetSprite(int folderId, int fileId)
    {
        if (folderId > Folders.Count)
        {
            throw new IndexOutOfRangeException(string.Format("Folder Id {0} fell outside of bounds {1}", folderId, Folders.Count));
        }
        var folder = Folders[folderId];
        return folder[fileId];
    }

    public void SetSprite(int folderId, int fileId, Sprite sprite)
    {
        Debug.Log(string.Format("Setting Folder:{0}  File:{1}   To Sprite '{2}'", folderId, fileId, sprite));
        int newFolders = folderId - Folders.Count + 1;
        if (newFolders > 0) Debug.Log("Adding " + newFolders.ToString() + " folders");
        for (int i = 0; i < newFolders; i++)
        {
            Folders.Add(new FolderMap());
        }

        Folders[folderId][fileId] = sprite;
    }
}

public class FolderMap
{
    [SerializeField]
    public int FolderId { get; set; }

    [SerializeField]
    private List<Sprite> Sprites = new List<Sprite>();

    public Sprite this[int fileId]
    {
        get
        {
            if (fileId > Sprites.Count)
                throw new IndexOutOfRangeException(string.Format("File Id {0} fell outside of bounds {1}", fileId, Sprites.Count));
            return Sprites[fileId];
        }
        set
        {
            int newSprites = fileId - Sprites.Count + 1;
            for (int i = 0; i < newSprites; i++)
            {
                Sprites.Add(null);
            }
            Sprites[fileId] = value;
        }
    }
}