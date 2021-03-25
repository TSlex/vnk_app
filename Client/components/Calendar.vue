<template>
  <v-sheet color="transparent">
    <v-sheet rounded="lg" class="pt-1">
      <v-toolbar flat>
        <v-btn outlined text>Добавить заказ</v-btn>
        <v-spacer></v-spacer>
        <v-text-field
          rounded
          outlined
          single-line
          hide-details
          dense
          flat
        ></v-text-field>
        <v-spacer></v-spacer>
          <v-toolbar-title v-if="loaded">{{
            $refs.calendar.title
          }}</v-toolbar-title>
        <v-btn fab text color="grey darken-2" @click="prevMonth()">
          <v-icon> mdi-chevron-left </v-icon>
        </v-btn>
        <v-btn fab text color="grey darken-2" @click="nextMonth()">
          <v-icon> mdi-chevron-right </v-icon>
        </v-btn>
      </v-toolbar>
    </v-sheet>
    <v-sheet height="700" class="pa-2" rounded="b-lg">
      <v-calendar
        ref="calendar"
        v-model="start"
        locale="ru"
        :weekdays="weekdays"
        :show-month-on-first="false"
        :short-weekdays="false"
        :hide-header="true"
      >
        <!-- Date -->
        <template v-slot:day-label="{ date }">
          <template>
            <span>{{ date }}</span>
            <v-divider></v-divider>
          </template>
        </template>
        <!-- Day content slot -->
        <template v-slot:day="{ date }">
          <template>
            <v-chip v-for="order in getDayOrders(date)" :key="Math.random(order)">
              <span
                v-for="attribute in order.featured"
                :key="attribute.name"
                class="text-caption"
                >{{ attribute.value }}</span
              >
            </v-chip>
          </template>
        </template>
      </v-calendar>
    </v-sheet>
  </v-sheet>
</template>

<script>
export default {
  data: () => ({
    start: new Date().toISOString(),
    loaded: false,
  }),
  props: ["weekdays", "orders"],
  methods: {
    getDayOrders(date) {
      return this.orders.filter((order) => order.date == date);
    },
    prevMonth() {
      this.$refs.calendar.prev();
    },
    nextMonth() {
      this.$refs.calendar.next();
    },
  },
  mounted() {
    this.$refs.calendar.checkChange();
    this.loaded = true;
  }
};
</script>
