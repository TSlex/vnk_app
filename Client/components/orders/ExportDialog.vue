<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title> Экспорт в PDF </v-card-title>
        <v-card-text>
          <v-container>
            <span>Тип заказа</span>
            <v-radio-group v-model.number="ordersType" row class="mt-0">
              <v-radio label="С датой" :value="0"></v-radio>
              <v-radio label="Без даты" :value="1"></v-radio>
            </v-radio-group>
            <span>Промежуток экспорта</span>
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
    <div ref="pdfPage" id="pdfPage">
      <v-container class="white"> ContentHere </v-container>
    </div>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import { SortOption } from "~/models/Enums/SortOption";

import { OrderGetDTO } from "~/models/OrderDTO";

import { jsPDF } from "jspdf";

@Component({
  components: {
    DateTimePicker,
  },
})
export default class ExportDialog extends Vue {
  @Prop()
  value!: boolean;

  @Prop({ default: true })
  hasDate!: boolean;

  ordersType = 0;

  startDatetime = "2021-04-01";
  endDatetime = "2021-04-30";

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

  onClose() {
    this.active = false;
  }

  mounted() {
    this.$uow.orders
      .getAllWithDate(
        0,
        2000,
        SortOption.False,
        undefined,
        undefined,
        undefined,
        (this.startDatetime as any) as Date,
        (this.endDatetime as any) as Date
      )
      .then((orders) => {
        this.orders = orders.data.items;
      });
  }

  onSubmit() {
    // const doc = new jsPDF();
    const doc = new jsPDF("landscape", "px", "A4");
    // doc.html("pdfPage")
    console.log(this.$refs.pdfPage as any);
    doc.html(this.$refs.pdfPage as any).then(() => {
      doc.save("a4.pdf");
    });

    // this.$uow.orders
    //   .getAllWithDate(
    //     0,
    //     1000,
    //     SortOption.False,
    //     undefined,
    //     undefined,
    //     (this.startDatetime as any) as Date,
    //     (this.endDatetime as any) as Date
    //   )
    //   .then((orders) => {
    //     console.log(orders);
    //   });
  }
}
</script>
