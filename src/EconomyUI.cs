using System;
using System.Linq;
using ModestTree;
using ModKit;
using PavonisInteractive.TerraInvicta;
using UniverseBounty.Effect;

namespace UniverseBounty
{
    // ReSharper disable InconsistentNaming
    public static class EconomyUI
    {
        private static TIFactionState? Faction;
        private const string ShipBuildTimeTemplate = "It currently takes <b>{0}/{1}/{2}</b> days to build a dreadnought via space dock/shipyard/spaceworks.";
        private const string ShipConstructionEffectName = "Effect_ShipConstructionTimeReduction10";
        private const string ControlPointBonusEffectName = "Effect_ControlPointMaintenanceBonus10";

        public static void OnGUI()
        {
            if (Faction == null) return;

            using (UI.VerticalScope())
            {
                UIElements.Header("RESOURCES");
                DrawResources();

                UIElements.Header("CONSTRUCTION");
                DrawConstruction();
            }
        }

        public static void Update()
        {
            Faction = GameControl.control.activePlayer;
        }

        private static void DrawResources()
        {
            DrawResourceButtons(
                "Money",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Money, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Money, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Money, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Money, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainMoneyIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Influence",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Influence, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Influence, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Influence, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Influence, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainInfluenceIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Operations",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Operations, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Operations, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Operations, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Operations, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainOpsIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Boost",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Boost, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Boost, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Boost, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Boost, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainBoostIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Water",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Water, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Water, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Water, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Water, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainWaterIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Metals",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Metals, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Metals, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Metals, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Metals, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainMetalsIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Noble Metals",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.NobleMetals, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.NobleMetals, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.NobleMetals, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.NobleMetals, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainNoblesIncome, 10))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Fissiles",
                new ResourceButtonAction("+ 100", AddToResourceAction(FactionResource.Fissiles, 100)),
                new ResourceButtonAction("+ 1,000", AddToResourceAction(FactionResource.Fissiles, 1000)),
                new ResourceButtonAction("+ 10,000", AddToResourceAction(FactionResource.Fissiles, 10000)),
                new ResourceButtonAction("+ 100,000", AddToResourceAction(FactionResource.Fissiles, 100000)),
                new ResourceButtonAction("+ 10/m", GainIncomeAction(InstantEffect.GainFissilesIncome, 10))
            );
            UI.Space(30);

            DrawResourceButtons(
                "Mission Control",
                new ResourceButtonAction("+ 1", GainIncomeAction(InstantEffect.GainMissionControl, 1)),
                new ResourceButtonAction("+ 5", GainIncomeAction(InstantEffect.GainMissionControl, 5)),
                new ResourceButtonAction("+ 10", GainIncomeAction(InstantEffect.GainMissionControl, 10)),
                new ResourceButtonAction("+ 20", GainIncomeAction(InstantEffect.GainMissionControl, 20))
            );
            UI.Space(10);

            DrawResourceButtons(
                "Control Points",
                new ResourceButtonAction("+ 10", ApplyEffectAction(ControlPointBonusEffectName))
            );
        }

        private static void DrawConstruction()
        {
            var effect = TemplateManager.Find<TIEffectTemplate>(ShipConstructionEffectName);
            var dreadnoughtTemplate = TemplateManager.Find<TIShipHullTemplate>("Dreadnought");

            var shipBuildingAmount = (1f - effect.value).ToString("P");
            var text = string.Format(
                ShipBuildTimeTemplate,
                dreadnoughtTemplate.constructionTime_Days(1, Faction).ToString("###"),
                dreadnoughtTemplate.constructionTime_Days(2, Faction).ToString("###"),
                dreadnoughtTemplate.constructionTime_Days(3, Faction).ToString("###")
            );

            using (UI.HorizontalScope())
            {
                UI.Label("STARSHIPS".orange().bold(), UI.MinWidth(220), UI.MaxWidth(220));
                UI.Space(10);
                UI.ActionButton($"- {shipBuildingAmount}", () => ApplyEffectAction(effect), UIStyles.StandardButtonStyle,
                    UI.MinWidth(150), UI.MaxWidth(150));
                UI.Space(10);
                UI.Label(text, UIStyles.Hint);
            }
            UI.Space(10);

            using (UI.HorizontalScope())
            {
                UI.Label("HAB MODULES".orange().bold(), UI.MinWidth(220), UI.MaxWidth(220));
                UI.Space(10);
                UI.ActionButton("COMPLETE", OnCompleteHabModulesAction, UIStyles.StandardButtonStyle, UI.MinWidth(150), UI.MaxWidth(150));
                UI.Space(10);
                UI.Label("Complete all hab modules under construction", UIStyles.Hint);
            }

            return;

            void OnCompleteHabModulesAction()
            {
                if (Faction == null) return;

                Faction.habModules
                    .Where(m => m.underConstruction)
                    .ForEach(m => m.hab.CompleteModuleConstruction(m));
            }
        }

        private static void DrawResourceButtons(string title, params ResourceButtonAction[] actions)
        {
            if (Faction == null) return;

            using (UI.HorizontalScope())
            {
                UI.Label(title.ToUpper().orange().bold(), UI.MinWidth(220), UI.MaxWidth(220));
                foreach (var action in actions)
                {
                    UI.ActionButton(action.Label, () => OnClick(action.Action), UIStyles.StandardButtonStyle, UI.MinWidth(150), UI.MaxWidth(150));
                    UI.Space(5);
                }
            }

            return;

            void OnClick(Action action)
            {
                if (Faction == null) return;
                action();
                Faction.SetResourceIncomeDataDirty();
            }
        }

        private static Action AddToResourceAction(FactionResource resource, float amount) => () =>
        {
            if (Faction != null)
                Faction.AddToCurrentResource(amount, resource);
        };

        private static Action ApplyEffectAction(string effectName)
        {
            var effect = TemplateManager.Find<TIEffectTemplate>(effectName);
            return effect == null ? () => { } : ApplyEffectAction(effect);
        }

        private static Action ApplyEffectAction(TIEffectTemplate effect) => () =>
        {
            if (Faction == null) return;
            TIEffectsState.AddEffect(effect, Faction);
            Faction.SetResourceIncomeDataDirty();
        };

        private static Action GainIncomeAction(InstantEffect effect, float amount) =>
            ApplyEffectAction(new TIEffectTemplate_GainIncome(effect, amount));

        private static string GetShipBuildingTimeDescription()
        {
            var dreadnoughtTemplate = TemplateManager.Find<TIShipHullTemplate>("Dreadnought");
            return string.Format(
                ShipBuildTimeTemplate,
                dreadnoughtTemplate.constructionTime_Days(1, Faction).ToString("###"),
                dreadnoughtTemplate.constructionTime_Days(2, Faction).ToString("###"),
                dreadnoughtTemplate.constructionTime_Days(3, Faction).ToString("###")
            );
        }

        private class ResourceButtonAction
        {
            public string Label { get; }
            public Action Action { get; }

            public ResourceButtonAction(string label, Action action)
            {
                Label = label;
                Action = action;
            }
        }
    }
}