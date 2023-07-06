using AutoMapper;
using Backend.Dtos.UserDtos;
using Backend.Models;

namespace Backend
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // mapping user
                config.CreateMap<User, AuthenticateResponse>();
                config.CreateMap<RegisterRequest, User>();
                config.CreateMap<UpdateRequest, User>()
                    .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                            return true;
                        }
                    ));
            });
            return mappingConfig;
        }
            
    }
}