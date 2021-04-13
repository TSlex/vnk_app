<template>
  <v-row justify="center" class="text-center" v-if="loaded">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Изменить тип атрибута</span>
          </v-card-title>
          <v-card-text>
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
              <v-select
                label="Тип данных"
                required
                :items="dataTypes"
                v-model="model.dataType"
                :rules="rules.type"
              ></v-select>
              <v-alert
                dense
                outlined
                type="warning"
                v-if="initialDataType != model.dataType"
              >
                Внимание, при смене типа данных, возможно неправильное
                отображение!
              </v-alert>
              <CustomValueField
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
                v-if="!model.usesDefinedValues"
              />
              <template v-if="attributeType.usesDefinedValues">
                <v-toolbar flat>
                  <v-btn text outlined large @click="valueDialog = true"
                    >Добавить</v-btn
                  >
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Допустимые значения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="valuesCount == 0">
                  <div class="pt-2" @click="valueDialog = true">
                    <a>Ничего не добавлено</a>
                  </div>
                </template>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="value in values"
                  :key="'value:' + value.id"
                >
                  <template>
                    <span v-if="isDateFormat" class="text-body-1">{{
                      value.value | formatDate
                    }}</span>
                    <span v-else-if="isDateTimeFormat" class="text-body-1">{{
                      value.value | formatDateTime
                    }}</span>
                    <span v-else-if="isBooleanFormat" class="text-body-1">{{
                      value.value | formatBoolean
                    }}</span>
                    <span v-else class="text-body-1">{{ value.value }}</span>
                    <span>
                      <v-icon
                        >mdi-star{{
                          model.defaultValueId === value.id ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon>mdi-lead-pencil</v-icon>
                      <v-icon>mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.values" v-model="values"></v-input>
              </template>
              <template v-if="attributeType.usesDefinedUnits">
                <v-toolbar flat>
                  <v-btn text outlined large @click="unitDialog = true"
                    >Добавить</v-btn
                  >
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Единицы измерения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="unitsCount == 0">
                  <div class="pt-2" @click="unitDialog = true">
                    <a>Ничего не добавлено</a>
                  </div>
                </template>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="unit in units"
                  :key="'unit:' + unit.id"
                >
                  <template>
                    <span class="text-body-1">{{ unit.value }}</span>
                    <span>
                      <v-icon
                        >mdi-star{{
                          model.defaultUnitId === unit.id ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon>mdi-lead-pencil</v-icon>
                      <v-icon>mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.units" v-model="units"></v-input>
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
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/models/Enums/DataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePatchDTO } from "~/models/AttributeTypeDTO";
import { notEmpty, required } from "~/utils/form-validation";
import { localize } from "~/utils/localizeDataType";

@Component({
  components: {
    CustomValueField,
  },
})
export default class AttributeTypesEdit extends Vue {
  loaded = false;

  initialDataType!: DataType;

  model: AttributeTypePatchDTO = {
    id: 0,
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    defaultValueId: 0,
    defaultUnitId: 0,
  };

  units: { id: number; value: string }[] = [];
  values: { id: number; value: string }[] = [];

  showError = false;

  id!: number;

  rules = {
    name: [required()],
    type: [required()],
    values: [notEmpty()],
    units: [notEmpty()],
  };

  get isBooleanFormat() {
    return this.attributeType!.dataType === DataType.Boolean;
  }

  get isDateFormat() {
    return this.attributeType!.dataType === DataType.Date;
  }

  get isDateTimeFormat() {
    return this.attributeType!.dataType === DataType.DateTime;
  }

  get attributeType() {
    return attributeTypesStore.selectedAttributeType;
  }

  get dataTypes() {
    let types: any = [];

    Object.values(DataType).forEach((element) => {
      let key = Number(element);
      if (!isNaN(Number(element)) && key >= 0) {
        types.push({
          text: localize(element as DataType),
          value: element as DataType,
        });
      }
    });

    return types;
  }

  get valuesCount() {
    return this.attributeType?.values?.length ?? 0;
  }

  get unitsCount() {
    return this.attributeType?.units?.length ?? 0;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.updateAttributeType(this.model).then((suceeded) => {
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

    attributeTypesStore.getAttributeType(this.id).then((suceeded) => {
      if (!suceeded) {
        this.$router.back();
      } else {
        _.merge(this.model, _.pick(this.attributeType, _.keys(this.model)));

        this.values = this.attributeType?.values ?? []
        this.units = this.attributeType?.units ?? []

        this.initialDataType = this.attributeType?.dataType!;

        this.loaded = true;
      }
    });
  }

  @Watch("model.dataType")
  onDataTypeChanged(newType: DataType, oldType: DataType) {
    if (newType != oldType && this.$refs.form) {
      (this.$refs.form as any).resetValidation();
    }
  }
}
</script>
