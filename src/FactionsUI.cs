using System.Linq;
using ModestTree;
using ModKit;
using PavonisInteractive.TerraInvicta;

namespace UniverseBounty
{
    // ReSharper disable InconsistentNaming
    public static class FactionsUI
    {
        private static TIFactionState? Faction;

        private static SelectionMode? Mode;
        private static GameStateID? SelectedFaction;

        public static void OnGUI()
        {
            if (Faction == null) return;

            using (UI.VerticalScope())
            {
                DrawHumanFactions();

                UI.Space(20);
                UI.Div();
                UI.Space(20);

                DrawAlienFaction();
            }
        }

        public static void Update()
        {
            Faction = GameControl.control.activePlayer;
        }

        private static void DrawHumanFactions()
        {
            if (Faction == null) return;
            var aiFactions = GameStateManager.AllHumanFactions().Where(f => f.ID != Faction.ID).ToList();

            foreach (var faction in aiFactions)
            {
                var isProjectsOpen = IsSelectedMode(SelectionMode.Projects, faction);
                var isOrganisationsOpen = IsSelectedMode(SelectionMode.Organisations, faction);

                var projectsGlyph = isProjectsOpen ? UI.DisclosureGlyphOn : UI.DisclosureGlyphOff;
                var organisationsGlyph = isOrganisationsOpen ? UI.DisclosureGlyphOn : UI.DisclosureGlyphOff;
                var hate = faction.GetFactionHate(Faction);

                using (UI.HorizontalScope())
                {
                    UI.Label(faction.displayName.ToUpper().orange().bold(), UI.MinWidth(300), UI.MaxWidth(300));
                    UI.Space(10);
                    UI.ActionButton($"Projects {projectsGlyph}", () => ToggleFaction(SelectionMode.Projects, faction),
                        UI.MinWidth(300), UI.MaxWidth(300));
                    UI.Space(10);
                    UI.ActionButton($"Organisations {organisationsGlyph}",
                        () => ToggleFaction(SelectionMode.Organisations, faction), UI.MinWidth(300), UI.MaxWidth(300));
                    UI.Space(10);
                    UI.ActionButton("Reveal Intel", () => RevealIntel(faction), UI.MinWidth(300), UI.MaxWidth(300));
                    UI.Space(10);
                    UI.ActionButton($"Reset Hate ({hate:N0})", () => ResetHate(faction), UI.MinWidth(300),
                        UI.MaxWidth(300));
                }

                UI.Space(10);

                if (IsSelectedMode(SelectionMode.Projects, faction))
                    DrawProjects(faction);

                if (IsSelectedMode(SelectionMode.Organisations, faction))
                    DrawOrganisations(faction);
            }
        }

        private static void DrawAlienFaction()
        {
            var faction = GameStateManager.AlienFaction();
            var hate = faction.GetFactionHate(Faction);

            using (UI.HorizontalScope())
            {
                UI.Label(faction.displayName.ToUpper().orange().bold(), UI.MinWidth(300), UI.MaxWidth(300));
                UI.Space(10);
                UI.ActionButton($"Reset Hate ({hate:N0})", () => ResetHate(faction), UI.MinWidth(300),
                    UI.MaxWidth(300));
            }
        }

        private static void DrawProjects(TIFactionState faction)
        {
            var projects = faction.StealableProjects(Faction).Chunk(4);

            using (UI.VerticalScope())
            {
                UI.Space(10);
                UI.Div();
                UI.Space(10);

                if (projects.IsEmpty())
                    UI.Label("No projects found.", UIStyles.Hint, UI.AutoWidth());
                else
                    UI.Label("Clicking a project will steal it and make it available for you to research.", UIStyles.Hint, UI.AutoWidth());

                foreach (var chunk in projects)
                {
                    using (UI.HorizontalScope())
                    {
                        foreach (var project in chunk)
                        {
                            UI.ActionButton(project.displayName, () => StealProject(project), UI.MinWidth(300),
                                UI.MaxWidth(300));
                            UI.Space(5);
                        }
                    }

                    UI.Space(5);
                }

                UI.Space(10);
                UI.Div();
                UI.Space(10);
            }
        }

        private static void DrawOrganisations(TIFactionState faction)
        {
            var organisations = faction.councilors.SelectMany(c => c.orgs).Where(o => o.AllowedOnFactionMarket(faction))
                .ToList().Chunk(4);

            using (UI.VerticalScope())
            {
                UI.Space(10);
                UI.Div();
                UI.Space(10);

                if (organisations.IsEmpty())
                    UI.Label("No organisations found.", UIStyles.Hint, UI.AutoWidth());
                else
                    UI.Label("Clicking an organisation will steal it and place it in your unassigned pool.", UIStyles.Hint, UI.AutoWidth());

                foreach (var chunk in organisations)
                {
                    using (UI.HorizontalScope())
                    {
                        foreach (var organisation in chunk)
                        {
                            UI.ActionButton(organisation.displayName, () => StealOrganisation(faction, organisation),
                                UI.MinWidth(300), UI.MaxWidth(300));
                            UI.Space(5);
                        }
                    }

                    UI.Space(5);
                }

                UI.Space(10);
                UI.Div();
                UI.Space(10);
            }
        }

        private static void ResetHate(TIFactionState faction)
        {
            if (Faction == null) return;
            faction.SetFactionHate(Faction, 0);
            Faction.SetFactionHate(faction, 0);
        }

        private static void RevealIntel(TIFactionState faction)
        {
            if (Faction == null) return;
            Faction.SetIntel(faction, TemplateManager.global.intelToSeeFactionProjects + 0.1f);
            faction.councilors.ForEach(c =>
                Faction.SetIntel(c, TemplateManager.global.intelToSeeCouncilorSecrets + 0.1f));
        }

        private static void StealProject(TIProjectTemplate project)
        {
            if (Faction == null) return;

            Faction.AddAvailableProject(project);
        }

        private static void StealOrganisation(TIFactionState faction, TIOrgState organisation)
        {
            if (Faction == null) return;
            var councilor = faction.councilors.FirstOrDefault(c => c.orgs.Any(o => o.ID == organisation.ID));
            if (councilor == null) return;

            Faction.AddOrgToFactionPool(organisation);
            councilor.RemoveOrg(organisation);
        }

        private static void ToggleFaction(SelectionMode mode, TIFactionState faction)
        {
            if (IsSelectedMode(mode, faction))
            {
                Mode = null;
                Faction = null;
            }
            else if (SelectedFaction == faction.ID)
                Mode = mode;
            else
            {
                Mode = mode;
                SelectedFaction = faction.ID;
            }
        }

        private static bool IsSelectedMode(SelectionMode mode, TIFactionState faction)
        {
            return Mode == mode && SelectedFaction == faction.ID;
        }

        private enum SelectionMode
        {
            Projects,
            Organisations
        }
    }
}