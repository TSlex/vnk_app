using AutoMapper;
using DAL.Contracts;

namespace DAL.App.Mapper
{
    public class UniversalMapper : IUniversalMapper
    {
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configuration;

        public UniversalMapper()
        {
            _configuration = new MapperConfiguration(config =>
            {
                CreateTwoWayMap<Entities.Attribute, DTO.Attribute>(config);
                CreateTwoWayMap<Entities.AttributeType, DTO.AttributeType>(config);
                CreateTwoWayMap<Entities.AttributeTypeUnit, DTO.AttributeTypeUnit>(config);
                CreateTwoWayMap<Entities.AttributeTypeValue, DTO.AttributeTypeValue>(config);
                CreateTwoWayMap<Entities.Order, DTO.Order>(config);
                CreateTwoWayMap<Entities.OrderAttribute, DTO.OrderAttribute>(config);
                CreateTwoWayMap<Entities.Template, DTO.Template>(config);
                CreateTwoWayMap<Entities.TemplateAttribute, DTO.TemplateAttribute>(config);

                CreateTwoWayMap<Entities.Enums.AttributeDataType, DTO.Enums.AttributeDataType>(config);
                
                CreateTwoWayMap<Entities.Identity.AppUser, DTO.Identity.AppUser>(config);
                CreateTwoWayMap<Entities.Identity.AppRole, DTO.Identity.AppRole>(config);

                config.AllowNullDestinationValues = true;
            });

            _mapper = _configuration.CreateMapper();
        }

        public MapperConfiguration Configuration => _configuration;

        public virtual TOutObject Map<TInObject, TOutObject>(TInObject inObject) =>
            _mapper.Map<TInObject, TOutObject>(inObject);

        private static void CreateTwoWayMap<TFirstObject, TSecondObject>(IProfileExpression config)
        {
            config.CreateMap<TFirstObject, TSecondObject>();
            config.CreateMap<TSecondObject, TFirstObject>();
            
            //map to itself for deep copying
            config.CreateMap<TFirstObject, TFirstObject>();
            config.CreateMap<TSecondObject, TSecondObject>();
        }
    }
}