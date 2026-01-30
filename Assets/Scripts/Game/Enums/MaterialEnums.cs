using Sirenix.OdinInspector;

public enum MaterialRarity
{
    Comun,
    Raro,
    Epico,
    Legendario
}

public enum MaterialType
{
    [LabelText("Cortante", SdfIconType.Gem)] Cutting,
    [LabelText("Punzante", SdfIconType.Eyedropper)]Piercing,
    [LabelText("Contundente", SdfIconType.Hammer)]Blunt,
    [LabelText("Mango")]Handle,
    [LabelText("Guarda")]Guard,
    [LabelText("Filoso")]Sharp,
    [LabelText("Defensivo", SdfIconType.Shield)]Defensive
}