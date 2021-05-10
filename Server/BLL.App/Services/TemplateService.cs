using System;
using System.Linq;
using System.Threading.Tasks;
using AppAPI._1._0;
using AppAPI._1._0.Common;
using AppAPI._1._0.Enums;
using BLL.App;
using BLL.App.Exceptions;
using DAL.App.DTO;
using DAL.Contracts;

namespace BLL.Contracts.Services
{
    public class TemplateService : BaseService<IAppUnitOfWork>, ITemplateService
    {
        public TemplateService(IAppUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<CollectionDTO<TemplateGetDTO>?> GetAllAsync(int pageIndex, int itemsOnPage, SortOption byName, string? searchKey)
        {
            var items = await UnitOfWork.Templates.GetAllAsync(pageIndex, itemsOnPage,
                (DAL.App.DTO.Enums.SortOption) byName, searchKey);

            return new CollectionDTO<TemplateGetDTO>()
            {
                Items = items.Select(MapTemplateToGetDTO),
                TotalCount = await UnitOfWork.Templates.CountAsync(searchKey)
            };
        }

        public async Task<TemplateGetDTO?> GetByIdAsync(long id)
        {
            if (!await UnitOfWork.Templates.AnyAsync(id))
            {
                throw new NotFoundException("Шаблон не найдет");
            }
            
            var item = await UnitOfWork.Templates.GetByIdAsync(id);

            return MapTemplateToGetDTO(item);
        }

        public async Task<long> CreateAsync(TemplatePostDTO templatePostDTO)
        {
            if (templatePostDTO.Attributes.Count == 0)
            {
                throw new ValidationException("В шаблоне должен быть как минимум один атрибут");
            }

            foreach (var templateAttributePostDTO in templatePostDTO.Attributes)
            {
                await ValidateAndFormatAttribute(templateAttributePostDTO);
            }

            var template = new Template()
            {
                Name = templatePostDTO.Name,
                TemplateAttributes = templatePostDTO.Attributes!.Select(oa => new TemplateAttribute
                {
                    Featured = oa.Featured,
                    AttributeId = oa.AttributeId,
                }).ToList()
            };

            var idCallback = await UnitOfWork.Templates.AddAsync(template);

            await UnitOfWork.SaveChangesAsync();

            return idCallback();
        }

        public async Task UpdateAsync(long id, TemplatePatchDTO templatePatchDTO)
        {
            var template = await ValidateAndReturnTemplateAsync(id, templatePatchDTO.Id);

            template.Name = templatePatchDTO.Name;

            await UnitOfWork.Templates.UpdateAsync(template);

            var templateAttributesCount = await UnitOfWork.TemplateAttributes.CountByTemplateId(id);

            foreach (var attributePatchDTO in templatePatchDTO.Attributes.OrderBy(dto => dto.PatchOption))
            {
                TemplateAttribute attribute;

                switch (attributePatchDTO.PatchOption)
                {
                    case PatchOption.Updated:
                        if (!(attributePatchDTO.Id.HasValue &&
                              await UnitOfWork.TemplateAttributes.AnyAsync(attributePatchDTO.Id.Value, id)))
                        {
                            throw new NotFoundException("Атрибут не найден");
                        }

                        await ValidateAndFormatAttribute(attributePatchDTO);

                        attribute =
                            await UnitOfWork.TemplateAttributes.FirstOrDefaultNoTrackAsync(attributePatchDTO.Id.Value)!;

                        attribute.Featured = attributePatchDTO.Featured;
                        attribute.AttributeId = attributePatchDTO.AttributeId;

                        await UnitOfWork.TemplateAttributes.UpdateAsync(attribute);
                        break;

                    case PatchOption.Created:

                        await ValidateAndFormatAttribute(attributePatchDTO);

                        attribute = new TemplateAttribute()
                        {
                            Featured = attributePatchDTO.Featured,
                            AttributeId = attributePatchDTO.AttributeId,
                            TemplateId = template.Id
                        };

                        await UnitOfWork.TemplateAttributes.AddAsync(attribute);

                        templateAttributesCount++;

                        break;

                    case PatchOption.Deleted:
                        if (!(attributePatchDTO.Id.HasValue &&
                              await UnitOfWork.TemplateAttributes.AnyAsync(attributePatchDTO.Id.Value)))
                        {
                            throw new NotFoundException("Атрибут не найден");
                        }

                        await UnitOfWork.TemplateAttributes.RemoveAsync(attributePatchDTO.Id.Value);

                        templateAttributesCount--;

                        break;
                }
            }

            if (templateAttributesCount < 1)
            {
                throw new ValidationException("В шаблоне должен быть как минимум один атрибут");
            }

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var template = await ValidateAndReturnTemplateAsync(id);

            var templateAttributes = await UnitOfWork.TemplateAttributes.GetAllByTemplateId(id);

            await UnitOfWork.TemplateAttributes.RemoveRangeAsync(templateAttributes);
            await UnitOfWork.Templates.RemoveAsync(template);

            await UnitOfWork.SaveChangesAsync();
        }

        #region Helpers

        private static TemplateGetDTO MapTemplateToGetDTO(Template item)
        {
            return new()
            {
                Id = item.Id,
                Name = item.Name,
                Attributes = item.TemplateAttributes!.Select(MapAttributesToDTO).ToList()
            };
        }
        
        private static TemplateAttributeGetDTO MapAttributesToDTO(TemplateAttribute oa)
        {
            return new()
            {
                Id = oa.Id,
                Featured = oa.Featured,
                Name = oa.Attribute!.Name,
                Type = oa.Attribute!.AttributeType!.Name,
                TypeId = oa.Attribute!.AttributeType.Id,
                AttributeId = oa.AttributeId,
                DataType = (AttributeDataType) oa.Attribute!.AttributeType!.DataType,
                UsesDefinedUnits = oa.Attribute!.AttributeType!.UsesDefinedUnits,
                UsesDefinedValues = oa.Attribute!.AttributeType!.UsesDefinedValues
            };
        }
        
        private async Task ValidateAndFormatAttribute(TemplateAttributePatchDTO templateAttributePatchDTO)
        {
            await ValidateAndFormatAttribute(new TemplateAttributePostDTO
            {
                Featured = templateAttributePatchDTO.Featured,
                AttributeId = templateAttributePatchDTO.AttributeId
            });
        }

        private async Task ValidateAndFormatAttribute(TemplateAttributePostDTO templateAttributePostDTO)
        {
            var attribute = await UnitOfWork.Attributes.AnyAsync(templateAttributePostDTO.AttributeId);

            if (!attribute)
            {
                throw new ValidationException("Один из создаваемых атрибутов неверен");
            }
        }
        
        private async Task<Template> ValidateAndReturnTemplateAsync(long id)
        {
            var template = await UnitOfWork.Templates.FirstOrDefaultAsync(id);

            if (template == null)
            {
                throw new NotFoundException("Шаблон не найден");
            }

            return template;
        }

        private async Task<Template> ValidateAndReturnTemplateAsync(long id, long templateDTOId)
        {
            if (id != templateDTOId)
            {
                throw new ValidationException("Идентификаторы должны совпадать");
            }

            var template = await UnitOfWork.Templates.FirstOrDefaultAsync(id);

            if (template == null)
            {
                throw new NotFoundException("Шаблон не найден");
            }

            return template;
        }

        #endregion
    }
}