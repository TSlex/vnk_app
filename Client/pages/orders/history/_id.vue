<template>
  <v-row justify="center" class="text-center">
    <v-col cols="8" class="mt-4">
      <template v-if="isMounted">
        <v-toolbar flat class="rounded-t-lg">
          <v-toolbar-title>
            История заказа "{{ order.name | textTruncate(50) }}"
          </v-toolbar-title>
        </v-toolbar>
        <v-data-table
          :loading="!fetched"
          :items="orders"
          :headers="headers"
          :items-per-page="itemsOnPage"
          hide-default-footer
          show-expand
          class="rounded-b-lg rounded-t-0"
        >
          <template v-slot:[`item.name`]="{ item }">
            {{ item.name | textTruncate(50) }}
          </template>
          <template v-slot:[`item.date`]="{ item }">
            {{ item.executionDateTime | formatDateTime }}
          </template>
          <template v-slot:[`item.createdAt`]="{ item }">
            {{ item.createdAt | formatDateTimeUTC }}
          </template>
          <template v-slot:[`item.changedAt`]="{ item }">
            {{ item.changedAt | formatDateTimeUTC }}
          </template>
          <template v-slot:[`item.changedBy`]="{ item }">
            {{ item.changedBy }}
          </template>
          <template v-slot:[`item.status`]="{ item }">
            <v-chip v-if="item.completed" color="success"> Выполнен </v-chip>
            <v-chip v-else-if="item.overdued" color="error"> Просрочен </v-chip>
            <v-chip v-else color="primary"> Запланирован </v-chip>
          </template>
          <template v-slot:expanded-item="{ item }">
            <td :colspan="isDateOrder ? 7 : 6">
              <v-container class="px-10">
                <v-container>
                  <v-expansion-panels accordion multiple hover flat>
                    <v-expansion-panel
                      v-for="attribute in item.attributes"
                      :key="attribute.id"
                    >
                      <v-expansion-panel-header
                        hide-actions
                        :class="
                          'pa-0 rounded-lg ' +
                          getHistoryStatusClass(item.id, attribute)
                        "
                      >
                        <div class="d-flex justify-space-between">
                          <span class="d-flex align-center">
                            <v-icon
                              class="text-body-1"
                              v-if="attribute.featured"
                              >mdi-star</v-icon
                            >
                            <v-icon class="text-body-1" v-else
                              >mdi-star-outline</v-icon
                            >
                            <span class="ml-1 text-body-1"
                              >{{ attribute.name | textTruncate(50) }}:</span
                            >
                          </span>
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
                            <template v-else>{{
                              attribute.value | textTruncate(40)
                            }}</template>
                            <template v-if="attribute.usesDefinedUnits">{{
                              attribute.unit | textTruncate(30)
                            }}</template>
                          </span>
                        </div>
                      </v-expansion-panel-header>
                      <v-expansion-panel-content
                        class="expansion-panel-content_no_wrap mt-1 rounded-lg"
                      >
                        <v-container class="grey lighten-3">
                          <div
                            class="d-flex justify-space-between mb-2"
                            v-if="item.name"
                          >
                            <span class="text-body-2">Атрибут:</span>
                            <v-chip
                              small
                              @click.stop="
                                onNavigateToAttribute(attribute.attributeId)
                              "
                              >{{ attribute.name }}</v-chip
                            >
                          </div>
                          <div
                            class="d-flex justify-space-between mb-2"
                            v-if="item.name"
                          >
                            <span class="text-body-2">Тип атрибута:</span>
                            <v-chip
                              small
                              @click.stop="onNavigateToType(attribute.typeId)"
                              >{{ attribute.type }}</v-chip
                            >
                          </div>
                          <div
                            class="d-flex justify-space-between mb-2"
                            v-if="item.name"
                          >
                            <span class="text-body-2">Формат:</span>
                            <v-chip small>{{
                              attribute.dataType | formatDataType
                            }}</v-chip>
                          </div>
                          <div
                            class="d-flex justify-space-between mb-2"
                            v-if="item.name"
                          >
                            <span class="text-body-2"
                              >Значения определены:</span
                            >
                            <v-chip small>{{
                              attribute.usesDefinedValues | formatBoolean
                            }}</v-chip>
                          </div>
                          <div
                            class="d-flex justify-space-between"
                            v-if="item.name"
                          >
                            <span class="text-body-2"
                              >Ед. измерения определены:</span
                            >
                            <v-chip small>{{
                              attribute.usesDefinedUnits | formatBoolean
                            }}</v-chip>
                          </div>
                        </v-container>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </v-container>
                <template v-if="item.notation">
                  <v-divider class="mt-n2"></v-divider>
                  <v-container class="d-flex justify-center my-3">
                    <span
                      :class="
                        'text-break' +
                        ` ${getNotationClass(item.id, item.notation)}`
                      "
                      >{{ item.notation }}</span
                    >
                  </v-container>
                </template>
              </v-container>
            </td>
          </template>
        </v-data-table>
        <v-pagination
          v-model="currentPage"
          :length="pagesCount"
          :total-visible="12"
          class="mt-2"
        ></v-pagination>
      </template>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "nuxt-property-decorator";
import { DataType } from "~/models/Enums/DataType";
import { OrderAttributeGetDTO, OrderHistoryDTO } from "~/models/OrderDTO";
import { sortedIndex } from "~/node_modules/@types/lodash";
import { ordersStore } from "~/store";

@Component({})
export default class OrderHistory extends Vue {
  id!: number;
  fetched = false;
  isMounted = false;
  currentPage = 1;

  dateTimeHeader = {
    text: "Дата исполнения",
    value: "date",
    align: "center",
    sortable: false,
  };

  headers = [
    { text: "Номер заказа", value: "name", align: "left", sortable: false },
    { text: "Создан", value: "createdAt", align: "center", sortable: false },
    { text: "Изменен", value: "changedAt", align: "center", sortable: false },
    { text: "Инициатор", value: "changedBy", align: "center", sortable: false },
    { text: "Статус", value: "status", align: "right", sortable: false },
  ];

  get currentPageIndex() {
    return (this.currentPage ?? 1) - 1;
  }

  get order() {
    return ordersStore.selectedOrder;
  }

  get orders() {
    return ordersStore.histryRecords;
  }

  get pagesCount() {
    return ordersStore.historyPagesCount;
  }

  get itemsOnPage() {
    return ordersStore.itemsOnPage;
  }

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  get isDateOrder() {
    return this.order?.executionDateTime != null;
  }


  getNotationClass(orderId: Number, notation: string) {
    let orderIndex = this.orders.findIndex((order: any) => order.id == orderId);

    let previousOrder =
      this.orders.length > orderIndex + 1 ? this.orders[orderIndex + 1] : null;

    if (previousOrder != null && previousOrder.notation != notation) {
      return "primary--text";
    } else {
      return "";
    }
  }

  getHistoryStatusClass(orderId: Number, attribute: OrderAttributeGetDTO) {
    if (attribute.wasDeleted) {
      return "text-decoration-line-through error--text";
    }

    let orderIndex = this.orders.findIndex((order: any) => order.id == orderId);

    let previousOrder =
      this.orders.length > orderIndex + 1 ? this.orders[orderIndex + 1] : null;

    if (previousOrder != null) {
      let prevAtt = this.getOrderAttribute(previousOrder, attribute);

      if (
        prevAtt != null &&
        (prevAtt.value != attribute.value || prevAtt.unit != attribute.unit)
      ) {
        return "primary--text";
      } else if (prevAtt == null) {
        return "success--text";
      }
    }

    return "";
  }

  getOrderAttribute(order: OrderHistoryDTO, attribute: OrderAttributeGetDTO) {
    return order.attributes.find(
      (a) =>
        a.id == attribute.id ||
        (a.masterId != null && a.masterId == attribute.masterId) ||
        a.id == attribute.masterId ||
        (a.masterId != null && a.masterId == attribute.id)
    );
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

  onNavigateToType(typeId: number) {
    this.$router.push(`/types/${typeId}`);
  }

  onNavigateToAttribute(attributeId: number) {
    this.$router.push(`/attributes/${attributeId}`);
  }

  @Watch("currentPage")
  fetchHistory() {
    this.fetched = false;
    ordersStore
      .getOrderHistory({ id: this.id, pageIndex: this.currentPageIndex })
      .then((suceeded) => {
        if (!suceeded) {
          this.$router.back();
        } else {
          this.fetched = true;
        }
      });
  }

  async mounted() {
    if (!this.id) {
      this.$router.back();
      return;
    }

    let order = await ordersStore.getOrder({
      id: this.id,
      checkDatetime: null,
    });

    ordersStore.context.commit("ORDER_HISTORY_CLEARED");

    if (!order) {
      this.$router.back();
      return;
    }

    if (this.isDateOrder) {
      this.headers.splice(1, 0, this.dateTimeHeader);
    }

    this.fetchHistory();

    this.isMounted = true;
  }
}
</script>
