using System;
using ModKit;
using PavonisInteractive.TerraInvicta;

namespace UniverseBounty
{
    // ReSharper disable InconsistentNaming
    public static class ResearchUI
    {   
        private static TIFactionState? Faction;
        private static float ResearchSpeedModifier = 1;

        public static void OnGUI()
        {
            if (Faction == null) return;

            using (UI.VerticalScope())
            {
                DrawUniversalResearch();

                UIElements.Header("전역 연구");

                DrawGlobalResearch();

                UIElements.Header("세력 프로젝트");

                DrawFactionProjects();
            }
        }

        public static void Update()
        {
            Faction = GameControl.control.activePlayer;
            ResearchSpeedModifier = TIGlobalValuesState.GetResearchSpeedModifier();
        }

        private static void DrawUniversalResearch()
        {
            using (UI.HorizontalScope())
            {
                UI.Label("공용".orange().bold(), UI.MinWidth(350), UI.MaxWidth(350));
                UI.Space(10);
                DrawResearchButton(1000, UniversalResearchAction);
                UI.Space(10);
                DrawResearchButton(2500, UniversalResearchAction);
                UI.Space(10);
                DrawResearchButton(5000, UniversalResearchAction);
                UI.Space(10);
                DrawResearchButton(10000, UniversalResearchAction);
            }
        }

        private static void DrawGlobalResearch()
        {
            for (var i = 0; i <= 2; i++)
            {
                using (UI.HorizontalScope())
                {
                    var slot = i;
                    var progress = GameStateManager.GlobalResearch().GetTechProgress(i);
                    UI.Label(progress.techTemplate.displayName.ToUpper().orange().bold(), UI.MinWidth(350), UI.MaxWidth(350));
                    UI.Space(10);
                    DrawResearchButton(100, v => GlobalResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(1000, v => GlobalResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(5000, v => GlobalResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(-1, v => GlobalResearchAction(progress, slot, v));
                }
                UI.Space(10);
            }
        }

        private static void DrawFactionProjects()
        {
            if (Faction == null) return;
            for (var i = 3; i <= 5; i++)
            {
                using (UI.HorizontalScope())
                {
                    var slot = i;
                    var progress = Faction.GetProjectProgressInSlot(slot);
                    if (progress == null) continue;

                    UI.Label(progress.projectTemplate.displayName.ToUpper().orange().bold(), UI.MinWidth(350), UI.MaxWidth(350));
                    UI.Space(10);
                    DrawResearchButton(100, v => FactionResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(1000, v => FactionResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(5000, v => FactionResearchAction(progress, slot, v));
                    UI.Space(10);
                    DrawResearchButton(-1, v => FactionResearchAction(progress, slot, v));
                }
                UI.Space(10);
            }
        }

        private static void DrawResearchButton(float value, Action<float> action)
        {
            var actualValue = value / ResearchSpeedModifier;
            var text = value > 0 ? $"+ {actualValue:N0}" : "완료";
            UI.ActionButton(text, () => action(actualValue), UIStyles.StandardButtonStyle, UI.MinWidth(150), UI.MaxWidth(150));
        }

        private static void UniversalResearchAction(float points)
        {
            if (Faction == null) return;

            Faction.DistributeResearchToSlots(points);
            
        }

        private static void GlobalResearchAction(TechProgress progress, int slot, float points)
        {
            float actualPoints;
            if (points < 0)
                actualPoints = progress.techTemplate.GetResearchCost(Faction) - progress.accumulatedResearch;
            else
                actualPoints = Math.Min(progress.techTemplate.GetResearchCost(Faction) - progress.accumulatedResearch, points);

            GameStateManager.GlobalResearch().AddResearchToTech(slot, actualPoints, Faction);
        }

        private static void FactionResearchAction(ProjectProgress progress, int slot, float points)
        {
            if (Faction == null) return;

            float actualPoints;
            if (points < 0)
                actualPoints = progress.projectTemplate.GetResearchCost(Faction) - progress.accumulatedResearch;
            else
                actualPoints = Math.Min(progress.projectTemplate.GetResearchCost(Faction) - progress.accumulatedResearch, points);

            Faction.AddResearchToProject(slot, actualPoints);
        }
    }
}
