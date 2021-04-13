<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline">Создать пользователя</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-text-field
              label="Имя"
              required
              v-model="model.firstName"
              :rules="rules.firstName"
            ></v-text-field>
            <v-text-field
              label="Фамилия"
              required
              v-model="model.lastName"
              :rules="rules.lastName"
            ></v-text-field>
            <v-text-field
              label="Эл.адрес"
              required
              v-model="model.email"
              :rules="rules.email"
            ></v-text-field>
            <v-text-field
              label="Пароль"
              required
              v-model="model.password"
              :rules="rules.password"
            ></v-text-field>
            <v-text-field
              label="Повтор пароля"
              required
              v-model="passwordConfirmation"
              :rules="rules.passwordConfirmation"
            ></v-text-field>
            <v-select
              class="mt-1"
              v-if="isRoot"
              :items="availableRoles"
              label="Роль"
              outlined
              v-model="model.role"
            ></v-select>
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click.stop="onClose()"
            >Отмена</v-btn
          >
          <v-btn color="blue darken-1" text type="submit">Создать</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { identityStore, usersStore } from "~/store";
import { UserPostDTO } from "~/models/Identity/UserDTO";
import {
  email,
  password,
  required,
  validate,
  minlength,
  maxlength,
} from "~/utils/form-validation";

@Component({})
export default class UserCreateDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  model: UserPostDTO = {
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    role: null,
  };

  rules = {
    firstName: [...validate(required), minlength(1), maxlength(128)],
    lastName: [...validate(required), minlength(1), maxlength(128)],
    email: validate(required, email),
    password: validate(required, password),
    passwordConfirmation: [
      (v: any) => v === this.model.password || "Пароли должны совпадать",
    ],
  };

  passwordConfirmation: string = "";

  get error() {
    return usersStore.error;
  }

  get isRoot() {
    return identityStore.isRoot;
  }

  get availableRoles() {
    return [
      { text: "Пользователь", value: "User" },
      { text: "Администратор", value: "Administrator" },
    ];
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

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      usersStore.createUser(this.model).then((succeeded) => {
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
