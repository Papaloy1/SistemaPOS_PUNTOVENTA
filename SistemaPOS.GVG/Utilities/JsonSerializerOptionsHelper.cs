using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SistemaPOS.GVG.Utilities
{
    /// <summary>
    /// Configuración de opciones JSON para toda la aplicación cliente
    /// </summary>
    public static class JsonSerializerOptionsHelper
    {
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                // Importante: Usar ReferenceHandler.Preserve para manejar ciclos de referencias
                ReferenceHandler = ReferenceHandler.Preserve,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                MaxDepth = 64
            };

            return options;
        }
    }
}
