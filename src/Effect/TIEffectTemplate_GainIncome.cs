namespace UniverseBounty.Effect
{
    // ReSharper disable InconsistentNaming
    public class TIEffectTemplate_GainIncome: TIEffectTemplate
    {
        public TIEffectTemplate_GainIncome(InstantEffect effect, float amount)
        {
            dataName = $"Effect_Mod{effect}{amount}";
            instantEffect = effect;
            value = amount;
            effectTarget = EffectTargetType.SourceFaction;
            effectDuration = EffectDuration.instant;
            duration_months = -1f;
        }
    }
}
