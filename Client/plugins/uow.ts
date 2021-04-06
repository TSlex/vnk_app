import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

import AttributeTypesRepo from "@/dal/AttributeTypesRepo"
import IdentityRepo from "@/dal/IdentityRepo"

interface IAppUnitofWork {
  attributeTypes: AttributeTypesRepo,
  identity: IdentityRepo
}

const AppUnitOfWork: Plugin = (ctx: Context, inject: Inject) => {
  const repositories: IAppUnitofWork = {
    attributeTypes: new AttributeTypesRepo(ctx.$axios),
    identity: new IdentityRepo(ctx.$axios)
  }

  inject('uow', repositories)
}

export default AppUnitOfWork

declare module 'vue/types/vue' {
  interface Vue {
    $uow: IAppUnitofWork
  }
}

declare module '@nuxt/types' {
  interface NuxtAppOptions {
    $uow: IAppUnitofWork
  }

  interface Context {
    $uow: IAppUnitofWork
  }
}
