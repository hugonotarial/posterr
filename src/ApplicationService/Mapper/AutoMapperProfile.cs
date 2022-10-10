using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using AutoMapper;
using Domain.Model;

namespace ApplicationService.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Posts
            CreateMap<PostInputDto, Post>();

            CreateMap<Post, PostOutputDto>();
            
            CreateMap<Post, PostGetOutputDto>()
                .AfterMap((src, dest) =>
                {
                    var formattedDate = src.CreatedAt.ToString("MMMM dd, yyyy");
                    dest.CreatedAt = formattedDate[0].ToString().ToUpper() + formattedDate.Substring(1);

                    dest.PostId = src.Id;
                    dest.PostUserName = src.UserName;
                });

            // Followers
            CreateMap<Follower, FollowerOutputDto>();

            // Users
            CreateMap<UserInputDto, User>();
            CreateMap<User, UserInsertedOutputDto>();            
            CreateMap<User, UserOutputDto>()
                .AfterMap((src, dest) =>
                {
                    var formattedDate = src.CreatedAt.ToString("MMMM dd, yyyy");
                    dest.CreatedAt = formattedDate[0].ToString().ToUpper() + formattedDate.Substring(1);
                });
        }
    }
}
