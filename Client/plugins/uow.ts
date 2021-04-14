import { Context, Plugin } from "@nuxt/types";
import { Inject } from "@nuxt/types/app";

import { AttributesRepo } from "@/dal/AttributesRepo"
import { AttributeTypesRepo } from "@/dal/AttributeTypesRepo"
import { IdentityRepo } from "@/dal/IdentityRepo"
import { AttributeTypeValuesRepo } from "~/dal/AttributeTypeValuesRepo";
import { AttributeTypeUnitsRepo } from "~/dal/AttributeTypeUnitsRepo";
import { TemplatesRepo } from "~/dal/TemplatesRepo";
import { TemplateAttributesRepo } from "~/dal/TemplateAttributesRepo";
import { OrdersRepo } from "~/dal/OrdersRepo";

interface IAppUnitofWork {
  attributes: AttributesRepo
  attributeTypes: AttributeTypesRepo,
  attributeTypeValues: AttributeTypeValuesRepo,
  attributeTypeUnits: AttributeTypeUnitsRepo,
  templates: TemplatesRepo,
  templateAttributes: TemplateAttributesRepo,
  identity: IdentityRepo,
  orders: OrdersRepo
}

const AppUnitOfWork: Plugin = (ctx: Context, inject: Inject) => {
  const repositories: IAppUnitofWork = {
    attributes: new AttributesRepo(ctx.$axios),
    attributeTypes: new AttributeTypesRepo(ctx.$axios),
    attributeTypeValues: new AttributeTypeValuesRepo(ctx.$axios),
    attributeTypeUnits: new AttributeTypeUnitsRepo(ctx.$axios),
    templates: new TemplatesRepo(ctx.$axios),
    templateAttributes: new TemplateAttributesRepo(ctx.$axios),
    identity: new IdentityRepo(ctx.$axios),
    orders: new OrdersRepo(ctx.$axios)
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
