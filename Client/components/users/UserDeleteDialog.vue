<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline"
            >Вы уверены, что хотите удалить "{{ fullName }}"?</span
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
import { usersStore } from "~/store";
import { UserPasswordPatchDTO } from "~/types/Identity/UserDTO";
import { password, required, validate } from "~/utils/form-validation";

@Component({})
export default class UserDeleteDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  id = 0;

  rules = {
    password: validate(required, password),
  };

  passwordConfirmation: string = "";

  get error() {
    return usersStore.error;
  }

  get fullName() {
    return `${this.selectedUser?.firstName} ${this.selectedUser?.lastName}`;
  }

  get selectedUser() {
    return usersStore.selectedUser;
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
    this.id = this.selectedUser?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      usersStore.deleteUser(this.id).then((succeeded) => {
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
