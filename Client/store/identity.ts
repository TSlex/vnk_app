import JwtDecode from 'jwt-decode';
import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { LoginDTO } from '~/types/Identity/LoginDTO';

@Module({
  namespaced: true,
  stateFactory: true,
  name: "identitystore"
})
export default class IdentityStore extends VuexModule {
  jwt: string | null = null

  get isAuthenticated() {
    return this.verifiedJwt !== null;
  }

  get verifiedJwt(): string | null {
    if (!this.jwt && process.browser) {
      this.jwt = localStorage.getItem('jwt')
    }

    if (this.jwt) {
      const decode = JwtDecode(this.jwt!) as Record<string, string>;
      const jwtExpires = parseInt(decode.exp)

      if (Date.now() >= jwtExpires * 1000) {
        this.jwt = null

        if (process.browser) {
          localStorage.removeItem('jwt')
        }
      }
    }

    return this.jwt;
  }

  @Mutation
  setJwt(jwt: string) {
    if (process.browser) {
      if (jwt) {
        localStorage.setItem('jwt', jwt)
      } else {
        localStorage.removeItem('jwt')
      }
    }

    this.jwt = jwt
  }

  @Action({ commit: "setJwt" })
  async login(loginDTO: LoginDTO) {
    return await $ctx.$uow.identity.login(loginDTO)
  }

  @Action({ commit: "setJwt" })
  async logout() {
    return null
  }
}
