<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <v-toolbar flat>
        <v-btn outlined text large>Добавить</v-btn>
        <v-spacer></v-spacer>
        <v-text-field
          rounded
          outlined
          single-line
          hide-details
          dense
          flat
          placeholder="Поиск"
          prepend-icon="mdi-magnify"
          clear-icon="mdi-close"
          clearable
        ></v-text-field>
      </v-toolbar>
      <v-data-table
        :items="attributes"
        :headers="headers"
        :items-per-page="12"
        sort-by="name"
        hide-default-footer
        :page.sync="page"
      >
        <template v-slot:[`item.type`]="{ item }">
            {{ item.type }}
        </template>
        <template v-slot:[`item.actions`]>
          <v-icon> mdi-pencil </v-icon>
          <v-icon> mdi-folder-clock </v-icon>
          <v-icon> mdi-delete </v-icon>
        </template>
      </v-data-table>
      <v-pagination
        v-model="page"
        :length="pageCount"
        :total-visible="12"
        class="mt-2"
      ></v-pagination>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";

@Component({})
export default class AttributesIndex extends Vue {
  headers = [
    { text: "Название", value: "name", align: "left" },
    { text: "Тип", value: "type", align: "left" },
    { value: "actions", sortable: false, align: "right" },
  ];
  attributes = [
    { name: "Фирма", type: "Строка" },
    { name: "Продукт", type: "Продукт" },
    { name: "Количество", type: "Мешки" },
    { name: "Фирма", type: "Строка" },
    { name: "Продукт", type: "Продукт" },
    { name: "Количество", type: "Мешки" },
    { name: "Фирма", type: "Строка" },
    { name: "Продукт", type: "Продукт" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
    { name: "Количество", type: "Мешки" },
  ];
  page = 1;
  pageCount = 200;
}
</script>
