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
              <CustomValueField  v-if="useDefaultValues"
                :dataType="model.dataType"
                v-model="model.defaultCustomValue"
                :label="`Значение по умолчанию`"
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
                  v-for="(value, i) in values"
                  :key="'value:' + value.value + i"
                >
                  <template v-if="!value.deleted">
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
                        v-if="value.id != null"
                        @click="featureValue(value.id)"
                        >mdi-star{{
                          model.defaultValueId === value.id ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="editValue(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeValue(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                  <template v-else>
                    <v-spacer></v-spacer>
                    <a @click="undoValueRemove(i)"
                      >значение удалено. отменить?</a
                    >
                    <v-spacer></v-spacer>
                  </template>
                </div>
                <v-input :rules="rules.values" v-model="actualValues"></v-input>
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
                  v-for="(unit, i) in units"
                  :key="'unit:' + unit.value + i"
                >
                  <template v-if="!unit.deleted">
                    <span class="text-body-1">{{ unit.value }}</span>
                    <span>
                      <v-icon
                        v-if="unit.id != null"
                        @click="featureUnit(unit.id)"
                        >mdi-star{{
                          model.defaultUnitId === unit.id ? "" : "-outline"
                        }}</v-icon
                      >
                      <v-icon @click="changeUnit(i)">mdi-lead-pencil</v-icon>
                      <v-icon @click="removeUnit(i)">mdi-delete</v-icon>
                    </span>
                  </template>
                  <template v-else>
                    <v-spacer></v-spacer>
                    <a @click="undoUnitRemove(i)"
                      >единица измерения удалена. отменить?</a
                    >
                    <v-spacer></v-spacer>
                  </template>
                </div>
                <v-input :rules="rules.units" v-model="actualUnits"></v-input>
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
        <v-input
          :rules="rules.defautValue"
          v-model="model.defaultValueId"
        ></v-input>
        <v-input
          :rules="rules.defautUnit"
          v-model="model.defaultUnitId"
        ></v-input>
      </v-form>
      <ValueAddDialog
        v-model="valueDialog"
        :model="value.value"
        :type="model.dataType"
        v-on:submit="submitValue"
        v-on:change="valueChange"
      />
      <UnitAddDialog
        v-model="unitDialog"
        :model="unit.value"
        v-on:submit="submitUnit"
        v-on:change="unitChange"
      />
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
import ValueAddDialog from "~/components/types/ValueAddDialog.vue";
import UnitAddDialog from "~/components/types/UnitAddDialog.vue";

@Component({
  components: {
    CustomValueField,
    ValueAddDialog,
    UnitAddDialog,
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

  units: {
    id: number | null;
    value: string;
    changed: boolean;
    deleted: boolean;
  }[] = [];

  values: {
    id: number | null;
    value: string;
    changed: boolean;
    deleted: boolean;
  }[] = [];

  useDefaultValues = true

  value = { value: "", id: null as number | null, index: 0, changeMode: false };
  unit = { value: "", id: null as number | null, index: 0, changeMode: false };

  showError = false;

  valueDialog = false;
  unitDialog = false;

  id!: number;

  rules = {
    name: [required()],
    type: [required()],
    values: [notEmpty()],
    units: [notEmpty()],
    defautValue: [
      (value: string) =>
        Number(value) >= 0 || "Необходимо указать значение по умолчанию",
    ],
    defautUnit: [
      (value: string) =>
        Number(value) >= 0 ||
        "Необходимо указать единицу измерения по умолчанию",
    ],
  };

  get error() {
    return attributeTypesStore.error;
  }

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

  get actualValues() {
    return this.values.filter((item) => !item.deleted);
  }

  get actualUnits() {
    return this.units.filter((item) => !item.deleted);
  }

  valueChange(value: string) {
    this.value.value = value;
  }

  unitChange(unit: string) {
    this.unit.value = unit;
  }

  featureValue(id: number) {
    this.model.defaultValueId = id;
  }

  submitValue() {
    if (this.value.changeMode) {
      this.values[this.value.index] = {
        id: this.value.id,
        value: this.value.value,
        changed: true,
        deleted: false,
      };
    } else {
      this.values.push({
        id: null,
        value: this.value.value,
        changed: false,
        deleted: false,
      });
    }

    this.value = { value: "", id: null, index: 0, changeMode: false };
    this.valueDialog = false;
  }

  changeValue(index: number) {
    this.value.value = this.values[index].value;
    this.value.id = this.values[index].id;
    this.value.index = index;
    this.value.changeMode = true;
    this.valueDialog = true;
  }

  removeValue(index: number) {
    if (this.values[index].id != null) {
      this.values[index].deleted = true;

      if (this.values[index].id == this.model.defaultValueId) {
        this.model.defaultValueId = -1;
      }
    } else {
      this.values.splice(index, 1);
    }
  }

  undoValueRemove(index: number) {
    this.values[index].deleted = false;
  }

  featureUnit(id: number) {
    this.model.defaultUnitId = id;
  }

  submitUnit() {
    if (this.unit.changeMode) {
      this.units[this.unit.index] = {
        id: this.unit.id,
        value: this.unit.value,
        changed: true,
        deleted: false,
      };
    } else {
      this.units.push({
        id: null,
        value: this.unit.value,
        changed: false,
        deleted: false,
      });
    }

    this.unit = { value: "", id: null, index: 0, changeMode: false };
    this.unitDialog = false;
  }

  changeUnit(index: number) {
    this.unit.value = this.units[index].value;
    this.unit.id = this.units[index].id;
    this.unit.index = index;
    this.unit.changeMode = true;
    this.unitDialog = true;
  }

  removeUnit(index: number) {
    if (this.units[index].id != null) {
      this.units[index].deleted = true;

      if (this.units[index].id == this.model.defaultUnitId) {
        this.model.defaultUnitId = -1;
      }
    } else {
      this.units.splice(index, 1);
    }
  }

  undoUnitRemove(index: number) {
    this.units[index].deleted = false;
  }

  onCancel() {
    this.$router.back();
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore
        .updateAttributeType({
          model: this.model,
          values: this.values,
          units: this.units,
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

    attributeTypesStore.getAttributeType(this.id).then((suceeded) => {
      if (!suceeded || this.attributeType?.systemicType) {
        this.$router.back();
      } else {
        _.merge(this.model, _.pick(this.attributeType, _.keys(this.model)));

        this.attributeType?.values.forEach((value) => {
          this.values.push({
            id: value.id,
            value: value.value,
            changed: false,
            deleted: false,
          });
        });

        this.attributeType?.units.forEach((unit) => {
          this.units.push({
            id: unit.id,
            value: unit.value,
            changed: false,
            deleted: false,
          });
        });

        this.useDefaultValues = !this.attributeType?.usesDefinedValues
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
