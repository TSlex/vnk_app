<template>
  <v-dialog v-model="active" max-width="700px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline"
            >Вы уверены, что хотите удалить этот тип?</span
          >
        </v-card-title>
          <v-container class="px-10">
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-alert battributeType="left" color="warning" dense outlined type="info">
              Данное дейсвие не может быть отменено!
            </v-alert>
          </v-container>
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
import { Component, Prop, Vue } from "nuxt-property-decorator"
import { attributeTypesStore } from "~/store";

@Component({})
export default class TypeDeleteDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  id = 0;

  get error() {
    return attributeTypesStore.error;
  }

  get attributeType() {
    return attributeTypesStore.selectedAttributeType;
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
    this.id = this.attributeType?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributeTypesStore.deleteAttributeType(this.id).then((succeeded) => {
        if (succeeded) {
          this.$router.push("/types")
        } else {
          this.showError = true;
        }
      });
    }
  }
}
</script>
