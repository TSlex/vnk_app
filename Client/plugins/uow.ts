import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

import AttributeTypesRepo from "@/dal/AttributeTypesRepo"

const AppUnitOfWork: Plugin = (ctx: Context, inject: Inject) => {
  const repositories = {
    attributeTypes: new AttributeTypesRepo(ctx.$axios)
  }

  inject('uow', repositories)
}

export default AppUnitOfWork

declare module 'vue/types/vue' {
  interface Vue {
    $uow(message: string): void
  }
}

declare module '@nuxt/types' {
  interface NuxtAppOptions {
    $uow(message: string): void
  }

  interface Context {
    $uow(message: string): void
  }
}

declare module 'vuex/types/index' {
  interface Store<S> {
    $uow(message: string): void
  }
}
