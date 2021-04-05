import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"

@Module({
  namespaced: true,
  stateFactory: true,
  name: "attributetypes"
})
export default class AttributeTypesStore extends VuexModule {
  attributeTypes: any = null

  @Mutation
  _getAll(payload: any) {
    this.attributeTypes = payload
  }

  @Action({commit: "_getAll"})
  async getAll() {
    return await $ctx.$uow.attributeTypes.getAll()
  }
}
