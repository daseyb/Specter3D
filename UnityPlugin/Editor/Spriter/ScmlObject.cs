using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class ScmlObject
    {
        public string Version { get; private set; }
        public string Generator { get; private set; }
        public string GeneratorVersion { get; private set; }
        public bool PixelArtMode { get; private set; }
        public IEnumerable<Folder> Folders { get { return folders; } }
        public IEnumerable<Entity> Entities { get { return entities; } }

        public ScmlObject(XmlDocument doc)
        {
            Parse(doc["spriter_data"]);
        }

        protected virtual void Parse(XmlElement element)
        {
            Version = element.GetString("element", "UNKNOWN");
            Generator = element.GetString("generator", "UNKNOWN");
            GeneratorVersion = element.GetString("generator_version", "UNKNOWN");

            string pixelArt = element.GetString("pixel_art_mode", "false");
            PixelArtMode = pixelArt == "true";
            

        }

        private void LoadFolders(XmlElement element)
        {
            var folderElems = element.GetElementsByTagName(Folder.XmlKey);
            foreach (XmlElement folderElem in folderElems)
            {
                folders.Add(new Folder(folderElem));
            }
        }

        private void LoadEntities(XmlElement element)
        {
            var entityElems = element.GetElementsByTagName(Entity.XmlKey);
            foreach (XmlElement entityElem in entityElems)
            {
                entities.Add(new Entity(entityElem));
            }
        }

        private List<Folder> folders = new List<Folder>();
        private List<Entity> entities = new List<Entity>();
    }
}
