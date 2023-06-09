﻿using trimlink.data.Models;

namespace trimlink.core.Records;

public record LinkDetails(int Id, string RedirectToUrl, string Token, DateTime UtcDateCreated, DateTime? UtcDateExpires)
{
    public int Id { get; } = Id;
    public string RedirectToUrl { get; } = RedirectToUrl ?? throw new ArgumentNullException(nameof(RedirectToUrl));
    public string Token { get; } = Token ?? throw new ArgumentNullException(nameof(Token));
    public DateTime UtcDateCreated { get; } = UtcDateCreated;
    public DateTime? UtcDateExpires { get; } = UtcDateExpires;

    public bool IsNeverExpires => UtcDateExpires is null;

    internal LinkDetails(Link link) : this(link.Id, link.RedirectToUrl, link.Token, link.UtcDateCreated, link.UtcDateExpires)
    {
    }
}