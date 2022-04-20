
using AutoMapper;
using TetrisAPI.Dtos;
using TetrisAPI.Models;

namespace TetrisAPI.Profiles
{
    public class TetrisProfile : Profile
    {
        public TetrisProfile()
        {
            // Source -> Target
            CreateMap<TetrisDtos,TetrisModel>()
                .ForMember(dest=>dest.NickName,act=>act.MapFrom(src=>src.NickName))
                .ForMember(dest=>dest.Score,act=>act.MapFrom(src=>src.Score));

            CreateMap<TetrisModel,TetrisReadDtos>();
        }
    }
}