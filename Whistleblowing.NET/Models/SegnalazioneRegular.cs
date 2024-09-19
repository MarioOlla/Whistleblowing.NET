using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whistleblowing.NET.Models
{

    public class SegnalazioneRegular
    {
        public enum Status
        {
            APERTO, LAVORAZIONE, CHIUSO
        }

       
        public int Id { get; set; }

        public string? FattoRiferitoA { get; set; }

        [Column("data_evento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime? DataEvento { get; set; }

        public string? LuogoEvento { get; set; }

        public string? SoggettoColpevole { get; set; }

        public string? AreaAziendale { get; set; }

        public string? SoggettiPrivatiCoinvolti { get; set; }

        public string? ImpreseCoinvolte { get; set;  }

        public string? PubbliciUfficialiPaCoinvolti { get; set; }

        public string? ModalitaConoscenzaFatto { get; set; }

        public string? SoggettiReferentiFatto { get; set; }

        public string?  AmmontarePagamentoOAltraUtilita { get; set; }

        public string? CircostanzeViolenzaMinaccia { get; set; }

        public string? DescrizioneFatto { get; set; }

        public string? MotivazioneFattoIllecito { get; set; }

        public string? Note { get; set; }

        public Status? status { get; set; }

        public Boolean? IsExternal { get; set; }

        public int UserId { get; set; }




    }
}
