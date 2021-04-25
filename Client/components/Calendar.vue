<template>
  <v-sheet color="transparent">
    <v-sheet rounded="lg" class="pt-1">
      <v-toolbar flat>
        <v-btn outlined text large to="orders/create" class="mr-2" v-if="isAdministrator"
          >Добавить заказ</v-btn
        >
        <v-btn outlined text large to="orders/create">Отчет</v-btn>
        <v-spacer></v-spacer>
        <v-text-field
          v-model="searchKey"
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
        ></v-text-field>
        <v-spacer></v-spacer>
        <v-toolbar-title v-if="isMounted">{{
          $refs.calendar.title
        }}</v-toolbar-title>
        <v-btn
          fab
          text
          color="grey darken-2"
          @click="onPrevMonth()"
          :loading="!fetched"
        >
          <v-icon> mdi-chevron-left </v-icon>
        </v-btn>
        <v-btn
          fab
          text
          color="grey darken-2"
          @click="onNextMonth()"
          :loading="!fetched"
        >
          <v-icon> mdi-chevron-right </v-icon>
        </v-btn>
      </v-toolbar>
    </v-sheet>
    <v-sheet height="700" class="pa-2" rounded="b-lg">
      <v-calendar
        ref="calendar"
        locale="ru"
        :weekdays="weekdays"
        :show-month-on-first="false"
        :short-weekdays="false"
        :hide-header="true"
        v-model="date"
      >
        <!-- Date -->
        <template v-slot:day-label="{ date }">
          <template>
            <CalendarMenu
              :date="date"
              v-on:order-sellect="onOrderSellect"
              :orders="getDayOrders(date)"
            />
            <v-divider></v-divider>
          </template>
        </template>
        <!-- Day content slot -->
        <template v-slot:day="{ date }">
          <div class="pa-2 text-center">
            <v-chip
              @click.stop="onOrderSellect(order)"
              v-for="order in getDayOrders(date, 3)"
              :key="order.id"
              class="mb-1 d-flex justify-center"
              style="width: 100%"
              small
              :color="
                order.completed
                  ? `success`
                  : order.overdued
                  ? `error`
                  : `primary`
              "
            >
              <span>
                {{
                  formatFeaturedAttributesInline(
                    getOrderFeaturedAttributes(order)
                  )
                }}
              </span>
            </v-chip>
          </div>
        </template>
      </v-calendar>
    </v-sheet>
  </v-sheet>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "nuxt-property-decorator";
import { OrderAttributeGetDTO, OrderGetDTO } from "~/models/OrderDTO";
import { identityStore, ordersStore } from "~/store";
import CalendarMenu from "~/components/CalendarMenu.vue";

@Component({
  components: {
    CalendarMenu,
  },
})
export default class Calendar extends Vue {
  showMenu = false;

  $refs!: {
    calendar: any;
  };

  date = null;

  fetched = false;
  isMounted = false;

  searchKey = "";

  @Prop() weekdays!: any[];

  startMonthDate = new Date();
  endMonthDate = new Date();

  get isAdministrator() {
    return identityStore.isAdministrator || identityStore.isRoot;
  }

  get orders() {
    return ordersStore.ordersByDate;
  }

  onOrderSellect(order: OrderGetDTO) {
    ordersStore.getOrder({ id: order.id, checkDatetime: null });
  }

  getDayOrders(date: string, limit?: number) {
    var orders = this.orders[date] ?? [];

    orders = _.sortBy(orders, ["!overdued", "completed"]);

    if (limit) {
      var orders = orders.slice(0, limit);
    }

    return orders;
  }

  getOrderFeaturedAttributes(order: OrderGetDTO) {
    return order.attributes.filter((attribute) => attribute.featured);
  }

  formatFeaturedAttributesInline(attributes: OrderAttributeGetDTO[]) {
    return attributes
      .map((attribute) => {
        let line = `${attribute.name.toLocaleLowerCase()}: ${attribute.value.toLocaleLowerCase()}`;

        if (attribute.usesDefinedUnits) {
          line += ` ${attribute.unit.toLocaleLowerCase()}`;
        }

        return line;
      })
      .join(", ");
  }

  async onPrevMonth() {
    if (!this.fetched) {
      return;
    }

    this.startMonthDate = this.$moment(this.startMonthDate)
      .subtract(1, "month")
      .toDate();
    this.endMonthDate = this.$moment(this.endMonthDate)
      .subtract(1, "month")
      .toDate();

    await this.fetchorders();
    this.$refs.calendar.prev();
  }

  async onNextMonth() {
    if (!this.fetched) {
      return;
    }

    this.startMonthDate = this.$moment(this.startMonthDate)
      .add(1, "month")
      .toDate();
    this.endMonthDate = this.$moment(this.endMonthDate)
      .add(1, "month")
      .toDate();

    await this.fetchorders();

    this.$refs.calendar.next();
  }

  mounted() {
    this.$refs.calendar.checkChange();

    this.startMonthDate = this.$moment(new Date())
      .startOf("month")
      .subtract(1, "months")
      .toDate();
    this.endMonthDate = this.$moment(new Date())
      .endOf("month")
      .add(1, "months")
      .toDate();

    this.fetchorders();

    this.isMounted = true;
  }

  async fetchorders() {
    this.fetched = false;
    var orders = await ordersStore.getCalendarOrders({
      searchKey: this.searchKey,
      startDatetime: this.startMonthDate,
      endDatetime: this.endMonthDate,
    });
    this.fetched = true;
    return orders;
  }

  @Watch("searchKey")
  updateWatcher() {
    this.fetchorders();
  }
}
</script>

<style>
.v-calendar-weekly__day-label {
  margin: 0;
}
</style>
