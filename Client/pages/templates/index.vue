<template>
  <v-row justify="center" class="text-center">
    <v-col cols="10" class="mt-4">
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
                <v-col cols="4" v-for="template in items" :key="template.id">
                  <v-card shaped>
                    <v-expansion-panels accordion multiple hover flat>
                      <v-expansion-panel>
                        <v-expansion-panel-header class="text-center">
                          <span class="text-h5"
                            >Шаблон "{{ template.name | textTruncate(30)}}"</span
                          >
                        </v-expansion-panel-header>
                        <v-expansion-panel-content
                          class="expansion-panel-content_no_wrap"
                        >
                          <v-divider></v-divider>
                          <!-- attributes -->
                          <v-expansion-panels
                            accordion
                            multiple
                            hover
                            flat
                            class="pa-2"
                          >
                            <v-expansion-panel
                              v-for="attribute in template.attributes"
                              :key="attribute.id"
                            >
                              <v-expansion-panel-header
                                hide-actions
                                class="pa-0 rounded-lg"
                              >
                                <div class="d-flex justify-space-between">
                                  <span class="text-body-1">{{
                                    attribute.name | textTruncate(50)
                                  }}</span>
                                  <span class="text-body-1">
                                    <v-icon v-if="attribute.featured"
                                      >mdi-star</v-icon
                                    >
                                    <v-icon v-else>mdi-star-outline</v-icon>
                                  </span>
                                </div>
                              </v-expansion-panel-header>
                              <v-expansion-panel-content
                                class="expansion-panel-content_no_wrap mt-1 rounded-lg"
                              >
                                <v-container class="grey lighten-3">
                                  <div
                                    class="d-flex justify-space-between mb-2"
                                  >
                                    <span class="text-body-2">Атрибут:</span>
                                    <v-chip
                                      small
                                      @click.stop="
                                        onNavigateToAttribute(
                                          attribute.attributeId
                                        )
                                      "
                                      >{{ attribute.name | textTruncate(50)}}</v-chip
                                    >
                                  </div>
                                  <div
                                    class="d-flex justify-space-between mb-2"
                                  >
                                    <span class="text-body-2"
                                      >Тип атрибута:</span
                                    >
                                    <v-chip
                                      small
                                      @click.stop="
                                        onNavigateToType(attribute.typeId)
                                      "
                                      >{{ attribute.type | textTruncate(50)}}</v-chip
                                    >
                                  </div>
                                  <div
                                    class="d-flex justify-space-between mb-2"
                                  >
                                    <span class="text-body-2">Формат:</span>
                                    <v-chip small>{{
                                      attribute.dataType | formatDataType
                                    }}</v-chip>
                                  </div>
                                  <div
                                    class="d-flex justify-space-between mb-2"
                                  >
                                    <span class="text-body-2"
                                      >Значения определены:</span
                                    >
                                    <v-chip small>{{
                                      attribute.usesDefinedValues
                                        | formatBoolean
                                    }}</v-chip>
                                  </div>
                                  <div
                                    class="d-flex justify-space-between mb-2"
                                  >
                                    <span class="text-body-2"
                                      >Ед. измерения определены:</span
                                    >
                                    <v-chip small>{{
                                      attribute.usesDefinedUnits | formatBoolean
                                    }}</v-chip>
                                  </div>
                                </v-container>
                              </v-expansion-panel-content>
                            </v-expansion-panel>
                          </v-expansion-panels>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>

                    <v-divider></v-divider>
                    <!-- actions -->
                    <v-container>
                      <v-btn
                        outlined
                        text
                        class="mr-1"
                        @click="onEdit(template.id)"
                        ><v-icon left>mdi-pencil</v-icon>Изменить</v-btn
                      >
                      <v-btn outlined text @click="onDelete(template.id)"
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
      <TemplateDeleteDialog
        v-model="deleteDialog"
        v-if="deleteDialog && template"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { templatesStore } from "~/store";
import { TemplateGetDTO } from "~/models/TemplateDTO";
import { SortOption } from "~/models/Enums/SortOption";
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
    let page = Number(this.$route.query.page);
    if (!isNaN(page) && page > 0) {
      this.currentPage = page;
    }

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

  @Watch("currentPage")
  updatePageQuery() {
    this.$router.push({path: this.$route.path, query: { ...this.$route.query, page: this.currentPage.toString() }})
  }

  @Watch("$route.query.page")
  updatePage(){
    let page = Number(this.$route.query.page);
    if (!isNaN(page) && page > 0) {
      this.currentPage = page;
    }
  }
}
</script>
