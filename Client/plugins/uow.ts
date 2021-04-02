import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

declare module 'vue/types/vue' {
  interface Vue {
    $uow(message: string): void
  }
}

declare module '@nuxt/types' {
  interface NuxtAppOptions {
    $uow(message: string): void
  }
  // nuxtContext.$myInjectedFunction
  interface Context {
    $uow(message: string): void
  }
}

declare module 'vuex/types/index' {
  interface Store<S> {
    $uow(message: string): void
  }
}

const AppUnitOfWork: Plugin = (ctx: Context, inject: Inject) => {
  const repositories = {
  }

  inject('uow', (msg: string) => console.log(`Hello ${msg}!`))
}

export default AppUnitOfWork
