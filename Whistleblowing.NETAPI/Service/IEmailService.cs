﻿namespace Whistleblowing.NETAPI.Service
{
	public interface IEmailService
	{
		Task SendEmailAsync(string email, string subject, string message);
	}
}
