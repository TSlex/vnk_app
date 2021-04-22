import { Module, VuexModule, Mutation, Action } from 'vuex-module-decorators'
import { $ctx } from "@/utils/vue-context"
import { config } from 'vuex-module-decorators'
import { CollectionDTO } from '~/models/Common/CollectionDTO'
import { SortOption } from '~/models/Enums/SortOption'
import { OrderGetDTO, OrderPostDTO, OrderPatchDTO, OrderHistoryDTO } from '~/models/OrderDTO'

config.rawError = true

@Module({
  namespaced: true,
  stateFactory: true,
  name: "orders"
})
export default class OrdersStore extends VuexModule {
  ordersDate: OrderGetDTO[] = []
  ordersNoDate: OrderGetDTO[] = []
  histryRecords: OrderHistoryDTO[] = []

  ordersDateCount = 0;
  ordersNoDateCount = 0;
  histryRecordsCount = 0;

  itemsOnPage = 12;

  selectedOrder: OrderGetDTO | null = null
  error: string | null = null

  get datePagesCount() {
    return Math.ceil(this.ordersDateCount / this.itemsOnPage)
  }

  get historyPagesCount() {
    return Math.ceil(this.histryRecordsCount / this.itemsOnPage)
  }

  get noDatePagesCount() {
    return Math.ceil(this.ordersNoDateCount / this.itemsOnPage)
  }

  @Mutation
  ORDER_CREATED(order: OrderGetDTO) {
    if (order.executionDateTime != null) {
      this.ordersDate.push(order)
    }
    else {
      this.ordersNoDate.push(order)
    }
  }

  @Mutation
  ORDER_UPDATED(order: OrderGetDTO) {
    if (order.executionDateTime != null) {
      this.ordersDate.forEach((element: OrderGetDTO, index: number) => {
        if (element.id === order.id) {
          this.ordersDate[index] = order
        }
      });
    }
    else {
      this.ordersNoDate.forEach((element: OrderGetDTO, index: number) => {
        if (element.id === order.id) {
          this.ordersNoDate[index] = order
        }
      });
    }
  }

  @Mutation
  ORDER_DELETED(order: OrderGetDTO) {
    if (order.executionDateTime != null) {
      this.ordersDate.forEach((element: OrderGetDTO, index: number) => {
        if (element.id === order.id) {
          this.ordersDate.splice(index, 1)
        }
      });
    }
    else {
      this.ordersNoDate.forEach((element: OrderGetDTO, index: number) => {
        if (element.id === order.id) {
          this.ordersNoDate.splice(index, 1)
        }
      });
    }
  }

  @Mutation
  ORDER_SELECTED(order: OrderGetDTO) {
    this.selectedOrder = order
  }

  @Mutation
  SELECTED_ORDER_CLEARED() {
    this.selectedOrder = null
  }

  @Mutation
  ORDERS_WITH_DATE_FETCHED(collection: CollectionDTO<OrderGetDTO>) {
    this.ordersDate = collection.items
    this.ordersDateCount = collection.totalCount
  }

  @Mutation
  ORDERS_WITHOT_DATE_FETCHED(collection: CollectionDTO<OrderGetDTO>) {
    this.ordersNoDate = collection.items
    this.ordersNoDateCount = collection.totalCount
  }

  @Mutation
  ORDER_HISTORY_FETCHED(collection: CollectionDTO<OrderHistoryDTO>) {
    this.histryRecords = collection.items
    this.histryRecordsCount = collection.totalCount
  }

  @Mutation
  ACTION_FAILED(error: string) {
    this.error = error
  }

  @Mutation
  CLEAR_ERROR() {
    this.error = null
  }

  @Action
  async getOrdersWithDate(payload: {
    pageIndex: number, byName: SortOption, completed?: boolean, overdued?: boolean,
    searchKey?: string, startDatetime?: Date, endDatetime?: Date,
    checkDatetime?: Date
  }) {
    let response = await $ctx.$uow.orders.getAllWithDate(
      payload.pageIndex, this.itemsOnPage, payload.byName, payload.completed, payload.overdued,
      payload.searchKey, payload.startDatetime, payload.endDatetime, payload.checkDatetime
    )

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDERS_WITH_DATE_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getOrdersWithoutDate(payload: {
    pageIndex: number, byName: SortOption, completed?: boolean,
    searchKey?: string
  }) {
    let response = await $ctx.$uow.orders.getAllWithoutDate(
      payload.pageIndex, this.itemsOnPage, payload.byName, payload.completed, payload.searchKey)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDERS_WITHOUT_DATE_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getOrderHistory(payload: {
    id: number, pageIndex: number
  }) {
    let response = await $ctx.$uow.orders.getHistory(
      payload.id, payload.pageIndex, this.itemsOnPage)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDER_HISTORY_FETCHED", response.data)
      return true
    }
  }

  @Action
  async getOrder(payload: { id: number, checkDatetime: Date | null }) {
    let response = await $ctx.$uow.orders.getById(payload.id, payload.checkDatetime)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDER_SELECTED", response.data)
      return true
    }
  }

  @Action
  async createOrder(model: OrderPostDTO) {
    let response = await $ctx.$uow.orders.add(model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDER_CREATED", model)
      return true
    }
  }

  @Action
  async updateOrder(model: OrderPatchDTO) {

    let response = await $ctx.$uow.orders.update(model.id, model)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {

      this.context.commit("CLEAR_ERROR")
      this.context.commit("ORDER_UPDATED", model)
      this.context.dispatch("getOrder", model.id)
      return true
    }
  }

  @Action
  async deleteOrder(id: number) {
    let response = await $ctx.$uow.orders.delete(id)

    if (response.error) {
      this.context.commit("ACTION_FAILED", response.error)
      return false
    } else {
      this.context.commit("CLEAR_ERROR")
      this.context.commit("SELECTED_ORDER_CLEARED")
      this.context.commit("ORDER_DELETED", id)
      return true
    }
  }
}
