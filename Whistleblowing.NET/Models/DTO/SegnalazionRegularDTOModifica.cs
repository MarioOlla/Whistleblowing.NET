using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whistleblowing.NET.Models.DTO
{
    public class SegnalazionRegularDTOModifica
    {
   
        public int Id { get; set; }

        public string? FattoRiferitoA { get; set; }

        public DateTime? DataEvento { get; set; }

        public string? LuogoEvento { get; set; }

        public string? SoggettoColpevole { get; set; }

        public string? AreaAziendale { get; set; }

        public string? SoggettiPrivatiCoinvolti { get; set; }

        public string? ImpreseCoinvolte { get; set; }

        public string? PubbliciUfficialiPaCoinvolti { get; set; }

        public string? ModalitaConoscenzaFatto { get; set; }

        public string? SoggettiReferentiFatto { get; set; }

        public string? AmmontarePagamentoOAltraUtilita { get; set; }

        public string? CircostanzeViolenzaMinaccia { get; set; }

        public string? DescrizioneFatto { get; set; }

        public string? MotivazioneFattoIllecito { get; set; }

        public string? Note { get; set; }

        [Required]
        [EnumDataType(typeof(Status), ErrorMessage = "Lo status deve essere APERTO, LAVORAZIONE o CHIUSO.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status? status { get; set; }
    }
}
