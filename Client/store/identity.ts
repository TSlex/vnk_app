import JwtDecode from 'jwt-decode';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { LoginDTO } from '~/types/Identity/LoginDTO';

@Module({
  namespaced: true,
  stateFactory: true,
  name: "identity"
})
export default class IdentityStore extends VuexModule {
  jwt: string | null = null
  loginError: string | null = null

  get isAuthenticated() {
    return this.verifiedJwt !== null;
  }

  get verifiedJwt(): string | null {
    if (!this.jwt) {
      this.context.commit("JWT_RESTORED")
    }

    if (this.jwt) {
      const decode = JwtDecode(this.jwt!) as Record<string, string>;
      const jwtExpires = parseInt(decode.exp)

      if (Date.now() >= jwtExpires * 1000) {
        this.context.commit("JWT_EXPIRED")
      }
    }

    return this.jwt;
  }

  @Mutation
  LOGIN_SUCCEEDED(jwt: string) {
    localStorage.setItem('jwt', jwt)

    this.jwt = jwt
    this.loginError = null
  }

  @Mutation
  LOGIN_FAILED(error: string) {
    this.loginError = error;
  }

  @Mutation
  LOGOUT() {
    this.jwt = null
    localStorage.removeItem('jwt')
  }

  @Mutation
  JWT_RESTORED() {
    this.jwt = localStorage.getItem('jwt')
  }

  @Mutation
  JWT_EXPIRED() {
    this.jwt = null
    localStorage.removeItem('jwt')
  }

  @Action
  async login(loginDTO: LoginDTO) {
    let response = await $ctx.$uow.identity.login(loginDTO)

    if (response.errorMessage) {
      this.context.commit("LOGIN_FAILED", response.errorMessage)
      return false
    } else {
      this.context.commit("LOGIN_SUCCEEDED", response.data)
      return true
    }
  }

  @Action
  async logout() {
    this.context.commit("LOGOUT")
  }
}
