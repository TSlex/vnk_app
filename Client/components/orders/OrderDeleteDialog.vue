<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title class="two-lines">
          <span class="headline">Вы уверены, что хотите удалить заказ</span>
          <span class="subtitle-1">"{{ order.name | textTruncate(50) }}?"</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-alert border="left" color="warning" dense outlined type="info">
              Данное дейсвие не может быть отменено!
            </v-alert>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="error darken-1" text type="submit">Удалить</v-btn>
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { ordersStore } from "~/store";

@Component({})
export default class OrderDeleteDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  id = 0;

  get error() {
    return ordersStore.error;
  }

  get order() {
    return ordersStore.selectedOrder;
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
    this.id = this.order?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      ordersStore.deleteOrder(this.id).then((succeeded) => {
        if (succeeded) {
          this.$router.push("/orders");
        } else {
          this.showError = true;
        }
      });
    }
  }
}
</script>
