<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>Настроить фильтрацию</v-card-title>
        <v-card-text>
          <v-container>
            <v-input label="Фильтровать по выполнению"> </v-input>
            <v-slider
              :tick-labels="['Все', 'Не выполненные', 'Выполненные']"
              :max="2"
              step="1"
              tick-size="4"
            ></v-slider>
            <br>
            <v-input label="Фильтровать по дате"> </v-input>
            <DateTimePicker :label="'Начальная дата'"/>
            <br>
            <DateTimePicker :label="'Конечная дата'"/>
            <br>
            <v-input label="Указать дату выполнения"> </v-input>
            <DateTimePicker :label="'Дата выполнения'"/>
            <br>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit"
            >Применить фильтр</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import DateTimePicker from "~/components/common/DateTimePicker.vue"

@Component({
  components: {
    DateTimePicker
  }
})
export default class FilterDialog extends Vue {
  @Prop()
  value!: boolean;

  @Prop()
  filter!: {
    startDatetime?: Date;
    endDatetime?: Date;
    checkDatetime?: Date;
    completed?: boolean;
  };

  model: {
    startDatetime?: Date;
    endDatetime?: Date;
    checkDatetime?: Date;
    completed?: boolean;
  } = {};

  get active() {
    return this.value;
  }

  set active(value) {
    this.$emit("input", value);
  }

  onClose() {
    this.active = false;
  }

  mounted() {
    this.model = { ...this.filter };
  }

  onSubmit() {
    this.filter = { ...this.model };
  }
}
</script>
