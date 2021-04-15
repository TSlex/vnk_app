<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить шаблон</span>
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
              ></v-text-field>
              <v-toolbar flat>
                <v-btn text outlined large @click="onAddAttribute"
                  >Добавить</v-btn
                >
                <v-spacer></v-spacer>
                <v-toolbar-title>Атрибуты</v-toolbar-title>
              </v-toolbar>
              <v-divider></v-divider>
              <template v-if="attributesCount == 0">
                <div class="pt-2" @click="onAddAttribute">
                  <a>Ничего не добавлено</a>
                </div>
              </template>
              <div
                class="d-flex justify-space-between pa-2 align-center"
                v-for="(attribute, i) in attributes"
                :key="i"
              >
                <template v-if="!attribute.deleted">
                  <template v-if="attribute.changeMode">
                    <AttributeSellect v-model="attribute.model" />
                    <div>
                      <v-icon @click="onSubmitAttribute(i)">mdi-check</v-icon>
                    </div>
                  </template>
                  <template v-else>
                    <v-list-item class="grey lighten-5">
                      <v-list-item-content>
                        <v-list-item-title
                          v-text="attribute.model.name"
                        ></v-list-item-title>
                        <v-divider class="mb-2 mt-1"></v-divider>
                        <v-list-item-subtitle>
                          <v-chip
                            small
                            v-text="attribute.model.type"
                            class="mr-2"
                          ></v-chip
                          ><v-chip small outlined>{{
                            attribute.model.dataType | formatDataType
                          }}</v-chip>
                        </v-list-item-subtitle>
                      </v-list-item-content>
                      <v-list-item-icon>
                        <v-icon @click="onFeatureAttribute(i)"
                          >mdi-star{{
                            attribute.featured ? "" : "-outline"
                          }}</v-icon
                        >
                        <v-icon @click="onEditAttribute(i)"
                          >mdi-lead-pencil</v-icon
                        >
                        <v-icon @click="onDeleteAttribute(i)"
                          >mdi-delete</v-icon
                        >
                      </v-list-item-icon>
                    </v-list-item>
                  </template>
                </template>
                <template v-else>
                  <v-spacer></v-spacer>
                  <a @click="onUndoDeleteAttribute(i)"
                    >атрибут удален. отменить?</a
                  >
                  <v-spacer></v-spacer>
                </template>
              </div>
              <v-input :rules="rules.attributes" v-model="attributes"></v-input>
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
import { Component, Vue } from "nuxt-property-decorator";
import { notEmpty, required } from "~/utils/form-validation";
import { TemplatePatchDTO } from "~/models/TemplateDTO";
import AttributeSellect from "~/components/common/AttributeSellect.vue";
import { AttributeGetDTO } from "~/models/AttributeDTO";
import { DataType } from "~/models/Enums/DataType";
import { templatesStore } from "~/store";

@Component({
  components: {
    AttributeSellect,
  },
})
export default class TemplatesEdit extends Vue {
  loaded = false;

  model: TemplatePatchDTO = {
    id: 0,
    name: "",
  };

  attributes: {
    id: number | null;
    model: AttributeGetDTO;
    featured: boolean;
    changeMode: boolean;
    changed: boolean;
    deleted: boolean;
  }[] = [];

  rules = {
    name: [required()],
    attributes: [notEmpty()],
  };

  id!: number;

  value = { value: "", index: 0, changeMode: false };

  showError = false;

  activeAutoComplete: number | null = null;

  valueDialog = false;

  get template() {
    return templatesStore.selectedTemplate;
  }

  get error() {
    return templatesStore.error;
  }

  get attributesCount() {
    return this.attributes?.length ?? 0;
  }

  onAddAttribute() {
    if (this.activeAutoComplete != null) {
      if ((this.$refs.form as any).validate()) {
        this.attributes[this.activeAutoComplete].changeMode = false;
        this.activeAutoComplete;
      } else {
        return;
      }
    }
    this.attributes.push({
      id: null,
      model: {
        id: 0,
        name: "",
        type: "",
        typeId: 0,
        dataType: DataType.Undefined,
        usesDefinedValues: false,
        usesDefinedUnits: false,
      },
      featured: false,
      changeMode: true,
      changed: false,
      deleted: false,
    });

    this.activeAutoComplete = this.attributes.length - 1;
  }

  onFeatureAttribute(index: number) {
    this.attributes[index].featured = !this.attributes[index].featured;
    if (this.attributes[index].id != null) {
      this.attributes[index].changed = true;
    }
  }

  onSubmitAttribute(index: number) {
    if ((this.$refs.form as any).validate()) {
      this.attributes[index].changeMode = false;

      if (this.attributes[index].id != null) {
        this.attributes[index].changed = true;
      }

      this.activeAutoComplete = null;
    }
  }

  onEditAttribute(index: number) {
    if (this.activeAutoComplete != null) {
      if ((this.$refs.form as any).validate()) {
        this.attributes[this.activeAutoComplete].changeMode = false;
        this.activeAutoComplete;
      } else {
        return;
      }
    }

    this.attributes[index].changeMode = true;
    this.activeAutoComplete = index;
  }

  onDeleteAttribute(index: number) {
    if (this.attributes[index].id != null) {
      this.attributes[index].deleted = true;
    } else {
      this.attributes.splice(index, 1);
    }
  }

  onUndoDeleteAttribute(index: number) {
    this.attributes[index].deleted = false;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      templatesStore
        .updateTemplate({
          model: this.model,
          attributes: _.map(this.attributes, (attribute) => {
            return {
              id: attribute.id,
              attributeId: attribute.model.id,
              featured: attribute.featured,
              changed: attribute.changed,
              deleted: attribute.deleted,
            };
          }),
        })
        .then((suceeded) => {
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

    templatesStore.getTemplate(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        _.merge(this.model, _.pick(this.template, _.keys(this.model)));

        this.template?.attributes.forEach((attribute) => {
          this.attributes.push({
            id: attribute.id,
            model: {
              id: attribute.attributeId,
              name: attribute.name,
              type: attribute.type,
              typeId: attribute.typeId,
              dataType: attribute.dataType,
              usesDefinedValues: false,
              usesDefinedUnits: false,
            },
            changed: false,
            deleted: false,
            featured: attribute.featured,
            changeMode: false,
          });
        });

        this.loaded = true;
      }
    });
  }
}
</script>
