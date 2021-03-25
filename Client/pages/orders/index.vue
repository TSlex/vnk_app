<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <v-toolbar flat>
        <v-spacer></v-spacer>
        <v-toolbar-title>Заказы</v-toolbar-title>
        <v-spacer></v-spacer>
      </v-toolbar>
      <v-data-table
        :items="Orders"
        :headers="headers"
        :items-per-page="5"
        hide-default-footer
      ></v-data-table>
      <v-pagination v-model="page" :length="pageCount"></v-pagination>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component
export default class Orders extends Vue {
  headers = [{ text: "Дата", value: "date" }];

  get Orders() {
    var orders = this.$store.getters["orders/getOrders"];
    return orders.map((order: any) => {
      return { date: order.date };
    });
  }
}
</script>
