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
              <v-text-field label="Название" required></v-text-field>
              <v-select
                label="Тип данных"
                required
                :items="dataTypes"
                v-model="model.dataType"
              ></v-select>
              <CustomValueField
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
                v-if="!model.usesDefinedValues"
              />
              <v-switch
                label="Значения определены"
                inset
                v-model="model.usesDefinedValues"
              ></v-switch>
              <template v-if="model.usesDefinedValues">
                <v-toolbar flat>
                  <v-btn text @click="valueDialog = true">Добавить</v-btn>
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Допустимые значения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="valuesCount > 0">
                  <div class="d-flex justify-space-between pa-2">
                    <span>{{ model.values[model.defaultValueIndex] }}</span
                    ><v-chip small>по-умолчанию</v-chip>
                  </div>
                </template>
                <template v-else>
                  <div class="pt-2">Ничего не добавлено</div>
                </template>
                <v-divider v-if="valuesCount > 1"></v-divider>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="(v, i) in notFeaturedValues"
                  :key="i"
                >
                  <span>{{ v }}</span>
                </div>
              </template>
              <v-switch
                label="Единицы определены"
                inset
                v-model="model.usesDefinedUnits"
              ></v-switch>
              <template v-if="model.usesDefinedUnits">
                <v-toolbar flat>
                  <v-btn text @click="unitDialog = true">Добавить</v-btn>
                  <v-spacer></v-spacer>
                  <v-toolbar-title>Единицы измерения</v-toolbar-title>
                </v-toolbar>
                <v-divider></v-divider>
                <template v-if="unitsCount > 0">
                <div class="d-flex justify-space-between pa-2">
                  <span>{{ model.units[model.defaultUnitIndex] }}</span
                  ><v-chip small>по-умолчанию</v-chip>
                </div>
                </template>
                <template v-else>
                  <div class="pt-2">Ничего не добавлено</div>
                </template>
                <v-divider v-if="unitsCount > 1"></v-divider>
                <div
                  class="d-flex justify-space-between pa-2"
                  v-for="(u, i) in notFeaturedUnits"
                  :key="i"
                >
                  <span>{{ u }}</span>
                </div>
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
        :model="value"
        :type="model.dataType"
        v-on:submit="addValue"
        v-on:change="valueChange"
      />
      <UnitAddDialog
        v-model="unitDialog"
        :model="unit"
        v-on:submit="addUnit"
        v-on:change="unitChange"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { attributeTypesStore } from "~/store";
import { DataType } from "~/types/Enums/DataType";
import { localize } from "~/utils/localizeDataType";
import CustomValueField from "~/components/common/CustomValueField.vue";
import { AttributeTypePostDTO } from "~/types/AttributeTypeDTO";
import ValueAddDialog from "~/components/types/ValueAddDialog.vue";
import UnitAddDialog from "~/components/types/UnitAddDialog.vue";

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
    usesDefinedValues: true,
    usesDefinedUnits: true,
    defaultValueIndex: 0,
    defaultUnitIndex: 0,
    values: [],
    units: [],
  };

  value = "";
  unit = "";

  showError = false;

  valueDialog = false;
  unitDialog = false;

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

  get notFeaturedValues() {
    return this.model.values.filter((value: string, index: number) => {
      return index != this.model.defaultValueIndex;
    });
  }

  get notFeaturedUnits() {
    return this.model.units.filter((unit: string, index: number) => {
      return index != this.model.defaultUnitIndex;
    });
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    // this.$router.back()
    if ((this.$refs.form as any).validate()) {
      console.log(this.model.defaultCustomValue);
    }
  }

  valueChange(value: string) {
    this.value = value;
  }

  unitChange(unit: string) {
    this.unit = unit;
  }

  addValue() {
    this.model.values.push(this.value);
    this.value = "";
    this.valueDialog = false;
  }

  changeValue(index: number) {
    this.value = this.model.values[index];
    this.removeValue(index);
    this.valueDialog = true;
  }

  removeValue(index: number) {
    this.model.values.splice(index, 1);
  }

  addUnit() {
    this.model.units.push(this.unit);
    this.unit = "";
    this.unitDialog = false;
  }

  changeUnit(index: number) {
    this.unit = this.model.units[index];
    this.removeUnit(index);
    this.unitDialog = true;
  }

  removeUnit(index: number) {
    this.model.units.splice(index, 1);
  }
}
</script>
