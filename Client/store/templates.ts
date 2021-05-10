import { SortOption } from './../models/Enums/SortOption';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { TemplateGetDTO, TemplatePatchDTO, TemplatePostDTO } from '~/models/TemplateDTO';
import { CollectionDTO } from '~/models/Common/CollectionDTO';
import { config } from 'vuex-module-decorators'

config.rawError = true

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
  async getTemplates(payload: { pageIndex: number, byName: SortOption, searchKey: string | null }) {
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
      return true
    }
  }

  @Action
  async updateTemplate(model: TemplatePatchDTO) {

    let response = await $ctx.$uow.templates.update(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {

      this.context.commit("CLEAR_ERROR")
      this.context.commit("TEMPLATE_UPDATED", model)
      this.context.dispatch("getTemplate", model.id)
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
