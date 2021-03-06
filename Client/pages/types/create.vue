<template>
  <v-row justify="center" class="text-center">
    <v-col cols="4" class="my-4">
      <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
        <v-card>
          <v-card-title>
            <span class="headline">Создать тип атрибута</span>
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
              <v-input
                :rules="rules.defaultValue"
                v-model="model.defaultCustomValue"
                v-if="!model.usesDefinedValues"
              >
                <CustomValueField
                  :dataType="model.dataType"
                  v-model="model.defaultCustomValue"
                  :label="`Значение по умолчанию`"
                />
              </v-input>
              <v-switch
                label="Значения определены"
                inset
                v-model="model.usesDefinedValues"
                v-if="definedValuesPossible"
              ></v-switch>
              <template v-if="model.usesDefinedValues && definedValuesPossible">
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
                  v-for="(v, i) in model.values"
                  :key="'value' + v + i"
                >
                  <template>
                    <span v-if="isDateFormat" class="text-body-1">{{
                      v | formatDate
                    }}</span>
                    <span v-else-if="isDateTimeFormat" class="text-body-1">{{
                      v | formatDateTime
                    }}</span>
                    <span v-else class="text-body-1">{{ v  | textTruncate(50)}}</span>
                    <span>
                      <v-icon @click="featureValue(i)"
                        >mdi-star{{
                          model.defaultValueIndex === i ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="changeValue(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeValue(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.values" v-model="model.values"></v-input>
              </template>
              <v-switch
                label="Единицы определены"
                inset
                v-model="model.usesDefinedUnits"
              ></v-switch>
              <template v-if="model.usesDefinedUnits">
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
                  v-for="(u, i) in model.units"
                  :key="'unit' + u + i"
                >
                  <template>
                    <span class="text-body-1">{{ u | textTruncate(50)}}</span>
                    <span>
                      <v-icon @click="featureUnit(i)"
                        >mdi-star{{
                          model.defaultUnitIndex === i ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="changeUnit(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeUnit(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                </div>
                <v-input :rules="rules.units" v-model="model.units"></v-input>
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
      <ValueAddDialog
        v-model="valueDialog"
        :model="value.value"
        :type="model.dataType"
        v-on:submit="addValue"
        v-on:change="valueChange"
      />
      <UnitAddDialog
        v-model="unitDialog"
        :model="unit.value"
        v-on:submit="addUnit"
        v-on:change="unitChange"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/models/Enums/DataType";
import { localize } from "~/utils/localizeDataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePostDTO } from "~/models/AttributeTypeDTO";
import ValueAddDialog from "~/components/types/ValueAddDialog.vue";
import UnitAddDialog from "~/components/types/UnitAddDialog.vue";
import { maxlength, notEmpty, required } from "~/utils/form-validation";

@Component({
  components: {
    CustomValueField,
    ValueAddDialog,
    UnitAddDialog,
  },
})
export default class AttributeTypesCreate extends Vue {
  model: AttributeTypePostDTO = {
    name: "",
    defaultCustomValue: "",
    dataType: DataType.String,
    usesDefinedValues: false,
    usesDefinedUnits: false,
    defaultValueIndex: 0,
    defaultUnitIndex: 0,
    values: [],
    units: [],
  };

  rules = {
    name: [required(), maxlength(100)],
    defaultValue: [required(), maxlength(200)],
    type: [required()],
    values: [notEmpty(), maxlength(100)],
    units: [notEmpty(), maxlength(100)],
  };

  value = { value: "", index: 0, changeMode: false };
  unit = { value: "", index: 0, changeMode: false };

  showError = false;

  valueDialog = false;
  unitDialog = false;

  get definedValuesPossible() {
    return (
      this.model.dataType != DataType.Undefined &&
      this.model.dataType != DataType.Boolean
    );
  }

  get error() {
    return attributeTypesStore.error;
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
    return this.model.values.length;
  }

  get unitsCount() {
    return this.model.units.length;
  }

  get isDateFormat() {
    return this.model.dataType === DataType.Date;
  }

  get isDateTimeFormat() {
    return this.model.dataType === DataType.DateTime;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.createAttributeType(this.model).then((suceeded) => {
        if (suceeded) {
          this.onCancel();
        } else {
          this.showError = true;
        }
      });
    }
  }

  valueChange(value: string) {
    this.value.value = value;
  }

  unitChange(unit: string) {
    this.unit.value = unit;
  }

  featureValue(index: number) {
    this.model.defaultValueIndex = index;
  }

  addValue() {
    if (this.value.changeMode) {
      this.model.values[this.value.index] = this.value.value;
    } else {
      this.model.values.push(this.value.value);
    }

    this.value = { value: "", index: 0, changeMode: false };
    this.valueDialog = false;
  }

  changeValue(index: number) {
    this.value.value = this.model.values[index];
    this.value.index = index;
    this.value.changeMode = true;
    this.valueDialog = true;
  }

  removeValue(index: number) {
    this.model.values.splice(index, 1);
  }

  featureUnit(index: number) {
    this.model.defaultUnitIndex = index;
  }

  addUnit() {
    if (this.unit.changeMode) {
      this.model.units[this.unit.index] = this.unit.value;
    } else {
      this.model.units.push(this.unit.value);
    }

    this.unit = { value: "", index: 0, changeMode: false };
    this.unitDialog = false;
  }

  changeUnit(index: number) {
    this.unit.value = this.model.units[index];
    this.unit.index = index;
    this.unit.changeMode = true;
    this.unitDialog = true;
  }

  removeUnit(index: number) {
    this.model.units.splice(index, 1);
  }

  @Watch("model.dataType")
  onDataTypeChanged(newType: DataType, oldType: DataType) {
    if (newType != oldType) {
      (this.$refs.form as any).resetValidation();
      this.model.defaultCustomValue = "";
      this.model.values = [];
    }
  }
}
</script>
