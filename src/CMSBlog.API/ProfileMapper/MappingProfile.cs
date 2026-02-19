using AutoMapper;
using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Models.Content;

namespace CMSBlog.API.ProfileMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostPagingDto, Post>();
            CreateMap<Post, PostPagingDto>().ReverseMap();
            CreateMap<PostDto, Post>();
            CreateMap<CreateUpdatePostRequest, Post>();
        }
    }
}
