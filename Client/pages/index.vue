<template>
  <v-row dense class="py-4 px-6 fill-height">
    <v-col>
      <Calendar :weekdays="weekdays" />
    </v-col>
    <v-col cols="3">
      <v-sheet height="700" rounded="lg">
        <!-- If sellected -->
        <template v-if="order">
          <v-list-item
            v-for="attribute in order.attributes"
            :key="attribute.name"
          >
            <v-list-item-content>
              <v-list-item-title v-text="attribute.name"></v-list-item-title>
              <v-list-item-subtitle>
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
              </v-list-item-subtitle>
            </v-list-item-content>
          </v-list-item>
          <v-divider></v-divider>
          <v-container class="d-flex justify-space-between">
            <v-btn outlined text large @click="onCheck(order.id)">Отметить</v-btn>
            <v-btn outlined text large @click="onDetails(order.id)">Подробнее</v-btn>
            <v-btn outlined text large @click="onEdit(order.id)">Изменить</v-btn>
          </v-container>
        </template>
        <!-- If not sellected -->
        <template v-else>
          <v-container
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
import { OrderAttributeGetDTO } from "~/models/OrderDTO";
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
