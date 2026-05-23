using AutoMapper;
using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Models.Content;
using CMSBlog.Core.Models.System;

namespace CMSBlog.API.ProfileMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostPagingDto, Post>();
            CreateMap<Post, PostPagingDto>().ReverseMap();
            CreateMap<PostDto, Post>();
            CreateMap<Post, PostDto>();
            CreateMap<CreateUpdatePostRequest, Post>();


            // Roles
            CreateMap<AppRole, RoleDto>();

            // Users
            CreateMap<AppUser, UserDto>();
        }
    }
}
