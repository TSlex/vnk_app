<template>
  <v-row justify="center" class="text-center">
    <v-col cols="8" class="mt-4">
      <template v-if="fetched">
        <v-container>
          <v-btn class="mr-2">Заказы с датой</v-btn>
          <v-btn>Заказы без даты</v-btn>
        </v-container>
        <v-toolbar flat class="rounded-t-lg">
          <v-btn outlined text large to="templates/create">Добавить</v-btn>
          <v-spacer></v-spacer>
          <v-text-field
            rounded
            outlined
            single-line
            hide-details
            dense
            flat
            placeholder="Поиск по названию"
            prepend-icon="mdi-magnify"
            clear-icon="mdi-close"
            clearable
            v-model="searchKey"
          ></v-text-field>
        </v-toolbar>
        <v-data-table
          @update:options="setOrdering"
          :items="templates"
          :headers="headers"
          :items-per-page="itemsOnPage"
          sort-by="name"
          hide-default-footer
          @click:row="openDetails"
          class="rounded-b-lg rounded-t-0"
        >
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
import { templatesStore } from "~/store";
import { TemplateGetDTO } from "~/models/TemplateDTO";
import { SortOption } from "~/models/Enums/SortOption";

@Component({
  components: {},
})
export default class templatesIndex extends Vue {
  fetched = false;
  createDialog = false;
  currentPage = 1;
  searchKey = "";
  byName = SortOption.False;
  byType = SortOption.False;

  headers = [{ text: "Название", value: "name", align: "center" }];

  deleteDialog = false;

  get currentPageIndex() {
    return (this.currentPage ?? 1) - 1;
  }

  get template() {
    return templatesStore.selectedTemplate;
  }

  get templates() {
    return templatesStore.templates;
  }

  get itemsOnPage() {
    return templatesStore.itemsOnPage;
  }

  get pagesCount() {
    return templatesStore.pagesCount;
  }

  onEdit(id: number) {
    this.$router.push(`templates/edit/${id}`);
  }

  onDelete(id: number) {
    templatesStore.getTemplate(id).then((succeeded) => {
      if (succeeded) {
        this.deleteDialog = true;
      }
    });
  }

  onNavigateToAttribute(attributeId: number) {
    this.$router.push(`attributes/${attributeId}`);
  }

  onNavigateToType(typeId: number) {
    this.$router.push(`types/${typeId}`);
  }

  openDetails(item: TemplateGetDTO) {
    this.$router.push(`templates/${item.id}`);
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
    this.fetchtemplates();
  }

  fetchtemplates() {
    templatesStore
      .getTemplates({
        pageIndex: this.currentPageIndex,
        byName: this.byName,
        searchKey: this.searchKey ?? "",
      })
      .then((_) => {
        this.fetched = true;
      });
  }

  @Watch("template")
  @Watch("currentPage")
  @Watch("searchKey")
  @Watch("byName")
  @Watch("byType")
  updateWatcher() {
    this.fetchtemplates();
  }
}
</script>
