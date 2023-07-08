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
                config.CreateMap<Account, AccountResponse>();

                config.CreateMap<Account, AuthenticateResponse>();

                config.CreateMap<CreateRequest, Account>();

                config.CreateMap<RegisterRequest, Account>();

                config.CreateMap<UpdateRequest, Account>()
                    .ForAllMembers(x => x.Condition(
                        (src, dest, prop) =>
                        {
                            // ignore null & empty string properties
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                            // ignore null role
                            if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                            return true;
                        }
                    ));
            });
            return mappingConfig;
        }
            
    }
}