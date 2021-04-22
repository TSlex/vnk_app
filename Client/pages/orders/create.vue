<template>
  <v-row justify="center" class="text-center">
    <v-col cols="6" class="my-4">
      <TemplateSellect v-on:apply="onTemplateApply" />
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
            <v-btn color="blue darken-1" text type="submit">Создать</v-btn>
            <v-spacer></v-spacer>
          </v-card-actions>
        </v-card>
      </v-form>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore, ordersStore } from "~/store";
import { notEmpty, required } from "~/utils/form-validation";
import { OrderPostDTO } from "~/models/OrderDTO";
import AttributeSellect from "~/components/common/AttributeSellect.vue";
import AttributeValueSellect from "~/components/common/AttributeValueSellect.vue";
import { AttributeGetDTO } from "~/models/AttributeDTO";
import { DataType } from "~/models/Enums/DataType";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import TemplateSellect from "~/components/orders/TemplateSellect.vue";
import { TemplateGetDTO } from "~/models/TemplateDTO";

@Component({
  components: {
    AttributeSellect,
    AttributeValueSellect,
    DateTimePicker,
    TemplateSellect,
  },
})
export default class OrderCreate extends Vue {
  model: OrderPostDTO = {
    completed: false,
    notation: "",
    executionDateTime: null,
    name: "Новый заказ",
    attributes: [],
  };

  attributes: {
    attribute: AttributeGetDTO;
    value: {
      customValue: string | null;
      valueId: number | null;
      unitId: number | null;
    };
    featured: boolean;
    changeMode: boolean;
  }[] = [];

  rules = {
    name: [required()],
    attributes: [notEmpty()],
  };

  value = { value: "", index: 0, changeMode: false };

  showError = false;

  activeAutoComplete: number | null = null;

  valueDialog = false;

  get completedLabel() {
    return "Уже выполнен? - " + (this.model.completed ? "Да" : "Нет");
  }

  get error() {
    return ordersStore.error;
  }

  get attributesCount() {
    return this.attributes?.length ?? 0;
  }

  onTemplateApply(template: TemplateGetDTO) {
    this.attributes = [];

    template.attributes.forEach((attribute) => {
      this.attributes.push({
        attribute: {
          id: attribute.attributeId,
          name: attribute.name,
          type: attribute.type,
          typeId: attribute.typeId,
          dataType: attribute.dataType,
          usesDefinedValues: attribute.usesDefinedValues,
          usesDefinedUnits: attribute.usesDefinedUnits,
        },
        value: {
          customValue: "",
          valueId: null,
          unitId: null,
        },
        featured: attribute.featured,
        changeMode: false,
      });
    });
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
      featured: false,
      changeMode: true,
    });

    this.activeAutoComplete = this.attributes.length - 1;
  }

  onFeatureAttribute(index: number) {
    this.attributes[index].featured = !this.attributes[index].featured;
  }

  onSubmitAttribute(index: number) {
    if ((this.$refs.form as any).validate()) {
      this.attributes[index].changeMode = false;
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
    this.attributes.splice(index, 1);
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      this.model.attributes = _.map(this.attributes, (attribute) => {
        return {
          attributeId: attribute.attribute.id,
          featured: attribute.featured,
          customValue: attribute.value.customValue,
          valueId: attribute.value.valueId,
          unitId: attribute.value.unitId,
        };
      });
      // console.log(this.model);

      ordersStore.createOrder(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }
}
</script>
