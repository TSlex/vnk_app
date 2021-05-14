using System.Collections.Generic;
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
    public class AttributeTypeService : BaseService<IAppUnitOfWork>, IAttributeTypeService
    {
        public AttributeTypeService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<AttributeTypeGetDTO>> GetAllAsync(int pageIndex, int itemsOnPage,
            SortOption byName, string? searchKey)
        {
            var items = await UnitOfWork.AttributeTypes.GetAllAsync(pageIndex, itemsOnPage,
                (DAL.App.DTO.Enums.SortOption) byName, searchKey);

            return new CollectionDTO<AttributeTypeGetDTO>()
            {
                Items = items.Select(MapAttributeTypeToDTO),
                TotalCount = await UnitOfWork.AttributeTypes.CountAsync(searchKey)
            };
        }

        public async Task<AttributeTypeDetailsGetDTO> GetByIdAsync(long id, int valuesCount, int unitsCount)
        {
            if (!await UnitOfWork.AttributeTypes.AnyAsync(id))
            {
                throw new NotFoundException("Тип не найдет");
            }

            var item = await UnitOfWork.AttributeTypes.GetDetailsByIdAsync(id, valuesCount, unitsCount);

            return MapAttributeTypeDetailsToDTO(item);
        }

        public async Task<long> CreateAsync(AttributeTypePostDTO attributeTypePostDTO)
        {
            if (attributeTypePostDTO.DefaultCustomValue == null &&
                (!attributeTypePostDTO.UsesDefinedValues || !attributeTypePostDTO.Values.Any()))
            {
                throw new ValidationException("У атрибута должно быть значение по умолчанию");
            }

            if (attributeTypePostDTO.UsesDefinedValues && !attributeTypePostDTO.Values.Any())
            {
                throw new ValidationException("У атрибута должно быть значение по умолчанию");
            }

            if (attributeTypePostDTO.UsesDefinedUnits && !attributeTypePostDTO.Units.Any())
            {
                throw new ValidationException("У атрибута должна быть единица измерения по умолчанию");
            }

            if (attributeTypePostDTO.Units.Any() && (0 > attributeTypePostDTO.DefaultUnitIndex ||
                                                     attributeTypePostDTO.DefaultUnitIndex >=
                                                     attributeTypePostDTO.Units.Count))
            {
                throw new ValidationException("Индекс не соотвествует массиву единиц измерений");
            }

            if (attributeTypePostDTO.Values.Any() && (0 > attributeTypePostDTO.DefaultValueIndex ||
                                                      attributeTypePostDTO.DefaultValueIndex >=
                                                      attributeTypePostDTO.Values.Count))
            {
                throw new ValidationException("Индекс не соотвествует массиву значений");
            }

            var attributeType = new AttributeType()
            {
                Name = attributeTypePostDTO.Name,
                DataType = (DAL.App.DTO.Enums.AttributeDataType) attributeTypePostDTO.DataType,
                DefaultCustomValue = attributeTypePostDTO.DefaultCustomValue,
                UsesDefinedUnits = attributeTypePostDTO.UsesDefinedUnits,
                UsesDefinedValues = attributeTypePostDTO.UsesDefinedValues,
                TypeUnits = attributeTypePostDTO.Units.Select(s =>
                    new AttributeTypeUnit {Value = s}).ToList(),
                TypeValues = attributeTypePostDTO.Values.Select(s =>
                    new AttributeTypeValue {Value = s}).ToList(),
            };

            var idCallBack = await UnitOfWork.AttributeTypes.AddAsync(attributeType);

            await UnitOfWork.SaveChangesAsync();

            attributeType.Id = idCallBack();
            
            var hasValues = attributeTypePostDTO.UsesDefinedValues && attributeTypePostDTO.Values.Any();
            var hasUnits = attributeTypePostDTO.UsesDefinedUnits && attributeTypePostDTO.Units.Any();

            AttributeType? typeWithValuesAndUnits = null;
            
            if (hasValues || hasUnits)
            {
                typeWithValuesAndUnits =
                    await UnitOfWork.AttributeTypes.GetWithValuesAndUnits(attributeType.Id);
            }

            if (hasValues && typeWithValuesAndUnits != null)
            {
                attributeType.DefaultValueId =
                    typeWithValuesAndUnits.TypeValues!.ToArray()[attributeTypePostDTO.DefaultValueIndex].Id;
            }

            if (hasUnits && typeWithValuesAndUnits != null)
            {
                attributeType.DefaultUnitId =
                    typeWithValuesAndUnits.TypeUnits!.ToArray()[attributeTypePostDTO.DefaultUnitIndex].Id;
            }

            if (hasValues || hasUnits)
            {
                attributeType.TypeUnits = new List<AttributeTypeUnit>();
                attributeType.TypeValues = new List<AttributeTypeValue>();
                
                await UnitOfWork.AttributeTypes.UpdateAsync(attributeType);
                await UnitOfWork.SaveChangesAsync();
            }

            return idCallBack();
        }

        public async Task UpdateAsync(long id, AttributeTypePatchDTO attributeTypePatchDTO)
        {
            var attributeType = await ValidateAndReturnAttributeTypeAsync(id, attributeTypePatchDTO);

            attributeType.Name = attributeTypePatchDTO.Name;
            attributeType.DefaultCustomValue = attributeTypePatchDTO.DefaultCustomValue;
            attributeType.DefaultUnitId = attributeTypePatchDTO.DefaultUnitId;
            attributeType.DefaultValueId = attributeTypePatchDTO.DefaultValueId;

            attributeType.DataType = (DAL.App.DTO.Enums.AttributeDataType) attributeTypePatchDTO.DataType;

            foreach (var valuePatchDTO in attributeTypePatchDTO.Values.OrderBy(dto => dto.PatchOption))
            {
                await HandleValuePatch(valuePatchDTO, attributeType);
            }
            
            foreach (var unitPatchDTO in attributeTypePatchDTO.Units.OrderBy(dto => dto.PatchOption))
            {
                await HandleUnitPatch(unitPatchDTO, attributeType);
            }

            await UnitOfWork.AttributeTypes.UpdateAsync(attributeType);
            await UnitOfWork.SaveChangesAsync();
        }

        private async Task HandleValuePatch(AttributeTypeValuePatchDTO valuePatchDTO, AttributeType attributeType)
        {
            AttributeTypeValue value;

            switch (valuePatchDTO.PatchOption)
            {
                case PatchOption.Updated:
                if (!(valuePatchDTO.Id.HasValue &&
                      await UnitOfWork.AttributeTypeValues.AnyAsync(valuePatchDTO.Id.Value, attributeType.Id)))
                {
                    throw new NotFoundException("Значение не найдено");
                }
                
                value = await UnitOfWork.AttributeTypeValues.FirstOrDefaultNoTrackAsync(valuePatchDTO.Id.Value);
                
                value.Value = valuePatchDTO.Value;
                
                await UnitOfWork.AttributeTypeValues.UpdateAsync(value);
                break;

                case PatchOption.Created:
                    value = new AttributeTypeValue
                    {
                        Value = valuePatchDTO.Value,
                        AttributeTypeId = attributeType.Id,
                    };

                    await UnitOfWork.AttributeTypeValues.AddAsync(value);
                    break;

                case PatchOption.Deleted:
                    if (!(valuePatchDTO.Id.HasValue &&
                          await UnitOfWork.AttributeTypeValues.AnyAsync(valuePatchDTO.Id.Value, attributeType.Id)))
                    {
                        throw new NotFoundException("Значение не найдено");
                    }
                
                    value = await UnitOfWork.AttributeTypeValues.FirstOrDefaultAsync(valuePatchDTO.Id.Value);
                
                    if (attributeType.DefaultValueId == valuePatchDTO.Id)
                    {
                        var newDefaultValue =
                            await UnitOfWork.AttributeTypeValues.NextOrDefaultAsync(attributeType.Id,
                                valuePatchDTO.Id.Value);
                
                        if (newDefaultValue == null)
                        {
                            throw new ValidationException("У атрибута должно быть значение по умолчанию");
                        }
                
                        attributeType.DefaultUnitId = newDefaultValue.Id;
                    }
                
                    var attributes = await UnitOfWork.OrderAttributes.GetAllByValueId(valuePatchDTO.Id.Value);
                
                    foreach (var attribute in attributes)
                    {
                        attribute.ValueId = attributeType.DefaultValueId;
                        await UnitOfWork.OrderAttributes.UpdateAsync(attribute);
                    }
                
                    await UnitOfWork.AttributeTypeValues.RemoveAsync(value);
                    break;
            }
        }

        private async Task HandleUnitPatch(AttributeTypeUnitPatchDTO unitPatchDTO, AttributeType attributeType)
        {
            AttributeTypeUnit unit;

            switch (unitPatchDTO.PatchOption)
            {
                case PatchOption.Updated:
                    if (!(unitPatchDTO.Id.HasValue &&
                          await UnitOfWork.AttributeTypeUnits.AnyAsync(unitPatchDTO.Id.Value, attributeType.Id)))
                    {
                        throw new NotFoundException("Единица измерения не найдена");
                    }

                    unit = await UnitOfWork.AttributeTypeUnits.FirstOrDefaultNoTrackAsync(unitPatchDTO.Id.Value);

                    unit.Value = unitPatchDTO.Value;

                    await UnitOfWork.AttributeTypeUnits.UpdateAsync(unit);
                    break;

                case PatchOption.Created:
                    unit = new AttributeTypeUnit
                    {
                        Value = unitPatchDTO.Value,
                        AttributeTypeId = attributeType.Id,
                    };

                    await UnitOfWork.AttributeTypeUnits.AddAsync(unit);
                    break;

                case PatchOption.Deleted:
                    if (!(unitPatchDTO.Id.HasValue &&
                          await UnitOfWork.AttributeTypeUnits.AnyAsync(unitPatchDTO.Id.Value, attributeType.Id)))
                    {
                        throw new NotFoundException("Единица измерения не найдена");
                    }

                    unit = await UnitOfWork.AttributeTypeUnits.FirstOrDefaultAsync(unitPatchDTO.Id.Value);

                    if (attributeType.DefaultUnitId == unitPatchDTO.Id)
                    {
                        var newDefaultUnit =
                            await UnitOfWork.AttributeTypeUnits.NextOrDefaultAsync(attributeType.Id,
                                unitPatchDTO.Id.Value);

                        if (newDefaultUnit == null)
                        {
                            throw new ValidationException("У типа должна быть единица измерения по умолчанию");
                        }

                        attributeType.DefaultUnitId = newDefaultUnit.Id;
                    }

                    var attributes = await UnitOfWork.OrderAttributes.GetAllByUnitId(unitPatchDTO.Id.Value);

                    foreach (var attribute in attributes)
                    {
                        attribute.UnitId = attributeType.DefaultUnitId;
                        await UnitOfWork.OrderAttributes.UpdateAsync(attribute);
                    }

                    await UnitOfWork.AttributeTypeUnits.RemoveAsync(unit);
                    break;
            }
        }

        public async Task DeleteAsync(long id)
        {
            var attributeType = await UnitOfWork.AttributeTypes.FirstOrDefaultNoTrackAsync(id);

            if (attributeType == null)
            {
                throw new NotFoundException("Тип не найден");
            }

            if (attributeType.SystemicType)
            {
                throw new ValidationException("Нельзя удалить системный тип");
            }

            if (await UnitOfWork.Attributes.AnyByTypeIdAsync(attributeType.Id))
            {
                throw new ValidationException("Нельзя удалить используемый тип");
            }

            await UnitOfWork.AttributeTypes.RemoveAsync(attributeType);
            await UnitOfWork.SaveChangesAsync();
        }

        #region Helpers

        private static AttributeTypeGetDTO MapAttributeTypeToDTO(AttributeType at)
        {
            return new()
            {
                Id = at.Id,
                Name = at.Name,
                DataType = (AttributeDataType) at.DataType,
                SystemicType = at.SystemicType,
                UsedCount = at.Attributes!.Count,
                UsesDefinedUnits = at.UsesDefinedUnits,
                UsesDefinedValues = at.UsesDefinedValues
            };
        }

        private static AttributeTypeDetailsGetDTO MapAttributeTypeDetailsToDTO(AttributeType at)
        {
            return new()
            {
                Id = at.Id,
                Name = at.Name,
                Units = at.TypeUnits!
                    .Select(u => new AttributeTypeUnitDetailsDTO
                    {
                        Id = u.Id,
                        Value = u.Value
                    })
                    .ToList(),
                Values = at.TypeValues!
                    .Select(v => new AttributeTypeValueDetailsDTO
                    {
                        Id = v.Id,
                        Value = v.Value
                    })
                    .ToList(),
                DataType = (AttributeDataType) at.DataType,
                SystemicType = at.SystemicType,
                UnitsCount = at.TypeUnits!.Count,
                UsedCount = at.Attributes!.Count,
                ValuesCount = at.TypeValues!.Count,
                DefaultCustomValue = at.DefaultCustomValue ?? "???",
                DefaultUnitId = at.DefaultUnitId,
                DefaultValueId = at.DefaultValueId,
                UsesDefinedUnits = at.UsesDefinedUnits,
                UsesDefinedValues = at.UsesDefinedValues
            };
        }

        private async Task<AttributeType> ValidateAndReturnAttributeTypeAsync(long id,
            AttributeTypePatchDTO attributeTypePatchDTO)
        {
            if (id != attributeTypePatchDTO.Id)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var attributeType = await UnitOfWork.AttributeTypes.FirstOrDefaultNoTrackAsync(id);

            if (attributeType == null)
            {
                throw new NotFoundException("Тип атрибута не найдет");
            }

            if (attributeType.SystemicType)
            {
                throw new NotFoundException("Нельзя менять системный атрибут");
            }

            if (attributeTypePatchDTO.DefaultCustomValue == null && !attributeType.UsesDefinedValues)
            {
                throw new ValidationException("У атрибута должно быть значение по умолчанию");
            }

            if (attributeType.UsesDefinedValues &&
                await UnitOfWork.AttributeTypeValues.FirstOrDefaultNoTrackAsync(attributeTypePatchDTO.DefaultValueId) ==
                null)
            {
                throw new ValidationException("Неверный идентификатор значения по умолчанию");
            }

            if (attributeType.UsesDefinedUnits &&
                await UnitOfWork.AttributeTypeUnits.FirstOrDefaultNoTrackAsync(attributeTypePatchDTO.DefaultUnitId) ==
                null)
            {
                throw new ValidationException("Неверный идентификатор единицы измерения по умолчанию");
            }

            return attributeType;
        }

        #endregion
    }
}