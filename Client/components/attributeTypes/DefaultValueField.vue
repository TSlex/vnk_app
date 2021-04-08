<template>
  <v-input v-if="isBoolean">
    <v-spacer></v-spacer>
    <v-switch :label="switchLabel" v-model="switchValue"></v-switch>
    <v-spacer></v-spacer>
  </v-input>
  <v-text-field
    v-else-if="isString"
    :label="label"
    v-model="stringValue"
  ></v-text-field>
  <v-text-field
    v-else-if="isInteger"
    :label="label"
    v-model="integerValue"
    type="number"
  ></v-text-field>
  <v-text-field
    v-else-if="isFloat"
    :label="label"
    v-model="floatValue"
    type="number"
  ></v-text-field>
  <v-date-picker
    v-else-if="isDate"
    full-width
    locale="ru"
    :first-day-of-week="1"
    v-model="dateValue"
    landscape
  ></v-date-picker>
  <v-time-picker
    format="24hr"
    full-width
    landscape
    v-else-if="isTime"
    locale="ru"
    :first-day-of-week="1"
    v-model="timeValue"
  ></v-time-picker>
  <div v-else-if="isDateTime">
    <v-tabs fixed-tabs v-model="dateTimeTab" class="mb-2">
      <v-tab>Дата</v-tab>
      <v-tab>Время</v-tab>
    </v-tabs>
    <v-tabs-items v-model="dateTimeTab">
      <v-tab-item>
        <v-card flat>
          <v-date-picker
            full-width
            locale="ru"
            :first-day-of-week="1"
            v-model="dateValue"
            landscape
          ></v-date-picker>
        </v-card>
      </v-tab-item>
      <v-tab-item>
        <v-card flat>
          <v-time-picker
            format="24hr"
            full-width
            landscape
            locale="ru"
            :first-day-of-week="1"
            v-model="timeValue"
          ></v-time-picker>
        </v-card>
      </v-tab-item>
    </v-tabs-items>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";
import { DataType } from "~/types/Enums/DataType";

@Component({})
export default class DefaultValueField extends Vue {
  @Prop()
  dataType!: DataType;

  switchValue = false;
  stringValue = "";
  integerValue = 0;
  floatValue = 0.0;
  timeValue = "";
  dateValue = "";

  dateTimeTab = null;

  label = "Значение по умолчанию";

  get switchLabel() {
    let value = this.switchValue ? "Правда" : "Ложь";
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
}
</script>
