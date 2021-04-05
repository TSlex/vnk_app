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
        @click:row="$router.push(`types/1`)"
      >
        <template v-slot:[`item.type`]="{ item }">
          <v-chip color="blue" dark v-if="item.protected">системный</v-chip>
          <v-chip color="lime" v-if="item.hasUnits">с единицами измерения</v-chip>
          <v-chip color="amber" v-if="item.hasValues">с определенными значениями</v-chip>
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
import { attributeTypesStore } from '~/store'

@Component({})
export default class AttributesIndex extends Vue {
  headers = [
    { text: "Название", value: "name", align: "left" },
    { text: "Тип", value: "type", align: "left" },
    { value: "actions", sortable: false, align: "right" },
  ];
  attributes = [
    {
      name: "Место назначение",
      protected: false,
      hasUnits: false,
      hasValues: true,
    },
    { name: "Строка", protected: true, hasUnits: false, hasValues: false },
    { name: "Вес", protected: false, hasUnits: true, hasValues: false },
    { name: "ALL", protected: true, hasUnits: true, hasValues: true },
  ];
  page = 1;
  pageCount = 200;

  mounted() {
    attributeTypesStore.getAll().then(() => {
      console.log(attributeTypesStore.attributeTypes)
    })
  }
}
</script>
