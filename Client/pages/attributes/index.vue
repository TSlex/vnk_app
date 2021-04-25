<template>
  <v-row justify="center" class="text-center">
    <v-col cols="8" class="mt-4">
      <template v-if="fetched">
        <v-toolbar flat class="rounded-t-lg">
          <v-btn outlined text large to="attributes/create">Добавить</v-btn>
          <v-spacer></v-spacer>
          <v-text-field
            rounded
            outlined
            single-line
            hide-details
            dense
            flat
            placeholder="Поиск по названию или типу"
            prepend-icon="mdi-magnify"
            clear-icon="mdi-close"
            clearable
            v-model="searchKey"
          ></v-text-field>
        </v-toolbar>
        <v-data-table
          @update:options="setOrdering"
          :items="attributes"
          :headers="headers"
          :items-per-page="itemsOnPage"
          sort-by="name"
          hide-default-footer
          @click:row="openDetails"
          class="rounded-b-lg rounded-t-0"
        >
          <template v-slot:[`item.`]="{ item }">
            <v-chip color="lime" v-if="item.usesDefinedUnits"
              >с единицами измерения</v-chip
            >
            <v-chip color="amber" v-if="item.usesDefinedValues"
              >с определенными значениями</v-chip
            >
          </template>
          <template v-slot:[`item.dataType`]="{ item }">
            <v-chip>{{ item.dataType | formatDataType }}</v-chip>
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
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { attributesStore } from "~/store";
import { AttributeGetDTO } from "~/models/AttributeDTO";
import { SortOption } from "~/models/Enums/SortOption";

@Component({})
export default class AttributesIndex extends Vue {
  fetched = false;
  createDialog = false;
  currentPage = 1;
  searchKey = "";
  byName = SortOption.False;
  byType = SortOption.False;

  headers = [
    { text: "Название", value: "name", align: "left" },
    { text: "Тип атрибута", value: "type", align: "right" },
    { text: "Особенности типа", value: "", sortable: false, align: "right" },
    { text: "Формат", value: "dataType", sortable: false, align: "right"},
  ];

  get currentPageIndex(){
    return (this.currentPage ?? 1) - 1
  }

  get attributes() {
    return attributesStore.attributes;
  }

  get itemsOnPage() {
    return attributesStore.itemsOnPage;
  }

  get pagesCount() {
    return attributesStore.pagesCount;
  }

  openDetails(item: AttributeGetDTO) {
    this.$router.push(`attributes/${item.id}`);
  }

  setOrdering(options: any) {
    this.byName = SortOption.False;
    this.byType = SortOption.False;

    switch (options.sortBy[0]) {
      case "name":
        this.byName =
          options.sortDesc[0] == undefined ? 0 : !!options.sortDesc[0] ? 1 : -1;
        break;
      case "type":
        this.byType =
          options.sortDesc[0] == undefined ? 0 : !!options.sortDesc[1] ? 1 : -1;
        break;
    }
  }

  mounted() {
    this.fetchAttributes();
  }

  fetchAttributes() {
    attributesStore
      .getAttributes({
        pageIndex: this.currentPageIndex,
        byName: this.byName,
        byType: this.byType,
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
    this.fetchAttributes();
  }
}
</script>
