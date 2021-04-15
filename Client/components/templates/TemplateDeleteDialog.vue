<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline"
            >Вы уверены, что хотите удалить "{{ template.name }}"?</span
          >
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-alert colored-border type="info">
              Данное дейсвие не может быть отменено!
            </v-alert>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit">Удалить</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { templatesStore, usersStore } from "~/store";

@Component({})
export default class TemplateDeleteDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  id = 0;

  get error() {
    return usersStore.error;
  }

  get template() {
    return templatesStore.selectedTemplate;
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
    this.id = this.template?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      templatesStore.deleteTemplate(this.id).then((succeeded) => {
        if (succeeded) {
          this.onClose();
        } else {
          this.showError = true;
        }
      });
    }
  }
}
</script>
