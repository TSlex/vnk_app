import { UserRolePatchDTO, UserPasswordPatchDTO, UserPatchDTO, UserPostDTO } from './../types/Identity/UserDTO';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/models/Identity/UserDTO'
import { identityStore } from '~/store';

@Module({
  namespaced: true,
  stateFactory: true,
  name: "users"
})
export default class UsersStore extends VuexModule {
  users: UserGetDTO[] = []
  selectedUser: UserGetDTO | null = null
  error: string | null = null

  @Mutation
  USER_CREATED(user: UserPostDTO) {
    let newUser = user as any as UserGetDTO
    newUser.roleLocalized = "Созданный"

    this.users.push(newUser)
  }

  @Mutation
  USER_UPDATED(user: UserGetDTO) {
    this.users.forEach((element: UserGetDTO, index: number) => {
      if (element.id === user.id) {
        this.users[index] = user
      }
    });
  }

  @Mutation
  USER_ROLE_UPDATED(userRole: UserRolePatchDTO) {
    this.users.forEach((element: UserGetDTO, index: number) => {
      if (element.id === userRole.id) {
        this.users[index].role = userRole.role
      }
    });
  }

  @Mutation
  USER_DELETED(user: UserGetDTO) {
    this.users.forEach((element: UserGetDTO, index: number) => {
      if (element.id === user.id) {
        this.users.splice(index, 1)
      }
    });
  }

  @Mutation
  USER_SELECTED(user: UserGetDTO) {
    this.selectedUser = user
  }

  @Mutation
  SELECTED_USER_CLEARED() {
    this.selectedUser = null
  }

  @Mutation
  USERS_FETCHED(users: UserGetDTO[]) {
    this.users = users
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
  async getUsers() {
    let response = await $ctx.$uow.identity.getAllUsers()

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("USERS_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getUser(id: number) {
    let response = await $ctx.$uow.identity.getUserById(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("USER_SELECTED", response.data)
      return true
    }
  }

  @Action
  async createUser(model: UserPostDTO) {
    let response = await $ctx.$uow.identity.addUser(model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("USER_CREATED", model)
      this.context.dispatch("getUsers")
      return true
    }
  }

  @Action
  async updateUser(model: UserPatchDTO) {
    let response = await $ctx.$uow.identity.updateUser(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("USER_UPDATED", model)
      this.context.dispatch("getUser", model.id)
      await identityStore.fetchData()
      return true
    }
  }

  @Action
  async updateUserPassword(model: UserPasswordPatchDTO) {
    let response = await $ctx.$uow.identity.updateUserPassword(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      return true
    }
  }

  @Action({ rawError: true })
  async updateUserRole(model: UserRolePatchDTO) {
    let response = await $ctx.$uow.identity.updateUserRole(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("USER_ROLE_UPDATED", model)
      this.context.dispatch("getUser", model.id)
      return true
    }
  }

  @Action
  async deleteUser(id: number) {
    let response = await $ctx.$uow.identity.deleteUser(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("SELECTED_USER_CLEARED")
      this.context.commit("USER_DELETED", id)
      return true
    }
  }
}
