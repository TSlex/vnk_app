<template>
  <v-menu
    v-model="showMenu"
    absolute
    offset-y
    style="max-width: 600px rounded-lg"
  >
    <template v-slot:activator="{ on, attrs }">
      <div
        class="blue-grey lighten-4 text-subtitle-2"
        v-bind="attrs"
        v-on="on"
        v-if="orders && orders.length > 3"
      >
        {{ formatDate(date) }}
        <v-icon small>mdi-chevron-down</v-icon>
      </div>
      <div
        v-else
        class="grey lighten-3 text-subtitle-2"
        v-bind="attrs"
        v-on="on"
      >
        {{ formatDate(date) }}
      </div>
    </template>
    <v-container class="white pa-4">
      <span class="text-body-1">Заказы на {{ date | formatDate }}</span>
      <v-divider></v-divider>
      <v-container v-if="orders && orders.length > 0">
        <v-chip
          @click.stop="onOrderSellect(order)"
          v-for="order in orders"
          :key="order.id"
          class="mb-2 d-flex justify-center"
          style="width: 100%"
          small
          :color="
            order.completed ? `success` : order.overdued ? `error` : `primary`
          "
        >
          <span>
            {{
              formatFeaturedAttributesInline(getOrderFeaturedAttributes(order))
            }}
          </span>
        </v-chip>
      </v-container>
      <v-container v-else>
        <span class="text-body-2">заказов нет</span>
      </v-container>
    </v-container>
  </v-menu>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "nuxt-property-decorator";
import { OrderAttributeGetDTO, OrderGetDTO } from "~/models/OrderDTO";

@Component({})
export default class CalendarMenu extends Vue {
  showMenu = false;

  @Prop()
  date!: string;

  @Prop()
  orders!: OrderGetDTO[];

  formatDate(date: Date){
    return this.$moment(date).format("DD.MM.YYYY")
  }

  getOrderFeaturedAttributes(order: OrderGetDTO) {
    return order.attributes.filter((attribute) => attribute.featured);
  }

  formatFeaturedAttributesInline(attributes: OrderAttributeGetDTO[]) {
    let text = attributes
      .map((attribute) => {
        let line = `${this.formatText(attribute.name)}: ${this.formatText(attribute.value)}`;

        if (attribute.usesDefinedUnits) {
          line += ` ${this.formatText(attribute.unit)}`;
        }

        return line;
      })
      .join(", ");

    return this.$options.filters!.textTruncate(text, 150)
  }

  formatText(value: string){
    return this.$options.filters!.textTruncate(value.toLocaleLowerCase(), 30)
  }

  onOrderSellect(order: OrderGetDTO) {
    this.$emit("order-sellect", order);
  }
}
</script>
