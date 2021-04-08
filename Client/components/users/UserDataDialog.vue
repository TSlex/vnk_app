<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline">Изменить пользователя {{fullName}}</span>
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
import { UserPatchDTO, UserPostDTO } from "~/types/Identity/UserDTO";
import {
  email,
  password,
  required,
  validate,
  minlength,
  maxlength,
} from "~/utils/form-validation";

@Component({})
export default class UserDataDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  model: UserPatchDTO = {
    id: 0,
    firstName: "",
    lastName: "",
    email: "",
  };

  rules = {
    firstName: [...validate(required), minlength(1), maxlength(128)],
    lastName: [...validate(required), minlength(1), maxlength(128)],
    email: validate(required, email),
  };

  get fullName() {
    return `${this.selectedUser?.firstName} ${this.selectedUser?.lastName}`;
  }

  get selectedUser() {
    return usersStore.selectedUser;
  }

  get error() {
    return usersStore.error;
  }

  get isRoot() {
    return identityStore.isRoot;
  }

  get active() {
    return this.value;
  }

  set active(value) {
    this.$emit("input", value);
  }

  mounted() {
    this.model = {...this.selectedUser} as UserPatchDTO
  }

  onClose() {
    this.active = false;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      usersStore.updateUser(this.model).then((succeeded) => {
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
