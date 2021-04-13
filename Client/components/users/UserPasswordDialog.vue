<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span v-if="!isPersonal" class="headline">Изменить пароль пользователю "{{ fullName }}"</span>
          <span v-else class="headline">Сменить пароль</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-text-field
              label="Текущий пароль"
              required
              v-model="model.currentPassword"
              :rules="rules.password"
            ></v-text-field>
            <v-text-field
              label="Новый пароль"
              required
              v-model="model.newPassword"
              :rules="rules.password"
            ></v-text-field>
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
import { identityStore, usersStore } from "~/store";
import { UserPasswordPatchDTO, UserPostDTO } from "~/models/Identity/UserDTO";
import {
  email,
  password,
  required,
  validate,
  minlength,
  maxlength,
} from "~/utils/form-validation";

@Component({})
export default class UserPasswordDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  model: UserPasswordPatchDTO = {
    id: 0,
    currentPassword: "",
    newPassword: "",
  };

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

  get isPersonal() {
    return !usersStore.selectedUser;
  }

  get selectedUser() {
    return usersStore.selectedUser || identityStore.userData;
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
    this.model.id = this.selectedUser?.id ?? 0;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      usersStore.updateUserPassword(this.model).then((succeeded) => {
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
