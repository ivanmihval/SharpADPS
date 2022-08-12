using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFSharpADPS.Helpers;
using WPFSharpADPS.Helpers.HashsumEngine;

namespace WPFSharpADPS.Models
{
    public class HashsumEngineViewObject: PropertyChangedModel
    {
        public IHashsumEngine HashsumEngine { get; set; }

        public string Name { get { return HashsumEngine.GetName(); } }

        public bool IsSelected
        {
            get { return SettingsManager.SettingsManager.Settings.HashsumEngineType == HashsumEngine.GetEngineType(); }
        }

        public static HashsumEngineViewObject FromIHashsumEngine(IHashsumEngine hashsumEngine)
        {
            return new HashsumEngineViewObject {HashsumEngine = hashsumEngine};
        }
    }
}
