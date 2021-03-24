<template>
  <v-row dense class="pa-4 fill-height">
    <v-col>
      <v-sheet>
        <v-toolbar flat>
          <v-btn fab text small color="grey darken-2">
            <v-icon small> mdi-chevron-left </v-icon>
          </v-btn>
          <v-btn fab text small color="grey darken-2">
            <v-icon small> mdi-chevron-right </v-icon>
          </v-btn>
          <v-toolbar-title> </v-toolbar-title>
          <v-spacer></v-spacer>
        </v-toolbar>
      </v-sheet>
      <v-sheet height="600" class="pa-1">
        <v-calendar
          ref="calendar"
          :weekdays="weekdays"
          :show-month-on-first="false"
          locale="ru"
          :short-weekdays="false"
        ></v-calendar>
      </v-sheet>
    </v-col>
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
