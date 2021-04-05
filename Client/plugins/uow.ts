import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

import AttributeTypesRepo from "@/dal/AttributeTypesRepo"
import IdentityRepo from "@/dal/IdentityRepo"

interface IRepo {
  attributeTypes: AttributeTypesRepo,
  identity: IdentityRepo
}

const AppUnitOfWork: Plugin = (ctx: Context, inject: Inject) => {
  const repositories: IRepo = {
    attributeTypes: new AttributeTypesRepo(ctx.$axios),
    identity: new IdentityRepo(ctx.$axios)
  }

  inject('uow', repositories)
}

export default AppUnitOfWork

declare module 'vue/types/vue' {
  interface Vue {
    $uow: IRepo
  }
}

declare module '@nuxt/types' {
  interface NuxtAppOptions {
    $uow: IRepo
  }

  interface Context {
    $uow: IRepo
  }
}

declare module 'vuex/types/index' {
  interface Store<S> {
    $uow: IRepo
  }
}
