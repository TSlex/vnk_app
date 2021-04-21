<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title> Экспорт в PDF </v-card-title>
        <v-card-text>
          <v-container>
            <span>Тип заказа</span>
            <v-radio-group v-model.number="ordersType" row>
              <v-radio label="С датой" :value="0"></v-radio>
              <v-radio label="Без даты" :value="1"></v-radio>
            </v-radio-group>
            <span>Промежуток экспорта</span>
            <DateTimePicker :label="'Начальная дата'" />
            <DateTimePicker :label="'Конечная дата'" />
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit">Экспортировать</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <template>
      <div>
        <vue-html2pdf
          :show-layout="false"
          :float-layout="true"
          :enable-download="true"
          :preview-modal="false"
          :paginate-elements-by-height="1400"
          :manual-pagination="false"
          :filename="filename"
          :pdf-quality="2"
          pdf-format="a4"
          pdf-orientation="portrait"
          pdf-content-width="800px"
          ref="pdf"
        >
          <section slot="pdf-content">
            <!-- PDF Content Here -->
            123
            <v-container>
              <div v-for="(orders, i) in ordersByDate" :key="i">
                <h1 class="text-h4">Заказы за {{ i | formatDate }}</h1>
                <hr>
                <v-card v-for="order in orders" :key="order.id">
                  {{ order.id }}
                </v-card>
              </div>
            </v-container>
          </section>
        </vue-html2pdf>
      </div>
    </template></v-dialog
  >
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import DateTimePicker from "~/components/common/DateTimePicker.vue";
import { SortOption } from "~/models/Enums/SortOption";

import { OrderGetDTO } from "~/models/OrderDTO";

@Component({
  components: {
    DateTimePicker,
  },
})
export default class ExportDialog extends Vue {
  @Prop()
  value!: boolean;

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
        1000,
        SortOption.False,
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
    (this.$refs.pdf as any).generatePdf();
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
