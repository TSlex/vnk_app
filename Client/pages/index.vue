<template>
  <v-row dense class="py-4 px-6 fill-height">
    <v-col>
      <Calendar :weekdays="weekdays" :orders="orders" />
    </v-col>
    <v-col cols="3">
      <v-sheet height="700" rounded="lg">
        <v-list-item v-for="attribute in attributes" :key="attribute.name">
          <v-list-item-content>
            <v-list-item-title v-text="attribute.name"></v-list-item-title>
            <v-list-item-subtitle
              v-text="attribute.value"
            ></v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import Calendar from "@/components/Calendar.vue";
import { ordersStore, attributesStore } from "~/store";

import { Component, Prop, Vue } from "nuxt-property-decorator";

@Component({
  components: {
    Calendar,
  },
})
export default class IndexPage extends Vue {
  weekdays = [1, 2, 3, 4, 5];

  get orders() {
    return ordersStore.orders;
  }

  get attributes() {
    return attributesStore.attributes;
  }
}
</script>
