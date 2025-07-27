using System;
using System.Linq;
using ModKit;
using PavonisInteractive.TerraInvicta;

namespace UniverseBounty
{
    public static class CouncilorsUI
    {
        // ReSharper disable once InconsistentNaming
        private static TIFactionState? Faction;
        // ReSharper disable once InconsistentNaming
        private static GameStateID? SelectedCouncilor;

        public static void OnGUI()
        {
            if (Faction == null) return;

            using (UI.VerticalScope())
            {
                DrawAttributeHeaders();
                UI.Space(10);
                foreach (var councilor in Faction.councilors)
                {
                    DrawCouncilor(councilor);
                    UI.Space(10);
                }
            }
        }

        public static void Update()
        {
            Faction = GameControl.control.activePlayer;
        }

        private static void DrawAttributeHeaders()
        {
            using (UI.HorizontalScope())
            {
                UI.Label("이름".bold(), UI.MinWidth(249), UI.MaxWidth(249));
                UI.Space(10);
                DrawAttributeHeader("나이");
                UI.Space(10);
                DrawAttributeHeader("설득");
                UI.Space(10);
                DrawAttributeHeader("조사");
                UI.Space(10);
                DrawAttributeHeader("첩보");
                UI.Space(10);
                DrawAttributeHeader("지휘");
                UI.Space(10);
                DrawAttributeHeader("관리");
                UI.Space(10);
                DrawAttributeHeader("과학");
                UI.Space(10);
                DrawAttributeHeader("안보");
                UI.Space(10);
                DrawAttributeHeader("충성");
                UI.Space(10);
            }
        }

        private static void DrawAttributeHeader(string label)
        {
            UI.Label(label.bold(), UI.MinWidth(99), UI.MaxWidth(99));
        }

        private static void DrawCouncilor(TICouncilorState councilor)
        {
            using (UI.VerticalScope())
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(councilor.displayName.ToUpper().orange().bold(), UI.MinWidth(250), UI.MaxWidth(250));
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.None);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Persuasion);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Investigation);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Espionage);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Command);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Administration);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Science);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Security);
                    UI.Space(10);
                    DrawCouncilorAttribute(councilor, CouncilorAttribute.Loyalty);
                    UI.Space(20);
                    UI.ActionButton(
                        "+20 경험치",
                        () =>
                        {
                            councilor.ChangeXP(20);
                            councilor.SetAttributesDirty();
                        },
                        UI.MinWidth(100),
                        UI.MaxWidth(100)
                        );
                    UI.Space(10);

                    var traitsGlyph = IsSelectedCouncilor(councilor) ? UI.DisclosureGlyphOn : UI.DisclosureGlyphOff;
                    UI.ActionButton(
                        $"특성 {traitsGlyph}",
                        () => { ToggleCouncilor(councilor); },
                        UI.MinWidth(100),
                        UI.MaxWidth(100)
                        );
                }

                if (!IsSelectedCouncilor(councilor)) return;

                UI.Space(10);
                UI.Div();
                UI.Space(10);

                DrawCouncilorTraits(councilor);

                UI.Space(10);
                UI.Div();
                UI.Space(10);
            }
        }

        private static void DrawCouncilorAttribute(TICouncilorState councilor, CouncilorAttribute attribute)
        {
            var isAge = attribute == CouncilorAttribute.None;
            var value = isAge ? councilor.age : councilor.attributes[attribute];
            var min = isAge ? 18 : 1;
            var max = isAge ? 99 : 25;

            UIElements.ValueAdjuster(ref value, OnAttributeChange, min, max, UI.MinWidth(100), UI.MaxWidth(100));

            return;

            void OnAttributeChange(int v)
            {
                if (isAge)
                    councilor.dateBorn.AddYears((v - councilor.age) * -1);
                else
                    councilor.attributes[attribute] = v;

                if (attribute == CouncilorAttribute.Loyalty)
                    councilor.attributes[CouncilorAttribute.ApparentLoyalty] = v;

                councilor.SetAttributesDirty();
            }
        }

        private static void DrawCouncilorTraits(TICouncilorState councilor)
        {
            var traits = TemplateManager.GetAllTemplates<TITraitTemplate>()
                .Where(t => t.dataName.IndexOf("DUMMY", StringComparison.OrdinalIgnoreCase) < 0)
                .Sorted(t => t.friendlyName)
                .ToList()
                .Chunk(6);

            using (UI.VerticalScope())
            {
                foreach (var chunk in traits)
                {
                    using (UI.HorizontalScope())
                    {
                        foreach (var trait in chunk)
                        {
                            DrawCouncilorTrait(councilor, trait);
                            UI.Space(5);
                        }
                    }
                    UI.Space(5);
                }
            }
        }

        private static void DrawCouncilorTrait(TICouncilorState councilor, TITraitTemplate trait)
        {
            var hasTrait = councilor.traitTemplateNames.Contains(trait.dataName);
            var glyph = hasTrait ? UI.ChecklyphOn : UI.CheckGlyphOff;
            var label = hasTrait ? trait.displayName.green() : trait.displayName;
            UI.ActionButton($"{glyph} {label}", ToggleTrait, UIStyles.StandardButtonStyle, UI.MinWidth(250), UI.MaxWidth(250));
            return;

            void ToggleTrait()
            {
                if (councilor.traitTemplateNames.Contains(trait.dataName))
                    councilor.RemoveTrait(trait);
                else
                    councilor.AddTrait(trait);

                councilor.SetAttributesDirty();
            }
        }

        private static bool IsSelectedCouncilor(TICouncilorState councilor) => SelectedCouncilor == councilor.ID;

        private static void ToggleCouncilor(TICouncilorState councilor)
        {
            if (SelectedCouncilor == councilor.ID)
                SelectedCouncilor = null;
            else
                SelectedCouncilor = councilor.ID;
        }
    }
}