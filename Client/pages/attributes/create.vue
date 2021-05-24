<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Создать атрибут</span>
          </v-card-title>
          <v-card-text class="pb-0">
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <v-text-field
                label="Название"
                required
                :rules="rules.name"
                v-model="model.name"
                class="mb-1"
              ></v-text-field>
              <v-autocomplete
                v-model="model.attributeTypeId"
                :items="availableTypes"
                :loading="isLoading"
                :search-input.sync="searchKey"
                hide-no-data
                hide-selected
                item-text="name"
                item-value="id"
                label="Тип атрибута"
                placeholder="Начните ввод для поиска"
                prepend-icon="mdi-database-search"
                :rules="rules.attribute"
              ></v-autocomplete>
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click.stop="onCancel()"
              >Отмена</v-btn
            >
            <v-btn color="blue darken-1" text type="submit">Создать</v-btn>
            <v-spacer></v-spacer>
          </v-card-actions>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { attributesStore, attributeTypesStore } from "~/store";
import { isSellected, maxlength, required } from "~/utils/form-validation";
import { AttributePostDTO } from "~/models/AttributeDTO";

@Component({
  components: {},
})
export default class AttributeTypesCreate extends Vue {
  model: AttributePostDTO = {
    name: "",
    attributeTypeId: 0,
  };

  rules = {
    name: [required(), maxlength(100)],
    attribute: [isSellected()],
  };

  searchKey = "";

  showError = false;
  isLoading = false;

  get availableTypes() {
    return attributeTypesStore.attributeTypes;
  }

  get error() {
    return attributesStore.error;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributesStore.createAttribute(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  @Watch("searchKey")
  onFetchRequired() {
    this.isLoading = true;
    attributeTypesStore
      .getAttributeTypes({
        pageIndex: 0,
        orderReversed: false,
        searchKey: this.searchKey,
      })
      .then((_) => {
        this.isLoading = false;
      });
  }
}
</script>
