using Consilium.Maui.Resources.Themes;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Consilium.Maui;

public static class ThemeManager {
    public static void ApplyTheme(string themeName) {
        ResourceDictionary themeDictionary = themeName switch {
            "Green" => new GreenTheme(),
            "Blue" => new BlueTheme(),
            "Purple" => new PurpleTheme(),
            "Pink" => new PinkTheme(),
            "Teal" => new TealTheme(),
            _ => new GreenTheme()
        };

        if (Application.Current?.Resources.MergedDictionaries is IList<ResourceDictionary> dicts) {
            // Remove previous theme dictionaries
            for (int i = dicts.Count - 1; i >= 0; i--) {
                if (dicts[i].GetType().Name.EndsWith("Theme"))
                    dicts.RemoveAt(i);
            }

            dicts.Add(themeDictionary);
        }
    }
}