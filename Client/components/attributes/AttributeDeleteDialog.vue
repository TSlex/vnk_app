<template>
  <v-dialog v-model="active" max-width="700px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline"
            >Вы уверены, что хотите удалить атрибут "{{ attribute.name }}"?</span
          >
        </v-card-title>
          <v-container class="px-10">
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-alert battribute="left" color="warning" dense outlined type="info">
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
import { attributesStore } from "~/store";

@Component({})
export default class AttributeDeleteDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  id = 0;

  get error() {
    return attributesStore.error;
  }

  get attribute() {
    return attributesStore.selectedAttribute;
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
    this.id = this.attribute?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      attributesStore.deleteAttribute(this.id).then((succeeded) => {
        if (succeeded) {
          this.$router.push("/attributes")
        } else {
          this.showError = true;
        }
      });
    }
  }
}
</script>
