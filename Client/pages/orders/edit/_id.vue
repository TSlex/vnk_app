<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="6" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Создать заказ</span>
          </v-card-title>
          <v-card-text class="pb-0">
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <!-- Name field -->
              <v-text-field
                label="Название"
                required
                :rules="rules.name"
                v-model="model.name"
              ></v-text-field>
              <!-- Deadline -->
              <DateTimePicker
                :label="'Дата исполнения'"
                v-model="model.executionDateTime"
              />
              <!-- Attributes section -->
              <v-toolbar flat class="toolbar-no-padding">
                <v-toolbar-title>Атрибуты заказа</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-btn text outlined large @click="onAddAttribute"
                  >Добавить</v-btn
                >
              </v-toolbar>
              <v-divider></v-divider>
              <template>
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
                      <AttributeSellect v-model="attribute.attribute" />
                      <div>
                        <v-icon @click="onSubmitAttribute(i)">mdi-check</v-icon>
                      </div>
                    </template>
                    <template v-else>
                      <v-list-item class="grey lighten-5">
                        <v-list-item-content>
                          <v-list-item-title
                            v-text="attribute.attribute.name"
                          ></v-list-item-title>
                          <v-divider class="mb-2 mt-1"></v-divider>
                          <v-list-item-subtitle>
                            <v-chip
                              small
                              v-text="attribute.attribute.type"
                              class="mr-2"
                            ></v-chip
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
                        <AttributeValueSellect
                          v-model="attribute.value"
                          :typeId="attribute.attribute.typeId"
                        />
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
                <v-input
                  :rules="rules.attributes"
                  v-model="attributes"
                ></v-input>
              </template>
              <!-- Notation field -->
              <v-textarea
                label="Примечание"
                v-model="model.notation"
                rows="1"
              ></v-textarea>
              <!-- Completion switch -->
              <template>
                <v-switch
                  :label="completedLabel"
                  v-model="model.completed"
                  inset
                  color="success"
                ></v-switch>
              </template>
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
import { ordersStore } from "~/store";
import { notEmpty, required } from "~/utils/form-validation";
import { OrderPatchDTO } from "~/models/OrderDTO";
import AttributeSellect from "~/components/common/AttributeSellect.vue";
import AttributeValueSellect from "~/components/common/AttributeValueSellect.vue";
import { AttributeGetDTO } from "~/models/AttributeDTO";
import { DataType } from "~/models/Enums/DataType";
import { PatchOption } from "~/models/Enums/PatchOption";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import TemplateSellect from "~/components/orders/TemplateSellect.vue";

@Component({
  components: {
    AttributeSellect,
    AttributeValueSellect,
    DateTimePicker,
    TemplateSellect,
  },
})
export default class OrderEdit extends Vue {
  id!: number;
  loaded = false;

  model: OrderPatchDTO = {
    id: 0,
    completed: false,
    notation: "",
    executionDateTime: null,
    name: "",
    attributes: [],
  };

  attributes: {
    id: number | null;
    attribute: AttributeGetDTO;
    value: {
      customValue: string | null;
      valueId: number | null;
      unitId: number | null;
    };
    changed: boolean;
    deleted: boolean;
    featured: boolean;
    changeMode: boolean;
  }[] = [];

  rules = {
    name: [required()],
    attributes: [(value: any[]) => value.filter((item) => !item.deleted).length > 0 || "В заказе должен быть как минимум один аттрибут"],
  };

  value = { value: "", index: 0, changeMode: false };

  showError = false;

  activeAutoComplete: number | null = null;

  valueDialog = false;

  get completedLabel() {
    return "Заказ выполнен? - " + (this.model.completed ? "Да" : "Нет");
  }

  get order() {
    return ordersStore.selectedOrder;
  }

  get error() {
    return ordersStore.error;
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
      id: 0,
      attribute: {
        id: 0,
        name: "",
        type: "",
        typeId: 0,
        dataType: DataType.Undefined,
        usesDefinedValues: false,
        usesDefinedUnits: false,
      },
      value: {
        customValue: "",
        valueId: null,
        unitId: null,
      },
      changed: false,
      deleted: false,
      featured: false,
      changeMode: true,
    });

    this.activeAutoComplete = this.attributes.length - 1;
  }

  onFeatureAttribute(index: number) {
    var attribute = this.attributes[index];
    attribute.featured = !this.attributes[index].featured;

    if (attribute.id != null) {
      attribute.changed = true;
    }
  }

  onSubmitAttribute(index: number) {
    var attribute = this.attributes[index];

    if ((this.$refs.form as any).validate()) {
      attribute.changeMode = false;

      if (attribute.id != null) {
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
    if ((this.$refs.form as any).validate()) {
      this.model.attributes = _.map(this.attributes, (attribute) => {
        return {
          id: attribute.id,
          patchOption: this.resolveAttribbutePatchOption(attribute),
          attributeId: attribute.attribute.id,
          featured: attribute.featured,
          customValue: attribute.value.customValue,
          valueId: attribute.value.valueId,
          unitId: attribute.value.unitId,
        };
      });

      ordersStore.updateOrder(this.model).then((suceeded) => {
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

    ordersStore
      .getOrder({ id: this.id, checkDatetime: null })
      .then((suceeded) => {
        if (!suceeded) {
          this.$router.back();
        } else {
          _.merge(this.model, _.pick(this.order, _.keys(this.model)));

          this.order?.attributes.forEach((attribute) => {
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
              value: {
                customValue: attribute.usesDefinedValues ? "" : attribute.value,
                valueId: attribute.valueId,
                unitId: attribute.unitId,
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
