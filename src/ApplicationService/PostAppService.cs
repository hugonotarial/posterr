using ApplicationService.Dto.Input;
using ApplicationService.Dto.Output;
using ApplicationService.Interface;
using AutoMapper;
using Domain.Model;
using Domain.ValueObject;
using Infra.Data.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationService
{
    public class PostAppService : IPostAppService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private const string QuotePost = "quote_post";
        private const string RepostPost = "repost";

        public PostAppService(IPostRepository postRepository, IUserRepository userRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<PostOutputDto>> AddAsync(string username, PostInputDto input)
        {
            var result = new Result<PostOutputDto>();
            var user = await _userRepository.GetUserByUserNameAsync(username).ConfigureAwait(false);

            var dailyPostsTotal = await _postRepository.GetPostsByPostDateAsync(username, DateTime.Now.ToString("yyyy-MM-dd")).ConfigureAwait(false);
            if (dailyPostsTotal >= 5)
            {
                result.AddError("limit_exceeded", "The user is not allowed to post more than 5 posts in one day.");
                return result;
            }

            if (input.PostOriginId is not null)
            {
                var postOrigin = await _postRepository.GetPostsByIdFilterAsync((int)input.PostOriginId, username).ConfigureAwait(false);
                if (postOrigin is null)
                {
                    result.AddError("origin_not_found", $"The {nameof(PostInputDto.PostOriginId)} given was not found");
                    return result;
                }

                if (input.Type == QuotePost && postOrigin.UserName == username)
                {
                    result.AddError("type_not_allowed", $"The type {QuotePost} is not allowed to the given {nameof(PostInputDto.PostOriginId)}");
                    return result;
                }

                if (input.Type == RepostPost && postOrigin.UserName != username)
                {
                    result.AddError("type_not_allowed", $"The type {RepostPost} is not allowed to the given {nameof(PostInputDto.PostOriginId)}");
                    return result;
                }
            }

            var model = _mapper.Map<PostInputDto, Post>(input);
            model.UserId = user.Id;

            var response = await _postRepository.AddAsync(model).ConfigureAwait(false);

            var insertedPost = await _postRepository.GetPostsByIdFilterAsync(response.Id, username);

            result.Content = _mapper.Map<Post, PostOutputDto>(insertedPost);

            return result;
        }

        public async Task<Result<IEnumerable<PostGetOutputDto>>> GetPostsByFilterAsync(string userName, int fetchNext, int offset, bool allPosts)
        {
            var response = await _postRepository.GetPostsByFilterAsync(userName, fetchNext, offset, allPosts).ConfigureAwait(false);
            var result = _mapper.Map<IEnumerable<Post>, IEnumerable<PostGetOutputDto>>(response);

            foreach (var post in result)
            {
                var postOriginId = post.PostOriginId;
                var postRecursive = post;

                while (postOriginId is not null)
                {
                    var postResponse = await _postRepository.GetPostsByIdFilterAsync((int)postOriginId, userName).ConfigureAwait(false);
                    postRecursive.PostOrigin = _mapper.Map<Post, PostGetOutputDto>(postResponse);
                    postOriginId = postRecursive.PostOrigin.PostOriginId;
                    postRecursive = postRecursive.PostOrigin;
                }
            }

            return new Result<IEnumerable<PostGetOutputDto>>(result);
        }
    }
}
