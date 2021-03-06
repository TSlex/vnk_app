<template>
  <v-row justify="center" class="text-center">
    <v-col cols="5" class="mt-4">
      <template v-if="fetched">
        <v-container>
          <v-btn to="/orders" class="mr-2" active-class="v-btn--hide-active"
            >Заказы с датой</v-btn
          >
          <v-btn to="/orders/nodate" active-class="v-btn--hide-active"
            >Заказы без даты</v-btn
          >
        </v-container>
        <v-toolbar flat class="rounded-t-lg">
          <v-btn outlined text large to="/orders/create">Добавить</v-btn>
          <v-btn
            outlined
            text
            large
            class="ml-2"
            @click.stop="exportDialog = true"
            >Отчет</v-btn
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
          @update:options="setOrdering"
          :items="orders"
          :headers="headers"
          :items-per-page="itemsOnPage"
          sort-by="name"
          hide-default-footer
          @click:row="openDetails"
          class="rounded-b-lg rounded-t-0"
        >
          <template v-slot:[`item.name`]="{ item }">
            {{ item.name | textTruncate(40) }}
          </template>
          <template v-slot:[`item.status`]="{ item }">
            <v-chip v-if="item.completed" color="success"> Выполнен </v-chip>
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
        :hasDeadline="false"
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
    { text: "Статус", value: "status", align: "right", sortable: false },
  ];

  get currentPageIndex() {
    return (this.currentPage ?? 1) - 1;
  }

  get orders() {
    return ordersStore.ordersNoDate;
  }

  get pagesCount() {
    return ordersStore.noDatePagesCount;
  }

  get itemsOnPage() {
    return ordersStore.itemsOnPage;
  }

  openDetails(item: OrderGetDTO) {
    this.$router.push(`/orders/${item.id}`);
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
    let page = Number(this.$route.query.page);
    if (!isNaN(page) && page > 0) {
      this.currentPage = page;
    }

    this.fetchorders();
  }

  fetchorders() {
    ordersStore
      .getOrdersWithoutDate({
        pageIndex: this.currentPageIndex,
        byName: this.byName,
        completed: this.filterModel.completed,
        searchKey: this.searchKey ?? "",
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

  @Watch("currentPage")
  updatePageQuery() {
    this.$router.push({path: this.$route.path, query: { ...this.$route.query, page: this.currentPage.toString() }})
  }

  @Watch("$route.query.page")
  updatePage(){
    let page = Number(this.$route.query.page);
    if (!isNaN(page) && page > 0) {
      this.currentPage = page;
    }
  }
}
</script>
