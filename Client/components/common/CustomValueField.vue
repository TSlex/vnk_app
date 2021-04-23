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
    <DateTimePicker
      v-model="fieldValue"
      :label="label"
      :hasTime="false"
      :rules="rules.date"
    />
  </div>
  <div v-else-if="isTime">
    <DateTimePicker
      v-model="fieldValue"
      :label="label"
      :hasDate="false"
      :rules="rules.time"
    />
  </div>
  <div v-else-if="isDateTime">
    <DateTimePicker
      v-model="fieldValue"
      :label="label"
      :rules="rules.dateTime"
    />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from "nuxt-property-decorator";
import { DataType } from "~/models/Enums/DataType";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import { required, integer, float } from "~/utils/form-validation";

@Component({
  components: {
    DateTimePicker,
  },
})
export default class CustomValueField extends Vue {
  @Prop()
  dataType!: DataType;

  @Prop()
  value!: string;

  dateTimeTab = null;

  rules = {
    string: [required()],
    date: [required()],
    time: [required()],
    dateTime: [required()],
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
    if (value == null) {
      this.$emit("input", "");
    } else {
      this.$emit("input", String(value));
    }
  }

  get switchLabel() {
    let value = this.fieldValue ? "Да" : "Нет";
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

    if (newType === DataType.Integer) {
      newValue = "0";
    } else if (newType === DataType.Float) {
      newValue = "0.00";
    } else if (newType === DataType.Boolean) {
      newValue = "false";
    }

    this.$emit("input", newValue);
  }
}
</script>
