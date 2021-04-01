import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

declare module 'vue/types/vue' {
  interface Vue {
    $hello(message: string): void
  }
}

declare module '@nuxt/types' {
  interface NuxtAppOptions {
    $hello(message: string): void
  }
  // nuxtContext.$myInjectedFunction
  interface Context {
    $hello(message: string): void
  }
}

declare module 'vuex/types/index' {
  interface Store<S> {
    $hello(message: string): void
  }
}

const MyPlugin: Plugin = (ctx: Context, inject: Inject) => {
  const repositories = {
  }

  inject('hello', (msg: string) => console.log(`Hello ${msg}!`))
}

export default MyPlugin
