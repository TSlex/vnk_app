using AutoMapper;

namespace DAL.App.Mapper
{
    public class Mapper
    {
        private readonly IMapper _mapper;

        public Mapper()
        {
            _mapper = new MapperConfiguration(config =>
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

                config.AllowNullDestinationValues = true;
            }).CreateMapper();
        }

        public virtual TOutObject Map<TInObject, TOutObject>(TInObject inObject) =>
            _mapper.Map<TInObject, TOutObject>(inObject);

        private static void CreateTwoWayMap<TFirstObject, TSecondObject>(IProfileExpression config)
        {
            config.CreateMap<TFirstObject, TSecondObject>();
            config.CreateMap<TSecondObject, TFirstObject>();
        }
    }
}