import JwtDecode from 'jwt-decode';

interface IState {
  jwt: string | null
}

export const state = () => ({
  jwt: null,
} as IState)

export const getters = {
  isAuthenticated(state: IState, getters: any): boolean {
    return getters.getJwt !== null;
  },

  getJwt(state: IState): string | null {
    if (!state.jwt && process.browser) {
      state.jwt = localStorage.getItem('jwt')
    }

    if (state.jwt) {
      const decode = JwtDecode(state.jwt!) as Record<string, string>;
      const jwtExpires = parseInt(decode.exp)

      if (Date.now() >= jwtExpires * 1000) {
        state.jwt = null

        if (process.browser) {
          localStorage.removeItem('jwt')
        }
      }
    }

    return state.jwt;
  },
}

export const mutations = {
  setJwt(state: IState, jwt: string | null) {
    if (process.browser) {
      if (jwt) {
        localStorage.setItem('jwt', jwt)
      } else {
        localStorage.removeItem('jwt')
      }
    }

    state.jwt = jwt;
  },
}

export const actions = {
  clearJwt({ commit }: any): void {
    commit('setJwt', null);
  },
}

