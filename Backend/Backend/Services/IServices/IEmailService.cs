﻿namespace Backend.Services.IServices;

public interface IEmailService
{
    Task Send(string to, string subject, string html);
}