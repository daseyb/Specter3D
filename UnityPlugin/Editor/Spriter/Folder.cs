using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Folder : KeyElem
    {
        public const string XmlKey = "folder";

        public string Name { get; private set; }
        public IEnumerable<File> Files { get; private set; }

        public Folder(XmlElement element)
            : base(element)
        {
        }

        protected override void Parse(XmlElement element)
        {
            base.Parse(element);

            Name = element.GetString("name", "");

            //Parse file elements
            var fileElements = element.GetElementsByTagName("file");
            var files = new List<File>(fileElements.Count);
            foreach(XmlElement fileElement in fileElements)
            {
                files.Add(new File(fileElement, this));
            }
            Files = files;
        }

		public File GetFile(int id)
		{
			return Files.Where (file => file.Id == id).FirstOrDefault();
		}
    }
}
