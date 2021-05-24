<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="5" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить заказ</span>
          </v-card-title>
          <v-card-text class="pb-0">
            <v-container>
              <v-alert dense text type="error" v-if="showError">
                {{ error }}
              </v-alert>
              <!-- Name field -->
              <v-text-field
                label="Номер заказа"
                required
                :rules="rules.name"
                v-model="model.name"
              ></v-text-field>
              <!-- Deadline -->
              <DateTimePicker
                :label="'Дата исполнения'"
                v-model="model.executionDateTime"
                :allowedDays="[1, 2, 3, 4, 5]"
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
                  class="d-flex justify-space-between pt-2 align-center"
                  v-for="(attribute, i) in attributes"
                  :key="attribute.attribute.id"
                >
                  <template v-if="!attribute.deleted">
                    <template v-if="attribute.changeMode">
                      <AttributeSellect v-model="attribute.attribute" />
                      <div class="ml-4">
                        <v-btn text outlined @click="onSubmitAttribute(i)"
                          >OK</v-btn
                        >
                      </div>
                    </template>
                    <template v-else>
                      <AttributeValueSellect
                        v-model="attribute.value"
                        :typeId="attribute.attribute.typeId"
                        :label="attribute.attribute.name"
                      />
                      <span>
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
                      </span>
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
                :rules="rules.notation"
                rows="1"
              ></v-textarea>
              <!-- Completion switch -->
              <template>
                <div class="d-flex justify-center">
                  <v-switch
                    :label="completedLabel"
                    v-model="model.completed"
                    inset
                    color="success"
                  ></v-switch>
                </div>
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
import { maxlength, notEmpty, required } from "~/utils/form-validation";
import { OrderAttributeGetDTO, OrderPatchDTO } from "~/models/OrderDTO";
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
    initValue: {
      customValue: string | null;
      valueId: number | null;
      unitId: number | null;
    };
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
    name: [required(), maxlength(100)],
    attributes: [
      maxlength(30),
      (value: any[]) =>
        value.filter((item) => !item.deleted).length > 0 ||
        "В заказе должен быть как минимум один атрибут",
    ],
    notation: [maxlength(1000)],
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
      initValue: {
        customValue: "",
        valueId: null,
        unitId: null,
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
        this.activeAutoComplete = null;
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
        if (
          attribute.id != null &&
          (attribute.initValue.customValue != attribute.value.customValue ||
            attribute.initValue.valueId != attribute.value.valueId ||
            attribute.initValue.unitId != attribute.value.unitId)
        ) {
          attribute.changed = true;
        }
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
                usesDefinedValues: attribute.usesDefinedValues,
                usesDefinedUnits: attribute.usesDefinedUnits,
              },
              initValue: {
                customValue: attribute.usesDefinedValues ? "" : attribute.value,
                valueId: attribute.valueId,
                unitId: attribute.unitId,
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
