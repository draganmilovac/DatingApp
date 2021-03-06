using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest=> dest.PhotoUrl, opt => {
                    opt.MapFrom((s,d)=>s.Photos?.FirstOrDefault(p=>p.IsMain) !=null ? s.Photos?.FirstOrDefault(p=>p.IsMain).Url : string.Empty );
                })
                .ForMember(dest=>dest.Age, opt=> {
                    opt.MapFrom((s,d)=>s.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
               .ForMember(dest=> dest.PhotoUrl, opt => {
                    opt.MapFrom((s,d)=>s.Photos?.FirstOrDefault(p=>p.IsMain) !=null ? s.Photos?.FirstOrDefault(p=>p.IsMain).Url : string.Empty );
                })
                .ForMember(dest=>dest.Age, opt=> {
                    opt.MapFrom((s,d)=>s.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
        }
    }
}