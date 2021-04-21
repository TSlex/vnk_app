<template>
  <div>
    <v-menu
      ref="picker"
      :close-on-content-click="false"
      :return-value.sync="dateValue"
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
        >{{dateValue | formatDateTime}}</v-text-field>
      </template>

      <v-sheet>
        <v-tabs fixed-tabs v-model="dateTimeTab" class="mb-2">
          <v-tab>Дата</v-tab>
          <v-tab>Время</v-tab>
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
        <v-sheet class="d-flex justify-center">
          <v-btn
            text
            large
            color="primary"
            @click="$refs.picker.isActive = false"
          >
            Отмена
          </v-btn>
          <v-btn
            text
            large
            color="primary"
            @click="$refs.picker.save(dateValue)"
          >
            ОК
          </v-btn>
        </v-sheet>
      </v-sheet>
    </v-menu>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component({})
export default class DateTimePicker extends Vue {
  @Prop()
  label!: string;

  @Prop()
  value!: string;

  timeValue = "";
  dateValue = "";

  dateTimeTab = null;

  get formatedValue() {
    return this.fieldValue;
  }

  get fieldValue() {
    return this.value;
  }

  set fieldValue(value: any) {
    let newValue = value;

    if (this.dataType === DataType.DateTime) {
      newValue = this.dateValue + "T" + this.timeValue;
    }

    this.$emit("input", String(newValue));
  }
}
</script>
