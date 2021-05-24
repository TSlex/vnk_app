<template>
  <v-row justify="center" class="text-center" v-if="loaded && order != null">
    <v-col cols="5" class="mt-4">
      <v-sheet rounded="lg" class="py-2">
        <v-container>
          <div class="d-flex justify-space-between mb-2" v-if="order.name">
            <span class="text-body-1"> <span></span>Номер заказа:</span>
            <span class="text-body-1">{{ order.name | textTruncate(50)}}</span>
          </div>
          <div class="d-flex justify-space-between mb-2" v-if="order.executionDateTime">
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
        <v-container>
          <v-expansion-panels accordion multiple hover flat>
            <v-expansion-panel
              v-for="attribute in order.attributes"
              :key="attribute.id"
            >
              <v-expansion-panel-header hide-actions class="pa-0 rounded-lg">
                <div class="d-flex justify-space-between">
                  <span class="d-flex align-center">
                    <v-icon class="text-body-1" v-if="attribute.featured">mdi-star</v-icon>
                    <v-icon class="text-body-1" v-else>mdi-star-outline</v-icon>
                    <span class="ml-1 text-body-1">{{ attribute.name | textTruncate(30)}}:</span>
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
                    <template v-else>{{ attribute.value| textTruncate(30) }}</template>
                    <template v-if="attribute.usesDefinedUnits">{{
                      attribute.unit| textTruncate(20)
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
                    v-if="order.name"
                  >
                    <span class="text-body-2">Атрибут:</span>
                    <v-chip
                      small
                      @click.stop="onNavigateToAttribute(attribute.attributeId)"
                      >{{ attribute.name | textTruncate(50)}}</v-chip
                    >
                  </div>
                  <div
                    class="d-flex justify-space-between mb-2"
                    v-if="order.name"
                  >
                    <span class="text-body-2">Тип атрибута:</span>
                    <v-chip
                      small
                      @click.stop="onNavigateToType(attribute.typeId)"
                      >{{ attribute.type | textTruncate(50)}}</v-chip
                    >
                  </div>
                  <div
                    class="d-flex justify-space-between mb-2"
                    v-if="order.name"
                  >
                    <span class="text-body-2">Формат:</span>
                    <v-chip small>{{
                      attribute.dataType | formatDataType
                    }}</v-chip>
                  </div>
                  <div
                    class="d-flex justify-space-between mb-2"
                    v-if="order.name"
                  >
                    <span class="text-body-2">Значения определены:</span>
                    <v-chip small>{{
                      attribute.usesDefinedValues | formatBoolean
                    }}</v-chip>
                  </div>
                  <div
                    class="d-flex justify-space-between"
                    v-if="order.name"
                  >
                    <span class="text-body-2">Ед. измерения определены:</span>
                    <v-chip small>{{
                      attribute.usesDefinedUnits | formatBoolean
                    }}</v-chip>
                  </div>
                </v-container>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-container>
        <v-divider class="mt-n2"></v-divider>
        <template v-if="order.notation">
          <v-container class="d-flex justify-center my-3">
            <span class="text-break">{{order.notation}}</span>
          </v-container>
          <v-divider></v-divider>
        </template>
        <v-container class="d-flex justify-center">
          <v-btn outlined text large @click="onEdit" class="mr-2"
            >Изменить</v-btn
          >
          <v-btn outlined text large @click="onDelete" class="mr-2"
            >Удалить</v-btn
          >
          <v-btn outlined text large @click="onHistory">История</v-btn>
        </v-container>
      </v-sheet>
      <OrderDeleteDialog v-model="deleteDialog" v-if="deleteDialog" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Vue } from "nuxt-property-decorator";
import { DataType } from "~/models/Enums/DataType";
import { OrderAttributeGetDTO } from "~/models/OrderDTO";
import { ordersStore } from "~/store";
import OrderDeleteDialog from "~/components/orders/OrderDeleteDialog.vue";

@Component({
  components: {
    OrderDeleteDialog,
  },
})
export default class OrderDetails extends Vue {
  id!: number;
  loaded = false;

  deleteDialog = false;

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

  async asyncData({ params }: any) {
    return { id: params.id };
  }

  onEdit() {
    this.$router.push(`edit/${this.order!.id}`);
  }

  onDelete() {
    this.deleteDialog = true;
  }

  onHistory() {
    this.$router.push(`history/${this.order!.id}`);
  }

  onNavigateToType(typeId: number) {
    this.$router.push(`/types/${typeId}`);
  }

  onNavigateToAttribute(attributeId: number) {
    this.$router.push(`/attributes/${attributeId}`);
  }

  updated() {
    if (!this.id) {
      this.$router.back();
    }
  }

  mounted() {
    if (!this.id) {
      this.$router.back();
    }

    ordersStore
      .getOrder({ id: this.id, checkDatetime: null })
      .then((suceeded) => {
        if (!suceeded) {
          this.$router.back();
        } else {
          this.loaded = true;
        }
      });
  }
}
</script>
