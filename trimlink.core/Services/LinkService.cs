﻿using System.Diagnostics.CodeAnalysis;
using trimlink.data;
using trimlink.data.Models;
using Microsoft.EntityFrameworkCore;
using trimlink.core.Interfaces;
using trimlink.core.Records;

namespace trimlink.core.Services;

public class LinkService : ILinkService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILinkValidator _linkValidator;

    public LinkService(
        IUnitOfWork unitOfWork,
        [AllowNull] IDateTimeProvider? dateTimeProvider = null,
        [AllowNull] ITokenGenerator? tokenGenerator = null,
        [AllowNull] ILinkValidator? linkValidator = null
    )
    {
        _unitOfWork = unitOfWork;
        
        _dateTimeProvider = dateTimeProvider ?? new SystemDateTimeProvider();
        _tokenGenerator = tokenGenerator ?? new TokenGenerator();
        _linkValidator = linkValidator ?? new LinkValidator();
    }

    private Link CreateLink(string toUrl, TimeSpan? expiresAfter = null)
    {
        string token = _tokenGenerator.GenerateToken();
        DateTime now = DateTime.UtcNow;
        Link link = new()
        {
            RedirectToUrl = toUrl,
            Token = token,
            IsMarkedForDeletion = false,
            UtcDateCreated = now,
            UtcDateExpires = now + expiresAfter // This evaluates to 'null' if either side of expression is null! Nifty.
        };
        return link;
    }

    private void HandleLinkValidation(string toUrl)
    {
        ArgumentException.ThrowIfNullOrEmpty(toUrl);
        LinkValidationResult result = _linkValidator.Validate(toUrl);
        if (result == LinkValidationResult.InvalidScheme)
        {
            throw new ArgumentException($"The given URL ({toUrl}) has an invalid scheme.");
        }
        else if (result == LinkValidationResult.InvalidRelative)
        {
            throw new ArgumentException($"The given URL ({toUrl}) cannot be a relative path.");
        }
    }

    public async Task<string> GenerateShortLink(string toUrl, TimeSpan? expiresAfter = null)
    {
        HandleLinkValidation(toUrl);

        Link link = CreateLink(toUrl, expiresAfter);
        await _unitOfWork.Links.AddAsync(link);
        await _unitOfWork.Save();

        return link.Token;
    }

    public async Task<string?> GetLongUrlById(int id)
    {
        Link? found = await _unitOfWork.Links.GetAsync(id);
        return found?.RedirectToUrl;
    }

    public async Task<string?> GetLongUrlByToken(string token)
    {
        Link? found = await _unitOfWork.Links.FindAsync(link => link.Token == token);
        return found?.RedirectToUrl;
    }

    public async Task<LinkDetails?> GetLinkDetailsById(int id)
    {
        Link? found = await _unitOfWork.Links.GetAsync(id);
        return found is null ?
            null :
            new LinkDetails(found);
    }

    public async Task<LinkDetails?> GetLinkDetailsByToken(string token)
    {
        Link? found = await _unitOfWork.Links.FindAsync(link => link.Token == token);
        return found is null ?
            null :
            new LinkDetails(found);
    }
}