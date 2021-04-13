import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/models/Identity/UserDTO'

@Module({
  namespaced: true,
  stateFactory: true,
  name: "templates"
})
export default class TemplatesStore extends VuexModule {
}
