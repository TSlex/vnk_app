<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <template v-if="loaded">
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
            placeholder="Поиск по названию"
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
import { Component, Vue } from "nuxt-property-decorator";
import { ordersStore } from "~/store";

@Component({})
export default class OrderHistory extends Vue {
  id!: number;
  loaded = false;

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    ordersStore
      .getOrderHistory({ id: this.id, pageIndex: 0 })
      .then((suceeded) => {
        if (!suceeded) {
          this.$router.back();
        } else {
          this.loaded = true;
        }
      });
  }
}
</script>
