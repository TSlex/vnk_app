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
                class="d-flex justify-space-between align-center"
                v-for="(attribute, i) in attributes"
                :key="i"
              >
                <template v-if="!attribute.deleted">
                  <template v-if="attribute.changeMode">
                    <AttributeSellect v-model="attribute.attribute" />
                    <div class="ml-4">
                      <v-btn text outlined @click="onSubmitAttribute(i)"
                        >OK</v-btn
                      >
                      <v-btn text outlined @click="onCancelAttribute(i)">X</v-btn>
                    </div>
                  </template>
                  <template v-else>
                    <v-list-item class="grey lighten-5">
                      <v-list-item-content>
                        <v-list-item-title>{{
                          attribute.attribute.name | textTruncate(50)
                        }}</v-list-item-title>
                        <v-divider class="mb-2 mt-1"></v-divider>
                        <v-list-item-subtitle>
                          <v-chip small class="mr-2">{{
                            attribute.attribute.type | textTruncate(40)
                          }}</v-chip
                          ><v-chip small outlined>{{
                            attribute.attribute.dataType | formatDataType
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
import { maxlength, notEmpty, required } from "~/utils/form-validation";
import { TemplatePatchDTO } from "~/models/TemplateDTO";
import AttributeSellect from "~/components/common/AttributeSellect.vue";
import { AttributeGetDTO } from "~/models/AttributeDTO";
import { DataType } from "~/models/Enums/DataType";
import { templatesStore } from "~/store";
import { PatchOption } from "~/models/Enums/PatchOption";

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
    attributes: [],
  };

  attributes: {
    id: number | null;
    attribute: AttributeGetDTO;
    featured: boolean;
    changeMode: boolean;
    changed: boolean;
    deleted: boolean;
  }[] = [];

  rules = {
    name: [required(), maxlength(100)],
    attributes: [
      maxlength(30),
      (value: any[]) =>
        value.filter((item) => !item.deleted).length > 0 ||
        "В шаблоне должен быть как минимум один атрибут",
    ],
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
      attribute: {
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
    var attribute = this.attributes[index];

    attribute.featured = !this.attributes[index].featured;
    if (attribute.id != null) {
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

  onCancelAttribute(index: number) {
    this.onDeleteAttribute(index);
    this.activeAutoComplete = null;
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

  resolveAttribbutePatchOption(attribute: any) {
    if (attribute.id < 1) {
      return PatchOption.Created;
    }
    if (attribute.deleted) {
      return PatchOption.Deleted;
    }
    if (attribute.changed) {
      return PatchOption.Updated;
    }
    return PatchOption.Unchanged;
  }

  onSubmit() {
    if (
      (this.$refs.form as any).validate() &&
      this.activeAutoComplete == null
    ) {
      this.model.attributes = _.map(this.attributes, (attribute) => {
        return {
          id: attribute.id,
          patchOption: this.resolveAttribbutePatchOption(attribute),
          attributeId: attribute.attribute.id,
          featured: attribute.featured,
        };
      });

      templatesStore.updateTemplate(this.model).then((suceeded) => {
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
            attribute: {
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
