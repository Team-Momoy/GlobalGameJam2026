using UnityEngine;

public static class WeightConversionTable
{
    #region Table
    // Handle (Mango)
    // - Liviano: 1
    // - Mediano: 2
    // - Pesado: 3

    // Guard (Guarda - Espada)
    // - Liviano: 1
    // - Mediano: 2
    // - Pesado: 3

    // Sharp (Filoso - Espada)
    // - Liviano: <=2
    // - Mediano: 3
    // - Pesado: 4

    // Piercing (Punzante - Lanza)
    // - Liviano: <=2
    // - Mediano: 3
    // - Pesado: 4

    // Cutting (Cortante - Hacha)
    // - Liviano: <2
    // - Mediano: >2 && <= 5
    // - Pesado: > 5

    // Blunt (Contundente - Martillo)
    // - Liviano: <2
    // - Mediano: >2 && <= 5
    // - Pesado: > 5

    // Defensive (Defensivo - Escudo)
    // - Liviano: <4
    // - Mediano: >=4 && <= 7
    // - Pesado: > 7
    #endregion

    public static string GetItemWeightIcon(MaterialType materialType, int weight)
    {
        switch (materialType)
        {
            case MaterialType.Handle:
                if (weight == 1) return "\U0001F609";
                if (weight == 2) return "\U0001F606";
                return "\U0001F605";
            
            case MaterialType.Guard:
                if (weight == 1) return "\U0001F609";
                if (weight == 2) return "\U0001F606";
                return "\U0001F605";
            
            case MaterialType.Sharp:
                if (weight <= 2) return "\U0001F609";
                if (weight == 3) return "\U0001F606";
                return "\U0001F605";
            
            case MaterialType.Piercing:
                if (weight <= 2) return "\U0001F609";
                if (weight == 3) return "\U0001F606";
                return "\U0001F605";

            case MaterialType.Cutting:
                if (weight < 3) return "\U0001F609";
                if (weight <= 5) return "\U0001F606";
                return "\U0001F605";

            case MaterialType.Blunt:
                if (weight < 3) return "\U0001F609";
                if (weight <= 5) return "\U0001F606";
                return "\U0001F605";
            
            case MaterialType.Defensive:
                if (weight < 4) return "\U0001F609";
                if (weight <= 7) return "\U0001F606";
                return "\U0001F605";

            default:
                return string.Empty;
        }
    }

    public static string GetWeaponWeightIcon(int weight){
        if(weight <= 3) return "\U0001F609";
        if(weight <= 7) return "\U0001F606";
        return "\U0001F605";
    }
}
