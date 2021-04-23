<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <template v-if="isMounted">
        <v-container>
          <v-btn to="/orders" class="mr-2" active-class="v-btn--hide-active"
            >Заказы с датой</v-btn
          >
          <v-btn to="/orders/nodate" active-class="v-btn--hide-active"
            >Заказы без даты</v-btn
          >
        </v-container>
        <v-toolbar flat class="rounded-t-lg">
          <v-btn outlined text large to="orders/create">Добавить</v-btn>
          <v-btn
            outlined
            text
            large
            class="ml-2"
            @click.stop="exportDialog = true"
            >Экспорт</v-btn
          >
          <v-spacer></v-spacer>
          <v-btn large icon @click.stop="filterDialog = true"
            ><v-icon>mdi-filter</v-icon></v-btn
          >
          <v-text-field
            rounded
            outlined
            single-line
            hide-details
            dense
            flat
            placeholder="Поиск по номеру заказа"
            prepend-icon="mdi-magnify"
            clear-icon="mdi-close"
            clearable
            v-model="searchKey"
          ></v-text-field>
        </v-toolbar>
        <v-data-table
          :loading="!fetched"
          @update:options="setOrdering"
          :items="orders"
          :headers="headers"
          :items-per-page="itemsOnPage"
          hide-default-footer
          @click:row="openDetails"
          class="rounded-b-lg rounded-t-0"
        >
          <template v-slot:[`item.date`]="{ item }">
            {{ item.executionDateTime | formatDateTime }}
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
      <FilterDialog
        v-model="filterDialog"
        :filter.sync="filterModel"
        v-if="filterDialog"
      />
      <ExportDialog v-model="exportDialog" v-if="exportDialog" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { ordersStore } from "~/store";
import { OrderGetDTO } from "~/models/OrderDTO";
import { SortOption } from "~/models/Enums/SortOption";

import FilterDialog from "~/components/orders/FilterDialog.vue";
import ExportDialog from "~/components/orders/ExportDialog.vue";

@Component({
  components: {
    FilterDialog,
    ExportDialog,
  },
})
export default class ordersIndex extends Vue {
  fetched = false;
  isMounted = false;

  createDialog = false;
  currentPage = 1;
  searchKey = "";
  byName = SortOption.False;

  filterModel: {
    startDatetime?: Date;
    endDatetime?: Date;
    checkDatetime?: Date;
    completed?: boolean;
    overdued?: boolean;
  } = {};

  filterDialog = false;
  exportDialog = false;

  headers = [
    { text: "Номер заказа", value: "name", align: "left" },
    { text: "Дата", value: "date", align: "center", sortable: false },
    { text: "Статус", value: "status", align: "right", sortable: false },
  ];

  get currentPageIndex() {
    return (this.currentPage ?? 1) - 1;
  }

  get orders() {
    return ordersStore.ordersDate;
  }

  get pagesCount() {
    return ordersStore.datePagesCount;
  }

  get itemsOnPage() {
    return ordersStore.itemsOnPage;
  }

  openDetails(order: OrderGetDTO) {
    this.$router.push(`/orders/${order.id}`);
  }

  setOrdering(options: any) {
    this.byName = SortOption.False;

    switch (options.sortBy[0]) {
      case "name":
        this.byName =
          options.sortDesc[0] == undefined ? 0 : !!options.sortDesc[0] ? 1 : -1;
        break;
    }
  }

  mounted() {
    this.fetchorders();
    this.isMounted = true;
  }

  fetchorders() {
    this.fetched = false;
    ordersStore
      .getOrdersWithDate({
        pageIndex: this.currentPageIndex,
        byName: this.byName,
        completed: this.filterModel.completed,
        overdued: this.filterModel.overdued,
        searchKey: this.searchKey ?? "",
        startDatetime: this.filterModel.startDatetime,
        endDatetime: this.filterModel.endDatetime,
        checkDatetime: this.filterModel.checkDatetime,
      })
      .then((_: any) => {
        this.fetched = true;
      });
  }

  @Watch("order")
  @Watch("currentPage")
  @Watch("searchKey")
  @Watch("byName")
  @Watch("filterModel")
  updateWatcher() {
    this.fetchorders();
  }
}
</script>
