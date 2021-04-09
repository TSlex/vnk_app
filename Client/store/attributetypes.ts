import { CollectionDTO } from './../types/Common/CollectionDTO';
import { AttributeTypeGetDetailsDTO, AttributeTypeGetDTO, AttributeTypePatchDTO, AttributeTypePostDTO } from '~/types/AttributeTypeDTO';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"

@Module({
  namespaced: true,
  stateFactory: true,
  name: "attributetypes"
})
export default class AttributeTypesStore extends VuexModule {
  attributeTypes: AttributeTypeGetDTO[] = []
  totalCount = 0;
  itemsOnPage = 12;
  selectedAttributeType: AttributeTypeGetDetailsDTO | null = null
  error: string | null = null

  get pagesCount(){
    return Math.ceil(this.totalCount / this.itemsOnPage)
  }

  @Mutation
  ATTRIBUTE_TYPE_CREATED(attributeType: AttributeTypeGetDTO) {
    this.attributeTypes.push(attributeType)
  }

  @Mutation
  ATTRIBUTE_TYPE_UPDATED(attributeType: AttributeTypeGetDTO) {
    this.attributeTypes.forEach((element: AttributeTypeGetDTO, index: number) => {
      if (element.id === attributeType.id) {
        this.attributeTypes[index] = attributeType
      }
    });
  }

  @Mutation
  ATTRIBUTE_TYPE_DELETED(attributeType: AttributeTypeGetDTO) {
    this.attributeTypes.forEach((element: AttributeTypeGetDTO, index: number) => {
      if (element.id === attributeType.id) {
        this.attributeTypes.splice(index, 1)
      }
    });
  }

  @Mutation
  ATTRIBUTE_TYPE_SELECTED(attributeType: AttributeTypeGetDetailsDTO) {
    this.selectedAttributeType = attributeType
  }

  @Mutation
  SELECTED_ATTRIBUTE_TYPE_CLEARED() {
    this.selectedAttributeType = null
  }

  @Mutation
  ATTRIBUTE_TYPES_FETCHED(collection: CollectionDTO<AttributeTypeGetDTO>) {
    this.attributeTypes = collection.items
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
  async getAttributeTypes(payload : {pageIndex: number, orderReversed: boolean, searchKey: string | null}) {
    let response = await $ctx.$uow.attributeTypes.getAll(payload.pageIndex, this.itemsOnPage, payload.orderReversed, payload.searchKey)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_TYPES_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getAttributeType(id: number) {
    let response = await $ctx.$uow.attributeTypes.getById(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_TYPE_SELECTED", response.data)
      return true
    }
  }

  @Action
  async createAttributeType(model: AttributeTypePostDTO) {
    let response = await $ctx.$uow.attributeTypes.add(model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_TYPE_CREATED", model)
      this.context.dispatch("getAttributeTypes")
      return true
    }
  }

  @Action
  async updateAttributeType(model: AttributeTypePatchDTO) {
    let response = await $ctx.$uow.attributeTypes.update(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_TYPE_UPDATED", model)
      this.context.dispatch("getAttributeType", model.id)
      return true
    }
  }

  // @Action
  // async deleteAttributeType(id: number) {
  //   let response = await $ctx.$uow.attributeTypes.deleteAttributeType(id)

  //   if (response.error) {
  //     this.context.commit("ACTION_FAILED", response.error)
  //     return false
  //   } else {
  //     this.context.commit("CLEAR_ERROR")
  //     this.context.commit("SELECTED_ATTRIBUTE_TYPE_CLEARED")
  //     this.context.commit("ATTRIBUTE_TYPE_DELETED", id)
  //     return true
  //   }
  // }
}
