﻿using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InterfaceLocalizer.Classes
{
    class CKeyValue : ITranslatable, IFormattable
    {
        private string key;
        private Dictionary<string, string> values;
        private string filename;

        public CKeyValue(string _key, string _language, string _text, string _filename)
        {
            key = _key;
            filename = _filename;
            values = new Dictionary<string, string>();
            values.Add(_language, _text);
        }

        public string GetOriginalText()
        {
            return "im original";
        }

        public string GetTranslation(string key)
        {
            if (values.ContainsKey(key))
                return values[key];
            else
                return "<NO DATA>";
        }

        public string GetFilename()
        {
            return filename;
        }

        public string GetPathString()
        {
            return key;
        }

        public void SetOriginalText(string originalText)
        {
            throw new NotImplementedException();
        }

        public void SetTranslation(string language, string translatedText)
        {
            values[language] = translatedText;
        }

        public bool Undone()
        {
            foreach (string text in values.Values)
                if (text == "<NO DATA>" || text == "")
                    return true;

            return false;            
        }

        public XElement GetPath()
        {
            throw new NotImplementedException();
        }

        public string ToString()
        {
            string result = key + ": " + values.Count;
            return result;        
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            string result = key + ": " + values.Count;
            return result;
        }
    }

    class CKeyValueManager : IManager
    {
        private Dictionary<object, ITranslatable> dict;

        public CKeyValueManager()
        {
            dict = new Dictionary<object, ITranslatable>();
        }

        public Dictionary<object, ITranslatable> GetFullDictionary()
        {
            return dict;
        }

        public void ClearAllData()
        {
            dict.Clear();
        }

        public void AddFileToManager(string filename)
        {
            foreach (string language in CFileList.LanguageToFile.Keys)
            {
                string translationPath = CFileList.LanguageToFile[language];
                ReadKeyValueFile(language, translationPath);
            }
        }

        private void ReadKeyValueFile(string language, string filename)
        {
            StreamReader reader = new StreamReader(filename, true);
            while (!reader.EndOfStream)
            {
                string rawline = reader.ReadLine();
                if (String.IsNullOrEmpty(rawline))      // skip empty lines
                    continue;
                int indexOfEquality = rawline.IndexOf("=");
                if (indexOfEquality == -1)              // skip lines without = sign for now
                    continue;
                int length = rawline.Length;                
                string key = rawline.Substring(0, indexOfEquality);
                string value = rawline.Substring(indexOfEquality+1, length - indexOfEquality-1);
                key = RemoveQuotes(key);
                value = RemoveQuotes(value);
                if (!dict.ContainsKey(key))
                {
                    CKeyValue chunk = new CKeyValue(key, language, value, filename);
                    dict.Add(key, chunk);
                }
                else
                {
                    dict[key].SetTranslation(language, value);
                }
            }
        }

        private string RemoveQuotes(string source)
        {
            source = source.Trim();
            string result = source;
            if( source.StartsWith("\""))
                result = source.Replace('"', ' ');
            if ( source.EndsWith(";"))
                result = result.Substring(0,result.Length-2);
            result = result.Trim();
            return result;
        }

        public void UpdateDataFromGridView(Telerik.WinControls.UI.RadGridView gridView)
        {
            throw new NotImplementedException();
        }

        public void SaveDataToFile(bool original)
        {
            throw new NotImplementedException();
        }
    }


}
