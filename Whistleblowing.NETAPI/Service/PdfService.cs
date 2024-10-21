using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Whistleblowing.NETAPI.Models.view;

namespace Whistleblowing.NETAPI.Service
{
	public class PdfService
	{

		public byte[] GenerateSegnalazioneRegularPdf(SegnalazioneRegularView segnalazioneRegular)
		{
			using (var stream = new MemoryStream())
			{
				//Creo il pdf Writer
				PdfWriter writer = new PdfWriter(stream);
				
				//pdfDocument
				PdfDocument pdf = new PdfDocument(writer);

				//documento pdf
				Document document = new Document(pdf);


				//Creo il titolo del documento
				document.Add(new Paragraph("Dettaglio Segnalazione")
					.SetTextAlignment(TextAlignment.CENTER)
					.SetFontSize(18)
					.SetBold());

				//creo uno spazio vuoto	
				document.Add(new Paragraph("\n"));


				//Aggiungo i dettagli della segnalazione
				document.Add(new Paragraph($"ID Segnalazione: {segnalazioneRegular.Id}"));
				document.Add(new Paragraph($"Fatto Riferito A: {segnalazioneRegular.FattoRiferitoA}"));
				document.Add(new Paragraph($"Data Evento: {segnalazioneRegular.DataEvento}"));
				document.Add(new Paragraph($"Luogo Evento: {segnalazioneRegular.LuogoEvento}"));
				document.Add(new Paragraph($"Soggetto Colpevole: {segnalazioneRegular.SoggettoColpevole}"));
				document.Add(new Paragraph($"Area Aziendale: {segnalazioneRegular.AreaAziendale}"));
				document.Add(new Paragraph($"Imprese Coinvolte: {segnalazioneRegular.ImpreseCoinvolte}"));
				document.Add(new Paragraph($"Pubblici Ufficiali/PA Coinvolti: {segnalazioneRegular.PubbliciUfficialiPaCoinvolti}"));
				document.Add(new Paragraph($"Modalità Conoscenza Fatto: {segnalazioneRegular.ModalitaConoscenzaFatto}"));
				document.Add(new Paragraph($"Ammontare Pagamento o Utilità: {segnalazioneRegular.AmmontarePagamentoOAltraUtilita}"));
				document.Add(new Paragraph($"Circostanze Violenza/Minaccia: {segnalazioneRegular.CircostanzeViolenzaMinaccia}"));
				document.Add(new Paragraph($"Descrizione Fatto: {segnalazioneRegular.DescrizioneFatto}"));
				document.Add(new Paragraph($"Motivazione Fatto Illecito: {segnalazioneRegular.MotivazioneFattoIllecito}"));
				document.Add(new Paragraph($"Note: {segnalazioneRegular.Note}"));

				//una volta inserito tutto chiudo il documento
				document.Close();	

				//ritorno il pdf come array di byte
				return stream.ToArray();	


			}
		}
	}
}
