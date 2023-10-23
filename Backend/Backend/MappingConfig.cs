using AutoMapper;
using Backend.Dtos.Subject;
using Backend.Dtos.RoomDtos;
using Backend.Dtos.SlotDtos;
using Backend.Dtos.UserDtos;
using Backend.Models;
using Backend.Dtos.SettingDtos;

namespace Backend
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // mapping setting
                config.CreateMap<SettingUpsertDtos, Setting>();
                // mapping slot
                config.CreateMap<CreateSlotRequest, Slot>();
                config.CreateMap<UpdateSlotRequest, Slot>();
                
                // mapping Room
                config.CreateMap<CreateRoomRequest, Room>();
                
                // mapping subject
                config.CreateMap<CreateSubjectRequest, Subject>();
                
                config.CreateMap<UpdateSubjectRequest, Subject>();

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
                config.CreateMap<UpdateSelfRequest, Account>()
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