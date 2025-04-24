using HarmonyLib;
using ModKit;
using PavonisInteractive.TerraInvicta;
using UnityEngine;
using UnityModManagerNet;

namespace UniverseBounty
{
    // [EnableReloading]
    // ReSharper disable InconsistentNaming
    public class Main
    {
        public static Settings ModSettings = new Settings();

        private static readonly NamedAction[] Tabs =
        {
            new NamedAction("ECONOMY", EconomyUI.OnGUI),
            new NamedAction("RESEARCH", ResearchUI.OnGUI),
            new NamedAction("FACTIONS", FactionsUI.OnGUI),
            new NamedAction("COUNCILORS", CouncilorsUI.OnGUI)
        };

        public static bool Load(UnityModManager.ModEntry entry)
        {
            var harmony = new Harmony(entry.Info.Id);
            harmony.PatchAll();

            ModSettings = UnityModManager.ModSettings.Load<Settings>(entry);

            entry.OnSaveGUI = OnSaveGUI;
            entry.OnGUI = OnGUI;
            entry.OnToggle = OnToggle;

            entry.Hotkey = new KeyBinding();
            entry.Hotkey.Change(KeyCode.U, true, false, false);
            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry entry, bool value)
        {
            ModSettings.Enabled = value;
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!ModSettings.Enabled || !IsInGame) return;

            if (GameStateManager.IsValid())
            {
                CouncilorsUI.Update();
                ResearchUI.Update();
                EconomyUI.Update();
                FactionsUI.Update();
            }

            using (UI.VerticalScope())
            {
                var selected = ModSettings.SelectedTab;
                if (selected >= Tabs.Length)
                    selected = 0;

                using (UI.HorizontalScope())
                {
                    for (var index = 0; index < Tabs.Length; index++)
                    {
                        var tab = Tabs[index];
                        DrawTab(index, tab);
                    }
                }
                UI.Space(10);

                GUILayout.BeginVertical((GUIStyle)"box");
                UI.Space(10);
                Tabs[selected].action();
                GUILayout.EndVertical();
            }
        }

        public static void OnSaveGUI(UnityModManager.ModEntry entry)
        {
            Main.ModSettings.Save(entry);
        }

        public static bool IsInGame => GameControl.initialized;

        private static void DrawTab(int index, NamedAction action)
        {
            var isSelected = index == ModSettings.SelectedTab;
            var label = isSelected ? action.name.orange().bold() : action.name.bold();

            UI.ActionButton(label, () => { ModSettings.SelectedTab = index; }, UI.MinWidth(200));
        }
    }
}