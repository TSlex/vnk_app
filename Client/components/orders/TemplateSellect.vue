<template>
  <v-card v-if="fetched">
    <v-card-title> <span class="headline">Применить шаблон</span></v-card-title>
    <v-card-text>
      <v-select
        v-model="template"
        :items="templates"
        item-text="name"
        item-value="id"
        label="Шаблон"
        return-object
        single-line
      ></v-select>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn color="blue darken-1" text @click="onTemplateApply()"
        >Применить</v-btn
      >
      <v-spacer></v-spacer>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";
import { SortOption } from "~/models/Enums/SortOption";
import { TemplateGetDTO } from "~/models/TemplateDTO";
import { templatesStore } from "~/store";

@Component({})
export default class TemplateSellect extends Vue {
  template: TemplateGetDTO | null = null;
  templates: TemplateGetDTO[] = [];

  fetched = false;

  // get templates() {
  //   return templatesStore.templates;
  // }

  onTemplateApply() {
    if (this.template != null) {
      this.$emit("apply", { ...this.template });
    }
  }

  mounted() {
    this.$uow.templates
      .getAll(0, 100, SortOption.False, null)
      .then((response) => {
        if (!response.error) {
          this.templates = response.data.items;
          this.fetched = true;
        }
      });
  }
}
</script>
