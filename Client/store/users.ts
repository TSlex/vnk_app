import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"

@Module({
  namespaced: true,
  stateFactory: true,
  name: "users"
})
export default class UsersStore extends VuexModule {
}
