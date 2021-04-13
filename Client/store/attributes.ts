import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/models/Identity/UserDTO'

@Module({
  namespaced: true,
  stateFactory: true,
  name: "attributes"
})
export default class AttributesStore extends VuexModule {
  attributes = [
    { name: "Время погрузки", value: "8:00" },
    { name: "Название фирмы", value: "BlablaCompany LTD" },
    { name: "Место доставки", value: "Marselle, France" },
    { name: "Продукт", value: "HCR-105" },
    { name: "Упаковка", value: "500 kg bags" },
    { name: "Номер заказа", value: "134561363452345" },
    { name: "Перевозчик", value: "NOVOTRADE LOGISTICS" },
  ]
}
