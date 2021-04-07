import JwtDecode from 'jwt-decode';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { LoginDTO } from '~/types/Identity/LoginDTO';
import { UserGetDTO } from '~/types/Identity/UserDTO';

@Module({
  namespaced: true,
  stateFactory: true,
  name: "identity"
})
export default class IdentityStore extends VuexModule {
  jwt: string | null = null
  loginError: string | null = null
  userData: UserGetDTO | null = null

  get fullName(){
    return `${this.userData?.firstName} ${this.userData?.lastName}`
  }

  get isAuthenticated() {
    if (this.jwt) {
      const decode = JwtDecode(this.jwt!) as Record<string, string>;
      const jwtExpires = parseInt(decode.exp)

      if (Date.now() >= jwtExpires * 1000) {
        return false
      }

      return true
    }

    return false;
  }

  get isAdministrator(){
    return this.userData?.role === "Administrator"
  }

  get isRoot(){
    return this.userData?.role === "Root"
  }

  @Mutation
  LOGIN_SUCCEEDED(jwt: string) {
    localStorage.setItem('jwt', jwt)

    this.jwt = jwt
    this.loginError = null
    $ctx.$axios.setToken(this.jwt!, "Bearer")
  }

  @Mutation
  LOGIN_FAILED(error: string) {
    this.loginError = error;
  }

  @Mutation
  LOGOUT() {}

  @Mutation
  JWT_RESTORED() {
    this.jwt = localStorage.getItem('jwt')
    $ctx.$axios.setToken(this.jwt!, "Bearer")
  }

  @Mutation
  JWT_EXPIRED() {}

  @Mutation
  CURRENT_USER_FETCHED(data: UserGetDTO){
    this.userData = data;
  }

  @Mutation
  REMOVE_JWT(){
    this.jwt = null
    localStorage.removeItem('jwt')
    $ctx.$axios.setToken(false)
  }

  @Action
  initializeIdentity(): string | null {
    if (!this.jwt) {
      this.context.commit("JWT_RESTORED")
    }

    if (this.jwt) {
      const decode = JwtDecode(this.jwt!) as Record<string, string>;
      const jwtExpires = parseInt(decode.exp)

      if (Date.now() >= jwtExpires * 1000) {
        this.context.commit("JWT_EXPIRED")
        this.context.commit("REMOVE_JWT")
      }
    }

    this.context.dispatch("fetchData")

    return this.jwt;
  }

  @Action
  async login(loginDTO: LoginDTO) {
    let response = await $ctx.$uow.identity.login(loginDTO)

    if (response.error) {
      this.context.commit("LOGIN_FAILED", response.error)
      return false
    } else {
      this.context.commit("LOGIN_SUCCEEDED", response.data)
      this.context.dispatch("fetchData")

      return true
    }
  }

  @Action
  async fetchData(){
    let response = await $ctx.$uow.identity.getCurrentUser()
    this.context.commit("CURRENT_USER_FETCHED", response.data)
  }

  @Action
  logout() {
    this.context.commit("LOGOUT")
    this.context.commit("REMOVE_JWT")
  }
}
