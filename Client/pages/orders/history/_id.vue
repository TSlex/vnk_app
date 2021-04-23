<template>
  <v-row justify="center" class="text-center">
    <v-col cols="8" class="mt-4">
      <template v-if="isMounted">
        <v-toolbar flat class="rounded-t-lg">
          <v-toolbar-title> История заказа "{{ order.name }}" </v-toolbar-title>
        </v-toolbar>
        <v-data-table
          :loading="!fetched"
          :items="orders"
          :headers="headers"
          :items-per-page="itemsOnPage"
          hide-default-footer
          class="rounded-b-lg rounded-t-0"
        >
          <template v-slot:[`item.date`]="{ item }">
            {{ item.executionDateTime | formatDateTime }}
          </template>
          <template v-slot:[`item.createdAt`]="{ item }">
            {{ item.createdAt | formatDateTimeUTC }}
          </template>
          <template v-slot:[`item.changedAt`]="{ item }">
            {{ item.changedAt | formatDateTimeUTC }}
          </template>
          <template v-slot:[`item.changedBy`]="{ item }">
            {{ item.changedBy }}
          </template>
          <template v-slot:[`item.status`]="{ item }">
            <v-chip v-if="item.completed" color="success"> Выполнен </v-chip>
            <v-chip v-else-if="item.overdued" color="error"> Просрочен </v-chip>
            <v-chip v-else color="primary"> Запланирован </v-chip>
          </template>
        </v-data-table>
        <v-pagination
          v-model="currentPage"
          :length="pagesCount"
          :total-visible="12"
          class="mt-2"
        ></v-pagination>
      </template>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { ordersStore } from "~/store";

@Component({})
export default class OrderHistory extends Vue {
  id!: number;
  fetched = false;
  isMounted = false;
  currentPage = 1;

  headers = [
    { text: "Номер заказа", value: "name", align: "left", sortable: false },
    {
      text: "Дата исполнения",
      value: "date",
      align: "center",
      sortable: false,
    },
    { text: "Создан", value: "createdAt", align: "center", sortable: false },
    { text: "Изменен", value: "changedAt", align: "center", sortable: false },
    { text: "Инициатор", value: "changedBy", align: "center", sortable: false },
    { text: "Статус", value: "status", align: "right", sortable: false },
  ];

  get currentPageIndex() {
    return (this.currentPage ?? 1) - 1;
  }

  get order() {
    return ordersStore.selectedOrder;
  }

  get orders() {
    return ordersStore.histryRecords;
  }

  get pagesCount() {
    return ordersStore.historyPagesCount;
  }

  get itemsOnPage() {
    return ordersStore.itemsOnPage;
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  @Watch("currentPage")
  fetchHistory() {
    this.fetched = false;
    ordersStore
      .getOrderHistory({ id: this.id, pageIndex: this.currentPageIndex })
      .then((suceeded) => {
        if (!suceeded) {
          this.$router.back();
        } else {

          this.fetched = true;
        }
      });
  }

  async mounted() {
    if (!this.id) {
      this.$router.back();
      return;
    }

    let order = await ordersStore.getOrder({
      id: this.id,
      checkDatetime: null,
    });

    if (!order) {
      this.$router.back();
      return;
    }

    this.fetchHistory();

    this.isMounted = true;
  }
}
</script>
