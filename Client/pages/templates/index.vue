<template>
  <v-row justify="center" class="text-center">
    <v-col cols="8" class="mt-4">
      <template v-if="fetched">
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
          <template v-slot:body="{ items }">
            <v-container>
              <v-row dense>
                <v-col cols="4" v-for="item in items" :key="item.id">
                  <v-card shaped>
                    <v-card-title class="d-block pa-2">
                      <span class="text-h5">Шаблон "{{ item.name }}"</span>
                    </v-card-title>
                    <v-divider></v-divider>
                    <v-list rounded>
                      <v-list-item
                        class="grey lighten-5"
                        v-for="attribute in item.attributes"
                        :key="attribute.id"
                        @click.prevent="
                          onNavigateToAttribute(attribute.attributeId)
                        "
                      >
                        <v-list-item-content>
                          <v-list-item-title
                            v-text="attribute.name"
                          ></v-list-item-title>
                          <v-divider class="mb-2 mt-1"></v-divider>
                          <v-list-item-subtitle>
                            <v-chip
                              small
                              v-text="attribute.type"
                              class="mr-2"
                              @click.stop="onNavigateToType(attribute.typeId)"
                            ></v-chip
                            ><v-chip small outlined>{{
                              attribute.dataType | formatDataType
                            }}</v-chip>
                          </v-list-item-subtitle>
                        </v-list-item-content>
                        <v-list-item-icon>
                          <v-icon v-if="attribute.featured">mdi-star</v-icon>
                          <v-icon v-else>mdi-star-outline</v-icon>
                        </v-list-item-icon>
                      </v-list-item>
                    </v-list>
                    <v-divider></v-divider>
                    <v-container>
                      <v-btn outlined text class="mr-1" @click="onEdit(item.id)"
                        ><v-icon left>mdi-pencil</v-icon>Изменить</v-btn
                      >
                      <v-btn outlined text @click="onDelete(item.id)"
                        ><v-icon left> mdi-delete </v-icon>Удалить</v-btn
                      >
                    </v-container>
                  </v-card>
                </v-col>
              </v-row>
            </v-container>
          </template>
        </v-data-table>
        <v-pagination
          v-model="currentPage"
          :length="pagesCount"
          :total-visible="12"
          class="mt-2"
        ></v-pagination>
      </template>
      <TemplateDeleteDialog v-model="deleteDialog" v-if="deleteDialog && template"/>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { templatesStore } from "~/store";
import { TemplateGetDTO } from "~/models/TemplateDTO";
import { SortOptions } from "~/models/Enums/SortOptions";
import TemplateDeleteDialog from "~/components/templates/TemplateDeleteDialog.vue";

@Component({
  components: {
    TemplateDeleteDialog,
  },
})
export default class templatesIndex extends Vue {
  fetched = false;
  createDialog = false;
  currentPage = 1;
  searchKey = "";
  byName = SortOptions.False;
  byType = SortOptions.False;

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
    this.$router.push(`templates/edit/${id}`)
  }

  onDelete(id: number) {
    templatesStore.getTemplate(id).then((succeeded) => {
      if (succeeded){
        this.deleteDialog = true
      }
    })
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
    this.byName = SortOptions.False;
    this.byType = SortOptions.False;

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
