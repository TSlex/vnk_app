import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { UserGetDTO } from '~/models/Identity/UserDTO'
import { config } from 'vuex-module-decorators'

config.rawError = true

@Module({
  namespaced: true,
  stateFactory: true,
  name: "orders"
})
export default class OrdersStore extends VuexModule {
  orders = [
    {
      date: "2021-03-19",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-17",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-19",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-04",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-31",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
  ]
}
