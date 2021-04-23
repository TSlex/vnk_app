<template>
  <div>
    <v-btn @click="onExport"></v-btn>
    <v-divider></v-divider>
    <div ref="pdfPage" id="pdfPage" v-if="fetched">
      <v-container class="white">
        <v-container v-for="(orders, i) in ordersByDate" :key="i">
          {{ i | formatDate }}
          <v-container v-for="order in orders" :key="order.id">
            <div class="d-flex justify-space-between mb-2" v-if="order.name">
              <span class="text-body-1">Название:</span>
              <span class="text-body-1">{{ order.name }}</span>
            </div>
            <div class="d-flex justify-space-between mb-2">
              <span class="text-body-1">Дата заказа:</span>
              <span class="text-body-1">{{
                order.executionDateTime | formatDateTime
              }}</span>
            </div>
            <div class="d-flex justify-space-between mb-2">
              <span class="text-body-1">Состояние:</span>
              <v-chip small v-if="order.completed" color="success">
                Выполнен
              </v-chip>
              <v-chip small v-else-if="order.overdued" color="error">
                Просрочен
              </v-chip>
              <v-chip small v-else color="primary"> Запланирован </v-chip>
            </div>
          </v-container>
          <v-divider></v-divider>
          <!-- <v-container>
            <div
              class="d-flex justify-space-between mb-2"
              v-for="attribute in order.attributes"
              :key="attribute.id"
            >
              <span class="text-body-1">{{ attribute.name }}:</span>
              <span class="text-body-1">
                <template v-if="isBooleanType(attribute)">{{
                  attribute.value | formatBoolean
                }}</template>
                <template v-else-if="isDateType(attribute)">{{
                  attribute.value | formatDate
                }}</template>
                <template v-else-if="isDateTimeType(attribute)">{{
                  attribute.value | formatDateTime
                }}</template>
                <template v-else>{{ attribute.value }}</template>
                <template v-if="attribute.usesDefinedUnits">{{
                  attribute.unit
                }}</template>
              </span>
            </div>
          </v-container> -->
          <!-- <v-divider class="mt-n2"></v-divider>
          <template v-if="order.notation">
            <v-container class="d-flex justify-center my-3">
              <span>{{ order.notation }}</span>
            </v-container>
            <v-divider></v-divider>
          </template> -->
        </v-container>
      </v-container>
    </div>
  </div>
</template>

<script lang="ts">
import jsPDF from "~/utils/jspdf-russian";
// import { jsPDF } from "jspdf";
import { Component, Vue } from "nuxt-property-decorator";
import { DataType } from "~/models/Enums/DataType";
import { SortOption } from "~/models/Enums/SortOption";
import { OrderAttributeGetDTO, OrderGetDTO } from "~/models/OrderDTO";

@Component({})
export default class blah extends Vue {
  fetched = false;
  orders: OrderGetDTO[] = [];

  startDatetime = "2021-04-01";
  endDatetime = "2021-04-30";

  get ordersByDate() {
    return _.groupBy(
      _.orderBy(this.orders, ["executionDateTime"], ["asc"]),
      (order) => this.$moment(order.executionDateTime).startOf("day")
    );
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
        this.fetched = true;
      });
  }

  isBooleanType(attribute: OrderAttributeGetDTO) {
    return attribute.dataType === DataType.Boolean;
  }

  isDateType(attribute: OrderAttributeGetDTO) {
    return attribute.dataType === DataType.Date;
  }

  isDateTimeType(attribute: OrderAttributeGetDTO) {
    return attribute.dataType === DataType.DateTime;
  }

  onExport() {
    var doc = jsPDF;

    // const doc = new jsPDF();
    // const doc = new jsPDF("landscape", "px", "A4");
    // // doc.addFont("/fonts/Nunito-Regular.ttf", "Nunito", "normal");
    // doc.setFont("Nunito-Regular")
    // doc.setFontSize(10);
    // doc.text("Привет", 10, 10)
    // // doc.html("pdfPage")
    // console.log(this.$refs.pdfPage as any);
    doc.html(this.$refs.pdfPage as any).then(() => {
      doc.save("a4.pdf");
    });
  }
}
</script>

<style lang="scss" scoped>
@font-face {
    font-family: 'Nunito Regular';
    src: url("/fonts/Nunito-Regular.ttf") format(truetype);
}

#pdfPage{
  font-family: 'Nunito Regular';
}
</style>
