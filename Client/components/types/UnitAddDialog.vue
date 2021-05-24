<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline">Добавить значение</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-text-field
              v-model="newValue"
              label="Значение поля"
              :rules="rules.value"
            />
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit">Сохранить</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { maxlength, required } from "~/utils/form-validation";

@Component({
  components: {},
})
export default class UnitAddDialog extends Vue {
  @Prop()
  value!: boolean;

  @Prop()
  model!: string;

  rules = {
    value: [required(), maxlength(200)],
  };

  get newValue() {
    return this.model;
  }

  set newValue(value) {
    this.$emit("change", value);
  }

  get active() {
    return this.value;
  }

  set active(value) {
    this.$emit("input", value);
  }

  onClose() {
    (this.$refs.form as any).resetValidation()
    this.active = false;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      this.$emit("submit");
      this.onClose();
    }
  }
}
</script>
