using System;
using ModKit;
using UnityEngine;

namespace UniverseBounty
{
    // ReSharper disable InconsistentNaming
    public static class UIElements
    {
        private static readonly GUIStyle HeaderStyle = new GUIStyle(UIStyles.AlignCenter)
        {
            fontSize = 16.point()
        };

        public static void ValueAdjuster(ref int value, Action<int> onChange, int min, int max,
            params GUILayoutOption[] options)
        {
            var v = value;
            using (UI.HorizontalScope(options))
            {
                if (v > min)
                    UI.ActionButton(" < ".bold(), () => OnValueChange(-1), UIStyles.SimpleButtonStyle);
                else
                    UI.ActionButton(" < ".grey(), () => { }, UIStyles.SimpleButtonStyle);

                UI.Label(v.ToString().orange().bold(), UIStyles.SimpleButtonStyle, UI.MinWidth(30), UI.MaxWidth(30));

                if (v < max)
                    UI.ActionButton(" > ".bold(), () => OnValueChange(1), UIStyles.SimpleButtonStyle);
                else
                    UI.ActionButton(" > ".grey(), () => { }, UIStyles.SimpleButtonStyle);
            }

            return;

            void OnValueChange(int delta)
            {
                onChange(v + delta);
            }
        }

        public static void Header(string title)
        {
            UI.Space(20);
            UI.Div();
            UI.Space(10);
            UI.Label(title.ToUpper().orange().bold(), HeaderStyle, UI.MinWidth(UI.ummWidth));
            UI.Space(10);
            UI.Div();
            UI.Space(20);
        }
    }
}