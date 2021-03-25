<template>
  <v-row dense class="py-4 px-6 fill-height">
    <v-col>
      <Calendar :weekdays="weekdays" :orders="orders"/>
    </v-col>
    <v-col cols="3">
      <v-sheet height="700" rounded="lg">
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
import Calendar from "@/components/Calendar";

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
    orders: [
      {
        date: "2021-03-19",
        featured: [
          { name: "Фирма", value: "БляИндустриес" },
          { name: "Продукт", value: "XML" },
          { name: "Количество", value: "blah" },
        ],
      },
      {
        date: "2021-03-17",
        featured: [
          { name: "Фирма", value: "БляИндустриес" },
          { name: "Продукт", value: "XML" },
          { name: "Количество", value: "blah" },
        ],
      },
      {
        date: "2021-03-19",
        featured: [
          { name: "Фирма", value: "БляИндустриес" },
          { name: "Продукт", value: "XML" },
          { name: "Количество", value: "blah" },
        ],
      },
      {
        date: "2021-03-04",
        featured: [
          { name: "Фирма", value: "БляИндустриес" },
          { name: "Продукт", value: "XML" },
          { name: "Количество", value: "blah" },
        ],
      },
      {
        date: "2021-03-31",
        featured: [
          { name: "Фирма", value: "БляИндустриес" },
          { name: "Продукт", value: "XML" },
          { name: "Количество", value: "blah" },
        ],
      },
    ],
  }),
  components: {
    Calendar,
  },
};
</script>
