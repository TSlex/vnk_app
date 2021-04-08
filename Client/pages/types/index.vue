<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <template v-if="fetched">
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
            v-model="searchKey"
          ></v-text-field>
        </v-toolbar>
        <v-data-table
          @update:options="setOrdering"
          :items="attributeTypes"
          :headers="headers"
          :items-per-page="itemsOnPage"
          sort-by="name"
          hide-default-footer
          @click:row="$router.push(`types/1`)"
        >
          <template v-slot:[`header.name`]="{}">
            <div>Hello</div>
          </template>
          <template v-slot:[`item.type`]="{ item }">
            <v-chip color="blue" dark v-if="item.systemicType"
              >системный</v-chip
            >
            <v-chip color="lime" v-if="item.usesDefinedUnits"
              >с единицами измерения</v-chip
            >
            <v-chip color="amber" v-if="item.usesDefinedValues"
              >с определенными значениями</v-chip
            >
          </template>
          <template v-slot:[`item.actions`]>
            <v-icon> mdi-pencil </v-icon>
            <v-icon> mdi-folder-clock </v-icon>
            <v-icon> mdi-delete </v-icon>
          </template>
        </v-data-table>
        <v-pagination
          v-model="currentPage"
          :length="pagesCount"
          :total-visible="12"
          class="mt-2"
        ></v-pagination>
      </template>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";

@Component({})
export default class AttributesIndex extends Vue {
  fetched = false;
  createDialog = false;
  _currentPage = 0;
  _searchKey = "";
  _orderReversed = false;

  headers = [
    { text: "Название", value: "name", align: "left" },
    { text: "Тип", sortable: false, value: "type", align: "left" },
    { value: "actions", sortable: false, align: "right" },
  ];

  get orderReversed() {
    return this._orderReversed;
  }

  set orderReversed(value) {
    this._orderReversed = value;
    this.fetchTypes();
  }

  get searchKey() {
    return this._searchKey;
  }

  set searchKey(value) {
    this._searchKey = value;
    this.fetchTypes();
  }

  get currentPage() {
    return this._currentPage;
  }

  set currentPage(value) {
    this._currentPage = --value;
    this.fetchTypes();
  }

  get attributeTypes() {
    return attributeTypesStore.attributeTypes;
  }

  get itemsOnPage() {
    return attributeTypesStore.itemsOnPage;
  }

  get pagesCount() {
    return attributeTypesStore.pagesCount;
  }

  setOrdering(options: any) {
    this.orderReversed = !!options.sortDesc[0];
  }

  mounted() {
    this.fetchTypes();
  }

  fetchTypes() {
    attributeTypesStore
      .getAttributeTypes({
        pageIndex: this._currentPage ?? 0,
        orderReversed: this._orderReversed ?? false,
        searchKey: this._searchKey ?? "",
      })
      .then((_) => {
        this.fetched = true;
      });
  }
}
</script>
