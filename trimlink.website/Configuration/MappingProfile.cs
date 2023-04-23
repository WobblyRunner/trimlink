﻿using AutoMapper;
using trimlink.website.Contracts;
using trimlink.data.Models;

namespace trimlink.website.Configuration;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LinkCreate, Link>();
        CreateMap<Link, LinkGet>();
    }
}