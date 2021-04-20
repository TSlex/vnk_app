<template>
  <v-autocomplete
    v-model="attribute"
    :items="availableTypes"
    :loading="isLoading"
    :search-input.sync="searchKey"
    hide-no-data
    item-text="name"
    item-value="id"
    label="Атрибут"
    placeholder="Начните ввод для поиска"
    prepend-icon="mdi-database-search"
    :rules="rules.attribute"
    return-object
  >
  </v-autocomplete>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "nuxt-property-decorator";
import { SortOption } from "~/models/Enums/SortOption";
import { attributesStore } from "~/store";
import { isSellected } from "~/utils/form-validation";

@Component({})
export default class AttributeSellect extends Vue {
  @Prop()
  value!: { id: number; name: string };

  searchKey = "";
  isLoading = false;

  rules = {
    attribute: [(value: { id: number; name: string }) => (value.id > 0) || `Данное поле обязательно`],
  };

  get attribute() {
    return this.value;
  }

  set attribute(value) {
    this.$emit("input", value);
  }

  get availableTypes() {
    return attributesStore.attributes;
  }

  @Watch("searchKey")
  onFetchRequired() {
    this.isLoading = true;
    attributesStore
      .getAttributes({
        pageIndex: 0,
        byName: SortOption.False,
        byType: SortOption.False,
        searchKey: this.searchKey,
      })
      .then((_) => {
        this.isLoading = false;
      });
  }
}
</script>
