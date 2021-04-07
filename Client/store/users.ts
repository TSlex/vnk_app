import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/types/Identity/UserDTO'

@Module({
  namespaced: true,
  stateFactory: true,
  name: "users"
})
export default class UsersStore extends VuexModule {
  users: UserGetDTO[] = []


}
