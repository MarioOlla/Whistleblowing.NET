﻿using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Whistleblowing.NETAPI.Models.view;
using System.IO;

namespace Whistleblowing.NETAPI.Service
{
    public class PdfService
    {
        public byte[] GenerateSegnalazioneRegularPdf(SegnalazioneRegularView segnalazioneRegular, string nomeUtente, string cognomeUtente)
        {
            using (var stream = new MemoryStream())
            {
                // Creo il pdf Writer
                PdfWriter writer = new PdfWriter(stream);

                // pdfDocument
                PdfDocument pdf = new PdfDocument(writer);

                // documento pdf
                Document document = new Document(pdf);

                // Creo il titolo del documento
                document.Add(new Paragraph("Dettaglio Segnalazione")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetBold());

                // Creo uno spazio vuoto
                document.Add(new Paragraph("\n"));

                // Creazione della tabella per il nome e cognome dell'utente
                Table userTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })) // Due colonne
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Colore per la prima colonna (blu chiaro)
                Color lightBlue = new DeviceRgb(173, 216, 230);

                // Funzione di utilità per creare una cella con lo stile personalizzato
                Action<string, bool> AddUserTableCell = (string text, bool isHeader) =>
                {
                    Cell cell = new Cell().Add(new Paragraph(text));
                    if (isHeader)
                    {
                        cell.SetBold();
                        cell.SetFontSize(10);
                        cell.SetBackgroundColor(lightBlue);
                    }
                    userTable.AddCell(cell);
                };

                // Popolo la tabella con il nome e cognome
                AddUserTableCell("Nome:", true);
                AddUserTableCell(nomeUtente, false);

                AddUserTableCell("Cognome:", true);
                AddUserTableCell(cognomeUtente, false);

                // Aggiungo la tabella al documento
                document.Add(userTable);

                // Creo uno spazio vuoto prima dei dettagli della segnalazione
                document.Add(new Paragraph("\n"));

                // Definisco la tabella con due colonne per i dettagli della segnalazione
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 }))
                    .SetWidth(UnitValue.CreatePercentValue(100));

                // Funzione di utilità per creare una cella con lo stile personalizzato
                Action<string, bool, bool> AddTableCell = (string text, bool isHeader, bool isLargeText) =>
                {
                    Cell cell = new Cell().Add(new Paragraph(text));
                    if (isHeader)
                    {
                        cell.SetBold();
                        cell.SetFontSize(10);
                        cell.SetBackgroundColor(lightBlue);
                    }
                    else
                    {
                        if (isLargeText)
                        {
                            cell.SetHeight(100);
                        }
                    }
                    table.AddCell(cell);
                };

                // Popolo la tabella con i dettagli della segnalazione
                AddTableCell("ID Segnalazione", true, false);
                AddTableCell(segnalazioneRegular.Id.ToString(), false, false);

                AddTableCell("Fatto Riferito A", true, false);
                AddTableCell(segnalazioneRegular.FattoRiferitoA, false, true);

                AddTableCell("Data Evento", true, false);
                AddTableCell(segnalazioneRegular.DataEvento.HasValue
                    ? segnalazioneRegular.DataEvento.Value.ToString("dd/MM/yyyy")
                    : "Non disponibile", false, false);

                AddTableCell("Luogo Evento", true, false);
                AddTableCell(segnalazioneRegular.LuogoEvento, false, false);

                AddTableCell("Soggetto Colpevole", true, false);
                AddTableCell(segnalazioneRegular.SoggettoColpevole, false, false);

                AddTableCell("Area Aziendale", true, false);
                AddTableCell(segnalazioneRegular.AreaAziendale, false, false);

                AddTableCell("Imprese Coinvolte", true, false);
                AddTableCell(segnalazioneRegular.ImpreseCoinvolte, false, false);

                AddTableCell("Pubblici Ufficiali/PA Coinvolti", true, false);
                AddTableCell(segnalazioneRegular.PubbliciUfficialiPaCoinvolti, false, false);

                AddTableCell("Modalità Conoscenza Fatto", true, false);
                AddTableCell(segnalazioneRegular.ModalitaConoscenzaFatto, false, false);

                AddTableCell("Ammontare Pagamento o Utilità", true, false);
                AddTableCell(segnalazioneRegular.AmmontarePagamentoOAltraUtilita, false, false);

                AddTableCell("Circostanze Violenza/Minaccia", true, false);
                AddTableCell(segnalazioneRegular.CircostanzeViolenzaMinaccia, false, false);

                AddTableCell("Descrizione Fatto", true, false);
                AddTableCell(segnalazioneRegular.DescrizioneFatto, false, true);

                AddTableCell("Motivazione Fatto Illecito", true, false);
                AddTableCell(segnalazioneRegular.MotivazioneFattoIllecito, false, false);

                AddTableCell("Note", true, false);
                AddTableCell(segnalazioneRegular.Note, false, true);

                // Aggiungo la tabella dei dettagli della segnalazione al documento
                document.Add(table);

                // Chiudo il documento
                document.Close();

                // Ritorno il pdf come array di byte
                return stream.ToArray();
            }
        }









        public byte[] GenerateSegnalazioneAnonimaPdf(SegnalazioneAnonimaView segnalazioneAnonima)
		{
			using (var stream = new MemoryStream())
			{
				// Creo il pdf Writer
				PdfWriter writer = new PdfWriter(stream);

				// pdfDocument
				PdfDocument pdf = new PdfDocument(writer);

				// documento pdf
				Document document = new Document(pdf);

				// Creo il titolo del documento
				document.Add(new Paragraph("Dettaglio Segnalazione")
					.SetTextAlignment(TextAlignment.CENTER)
					.SetFontSize(18)
					.SetBold());

				// creo uno spazio vuoto
				document.Add(new Paragraph("\n"));

				// Definisco la tabella con due colonne
				Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 })) // Prima colonna piccola (1 parte) e seconda più grande (4 parti)
					.SetWidth(UnitValue.CreatePercentValue(100)); // Imposta la larghezza al 100% della pagina

				// Colore per la prima colonna (blu chiaro)
				Color lightBlue = new DeviceRgb(173, 216, 230);

				// Funzione di utilità per creare una cella con lo stile personalizzato
				Action<string, bool, bool> AddTableCell = (string text, bool isHeader, bool isLargeText) =>
				{
					Cell cell = new Cell().Add(new Paragraph(text));
					if (isHeader)
					{
						cell.SetBold(); // Grassetto per l'intestazione
						cell.SetFontSize(10); // Testo più piccolo per la prima colonna
						cell.SetBackgroundColor(lightBlue); // Colore blu chiaro per la prima colonna
					}
					else
					{
						if (isLargeText)
						{
							cell.SetHeight(100); // Imposta un'altezza maggiore per le celle con molto testo
						}
					}
					table.AddCell(cell);
				};

				// Popolo la tabella con i dettagli della segnalazione
				AddTableCell("ID Segnalazione", true, false);
				AddTableCell(segnalazioneAnonima.Id.ToString(), false, false);

				AddTableCell("Fatto Riferito A", true, false);
				AddTableCell(segnalazioneAnonima.FattoRiferitoA, false, true); // Campo grande per Fatto Riferito A

				AddTableCell("Data Evento", true, false);
				AddTableCell(segnalazioneAnonima.DataEvento.HasValue
					? segnalazioneAnonima.DataEvento.Value.ToString("dd/MM/yyyy")
					: "Non disponibile", false, false);

				AddTableCell("Luogo Evento", true, false);
				AddTableCell(segnalazioneAnonima.LuogoEvento, false, false);

				AddTableCell("Soggetto Colpevole", true, false);
				AddTableCell(segnalazioneAnonima.SoggettoColpevole, false, false);

				AddTableCell("Area Aziendale", true, false);
				AddTableCell(segnalazioneAnonima.AreaAziendale, false, false);

				AddTableCell("Imprese Coinvolte", true, false);
				AddTableCell(segnalazioneAnonima.ImpreseCoinvolte, false, false);

				AddTableCell("Pubblici Ufficiali/PA Coinvolti", true, false);
				AddTableCell(segnalazioneAnonima.PubbliciUfficialiPaCoinvolti, false, false);

				AddTableCell("Modalità Conoscenza Fatto", true, false);
				AddTableCell(segnalazioneAnonima.ModalitaConoscenzaFatto, false, false);

				AddTableCell("Ammontare Pagamento o Utilità", true, false);
				AddTableCell(segnalazioneAnonima.AmmontarePagamentoOAltraUtilita, false, false);

				AddTableCell("Circostanze Violenza/Minaccia", true, false);
				AddTableCell(segnalazioneAnonima.CircostanzeViolenzaMinaccia, false, false);

				AddTableCell("Descrizione Fatto", true, false);
				AddTableCell(segnalazioneAnonima.DescrizioneFatto, false, true); // Campo grande per Descrizione Fatto

				AddTableCell("Motivazione Fatto Illecito", true, false);
				AddTableCell(segnalazioneAnonima.MotivazioneFattoIllecito, false, false);

				AddTableCell("Note", true, false);
				AddTableCell(segnalazioneAnonima.Note, false, true); // Campo grande per Note

                
                // Aggiungo la tabella al documento
                document.Add(table);

				// una volta inserito tutto chiudo il documento
				document.Close();

				// ritorno il pdf come array di byte
				return stream.ToArray();
			}
		}
	}
}
