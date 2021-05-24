<template>
  <v-dialog v-model="active" max-width="600px">
    <v-form class="mt-6" @submit.prevent="onSubmit()" ref="form">
      <v-card>
        <v-card-title>
          <span class="headline">Сменить роль пользователю</span>
          <span class="subtitle-1">"{{ fullName | textTruncate(50) }}"</span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <v-alert dense text type="error" v-if="showError">{{
              error
            }}</v-alert>
            <v-select
              class="mt-1"
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
          <v-btn color="blue darken-1" text type="submit">Сохранить</v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
  </v-dialog>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { identityStore, usersStore } from "~/store";
import { UserRolePatchDTO } from "~/models/Identity/UserDTO";
import { required } from "~/utils/form-validation";

@Component({})
export default class UserRoleDialog extends Vue {
  @Prop()
  value!: boolean;

  showError = false;

  model: UserRolePatchDTO = {
    role: "",
    id: 0,
  };

  rules = {
    role: [required()],
  };

  get error() {
    return usersStore.error;
  }

  get fullName() {
    return `${this.selectedUser?.firstName} ${this.selectedUser?.lastName}`;
  }

  get selectedUser() {
    return usersStore.selectedUser;
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

  mounted() {
    this.model.id = this.selectedUser?.id ?? 0;
    this.model.role = this.selectedUser?.role ?? "";
  }

  onClose() {
    this.active = false;
  }

  onSubmit() {
    if ((this.$refs.form as any).validate()) {
      usersStore.updateUserRole(this.model).then((succeeded) => {
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

