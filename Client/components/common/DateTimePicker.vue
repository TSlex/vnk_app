<template>
  <div>
    <v-menu
      ref="picker"
      :close-on-content-click="false"
      :return-value.sync="fieldValue"
      rounded="lg"
      min-width="290px"
      absolute
      z-index="999"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-text-field
          :label="label"
          prepend-icon="mdi-calendar"
          readonly
          v-bind="attrs"
          v-on="on"
          v-model="formatedFieldValue"
        ></v-text-field>
      </template>
      <v-sheet>
        <v-form @submit.prevent="onSubmit()" ref="form">
          <v-tabs fixed-tabs v-model="dateTimeTab" class="mb-2">
            <v-tab>Дата</v-tab>
            <v-tab :disabled="!dateIsCorrect">Время</v-tab>
          </v-tabs>
          <v-tabs-items v-model="dateTimeTab">
            <v-tab-item>
              <v-card flat>
                <v-date-picker
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
                  landscape
                  locale="ru"
                  :first-day-of-week="1"
                  v-model="timeValue"
                ></v-time-picker>
              </v-card>
            </v-tab-item>
          </v-tabs-items>
          <v-input :messages="error" :error="!!error" class="mx-2"></v-input>
          <v-sheet class="d-flex justify-center">
            <v-btn text large color="primary" @click="onClear()">
              Очистить
            </v-btn>
            <v-btn text large color="primary" @click="onClose()">
              Отмена
            </v-btn>
            <v-btn text large color="primary" type="submit">ОК</v-btn>
          </v-sheet>
        </v-form>
      </v-sheet>
    </v-menu>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component({})
export default class DateTimePicker extends Vue {
  error = "";

  @Prop()
  label!: string;

  @Prop()
  value!: string;

  timeValue: null | string = null;
  dateValue: null | string = null;

  dateTimeTab = null;

  get formatedFieldValue(){
    return (this.$options.filters as any).formatDateTime(this.fieldValue)
  }

  get fieldValue() {
    return this.value;
  }

  set fieldValue(value) {
    this.$emit("input", value);
  }

  get dateIsCorrect() {
    return this.dateValue != null && /\d{4}-\d{2}-\d{2}/.test(this.dateValue);
  }

  onSubmit() {
    if (!this.dateIsCorrect) {
      this.error = "Дата должна быть указана";
    } else {
      if (this.timeValue != null) {
        (this.$refs.picker as any).save(this.dateValue + "T" + this.timeValue);
      } else {
        (this.$refs.picker as any).save(this.dateValue);
      }
    }
  }

  onClose() {
    (this.$refs.picker as any).isActive = false;
  }

  onClear() {
    this.dateValue = null;
    this.timeValue = null;
    this.dateTimeTab = null;
    this.error = "";

    (this.$refs.picker as any).save(null);
  }
}
</script>
