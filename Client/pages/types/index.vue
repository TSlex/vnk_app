<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="mt-4">
      <template v-if="fetched">
        <v-toolbar flat class="rounded-t-lg">
          <v-btn outlined text large to="types/create">Добавить</v-btn>
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
        <v-divider></v-divider>
        <v-data-table
          @update:options="setOrdering"
          :items="attributeTypes"
          :headers="headers"
          :items-per-page="itemsOnPage"
          sort-by="name"
          hide-default-footer
          @click:row="openDetails"
          class="rounded-b-lg rounded-t-0"
        >
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
            <v-chip v-if="isSimpleType(item)"
              >обычный</v-chip
            >
          </template>
          <template v-slot:[`item.actions`]>
            <v-icon> mdi-pencil </v-icon>
          </template>
        </v-data-table>
        <v-pagination
          v-model="currentPage"
          :length="pagesCount"
          :total-visible="10"
          class="mt-2"
        ></v-pagination>
      </template>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { AttributeTypeGetDTO } from "~/models/AttributeTypeDTO";

@Component({})
export default class AttributeTypesIndex extends Vue {
  fetched = false;
  createDialog = false;
  currentPage = 1;
  searchKey = "";
  orderReversed = false;

  headers = [
    { text: "Название", value: "name", align: "left" },
    { text: "Тип", sortable: false, value: "type", align: "right" },
  ];

  isSimpleType(item: AttributeTypeGetDTO){
    return !item.systemicType && !item.usesDefinedValues && !item.usesDefinedUnits
  }

  get currentPageIndex(){
    return (this.currentPage ?? 1) - 1
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

  openDetails(item: AttributeTypeGetDTO) {
    this.$router.push(`types/${item.id}`)
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
        pageIndex: this.currentPageIndex,
        orderReversed: this.orderReversed ?? false,
        searchKey: this.searchKey ?? "",
      })
      .then((_) => {
        this.fetched = true;
      });
  }

  @Watch("currentPage")
  @Watch("searchKey")
  @Watch("byName")
  @Watch("byType")
  updateWatcher() {
    this.fetchTypes();
  }
}
</script>
