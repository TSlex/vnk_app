<template>
  <div>
    <v-menu
      ref="picker"
      :close-on-content-click="false"
      :return-value.sync="fieldValue"
      rounded="lg"
      min-width="290px"
      absolute
      :content-class="forceCentered ? 'modal-center' : ''"
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
          :rules="rules"
        ></v-text-field>
      </template>
      <v-sheet>
        <v-form @submit.prevent="onSubmit()" ref="form">
          <v-tabs fixed-tabs v-model="dateTimeTab" class="mb-2">
            <v-tab v-if="hasDate || !hasTime">Дата</v-tab>
            <v-tab :disabled="!timeTabEnabled" v-if="hasTime">Время</v-tab>
          </v-tabs>
          <v-tabs-items v-model="dateTimeTab">
            <v-tab-item v-if="hasDate || !hasTime">
              <v-card flat>
                <v-date-picker
                  locale="ru"
                  :first-day-of-week="1"
                  v-model="dateValue"
                  landscape
                  :allowed-dates="allowedDates"
                ></v-date-picker>
              </v-card>
            </v-tab-item>
            <v-tab-item v-if="hasTime">
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
  @Prop({})
  rules: any;

  @Prop({ default: () => [0, 1, 2, 3, 4, 5, 6] })
  allowedDays!: number[];

  @Prop({ default: true })
  hasDate!: boolean;

  @Prop({ default: true })
  hasTime!: boolean;

  error = "";

  @Prop()
  label!: string;

  @Prop()
  value!: string;

  @Prop({default: false})
  forceCentered!: boolean;

  timeValue: null | string = null;
  dateValue: null | string = null;

  dateTimeTab = null;

  get timeTabEnabled() {
    return (this.dateIsCorrect && this.hasTime) || !this.hasDate;
  }

  get formatedFieldValue() {
    if (this.hasDate && this.hasTime) {
      return (this.$options.filters as any).formatDateTime(this.fieldValue);
    } else if (this.hasTime) {
      return this.fieldValue;
    }
    return (this.$options.filters as any).formatDate(this.fieldValue);
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

  get timeIsCorrect() {
    return this.timeValue != null && /\d{2}:\d{2}/.test(this.timeValue);
  }

  allowedDates(val: any) {
    return _.includes(this.allowedDays, this.$moment(val).day());
  }

  onSubmit() {
    let timeValid = !(this.hasTime && !this.timeIsCorrect);
    let dateValid = !(this.hasDate && !this.dateIsCorrect);

    if (!timeValid && !dateValid) {
      this.error = "Дата и время должны быть указаны";
      return;
    } else if (timeValid && !dateValid) {
      this.error = "Дата должна быть указана";
      return;
    } else if (!timeValid && dateValid) {
      this.error = "Время должно быть указано";
      return;
    } else {
      if (this.hasDate && this.hasTime) {
        (this.$refs.picker as any).save(this.dateValue + "T" + this.timeValue);
      } else if (!this.hasDate && this.hasTime) {
        (this.$refs.picker as any).save(this.timeValue);
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

  mounted() {
      this.timeValue = "12:00";
  }
}
</script>
