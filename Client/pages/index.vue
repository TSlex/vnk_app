<template>
  <v-row dense class="pa-4 fill-height">
    <v-col>
      <!-- calendar -->
      <v-sheet rounded="lg" class="pt-1">
        <v-toolbar flat>
          <v-btn outlined text>Добавить заказ</v-btn>
          <v-spacer></v-spacer>
          <v-text-field rounded outlined single-line hide-details dense flat></v-text-field>
          <v-spacer></v-spacer>
          <v-toolbar-title>Март</v-toolbar-title>
          <v-btn fab text color="grey darken-2">
            <v-icon> mdi-chevron-left </v-icon>
          </v-btn>
          <v-btn fab text color="grey darken-2">
            <v-icon> mdi-chevron-right </v-icon>
          </v-btn>
        </v-toolbar>
      </v-sheet>
      <v-sheet height="600" class="pa-2" rounded="b-lg">
        <v-calendar
          ref="calendar"
          :weekdays="weekdays"
          :show-month-on-first="false"
          locale="ru"
          :short-weekdays="false"
        ></v-calendar>
      </v-sheet>
    </v-col>

    <!-- info bar -->
    <v-col cols="3">
      <v-sheet height="600" rounded="lg">
        <v-list-item v-for="attribute in attributes" :key="attribute.name">
          <v-list-item-content>
            <v-list-item-title v-text="attribute.name"></v-list-item-title>
            <v-list-item-subtitle
              v-text="attribute.value"
            ></v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script>
export default {
  data: () => ({
    weekdays: [1, 2, 3, 4, 5],
    attributes: [
      { name: "Время погрузки", value: "8:00" },
      { name: "Название фирмы", value: "BlablaCompany LTD" },
      { name: "Место доставки", value: "Marselle, France" },
      { name: "Продукт", value: "HCR-105" },
      { name: "Упаковка", value: "500 kg bags" },
      { name: "Номер заказа", value: "134561363452345" },
      { name: "Перевозчик", value: "NOVOTRADE LOGISTICS" },
    ],
  }),
};
</script>
