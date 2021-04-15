<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить атрибут</span>
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
                v-model="currentType"
                :items="availableTypes"
                :loading="isLoading"
                :search-input.sync="searchKey"
                item-text="name"
                item-value="id"
                label="Тип атрибута"
                placeholder="Начните ввод для поиска"
                prepend-icon="mdi-database-search"
                :rules="rules.attribute"
                return-object
              ></v-autocomplete>
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue darken-1" text @click.stop="onCancel()"
              >Отмена</v-btn
            >
            <v-btn color="blue darken-1" text type="submit">Сохранить</v-btn>
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
import { required } from "~/utils/form-validation";
import { AttributePatchDTO} from "~/models/AttributeDTO";

@Component({
  components: {},
})
export default class AttributeTypesCreate extends Vue {
  id!: number;
  loaded = false;

  model: AttributePatchDTO = {
    id: 0,
    name: "",
    attributeTypeId: 0,
  };

  currentType = {
    id: 0,
    name: ""
  }

  rules = {
    name: [required()],
    attribute: [required()],
  };

  searchKey = "";

  showError = false;
  isLoading = false;

  get attribute() {
    return attributesStore.selectedAttribute;
  }

  get availableTypes() {
    return attributeTypesStore.attributeTypes;
  }

  get error() {
    return attributeTypesStore.error;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributesStore.updateAttribute(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    attributesStore.getAttribute(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        this.model.id = this.attribute?.id!
        this.model.name = this.attribute?.name!
        this.model.attributeTypeId = this.attribute?.typeId!

        this.currentType.id = this.attribute?.typeId!
        this.currentType.name = this.attribute?.type!

        this.loaded = true;
      }
    });
  }

  @Watch("currentType")
  onTypeChanged(val: any){
    this.model.attributeTypeId = val.id
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
