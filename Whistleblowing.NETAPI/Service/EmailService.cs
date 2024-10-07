
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Whistleblowing.NETAPI.Models;

namespace Whistleblowing.NETAPI.Service
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;
		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException(nameof(email), "L'indirizzo email non può essere nullo o vuoto.");
			}

			try
			{
				// Verifica se l'email è valida
				var mailAddress = new MailAddress(email);

				var mailMessage = new MailMessage
				{
					From = new MailAddress(_emailSettings.UserName),
					Subject = subject,
					Body = message,
					IsBodyHtml = true
				};

				// Aggiungi l'email validata alla lista dei destinatari
				mailMessage.To.Add(email);

				using var smtpClient = new SmtpClient
				{
					Host = _emailSettings.Host,
					Port = _emailSettings.Port,
					EnableSsl = true,
					Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
				};

				await smtpClient.SendMailAsync(mailMessage);
			}
			catch (FormatException ex)
			{
				// Gestisci eccezione nel caso di formato email non valido
				throw new ArgumentException("Indirizzo email non valido.", nameof(email), ex);
			}
			catch (Exception ex)
			{
				// Gestione degli errori nell'invio dell'email
				throw new Exception("Si è verificato un errore durante l'invio dell'email.", ex);
			}
		}

	}
}
