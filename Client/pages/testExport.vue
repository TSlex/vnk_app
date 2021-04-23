<template>
  <div>
    <v-btn @click="onExport"></v-btn>
    <v-divider></v-divider>
    <div ref="pdfPage" id="pdfPage" v-if="fetched">
      <div v-for="(orders, i) in ordersByDate" :key="i">
        <h3 class="day">Дата: {{ i | formatDate }}</h3>
        <table v-for="order in orders" :key="order.id" class="day-order">
          <tr v-if="order.name" class="order-row">
            <td class="align-left">Номер заказа:</td>
            <td class="align-right">{{ order.name }}</td>
          </tr>
          <tr v-if="order.executionDateTime" class="order-row">
            <td class="align-left">Дата заказа:</td>
            <td class="align-right">
              {{ order.executionDateTime | formatDateTime }}
            </td>
          </tr>
          <template v-if="order.executionDateTime">
            <div class="order-row">
              <td class="align-left">Состояние:</td>
              <td class="align-right" v-if="order.completed">Выполнен</td>
              <td class="align-right" v-else-if="order.overdued">Просрочен</td>
              <td class="align-right" v-else>Запланирован</td>
            </div>
          </template>
          <template>
            <tr
              v-for="attribute in order.attributes"
              :key="attribute.id"
              class="order-row"
            >
              <td class="align-left">{{ attribute.name }}:</td>
              <td class="align-right">
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
              </td>
            </tr>
          </template>
          <template v-if="order.notation">
            <tr>
              <td class="align-center">{{ order.notation }}</td>
            </tr>
          </template>
        </table>
        <br>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { generatePdf } from "~/utils/jspdf-russian";
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
    generatePdf(this.$refs.pdfPage).then((pdf) => {
      var string = pdf.output("datauristring");
      var embed = "<embed width='100%' height='100%' src='" + string + "'/>";
      var x = window.open()!;
      x.document.open();
      x.document.write(embed);
      x.document.close();
    });
  }
}
</script>

<style lang="scss" scoped>
@font-face {
  font-family: "Nunito Regular";
  src: url("/fonts/Nunito-Regular.ttf") format(truetype);
}

#pdfPage {
  font-family: "Nunito Regular";
  width: 840px;
  display: flex;
  flex-direction: column;
  margin: auto;
  padding: 20px;
  background: white;

  .day {
    margin-bottom: 10px;
    border-bottom: 1px solid gray;
  }

  .day-order {
    margin: 5px auto;
    margin-bottom: 10px;
    // padding: 5px;
    border: 1px solid grey;
    width: 80%;

    .order-row {
      display: flex;
      justify-content: space-between;
    }
  }
}

table,
tr {
  border: 1px solid gray;
  border-collapse: collapse !important;
}

td {
  border-collapse: collapse !important;
  &.align-left {
    text-align: left;
    width: 40%;
  }

  &.align-right {
    text-align: right;
    width: 100%;
  }

  &.align-center {
    text-align: center;
  }
}
</style>
