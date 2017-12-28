﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace InterfaceLocalizer.Classes
{
    class MultiDataForGroups : CMultiData
    {
        public MultiDataForGroups(string _key, string language, string text, string _filename, XmlPath _path) 
            : base(_key, language, text, _filename, _path)
        {
        }

        public override bool Troublesome(out TroubleType trouble)
        {
            // think about new type of detection
            trouble = TroubleType.none;

            return false;
        }

    }


    class MultiDataManagerForGroups : CMultiManager
    {
        public MultiDataManagerForGroups()
        { 
        }

        public override void AddFileToManager(string filename)
        {
            //string language = CFileList.LanguageToFile.Where(u => u.Value == filename).First().Key;
            string groupAndLang = CFileList.FileToGroupAndLanguage[filename];
            parseXmlFile(groupAndLang, filename);
        }

        public override void UpdateDataFromGridView(RadGridView gridView)
        {
            for (int row = 0; row < gridView.RowCount; row++)
            {
                string id = gridView.Rows[row].Cells["columnID"].Value.ToString();
                string filename = gridView.Rows[row].Cells["columnFileName"].Value.ToString();
                string tags = gridView.Rows[row].Cells["columnTags"].Value.ToString();

                if (!xmlDict.ContainsKey(id))
                {
                    XmlPath tempPath = new XmlPath();
                    tempPath.Push(new PathAtom("string", id));
                    tempPath.Push(new PathAtom("resources", ""));
                    xmlDict.Add(id, new CMultiData(id, "", "", filename, tempPath));
                }

                for (int i = 0; i < CFileList.GetNumberOfFiles(); i++)
                {
                    string columnName = "columnTranslation" + i.ToString();
                    string language = CFileList.LanguageToFile.Keys.ElementAt(i);
                    string translation = gridView.Rows[row].Cells[columnName].Value.ToString();
                    xmlDict[id].SetTranslation(language, translation);
                }
            }
        }


        public override void SaveDataToFile(bool original)
        {
            foreach (string language in CFileList.LanguageToFile.Keys)
            {
                string path = CFileList.LanguageToFile[language];
                if (Extension.Get(path) != Extensions.xml)
                    continue;
                XDocument doc = new XDocument();

                doc = XDocument.Load(path);
                IEnumerable<XElement> del = doc.Root.Descendants().ToList();
                del.Remove();
                doc.Save(path);

                foreach (CMultiData text in xmlDict.Values)
                {
                    string value = text.GetTranslation(language);
                    XElement localPath = text.GetPath();
                    XElement noRoot = localPath.Descendants().First();
                    XElement child = noRoot;

                    while (child.HasElements)
                    {
                        XName name = child.Descendants().First().Name;
                        child = child.Element(name);
                    }

                    child.SetValue(value);
                    doc.Root.Add(noRoot);
                }

                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                settings.NewLineOnAttributes = false;
                using (System.Xml.XmlWriter w = System.Xml.XmlWriter.Create(path, settings))
                {
                    doc.Save(w);
                }
                
            }
        }

    }
}