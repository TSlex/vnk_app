import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/types/Identity/UserDTO'

@Module({
  namespaced: true,
  stateFactory: true,
  name: "common"
})
export default class CommonStore extends VuexModule {
  @Action
  async checkServer() {
    let response = await $ctx.$uow.identity.serverOnline()

    return response
  }
}
