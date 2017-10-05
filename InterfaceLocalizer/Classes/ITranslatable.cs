﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace InterfaceLocalizer.Classes
{
    interface ITranslatable
    {
        string GetOriginalText();
        string GetTranslation(String key);
        string GetFilename();
        string GetPathString();
        void SetOriginalText(string originalText);
        void SetTranslation(String key, string translatedText);
        bool Troublesome();
        XElement GetPath();
    }

    interface IManager
    {
        Dictionary<object, ITranslatable> GetFullDictionary();
        void ClearAllData();
        void AddFileToManager(string filename);
        void UpdateDataFromGridView(RadGridView gridView);
        void SaveDataToFile(bool original);
    }
}
