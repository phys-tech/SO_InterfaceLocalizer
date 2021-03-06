﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

using InterfaceLocalizer.Classes;
using InterfaceLocalizer.Properties;

namespace InterfaceLocalizer.GUI
{
    public partial class StatisticsForm : Telerik.WinControls.UI.RadForm
    {
        AppSettings appSettings;
        IManager manager;
        List<string> fileList;
        int filesCount = 0;
        int phrasesCount = 0;
        int symbolsCount = 0;
        int engSymbols = 0;
        int nonLocalizedPhrases = 0;
        int nonLocalizedSymbols = 0;
        Dictionary<TroubleType, int> troubleDict = new Dictionary<TroubleType, int>();

        public StatisticsForm(AppSettings _appSettings, WorkMode mode)
        {
            InitializeComponent();
            appSettings = _appSettings;
            fileList = CFileList.GetProperList(mode);
            manager = ManagerFactory.CreateManager(mode, fileList.First());
            foreach(TroubleType type in Enum.GetValues(typeof(TroubleType)))
                troubleDict.Add(type, 0);
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            rbChecked.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            this.Left = appSettings.StatsFormLeft;
            this.Top = appSettings.StatsFormTop;
        }

        private void rbTotal_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            //calcStats(CFileList.AllFiles);
            calcStats();
        }

        private void rbChecked_ToggleStateChanged(object sender, Telerik.WinControls.UI.StateChangedEventArgs args)
        {
            //calcStats(CFileList.CheckedFiles);
            calcStats();
        }

        private void calcStats()    // List<string> fileList
        {
            nullData();            
            filesCount = fileList.Count;
            foreach (string file in fileList)
                manager.AddFileToManager(file);

            Dictionary<object, ITranslatable> texts = manager.GetFullDictionary();
            TroubleType trouble;
            phrasesCount = texts.Count;
            foreach (ITranslatable text in texts.Values)
            {
                symbolsCount += text.GetOriginalText().Length;
                if (text.Troublesome(out trouble))
                {
                    nonLocalizedPhrases++;
                    troubleDict[trouble]++;
                    nonLocalizedSymbols += text.GetOriginalText().Length;
                }
                //else
                engSymbols += text.GetTranslation("eng").Length;

            }
            showStats();
        }

        private void showStats()
        {
            string stats = "Files for translation: " + filesCount.ToString();
            stats += "\nPhrases totally: " + phrasesCount.ToString();
            stats += "\nSymbols totally: " + symbolsCount.ToString();
            stats += "\n\n";

            stats += "Symbols translated: " + engSymbols.ToString();
            stats += "\nPhrases left to translate: " + nonLocalizedPhrases.ToString();
            stats += "\nSymbols left to translate: " + nonLocalizedSymbols.ToString();
            foreach (TroubleType trouble in Enum.GetValues(typeof(TroubleType)))
                if (trouble != TroubleType.none)
                    stats += "\n "+ Enum.GetName(typeof(TroubleType), trouble) + ": "+troubleDict[trouble].ToString();

            lStats.Text = stats;
            lStats.Update();
        }

        private void nullData()
        {
            filesCount = 0;
            phrasesCount = 0;
            symbolsCount = 0;
            engSymbols = 0;
            nonLocalizedPhrases = 0;
            nonLocalizedSymbols = 0;         
        }

        private void StatisticsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            appSettings.StatsFormLeft = this.Left;
            appSettings.StatsFormTop = this.Top;
            appSettings.SaveSettings();
        }
    }
}
