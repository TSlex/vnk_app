using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using BLL.Base;
using BLL.Base.Exceptions;
using BLL.Contracts.Services;
using DAL.App.DTO;
using DAL.Contracts;

namespace BLL.App.Services
{
    public class AttributeService : BaseService<IAppUnitOfWork>, IAttributeService
    {
        public AttributeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<AttributeGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName,
            SortOption byType, string? searchKey)
        {
            var items = await UnitOfWork.Attributes.GetAllAsync(pageIndex, itemsOnPage,
                (DAL.App.DTO.Enums.SortOption) byName, (DAL.App.DTO.Enums.SortOption) byType, searchKey);

            return new CollectionDTO<AttributeGetDTO>()
            {
                Items = items.Select(MapAttributeToGetDTO),
                TotalCount = await UnitOfWork.Attributes.CountAsync(searchKey)
            };
        }

        public async Task<AttributeDetailsGetDTO> GetByIdAsync(long id)
        {
            if (!await UnitOfWork.Attributes.AnyAsync(id))
            {
                throw new NotFoundException("Атрибут не найдет");
            }

            var item = await UnitOfWork.Attributes.GetByIdAsync(id);

            return MapAttributeDetailsToGetDTO(item);
        }

        public async Task<long> CreateAsync(AttributePostDTO attributePostDTO)
        {
            if (!await UnitOfWork.AttributeTypes.AnyAsync(attributePostDTO.AttributeTypeId))
            {
                throw new NotFoundException("Тип атрибута не найдет");
            }

            var attribute = new Attribute()
            {
                Name = attributePostDTO.Name,
                AttributeTypeId = attributePostDTO.AttributeTypeId
            };

            var idCallback = await UnitOfWork.Attributes.AddAsync(attribute);
            await UnitOfWork.SaveChangesAsync();

            return idCallback();
        }

        public async Task UpdateAsync(long id, AttributePatchDTO attributePatchDTO)
        {
            if (id != attributePatchDTO.Id)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var attribute = await UnitOfWork.Attributes.FirstOrDefaultNoTrackAsync(id);

            if (attribute == null)
            {
                throw new NotFoundException("Aтрибут не найдет");
            }

            attribute.Name = attributePatchDTO.Name;


            if (attributePatchDTO.AttributeTypeId != null)
            {
                var orderAttributes = await UnitOfWork.OrderAttributes.GetAllByAttributeIdAsync(id);

                if (orderAttributes.Any() && attribute.AttributeTypeId != attributePatchDTO.AttributeTypeId)
                {
                    throw new ValidationException("Нельзя изменить тип используемого атрибута");
                }

                var type = await UnitOfWork.AttributeTypes.FirstOrDefaultNoTrackAsync(attributePatchDTO.AttributeTypeId
                    .Value);

                if (type == null)
                {
                    throw new NotFoundException("Тип атрибута не найдет");
                }

                attribute.AttributeTypeId = attributePatchDTO.AttributeTypeId.Value;
            }

            await UnitOfWork.Attributes.UpdateAsync(attribute);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            if (!await UnitOfWork.Attributes.AnyAsync(id))
            {
                throw new NotFoundException("Aтрибут не найдет");
            }

            var orderAttributes = await UnitOfWork.OrderAttributes.GetAllByAttributeIdAsync(id);

            if (orderAttributes.Any())
            {
                throw new ValidationException("Нельзя удалить используемый атрибут");
            }

            await UnitOfWork.Attributes.RemoveAsync(id);
            await UnitOfWork.SaveChangesAsync();
        }

        #region Helpers

        private static AttributeGetDTO MapAttributeToGetDTO(Attribute item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.AttributeType!.Name,
                TypeId = item.AttributeTypeId,
                DataType = (AttributeDataType) item.AttributeType!.DataType,
                UsesDefinedUnits = item.AttributeType!.UsesDefinedUnits,
                UsesDefinedValues = item.AttributeType!.UsesDefinedValues
            };
        }

        private static AttributeDetailsGetDTO MapAttributeDetailsToGetDTO(Attribute item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                UsedCount = item.OrderAttributes!.Count,
                Type = item.AttributeType!.Name,
                TypeId = item.AttributeTypeId,
                DataType = (AttributeDataType) item.AttributeType!.DataType,
                DefaultUnit = item.AttributeType!.TypeUnits!.FirstOrDefault()?.Value ?? "",
                DefaultValue = item.AttributeType!.TypeValues!.FirstOrDefault()?.Value ??
                               item.AttributeType.DefaultCustomValue ?? "",
            };
        }

        #endregion
    }
}