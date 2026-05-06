using AutoMapper;
using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Entities.Models;

namespace HelpDeskTicketing.Mapping;

public class MappingProfile :Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserDTO, User>();
    }
}