<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title> Экспорт в PDF </v-card-title>
        <v-card-text>
          <v-container>
            <span class="text-body-1">Тип заказов</span>
            <v-radio-group v-model.number="ordersType" row class="mt-0">
              <v-radio label="С датой" :value="0"></v-radio>
              <v-radio label="Без даты" :value="1"></v-radio>
            </v-radio-group>
            <span class="text-body-1">Состояние</span>
            <v-slider
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
            <br />
            <span class="text-body-1">Промежуток экспорта</span>
            <DateTimePicker
              :label="'Начальная дата'"
              v-model="startDatetime"
              :forceCentered="true"
            />
            <DateTimePicker
              :label="'Конечная дата'"
              v-model="endDatetime"
              :forceCentered="true"
            />
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit">Экспортировать</v-btn>
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-form>
    <!-- pdf content -->
    <div style="display: none">
      <PdfPage
        :ordersByDate="ordersByDate"
        v-if="fetched"
        v-on:rendered="generatePdf"
        ref="pdfPage"
      />
    </div>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import PdfPage from "~/components/orders/PdfPage.vue";
import { SortOption } from "~/models/Enums/SortOption";
import { OrderGetDTO } from "~/models/OrderDTO";
import { generatePdf } from "~/utils/jspdf-russian";

@Component({
  components: {
    DateTimePicker,
    PdfPage,
  },
})
export default class ExportDialog extends Vue {
  fetched = false;

  @Prop()
  value!: boolean;

  @Prop({ default: true })
  hasDate!: boolean;

  ordersType = 0;

  _completed?: boolean;
  _overdued?: boolean;

  startDatetime: Date | null = this.$moment().toDate();
  endDatetime: Date | null = this.$moment().add(1, "weeks").toDate();

  orders: OrderGetDTO[] = [];

  get ordersByDate() {
    return _.groupBy(
      _.orderBy(this.orders, ["executionDateTime"], ["asc"]),
      (order) => this.$moment(order.executionDateTime).startOf("day")
    );
  }

  get filename() {
    return new Date() + "_" + this.startDatetime + ":" + this.endDatetime;
  }

  get active() {
    return this.value;
  }

  set active(value) {
    this.$emit("input", value);
  }

  get completed() {
    return this._completed == undefined ? 0 : this._completed ? 2 : 1;
  }

  set completed(value: number) {
    switch (value) {
      case 1:
        this._completed = false;
        break;
      case 2:
        this._completed = true;
        break;
      default:
        this._completed = undefined;
    }
  }

  get overdued() {
    return this._overdued == undefined ? 0 : this._overdued ? 2 : 1;
  }

  set overdued(value: number) {
    switch (value) {
      case 1:
        this._overdued = false;
        break;
      case 2:
        this._overdued = true;
        break;
      default:
        this._overdued = undefined;
    }
  }

  onClose() {
    this.active = false;
  }

  async generatePdf() {
    // @ts-ignore
    let pdf = await generatePdf(this.$refs.pdfPage.$refs.pdfContent);

    var string = pdf.output("datauristring");
    var embed = "<embed width='100%' height='100%' src='" + string + "'/>";

    var x = window.open()!;

    x.document.open();
    x.document.write(embed);
    x.document.close();
  }

  async onSubmit() {
    this.fetched = false;
    if ((this.$refs.form as any).validate()) {
      let orders = await this.$uow.orders.getAllWithDate(
        0,
        2000,
        SortOption.False,
        this._completed,
        this._overdued,
        undefined,
        this.startDatetime as any,
        this.endDatetime as any
      );

      this.orders = orders.data.items;
      this.fetched = true;
    }
  }
}
</script>
