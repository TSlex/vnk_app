import { SortOptions } from './../models/Enums/SortOptions';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { TemplateGetDTO, TemplatePatchDTO, TemplatePostDTO } from '~/models/TemplateDTO';
import { CollectionDTO } from '~/models/Common/CollectionDTO';

@Module({
  namespaced: true,
  stateFactory: true,
  name: "templates"
})
export default class TemplatesStore extends VuexModule {
  templates: TemplateGetDTO[] = []
  totalCount = 0;
  itemsOnPage = 12;
  selectedTemplate: TemplateGetDTO | null = null
  error: string | null = null

  get pagesCount() {
    return Math.ceil(this.totalCount / this.itemsOnPage)
  }

  @Mutation
  TEMPLATE_CREATED(template: TemplateGetDTO) {
    this.templates.push(template)
  }

  @Mutation
  TEMPLATE_UPDATED(template: TemplateGetDTO) {
    this.templates.forEach((element: TemplateGetDTO, index: number) => {
      if (element.id === template.id) {
        this.templates[index] = template
      }
    });
  }

  @Mutation
  TEMPLATE_DELETED(template: TemplateGetDTO) {
    this.templates.forEach((element: TemplateGetDTO, index: number) => {
      if (element.id === template.id) {
        this.templates.splice(index, 1)
      }
    });
  }

  @Mutation
  TEMPLATE_SELECTED(template: TemplateGetDTO) {
    this.selectedTemplate = template
  }

  @Mutation
  SELECTED_TEMPLATE_CLEARED() {
    this.selectedTemplate = null
  }

  @Mutation
  TEMPLATES_FETCHED(collection: CollectionDTO<TemplateGetDTO>) {
    this.templates = collection.items
    this.totalCount = collection.totalCount
  }

  @Mutation
  ACTION_FAILED(error: string) {
    this.error = error
  }

  @Mutation
  CLEAR_ERROR() {
    this.error = null
  }

  @Action
  async getTemplates(payload: { pageIndex: number, byName: SortOptions, searchKey: string | null }) {
    let response = await $ctx.$uow.templates.getAll(payload.pageIndex, this.itemsOnPage, payload.byName, payload.searchKey)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("TEMPLATES_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getTemplate(id: number) {
    let response = await $ctx.$uow.templates.getById(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("TEMPLATE_SELECTED", response.data)
      return true
    }
  }

  @Action
  async createTemplate(model: TemplatePostDTO) {
    let response = await $ctx.$uow.templates.add(model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("TEMPLATE_CREATED", model)
      this.context.dispatch("getTemplates")
      return true
    }
  }

  @Action
  async updateTemplate(payload: {
    model: TemplatePatchDTO,
    attributes: TemplateAttributeCommitDTO[],
  }) {

    let response = await $ctx.$uow.templates.update(payload.model.id, payload.model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {

      await Promise.all(payload.attributes.sort((a, b) => {
        return a.id != null ? 1 : -1
      }).map(async (attributes) => {
        if (attributes.id == null) {
          await $ctx.$uow.templateAttributes.add(payload.model.id, {attributeId:attributes.attributeId, featured:attributes.featured})
        }
        else if (attributes.deleted) {
          await $ctx.$uow.templateAttributes.delete(attributes.id)
        }
        else if (attributes.changed) {
          await $ctx.$uow.templateAttributes.update(attributes.id, { id: attributes.id, attributeId:attributes.attributeId, featured:attributes.featured })
        }
      }))

      this.context.commit("CLEAR_ERROR")
      this.context.commit("TEMPLATE_UPDATED", payload.model)
      this.context.dispatch("getTemplate", payload.model.id)
      return true
    }
  }

  @Action
  async deleteTemplate(id: number) {
    let response = await $ctx.$uow.templates.delete(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("SELECTED_TEMPLATE_CLEARED")
      this.context.commit("TEMPLATE_DELETED", id)
      return true
    }
  }
}

interface TemplateAttributeCommitDTO {
  id: number | null;
  featured: boolean;
  attributeId: number;
  changed: boolean;
  deleted: boolean;
}
