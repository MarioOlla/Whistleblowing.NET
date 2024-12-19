using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models.DTO
{
    [Table("SegnalazioneRegulars")]

    public class SegnalazioneRegularDTOInserimento
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

        public bool? IsDeleted { get; set; }

    }
}
