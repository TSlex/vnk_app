import { CollectionDTO } from '~/models/Common/CollectionDTO';
import { AttributeTypeDetailsGetDTO, AttributeTypeGetDTO, AttributeTypePatchDTO, AttributeTypePostDTO } from '~/models/AttributeTypeDTO';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { config } from 'vuex-module-decorators'

config.rawError = true

@Module({
  namespaced: true,
  stateFactory: true,
  name: "attributetypes"
})
export default class AttributeTypesStore extends VuexModule {
  attributeTypes: AttributeTypeGetDTO[] = []
  totalCount = 0;
  itemsOnPage = 12;
  selectedAttributeType: AttributeTypeDetailsGetDTO | null = null
  error: string | null = null

  get pagesCount() {
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
  ATTRIBUTE_TYPE_SELECTED(attributeType: AttributeTypeDetailsGetDTO) {
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
  async getAttributeTypes(payload: { pageIndex: number, orderReversed: boolean, searchKey: string | null }) {
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
      return true
    }
  }

  @Action
  async updateAttributeType(payload: {
    model: AttributeTypePatchDTO,
    values: AttributeTypeValueCommitDTO[],
    units: AttributeTypeUnitCommitDTO[]
  }) {


    let response = await $ctx.$uow.attributeTypes.update(payload.model.id, payload.model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {

      await Promise.all(payload.values.sort((a, b) => {
        if (a.id == null && b.id == null) {
          return a.value > b.value ? 1 : -1
        }
        return a.id != null ? 1 : -1
      }).map(async (value) => {
        if (value.id == null) {
          await $ctx.$uow.attributeTypeValues.add({ value: value.value, attributeTypeId: payload.model.id })
        }
        else if (value.deleted) {
          await $ctx.$uow.attributeTypeValues.delete(value.id)
        }
        else if (value.changed) {
          await $ctx.$uow.attributeTypeValues.update(value.id, { id: value.id, value: value.value })
        }
      }))

      await Promise.all(payload.units.sort((a, b) => {
        if (a.id == null && b.id == null) {
          return a.value > b.value ? 1 : -1
        }
        return a.id != null ? 1 : -1
      }).map(async (unit) => {
        if (unit.id == null) {
          await $ctx.$uow.attributeTypeUnits.add({ value: unit.value, attributeTypeId: payload.model.id })
        }
        else if (unit.deleted) {
          await $ctx.$uow.attributeTypeUnits.delete(unit.id)
        }
        else if (unit.changed) {
          await $ctx.$uow.attributeTypeUnits.update(unit.id, { id: unit.id, value: unit.value })
        }
      }))

      this.context.commit("CLEAR_ERROR")
      this.context.commit("ATTRIBUTE_TYPE_UPDATED", payload.model)
      this.context.dispatch("getAttributeType", payload.model.id)
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

interface AttributeTypeValueCommitDTO {
  id: number | null;
  value: string;
  changed: boolean;
  deleted: boolean;
}

interface AttributeTypeUnitCommitDTO {
  id: number | null;
  value: string;
  changed: boolean;
  deleted: boolean;
}
