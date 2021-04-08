<template>
  <v-col>
    <template v-if="fetched">
      <template v-if="!isPersonal">
        <v-toolbar dense flat>
          <v-btn text @click.stop="onUserClosed()"
            ><v-icon>mdi-keyboard-return</v-icon>Назад</v-btn
          >
          <v-spacer></v-spacer>
        </v-toolbar>
        <v-divider></v-divider>
      </template>
      <div class="py-6">
        <div class="d-flex justify-space-between">
          <span>Имя:</span><span>{{ userData.firstName }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Фамилия:</span><span>{{ userData.lastName }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Эл.адрес:</span><span>{{ userData.email }}</span>
        </div>
        <div class="d-flex justify-space-between">
          <span>Роль:</span><span>{{ userData.roleLocalized }}</span>
        </div>
      </div>
      <v-divider></v-divider>
      <template v-if="isButtonsEnabled">
        <v-btn
          class="mt-3 mr-2"
          v-if="isChangeButtonEnabled"
          @click.stop="changeDialog = true"
          >Изменить данные</v-btn
        >
        <v-btn
          class="mt-3 mr-2"
          v-if="isChangeRoleButtonEnabled"
          @click.stop="roleChangeDialog = true"
          >Изменить роль</v-btn
        >
        <v-btn
          class="mt-3 mr-2"
          v-if="isChangePasswordButtonEnabled"
          @click.stop="passwordChangeDialog = true"
          >Изменить пароль</v-btn
        >
        <v-btn
          class="mt-3 mr-2"
          v-if="isDeleteButtonEnabled"
          @click.stop="deleteDialog = true"
          >Удалить</v-btn
        >
      </template>
      <template v-else
        ><div class="mt-2">
          Данный пользователь не может быть изменен
        </div></template
      >
    </template>
    <UserRoleDialog v-model="roleChangeDialog" v-if="roleChangeDialog" />
    <UserDataDialog v-model="changeDialog" v-if="changeDialog" />
  </v-col>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "nuxt-property-decorator";
import { identityStore, usersStore } from "~/store";
import { UserGetDTO } from "~/types/Identity/UserDTO";
import UserRoleDialog from "~/components/users/UserRoleDialog.vue";
import UserDataDialog from "~/components/users/UserDataDialog.vue";

@Component({
  components: {
    UserRoleDialog,
    UserDataDialog,
  },
})
export default class UserData extends Vue {
  fetched = false;
  roleChangeDialog = false;
  passwordChangeDialog = false;
  changeDialog = false;
  deleteDialog = false;

  get isPersonal() {
    return !this.sellectedUser;
  }

  get isCurrentUserSellected() {
    return this.isPersonal || this.sellectedUser?.id == this.currentUser?.id;
  }

  get isButtonsEnabled() {
    return (
      this.isDeleteButtonEnabled ||
      this.isChangeButtonEnabled ||
      this.isChangeRoleButtonEnabled ||
      this.isChangePasswordButtonEnabled
    );
  }

  get isDeleteButtonEnabled() {
    return (
      !this.isCurrentUserSellected &&
      this.userHaveAccessToUser(this.currentUser!, this.sellectedUser!)
    );
  }

  get isChangeButtonEnabled() {
    if (this.isCurrentUserSellected && this.currentUser?.role == "Root") {
      return false;
    }
    return (
      this.isCurrentUserSellected ||
      this.userHaveAccessToUser(this.currentUser!, this.sellectedUser!)
    );
  }

  get isChangeRoleButtonEnabled() {
    return !this.isCurrentUserSellected && this.currentUser!.role === "Root";
  }

  get isChangePasswordButtonEnabled() {
    if (this.isCurrentUserSellected && this.currentUser?.role == "Root") {
      return false;
    }

    return (
      this.isCurrentUserSellected ||
      this.userHaveAccessToUser(this.currentUser!, this.sellectedUser!)
    );
  }

  get userData() {
    if (!this.isPersonal) {
      return this.sellectedUser;
    }
    return identityStore.userData;
  }

  get currentUser() {
    return identityStore.userData;
  }

  get sellectedUser() {
    return usersStore.selectedUser;
  }

  userHaveAccessToUser(currentUser: UserGetDTO, targetUser: UserGetDTO) {
    if (targetUser.id === currentUser.id) return true;
    if (targetUser.role === "Root") return false;
    if (targetUser.role === "Administrator" && currentUser.role === "Root")
      return true;
    if (targetUser.role === "User" && currentUser.role != "User") return true;

    return false;
  }

  onUserClosed() {
    usersStore.SELECTED_USER_CLEARED();
  }

  mounted() {
    if (this.isPersonal) {
      identityStore.fetchData().then((_) => (this.fetched = true));
    } else {
      this.fetched = true;
    }
  }
}
</script>

