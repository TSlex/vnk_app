import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { AttributeGetDTO, AttributePostDTO, AttributePatchDTO, AttributeDetailsGetDTO } from '~/models/AttributeDTO';
import { CollectionDTO } from '~/models/Common/CollectionDTO';
import { SortOptions } from '~/models/Enums/SortOptions';
import { config } from 'vuex-module-decorators'

config.rawError = true

@Module({
  namespaced: true,
  stateFactory: true,
  name: "attributes"
})
export default class AttributesStore extends VuexModule {
  attributes: AttributeGetDTO[] = []
  totalCount = 0;
  itemsOnPage = 12;
  selectedAttribute: AttributeDetailsGetDTO | null = null
  error: string | null = null

  get pagesCount() {
    return Math.ceil(this.totalCount / this.itemsOnPage)
  }

  @Mutation
  ATTRIBUTE_CREATED(attribute: AttributeGetDTO) {
    this.attributes.push(attribute)
  }

  @Mutation
  ATTRIBUTE_UPDATED(attribute: AttributeGetDTO) {
    this.attributes.forEach((element: AttributeGetDTO, index: number) => {
      if (element.id === attribute.id) {
        this.attributes[index] = attribute
      }
    });
  }

  @Mutation
  ATTRIBUTE_DELETED(attribute: AttributeGetDTO) {
    this.attributes.forEach((element: AttributeGetDTO, index: number) => {
      if (element.id === attribute.id) {
        this.attributes.splice(index, 1)
      }
    });
  }

  @Mutation
  ATTRIBUTE_SELECTED(attribute: AttributeDetailsGetDTO) {
    this.selectedAttribute = attribute
  }

  @Mutation
  SELECTED_ATTRIBUTE_CLEARED() {
    this.selectedAttribute = null
  }

  @Mutation
  ATTRIBUTES_FETCHED(collection: CollectionDTO<AttributeGetDTO>) {
    this.attributes = collection.items
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
  async getAttributes(payload: { pageIndex: number, byName: SortOptions, byType: SortOptions, searchKey: string | null }) {
    let response = await $ctx.$uow.attributes.getAll(payload.pageIndex, this.itemsOnPage, payload.byName, payload.byType, payload.searchKey)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTES_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getAttribute(id: number) {
    let response = await $ctx.$uow.attributes.getById(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_SELECTED", response.data)
      return true
    }
  }

  @Action
  async createAttribute(model: AttributePostDTO) {
    let response = await $ctx.$uow.attributes.add(model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_CREATED", model)
      return true
    }
  }

  @Action
  async updateAttribute(model: AttributePatchDTO) {
    let response = await $ctx.$uow.attributes.update(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_UPDATED", model)
      this.context.dispatch("getAttribute", model.id)
      return true
    }
  }

  // @Action
  // async deleteAttribute(id: number) {
  //   let response = await $ctx.$uow.attributes.deleteAttribute(id)

  //   if (response.error) {
  //     this.context.commit("ACTION_FAILED", response.error)
  //     return false
  //   } else {
  //     this.context.commit("CLEAR_ERROR")
  //     this.context.commit("SELECTED_ATTRIBUTE_CLEARED")
  //     this.context.commit("ATTRIBUTE_DELETED", id)
  //     return true
  //   }
  // }
}
