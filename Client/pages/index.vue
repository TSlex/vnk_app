<template>
  <v-row dense class="py-4 px-6 fill-height page">
    <v-col>
      <Calendar :weekdays="weekdays" />
    </v-col>
    <v-col cols="3">
      <v-sheet rounded="lg">
        <!-- If sellected -->
        <template v-if="order">
          <v-container>
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
              <v-chip small v-if="order.completed" color="success"> Выполнен </v-chip>
              <v-chip small v-else-if="order.overdued" color="error">
                Просрочен
              </v-chip>
              <v-chip small v-else color="primary"> Запланирован </v-chip>
            </div>
          </v-container>
          <v-divider></v-divider>
          <v-container>
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
          </v-container>
          <v-divider class="mt-n2"></v-divider>
          <template v-if="order.notation">
            <v-container class="d-flex justify-center my-3">
              <span>{{ order.notation }}</span>
            </v-container>
            <v-divider></v-divider>
          </template>
          <v-container class="d-flex justify-space-between">
            <v-btn outlined text large @click="onCheck(order)">Отметить</v-btn>
            <v-btn outlined text large @click="onDetails(order.id)"
              >Подробнее</v-btn
            >
            <v-btn outlined text large @click="onEdit(order.id)"
              >Изменить</v-btn
            >
          </v-container>
        </template>
        <!-- If not sellected -->
        <template v-else>
          <v-container class="d-flex justify-center"
            ><span class="text-body-1">Ничего не выбрано</span></v-container
          >
        </template>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import Calendar from "@/components/Calendar.vue";
import { ordersStore } from "~/store";

import { Component, Prop, Vue } from "nuxt-property-decorator";
import { OrderAttributeGetDTO, OrderGetDTO } from "~/models/OrderDTO";
import { DataType } from "~/models/Enums/DataType";

@Component({
  components: {
    Calendar,
  },
})
export default class IndexPage extends Vue {
  weekdays = [1, 2, 3, 4, 5];

  get order() {
    return ordersStore.selectedOrder;
  }

  onCheck(order: OrderGetDTO) {
    ordersStore.updateOrderCompletion({
      id: order.id,
      completed: !order.completed,
    });
  }

  onEdit(orderId: number) {
    this.$router.push(`orders/edit/${orderId}`);
  }

  onDetails(orderId: number) {
    this.$router.push(`orders/${orderId}`);
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
}
</script>
