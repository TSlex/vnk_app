<template>
  <v-input v-if="isBoolean">
    <v-spacer></v-spacer>
    <v-switch :label="switchLabel" v-model="fieldValue"></v-switch>
    <v-spacer></v-spacer>
  </v-input>
  <v-text-field
    v-else-if="isString"
    :label="label"
    v-model="fieldValue"
    :rules="rules.string"
  ></v-text-field>
  <v-text-field
    v-else-if="isInteger"
    :label="label"
    v-model="fieldValue"
    type="number"
    :rules="rules.integer"
  ></v-text-field>
  <v-text-field
    v-else-if="isFloat"
    :label="label"
    v-model="fieldValue"
    type="number"
    step=".01"
    :rules="rules.float"
  ></v-text-field>
  <div v-else-if="isDate">
  <v-input
    v-model="fieldValue"
    :messages="label"
  >
    <v-date-picker
      full-width
      locale="ru"
      :first-day-of-week="1"
      v-model="fieldValue"
      landscape
      :label="label"
    ></v-date-picker>
  </v-input>
  <v-input :rules="rules.dateTime" v-model="fieldValue"></v-input>
  </div>
  <div v-else-if="isTime">
    <v-input v-model="fieldValue" :messages="label">
      <v-time-picker
        format="24hr"
        full-width
        landscape
        locale="ru"
        :first-day-of-week="1"
        v-model="fieldValue"
      ></v-time-picker>
    </v-input>
    <v-input :rules="rules.dateTime" v-model="fieldValue"></v-input>
  </div>
  <div v-else-if="isDateTime">
    <v-tabs fixed-tabs v-model="dateTimeTab" class="mb-2">
      <v-tab>Дата</v-tab>
      <v-tab>Время</v-tab>
    </v-tabs>
    <v-tabs-items v-model="dateTimeTab">
      <v-tab-item>
        <v-card flat>
          <v-input :messages="label">
            <v-date-picker
              full-width
              locale="ru"
              :first-day-of-week="1"
              v-model="dateValue"
              landscape
            ></v-date-picker>
          </v-input>
        </v-card>
      </v-tab-item>
      <v-tab-item>
        <v-card flat>
          <v-input :messages="label">
            <v-time-picker
              format="24hr"
              full-width
              landscape
              locale="ru"
              :first-day-of-week="1"
              v-model="timeValue"
            ></v-time-picker>
          </v-input>
        </v-card>
      </v-tab-item>
    </v-tabs-items>
    <v-input :rules="rules.dateTime" v-model="fieldValue"></v-input>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "nuxt-property-decorator";
import { DataType } from "~/types/Enums/DataType";
import {
  required,
  integer,
  float,
  dateTimeRequired,
} from "~/utils/form-validation";

@Component({})
export default class CustomValueField extends Vue {
  @Prop()
  dataType!: DataType;

  @Prop()
  value = "";

  timeValue = "";
  dateValue = "";

  dateTimeTab = null;

  rules = {
    string: [required()],
    date: [required()],
    time: [required()],
    dateTime: [dateTimeRequired()],
    integer: [required(), integer()],
    float: [required(), float()],
  };

  @Prop()
  label!: string;

  get fieldValue() {
    switch (this.dataType) {
      case DataType.Boolean:
        return this.value === "true" ? true : false;
      default:
        return this.value;
    }
  }

  set fieldValue(value: any) {
    let newValue = value;

    if (this.dataType === DataType.DateTime) {
      newValue = this.dateValue + "T" + this.timeValue;
    }

    this.$emit("input", String(newValue));
  }

  get switchLabel() {
    let value = this.fieldValue ? "Правда" : "Ложь";
    return `${this.label}: ${value}`;
  }

  get isBoolean() {
    return this.dataType === DataType.Boolean;
  }

  get isString() {
    return this.dataType === DataType.String;
  }

  get isInteger() {
    return this.dataType === DataType.Integer;
  }

  get isFloat() {
    return this.dataType === DataType.Float;
  }

  get isDate() {
    return this.dataType === DataType.Date;
  }

  get isTime() {
    return this.dataType === DataType.Time;
  }

  get isDateTime() {
    return this.dataType === DataType.DateTime;
  }

  @Watch("dataType")
  onDataTypeChanged(newType: DataType) {
    let newValue = "";
    this.timeValue = "";
    this.dateValue = "";

    if (newType === DataType.Integer) {
      newValue = "0";
    } else if (newType === DataType.Float) {
      newValue = "0.00";
    } else if (newType === DataType.Boolean) {
      newValue = "false";
    }

    this.$emit("input", newValue);
  }

  @Watch("timeValue")
  onDateTimeTimeChanged() {
    this.fieldValue = "";
  }

  @Watch("dateValue")
  onDateTimeDateChanged() {
    this.fieldValue = "";
  }
}
</script>
