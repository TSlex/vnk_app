<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>Настроить фильтрацию</v-card-title>
        <v-card-text>
          <v-container>
            <span class="text-body-1">Фильтровать по выполнению</span>
            <v-slider v-if="hasDeadline"
              :tick-labels="['Все', 'Будущие', 'Прошедшие']"
              :max="2"
              step="1"
              tick-size="4"
              v-model.number="overdued"
            ></v-slider>
            <v-slider
              :tick-labels="['Все', 'Не выполненные', 'Выполненные']"
              :max="2"
              step="1"
              tick-size="4"
              v-model.number="completed"
            ></v-slider>
            <template v-if="hasDeadline">
              <br />
              <span class="text-body-1">Фильтровать по дате</span>
              <DateTimePicker
                :label="'Начальная дата'"
                v-model="model.startDatetime"
                :forceCentered="true"
              />
              <DateTimePicker
                :label="'Конечная дата'"
                v-model="model.endDatetime"
                :forceCentered="true"
              />
              <br />
              <span class="text-body-1">Указать дату проверки</span>
              <DateTimePicker
                :label="'Дата проверки'"
                v-model="model.checkDatetime"
                :forceCentered="true"
              />
            </template>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClear()"
            >Очистить</v-btn
          >
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit"
            >Применить фильтр</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import DateTimePicker from "~/components/common/DateTimePicker.vue";

@Component({
  components: {
    DateTimePicker,
  },
})
export default class FilterDialog extends Vue {
  @Prop()
  value!: boolean;

  @Prop({ default: true })
  hasDeadline!: boolean;

  @Prop()
  filter!: {
    startDatetime?: Date;
    endDatetime?: Date;
    checkDatetime?: Date;
    completed?: boolean;
    overdued?: boolean;
  };

  model: {
    startDatetime?: Date;
    endDatetime?: Date;
    checkDatetime?: Date;
    completed?: boolean;
    overdued?: boolean;
  } = {};

  get active() {
    return this.value;
  }

  set active(value) {
    this.$emit("input", value);
  }

  get completed() {
    return this.model.completed == undefined ? 0 : this.model.completed ? 2 : 1;
  }

  set completed(value: number) {
    switch (value) {
      case 1:
        this.model.completed = false;
        break;
      case 2:
        this.model.completed = true;
        break;
      default:
        this.model.completed = undefined;
    }
  }

  get overdued() {
    return this.model.overdued == undefined ? 0 : this.model.overdued ? 2 : 1;
  }

  set overdued(value: number) {
    switch (value) {
      case 1:
        this.model.overdued = false;
        break;
      case 2:
        this.model.overdued = true;
        break;
      default:
        this.model.overdued = undefined;
    }
  }

  onClose() {
    this.active = false;
  }

  onClear() {
    this.model = {
      startDatetime: undefined,
      endDatetime: undefined,
      completed: undefined,
      checkDatetime: undefined,
    };
  }

  onSubmit() {
    this.$emit("update:filter", { ...this.model });
    this.onClose();
  }

  mounted() {
    this.model = { ...this.filter };
  }
}
</script>
